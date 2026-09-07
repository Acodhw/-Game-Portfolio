using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSnail : MonoBehaviour
{
    public float movementSpeed;
    public float groundCheckRadius;
    public float slopeCheckDistance;
    public float maxSlopeAngle;
    public float PlayerFindDistance;
    public float PlayerFollowDistance;
    public float PlayerKeepDistance;
    public float findTime = 5;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D fullFriction;
    public GameObject FindedPlayerIcon;
    public GameObject ShotOBJ;
    public Animator EnemyAnimator;
    public EnemyControll enemyControll;
    public AudioClip AttackSound;
    public AudioClip HitSound;
    public AudioClip DieSound;

    private AudioSource audioSource;
    private float xInput;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    private float findPlayerTime;


    private int facingDirection = 1;

    private bool isGrounded;
    private bool isOnSlope;
    private bool canWalkOnSlope;
    private bool AttakingPlayer;
    bool dying = false;

    private Vector2 newVelocity;
    private Vector2 capsuleColliderSize;

    private Vector2 slopeNormalPerp;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Transform player;



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("MainCharacter").transform;
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        capsuleColliderSize = cc.size;
        Invoke("MovingCheck", Random.Range(1.75f, 4f));
    }

    void MovingCheck()
    {
        if (findPlayerTime <= 0)
        {
            xInput = Random.Range(-1, 2);
        }
        Invoke("MovingCheck", Random.Range(1f, 2f));

    }

    private void Update()
    {
        if (enemyControll.getDamage)
        {
            enemyControll.getDamage = false;
            audioSource.PlayOneShot(HitSound);
            findPlayerTime = findTime;
        }

        if (enemyControll.HP <= 0 && !dying) StartCoroutine("Die");

        FindedPlayerIcon.SetActive(findPlayerTime > 0);
        if (xInput == 1 && facingDirection == -1)
        {
            Flip();
        }
        else if (xInput == -1 && facingDirection == 1)
        {
            Flip();
        }
        if (findPlayerTime > 0)
        {
            if (!AttakingPlayer)
            {
                if (player.position.x - transform.position.x > 0 && facingDirection == -1)
                {
                    Flip();
                }
                else if (player.position.x - transform.position.x < 0 && facingDirection == 1)
                {
                    Flip();
                }
            }
            float tmp = 0;
            if (Mathf.Abs(player.position.x - transform.position.x) > PlayerFollowDistance || (transform.position.y - player.position.y < -2 || transform.position.y - player.position.y > 1))
            {
                tmp = transform.position.x - player.position.x;
                xInput = !AttakingPlayer ? -(int)(tmp / Mathf.Abs(tmp)) : 0;
            }
            else if (Mathf.Abs(player.position.x - transform.position.x) < PlayerKeepDistance)
            {
                tmp = transform.position.x - player.position.x;
                xInput = !AttakingPlayer ? (int)(tmp / Mathf.Abs(tmp)) : 0;
            }
            else
            {
                if (!AttakingPlayer)
                {
                    AttakingPlayer = true;
                    StartCoroutine("Attack");
                }
                xInput = 0;
            }
            findPlayerTime -= Time.deltaTime;
        }
        else if (findPlayerTime < 0) findPlayerTime = 0;
        else
        {
            if (xInput == 1 && facingDirection == -1)
            {
                Flip();
            }
            else if (xInput == -1 && facingDirection == 1)
            {
                Flip();
            }
        }
    }
    IEnumerator Attack()
    {
        audioSource.PlayOneShot(AttackSound);
        yield return new WaitForSeconds(0.15f);
        ShotsEvent se = Instantiate(ShotOBJ, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
        se.shotDirection = player.transform.position - transform.position;
        se.ShotObj();
        yield return new WaitForSeconds(1.2f);
        AttakingPlayer = false;
    }

    IEnumerator Die()
    {
        audioSource.PlayOneShot(DieSound);
        dying = true;
        EnemyAnimator.SetTrigger("Die");

        yield return new WaitForSeconds(0.6f);
        Rigidbody2D rb;
        if (Random.Range(0, 100) < enemyControll.HPitemFallProbability)
        {
            rb = Instantiate(enemyControll.HPItem, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 4f)), ForceMode2D.Impulse);
        }
        if (Random.Range(0, 100) < enemyControll.MPitemFallProbability)
        {
            rb = Instantiate(enemyControll.MPItem, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(4f, 4f)), ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        PlayerFind();
        CheckGround();
        SlopeCheck();
        ApplyMovement();
    }

    void PlayerFind()
    {
        if (PlayerFindDistance >= Vector2.Distance(player.position, transform.position))
        {
            Debug.DrawRay(transform.position, (Vector2)(player.position - transform.position).normalized * PlayerFindDistance, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (Vector2)(player.position - transform.position).normalized, PlayerFindDistance, 1 << 6 | 1 << 3);
            if (hit.collider.gameObject.layer == 6) findPlayerTime = findTime;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        Vector2 checkPos = transform.position - (Vector3)(new Vector2((capsuleColliderSize.x / 1.5f) * -xInput, capsuleColliderSize.y / 2));
        Debug.DrawRay(checkPos, Vector2.down * slopeCheckDistance, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
        if (!hit && !isOnSlope && findPlayerTime <= 0) xInput *= -1;
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

    private void ApplyMovement()
    {
        if (isGrounded && !isOnSlope)
        {
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.linearVelocity = newVelocity;
        }
        else if (isGrounded && isOnSlope && canWalkOnSlope)
        {
            newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xInput, movementSpeed * slopeNormalPerp.y * -xInput);
            rb.linearVelocity = newVelocity;
        }
        else if (!isGrounded)
        {
            newVelocity.Set(movementSpeed * xInput, rb.linearVelocity.y);
            rb.linearVelocity = newVelocity;
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
