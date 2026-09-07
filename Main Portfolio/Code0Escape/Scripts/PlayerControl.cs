using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Moving Setting")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private int maxJumpCount = 2;

    [Header("GroundSetting")]
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask groundLayer;
 
    private bool ItemCool = true;

    private PlayerState state;
    private SystemSetting settings;
    private CharacterController player;

    private float horizentalInput;
    private float verticalInput;
    private const float sensitivityValue = 0.75f;

    private bool isInputJumpKey = false;

    private Vector3 moveVelocity;
    private float velocityY;

    private float dash = 1f;
    private float buffSpeeedRank = 1;

    private bool isOnGround;

    private float rotationSpeed = 2.0f; // 회전 속도

    private float maxLookUp = -90.0f; // 최대 위쪽 회전 각도
    private float maxLookDown = 90.0f; // 최대 아래쪽 회전 각도
    private float rotationX = 0;

    void Start()
    {       
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();
        settings = GameObject.Find("GameManager").GetComponent<SystemSetting>();       
        player = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (state.GetHP() <= 0) {
            SceneManager.LoadScene("GameOver");
        }
        if (Time.timeScale > 0)
        {
            rotationSpeed = settings.GetSensitvity();
            horizentalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            dash = Input.GetButton("Fire3") ? 1.75f : 1;

            IsGrounded();

            animator.SetFloat("Y_velocity", velocityY);
            animator.SetBool("OnGround", isOnGround);
            animator.SetBool("Attack", Input.GetButton("Fire1"));

            if (horizentalInput != 0 || verticalInput != 0) animator.SetTrigger(dash == 1 ? "walk" : "run");
            else animator.SetTrigger("idle");

            if (isOnGround)
            {
                velocityY = 0;
                RaycastHit groundRayCast;
                Physics.Raycast(new Ray(transform.position, Vector3.down), out groundRayCast, 5f, groundLayer);
                moveVelocity = Vector3.ProjectOnPlane((transform.right * horizentalInput + transform.forward * verticalInput).normalized * moveSpeed * dash * buffSpeeedRank, groundRayCast.normal);
            }

            isInputJumpKey = Input.GetButtonDown("Jump");
            if (isInputJumpKey)
            {
                moveVelocity = (transform.right * horizentalInput + transform.forward * verticalInput).normalized * moveSpeed;
                if (isOnGround)
                {
                    animator.SetTrigger("Jump");
                    velocityY = jumpPower * buffSpeeedRank;
                    jumpCount = 1;
                    isInputJumpKey = false;
                }
                else if (jumpCount < maxJumpCount)
                {

                    velocityY = jumpPower * buffSpeeedRank;
                    jumpCount++;
                    isInputJumpKey = false;
                }
            }
            player.Move(moveVelocity * Time.deltaTime + (Vector3.up * velocityY));
            RotatePlayer(); // 플레이어 회전 함수 호출

            if (ItemCool) {
                if (Input.GetKeyDown(KeyCode.E) && state.GetHpitem() > 0)
                {
                    ItemCool = false;
                    state.SetHpitem(state.GetHpitem() - 1);
                    state.Heal(Mathf.RoundToInt(state.GetMaxHP() * 0.15f));
                    StartCoroutine("itemcooling");
                }
                if (Input.GetKeyDown(KeyCode.R) && state.GetBuffitem() > 0 && buffSpeeedRank == 1)
                {
                    ItemCool = false;
                    state.SetBuffitem(state.GetBuffitem() - 1);
                    StartCoroutine("Buffing");
                    StartCoroutine("itemcooling");
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) 
                state.SetNowslot(1);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                state.SetNowslot(2);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                state.SetNowslot(3);

        }
    }

    IEnumerator Buffing()
    {
        buffSpeeedRank = 1.5f;
        yield return new WaitForSeconds(3f);
        buffSpeeedRank = 1;
    }
    IEnumerator itemcooling() {
        yield return new WaitForSeconds(0.1f);
        ItemCool = true;
    }

    private void FixedUpdate()
    {
        velocityY -= 0.98f * Time.fixedDeltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }

    public void IsGrounded()
    {
        isOnGround = Physics.BoxCast(transform.position, new Vector3(0.6f, 0.1f, 0.6f), -transform.up, transform.rotation, maxDistance, groundLayer);
    }

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 플레이어 회전
        transform.rotation *= Quaternion.Euler(0, mouseX * rotationSpeed * Time.timeScale, 0) ;

        // 카메라 회전
        rotationX -= mouseY * rotationSpeed * Time.timeScale;
        rotationX = Mathf.Clamp(rotationX, maxLookUp, maxLookDown);
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }
}
