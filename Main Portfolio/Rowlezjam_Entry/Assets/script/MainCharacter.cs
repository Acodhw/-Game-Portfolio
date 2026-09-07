using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MainCharacter : MonoBehaviour
{
    GameManager gameManager;
    MainUi mainUi;
    AudioSource audioSource;
    Animator animator;
    SpriteRenderer spriteRenderer;

    public GameObject[] NoElementShots;
    public GameObject[] FireShots;
    public GameObject[] WaterShots;
    public GameObject[] GroundShots;
    public GameObject[] WindShots;
    public AudioClip[] SoundEffects;

    public float movementSpeed;
    public float groundCheckRadius;
    public float jumpForce;
    public float slopeCheckDistance;
    public float maxSlopeAngle;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D fullFriction;

    float xInput;
    float slopeDownAngle;
    float slopeSideAngle;
    float lastSlopeAngle;
    float hitCooltime = 0;
    float nowAlpha = 1;

    int facingDirection = 1;

    bool isGrounded;
    bool isOnSlope;
    bool isJumping;
    bool canWalkOnSlope;
    bool canJump;
    [HideInInspector]
    public bool canAvoid;
    bool isAvoiding;
    bool canAttack = true;
    bool dying = false;

    bool isOnInterection = false;
    bool isOnWater;

    Vector2 newVelocity;
    Vector2 newForce;
    Vector2 capsuleColliderSize;

    Vector2 slopeNormalPerp;
    

    Rigidbody2D rb;
    CapsuleCollider2D cc;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.SettingMainChar(this);
        if (gameManager.MovingToSavePosition)
        {
            transform.position = gameManager.PotalMovePosition;
            gameManager.MovingToSavePosition = false;
        }
        mainUi = GameObject.Find("Canvas").GetComponent<MainUi>();
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine("AvoidCooltime");
        capsuleColliderSize = cc.size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.tag == "DeathZone") gameManager.Hp = 0;
    }


    public void GetItem(bool getedHP) {       
        audioSource.PlayOneShot(SoundEffects[4]);
        if (getedHP)
            gameManager.Heal(4);
        else
            gameManager.MPHeal(3);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 4) isOnWater = true;
        if (collision.tag == "Interaction") isOnInterection = true;
        if (collision.tag == "EnemyAttack" && hitCooltime <= 0) {
            hitCooltime = 0.75f;
            audioSource.PlayOneShot(SoundEffects[5]);
            gameManager.Damage(collision.GetComponent<EnemyAttack>().Damage);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 4) isOnWater = false;
        if (collision.tag == "Interaction") isOnInterection = false;
    }

    private void Update()
    {
        if (gameManager.Hp <= 0 && !dying) {
            dying = true;
            StartCoroutine("Death");
        }
        if (!mainUi.paused)
        {
            animator.SetBool("onGround", isGrounded);
            animator.SetFloat("Yvelocity", rb.linearVelocity.y);
            if (!mainUi.talking && mainUi.UIStart && !dying)
            {
                CheckInput();
            }
            else {
                xInput = 0;
            }
            CoolingHit();
            CheckColor();
        }
    }

    IEnumerator Death() {
        mainUi.GameOverd = true;
        audioSource.PlayOneShot(SoundEffects[6]);
        animator.SetTrigger("retire");
        rb.linearVelocity = Vector2.zero;
        xInput = 0;
        yield return new WaitForSeconds(1f);
        StartCoroutine(mainUi.GameOver());
    }

    private void FixedUpdate()
    {
        if (!mainUi.paused)
        {
            CheckGround();
            SlopeCheck();
            ApplyMovement();
        }
    }


    private void CheckColor() {
        Color eleColor = Color.white;
        switch (gameManager.nowElement) {
            case 0:
                eleColor = new Color(1, 1, 1, nowAlpha);
                break;
            case 1:
                eleColor = new Color(0.95f, 0.5f, 0.3f, nowAlpha);
                break;
            case 2:
                eleColor = new Color(0.4f, 0.5f, 0.98f, nowAlpha);
                break;
            case 3:
                eleColor = new Color(0.9f, 0.7f, 0.4f, nowAlpha);
                break;
            case 4:
                eleColor = new Color(0.4f, 0.95f, 0.75f, nowAlpha);
                break;
        }
        spriteRenderer.color = eleColor;
    }
    private void CoolingHit() {
        if (hitCooltime > 0)
        {
            hitCooltime -= Time.deltaTime;
            nowAlpha = 0.6f;
        }
        else if (hitCooltime < 0) hitCooltime = 0;
        else nowAlpha = 1;
    }
    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        animator.SetBool("walk", xInput != 0);

        if (xInput == 1 && facingDirection == -1)
        {
            Flip();
        }
        else if (xInput == -1 && facingDirection == 1)
        {
            Flip();
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonDown("Check"))
        {
            if (!isOnInterection && canAttack) Attack();
        }

        if (Input.GetButtonDown("Change")) {
            ChangeElemental();
            
        }

        if (Input.GetButtonDown("Avoid") && canAvoid)
        {         
            AvoidAttacks();
        }
    }

    private void AvoidAttacks() {    
        canAvoid = false;
        isAvoiding = true;
        newVelocity.Set(0.0f, 0.0f);
        rb.linearVelocity = newVelocity;
        xInput = Input.GetAxisRaw("Horizontal");
        if (xInput > 0)
            rb.AddForce(new Vector2(2, 1).normalized * jumpForce * 0.75f, ForceMode2D.Impulse);
        else if (xInput < 0)
            rb.AddForce(new Vector2(-2, 1).normalized * jumpForce * 0.75f, ForceMode2D.Impulse);
        else
        {
            if (facingDirection < 0)
                rb.AddForce(new Vector2(2, 1).normalized * jumpForce * 0.75f , ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(-2, 1).normalized * jumpForce * 0.75f, ForceMode2D.Impulse);
        }
        audioSource.PlayOneShot(SoundEffects[1]);
        hitCooltime = 0.5f;
        StartCoroutine("AvoidCooltime");
    }

    private IEnumerator AvoidCooltime() {
        yield return new WaitForSeconds(0.5f);
        isAvoiding = false;
        yield return new WaitForSeconds(2.5f);
        canAvoid = true;
    }

    private void ChangeElemental() {
        int i = gameManager.nowElement;
        if (Input.GetAxisRaw("Change") > 0) {
            do {
                i++;
                if (i == 5) i = 0;
                if (gameManager.havingElement[i]) break;
            }
            while (i != gameManager.nowElement);
        }
        else
        {
            do
            {
                i--;
                if (i == -1) i = 4;
                if (gameManager.havingElement[i]) break;
            }
            while (i != gameManager.nowElement);
        }
        gameManager.nowElement = i;
    }

    private void Attack()
    {
        
        canAttack = false;
        animator.SetTrigger("Attack");
        if (gameManager.Mp > 0)
        {        
            audioSource.PlayOneShot(SoundEffects[2]);
            gameManager.Mp -= 1;

            ShotsEvent shots = new ShotsEvent();
            switch (gameManager.nowElement)
            {
                case 0:
                    shots = Instantiate(NoElementShots[gameManager.nowCatched], transform.position + Vector3.right * facingDirection * 0.75f, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    break;
                case 1:
                    shots = Instantiate(FireShots[gameManager.nowCatched], transform.position + Vector3.right * facingDirection * 0.75f, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    break;
                case 2:
                    shots = Instantiate(WaterShots[gameManager.nowCatched], transform.position + Vector3.right * facingDirection * 0.75f, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    break;
                case 3:
                    shots = Instantiate(GroundShots[gameManager.nowCatched], transform.position + Vector3.right * facingDirection * 0.75f, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    break;
                case 4:
                    shots = Instantiate(WindShots[gameManager.nowCatched], transform.position + Vector3.right * facingDirection * 0.75f, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    break;
            }
            shots.shotDirection = new Vector2(facingDirection * shots.shotDirection.x, shots.shotDirection.y);
            if (facingDirection < 1) shots.transform.localScale = new Vector3(-shots.transform.localScale.x, shots.transform.localScale.y, shots.transform.localScale.z);
            shots.ShotObj();
        }
        else audioSource.PlayOneShot(SoundEffects[3]);
        StartCoroutine("AttackCool");
    }

    IEnumerator AttackCool() {
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        if (rb.linearVelocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            canJump = true;
        }

    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, capsuleColliderSize.y / 2));

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

        if (slopeHitFront)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

        }
        else if (slopeHitBack)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }

            lastSlopeAngle = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && canWalkOnSlope && xInput == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            canJump = false;
            isJumping = true;
            newVelocity.Set(0.0f, 0.0f);
            rb.linearVelocity = newVelocity;
            newForce.Set(0.0f, jumpForce * (isOnWater ? 0.5f : 1));
            rb.AddForce(newForce, ForceMode2D.Impulse);
            audioSource.PlayOneShot(SoundEffects[0]);
        }
    }

    private void ApplyMovement()
    {
        if (!isAvoiding)
        {
            if (isGrounded && !isOnSlope && !isJumping) //if not on slope
            {
                newVelocity.Set(movementSpeed * xInput, 0.0f);
                rb.linearVelocity = newVelocity;
            }
            else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping) //If on slope
            {
                newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xInput, movementSpeed * slopeNormalPerp.y * -xInput);
                rb.linearVelocity = newVelocity;
            }
            else if (!isGrounded) //If in air
            {
                newVelocity.Set(movementSpeed * xInput, rb.linearVelocity.y);
                rb.linearVelocity = newVelocity;
            }
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
