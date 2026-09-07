using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerControl : MonoBehaviour
{
    private CharacterController controller;
    private Platforms[] platforms;
    public Transform lastestGround;

    [Header("Move Settings")]
    public Animator anim;
    public float moveSpeed = 6f;
    public float sprintSpeed = 10f;
    public float rotationSpeed = 15f;
    public float gravity = -20f;

    [Header("Sprint")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 0.5f;

    [Header("Jump Value")]
    public float maxJumpHeight = 2.5f;
    public float airAcceleration = 3f;
    [Tooltip("코요테 타임")]
    public float coyoteTime = 0.15f;
    [Tooltip("하강 시 중력 배율")]
    public float fallGravityMult = 1.5f;

    private float dynamicUpGravityMult = 1f;
    private float dynamicJumpVelocity;

    [Header("Sliding Settings")]
    public float slideSpeed = 12f;
    public float slideControlSpeed = 5f;

    [Header("Skills Settings")]
    public bool sideViewed;
    public GameObject quarterViewVirtual;
    public GameObject sideViewVirtual;
    public GameObject orthoViewVirtual;
    public CinemachineTargetGroup targerView;
    public Camera mainCam;
    public Transform map;
    public MapManager mapManager;

    [Header("Ground Check Value")]
    public LayerMask groundMask;
    [Range(0f, 1f)] public float groundCheckOffset = 0.15f;
    [Range(0.1f, 1f)] public float castRadiusMultiplier = 0.9f;

    [Header("Weapon / Armor Settings")]
    [Tooltip("플레이어 손에 장착된 무기/방어구 오브젝트")]
    public GameObject armorObj;

    [Header("Attack Settings")]
    public float attackMoveDistance = 0.5f;
    [Tooltip("공격 전진 속도 커브.")]
    public AnimationCurve attackMoveCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    private readonly float attack1Duration = 0.7f / 1.5f;
    private readonly float attack2Duration = 0.6f / 1.5f;

    [Header("Interaction Settings")]
    [Tooltip("상호작용 가능한 거리")]
    public float interactRange = 2f;
    [Tooltip("상호작용 감지 반경")]
    public float interactRadius = 1f;
    [Tooltip("상호작용을 감지할 레이어")]
    public LayerMask interactMask = ~0;

    private MonoBehaviour currentTargetInteractable;

    [Header("Audio Settings")]
    [Tooltip("플레이어 행동에 대한 오디오 클립들")]
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip skillUseSound;
    public AudioClip dashSound; 

    [Header("Game Over Settings")]
    [Tooltip("게임 오버 씬 이름")]
    public string gameOverSceneName = "GameOverScene";
    [Tooltip("페이드 이미지")]
    public Image gameOverFadeImage;

    private AudioSource walkAudioSource;
    private bool wasGrounded; 

    [Header("UI - Gear Skill")]
    public RectTransform gearUI;
    public Image skillIconUI;
    [Tooltip("0:오류(빈칸), 1:망치, 2:플라이, 3:변환 아이콘 이미지")]
    public Sprite[] skillSprites;

    [Header("UI - Status")]
    public Slider HPBar;
    public Slider Stamina;
    public GameObject fade;
    public UIManager transformUI;

    private const float MAX_STAMINA = 100f;
    private const float STAMINA_SPRINT_COST = 10f;     // 스프린트 시작 순간 즉시 차감
    private const float STAMINA_RUN_RATE = 5f;         // 달리는 동안 초당 소모
    private const float STAMINA_REGEN_RATE = 10f;      // 아무 행동 없을 때 초당 회복
    private const float STAMINA_2D_RATE = 5f;          // 2D 모드 초당 소모

    private PlayerState playerState;

    private Vector3 currentMoveInput;
    private bool wantSprintInput;
    private Vector3 platformVelocity;
    private MovePlatforms currentMovePlatform;

    private Vector3 moveVelocity;
    private Vector3 dashDirection;
    private Vector3 groundNormal;

    private float currentSlopeAngle;
    private float verticalVelocity;
    private float dashTimer;
    private float dashCooldownTimer;
    private float coyoteTimer;

    private float skillSwitchCooldownTimer = 0f;
    private bool isSkillAxisInUse = false;

    private bool isGround;
    private bool isDashing;
    private bool isUsingSkill;
    private bool isCameraMoving;
    private bool SkillCooling;
    private bool canAirDash = true;
    public bool is2D = false;
    private bool effectivelyGrounded;
    private bool isSteepSlope;
    private bool isFlyingMode;
    private bool lastSideViewed;
    private bool wasMoving;

    private bool isAttacking = false;
    private bool attackCancelByDash = false;

    private bool isDead = false;

    private float invincibilityTimer = 0f;

    private float quickSlotCooldownTimer = 0f;

    private GameObject currentTargetObj;
    private Vector3 originalTargetWorldPos;
    private Transform originalTargetParent;
    private bool targetOriginalRendererState;
    private Transform originalPlayerParent;
    private Vector3 originalPlayerPos;
    private GameObject visualBoundary;
    private Transform oldFollow;
    private Transform oldLookAt;

    private bool sprintLocked = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        originalPlayerParent = transform.parent;

        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true;
        walkAudioSource.playOnAwake = false;
        walkAudioSource.spatialBlend = 0f;
        walkAudioSource.clip = walkSound;
    }

    private void Start()
    {
        platforms = FindObjectsByType<Platforms>(FindObjectsSortMode.None);
        lastSideViewed = sideViewed;
        ApplyCameraSettings();

        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            playerState = gm.GetComponent<PlayerState>();
            if (playerState != null)
            {
                playerState.playerTransform = this.transform;

                bool anySkillActive = false;
                for (int i = 1; i <= 3; i++)
                {
                    if (playerState.skillActive.Length > (i - 1) && playerState.skillActive[i - 1])
                    {
                        anySkillActive = true;
                        break;
                    }
                }

                if (!anySkillActive)
                    playerState.selectedSkill = 0;

                if (skillIconUI != null && skillSprites.Length > playerState.selectedSkill)
                    skillIconUI.sprite = skillSprites[playerState.selectedSkill];
            }
        }
    }

    void FixedUpdate()
    {
        UpdatePhysicalState();
    }

    void Update()
    {
        PlayerAttack pa = armorObj.GetComponent<PlayerAttack>();
        pa.AttackVector = transform.forward;
        pa.Damage = playerState.power;

        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        if (quickSlotCooldownTimer > 0f)
        {
            quickSlotCooldownTimer -= Time.deltaTime;
        }

        if (isDead) return;

        CheckInteraction();

        if (playerState != null && playerState.currentHP <= 0 && !isDead)
        {
            Die();
        }
        if (!isUsingSkill && sideViewed != lastSideViewed)
        {
            lastSideViewed = sideViewed;
            ApplyCameraSettings();
        }
        if (playerState != null && armorObj != null)
        {
            if (armorObj.activeSelf != playerState.attackActive)
                armorObj.SetActive(playerState.attackActive);
        }
        if (!transformUI.isPaused && !transformUI.onInventory && !transformUI.isTalking)
        {
            if (!isUsingSkill)
            {
                GatherInputs();
                TryUseQuickSlot();

                if (isAttacking)
                {
                    HandleDashInput();
                    ChangeSkill();
                }
                else
                {
                    HandleDashInput();
                    HandleJumpInput();
                    UsingSkill();
                    ChangeSkill();
                    TryAttack();
                }
            }
        }
        else
        {
            currentMoveInput = Vector3.zero;
            wantSprintInput = false;
        }
        if (!isUsingSkill)
        {
            ProcessMovement();
        }

        UpdateStamina();
        UpdateUI();
        AnimationRenewal();
        UpdateAudio();
    }
    private void UpdateAudio()
    {
        if (GameManager.Instance != null && walkAudioSource != null)
        {
            walkAudioSource.volume = GameManager.Instance.currentConfig.masterVolume * GameManager.Instance.currentConfig.sfVolume;
        }
        bool isMovingOnGround = effectivelyGrounded && currentMoveInput.sqrMagnitude > 0.01f &&
                                !isDashing && !isAttacking && !isDead && !transformUI.isPaused && !isUsingSkill;

        if (isMovingOnGround && Time.timeScale > 0f)
        {
            bool isSprinting = wantSprintInput && !sprintLocked && !isSteepSlope;
            walkAudioSource.pitch = isSprinting ? 1.5f : 1.0f;

            if (!walkAudioSource.isPlaying) walkAudioSource.Play();
        }
        else
        {
            if (walkAudioSource.isPlaying) walkAudioSource.Pause();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FallTrap fallTrap = other.GetComponent<FallTrap>();
        if (fallTrap != null)
        {
            if (playerState != null)
            {
                int damage = Mathf.CeilToInt(playerState.maxHP * 0.1f);
                playerState.TakeDamage(damage, null);
            }

            if (fallTrap.safeZone != null)
            {
                if (controller != null) controller.enabled = false;
                transform.position = fallTrap.safeZone.position;
                if (controller != null) controller.enabled = true;
            }
            return;
        }

        if (isDead || isDashing || isFlyingMode || invincibilityTimer > 0f) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyAttack"))
        {
            EnemyAttack enemyAttack =
    other.attachedRigidbody ?
    other.attachedRigidbody.GetComponent<EnemyAttack>() :
    other.GetComponentInParent<EnemyAttack>();
            if (enemyAttack != null && playerState != null)
            {
                invincibilityTimer = 0.1f;
                playerState.TakeDamage(enemyAttack.damage, enemyAttack.hitEffectPrefab);
            }
        }
    }
    private void CheckInteraction()
    {
        if (isDead || transformUI.isPaused || transformUI.onInventory || transformUI.isTalking || is2D)
        {
            if (currentTargetInteractable != null)
            {
                transformUI.SetInteractAlert(false);
                currentTargetInteractable = null;
            }
            return;
        }

        Vector3 origin = transform.position + Vector3.up * (controller.height / 2f);

        Collider[] colliders = Physics.OverlapSphere(origin, interactRange, interactMask);

        MonoBehaviour bestEvent = null;
        float maxScore = -1f;

        foreach (Collider col in colliders)
        {
            InteractableEvent interactEvent = null;
            Portal portal = null;

            if (col.CompareTag("InteractionObj"))
            {
                interactEvent = col.GetComponent<InteractableEvent>();
                if (interactEvent == null) interactEvent = col.GetComponentInParent<InteractableEvent>();
            }

            portal = col.GetComponent<Portal>();
            if (portal == null) portal = col.GetComponentInParent<Portal>();

            if (portal != null && portal.portalMode != Portal.PortalMode.Interact)
            {
                portal = null;
            }

            if (interactEvent != null || portal != null)
            {
                Vector3 targetPos = interactEvent != null ? interactEvent.transform.position : portal.transform.position;
                Vector3 dirToTarget = targetPos - transform.position;
                dirToTarget.y = 0f; 

                if (dirToTarget.sqrMagnitude < 0.001f)
                    dirToTarget = transform.forward;

                float angle = Vector3.Angle(transform.forward, dirToTarget.normalized);
                float distance = Vector3.Distance(origin, targetPos);

                if (angle <= 75f)
                {
                    float score = (1f - (angle / 75f)) * 0.5f + (1f - (distance / interactRange)) * 0.5f;
                    if (score > maxScore)
                    {
                        maxScore = score;
                        bestEvent = interactEvent != null ? (MonoBehaviour)interactEvent : (MonoBehaviour)portal;
                    }
                }
            }
        }
        if (bestEvent != null)
        {
            if (currentTargetInteractable != bestEvent)
            {
                currentTargetInteractable = bestEvent;
                transformUI.SetInteractAlert(true);
            }

            if (Input.GetButtonDown("Interaction"))
            {
                transformUI.SetInteractAlert(false);

                if (bestEvent is InteractableEvent evt)
                {
                    evt.StartInteraction();
                }
                else if (bestEvent is Portal p)
                {
                    p.StartInteraction();
                }
            }
        }
        else
        {
            if (currentTargetInteractable != null)
            {
                transformUI.SetInteractAlert(false);
                currentTargetInteractable = null;
            }
        }
    }

    #region 1. 물리적 상태 갱신 (FixedUpdate)

    private void UpdatePhysicalState()
    {
        CheckGroundAndSlope();
        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.fixedDeltaTime;
        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                if (moveVelocity.magnitude > sprintSpeed)
                    moveVelocity = dashDirection * sprintSpeed;
            }
        }
        isSteepSlope = currentSlopeAngle > controller.slopeLimit;
        effectivelyGrounded = isGround && (verticalVelocity <= 0.1f || controller.isGrounded);

        if (effectivelyGrounded)
        {
            canAirDash = true;
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime; 
        }

        if (!wasGrounded && effectivelyGrounded)
        {
            if (AudioManager.Instance != null && landSound != null)
                AudioManager.Instance.PlaySFX(landSound);
        }
        wasGrounded = effectivelyGrounded;
    }

    private void CheckGroundAndSlope()
    {
        float radius = controller.radius * castRadiusMultiplier;
        Vector3 bottomSphereCenter = transform.position + controller.center + Vector3.down * (controller.height / 2f - controller.radius);
        float castDistance = controller.stepOffset + groundCheckOffset;

        if (Physics.SphereCast(bottomSphereCenter, radius, Vector3.down, out RaycastHit hit, castDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            isGround = true;
            groundNormal = hit.normal;
            lastestGround = hit.transform;
            currentSlopeAngle = Vector3.Angle(Vector3.up, groundNormal);

            MovePlatforms mp = hit.transform.GetComponent<MovePlatforms>();
            if (mp != null)
            {
                currentMovePlatform = mp;
                platformVelocity = mp.GetVelocity();

                if (is2D) platformVelocity.z = 0f;
            }
            else
            {
                currentMovePlatform = null;
                platformVelocity = Vector3.zero;
            }
        }
        else
        {
            isGround = false;
            groundNormal = Vector3.up;
            currentSlopeAngle = 0f;
            currentMovePlatform = null;
        }
    }

    #endregion

    #region 2. 입력 및 액션 분리 (Update)

    private void GatherInputs()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = is2D ? 0f : Input.GetAxisRaw("Vertical");
        currentMoveInput = new Vector3(x, 0f, z).normalized;
        wantSprintInput = Input.GetButton("Sprint");

        if (!wantSprintInput)
        {
            sprintLocked = false;
        }

        if (playerState != null && playerState.currentStamina <= 0f)
        {
            sprintLocked = true;
        }
    }

    private void UpdateStamina()
    {
        bool isCurrentlyMoving = currentMoveInput.sqrMagnitude > 0.01f;

        bool canSprint =
            !isDead &&
            !isUsingSkill &&
            !isAttacking &&
            effectivelyGrounded &&
            !isSteepSlope &&
            wantSprintInput &&
            isCurrentlyMoving &&
            !sprintLocked &&
            playerState.currentStamina > 0f;

        if (is2D)
        {
            if (isCurrentlyMoving)
            {
                playerState.currentStamina = Mathf.Max(
                    0f,
                    playerState.currentStamina - STAMINA_2D_RATE * Time.deltaTime);
            }

            if (playerState.currentStamina <= 0f)
            {
                StartCoroutine(DimChangeCor());
                isUsingSkill = true;
            }
        }
        else if (isFlyingMode)
        {
            
        }
        else if (canSprint)
        {
            playerState.currentStamina = Mathf.Max(
                0f,
                playerState.currentStamina - STAMINA_RUN_RATE * Time.deltaTime);

            if (playerState.currentStamina <= 0f)
            {
                playerState.currentStamina = 0f;
                sprintLocked = true;
            }
        }
        else
        {
            if (effectivelyGrounded && !isAttacking && !isDashing)
            {
                playerState.currentStamina = Mathf.Min(
                    playerState.maxStamina,
                    playerState.currentStamina + STAMINA_REGEN_RATE * Time.deltaTime);
            }
        }
    }

    private void HandleDashInput()
    {
        if (sprintLocked)
        {
            if (anim != null) anim.SetBool("isRunning", false);
            return;
        }

        if (Input.GetButtonDown("Sprint") && dashCooldownTimer <= 0f)
        {
            bool canInitiateDash = (effectivelyGrounded && !isSteepSlope) || (!effectivelyGrounded && canAirDash);

            if (canInitiateDash && playerState != null && playerState.currentStamina >= STAMINA_SPRINT_COST)
            {
                playerState.currentStamina = Mathf.Max(0f, playerState.currentStamina - STAMINA_SPRINT_COST);

                if (isAttacking)
                {
                    attackCancelByDash = true;
                }

                if (!effectivelyGrounded)
                    canAirDash = false;

                isDashing = true;

                if (AudioManager.Instance != null && dashSound != null)
                    AudioManager.Instance.PlaySFX(dashSound);

                if (anim != null)
                {
                    anim.ResetTrigger("idle");
                    anim.ResetTrigger("move");
                    anim.SetTrigger("Avoid");
                }

                dashTimer = dashDuration;
                dashCooldownTimer = dashCooldown;

                if (currentMoveInput != Vector3.zero)
                {
                    dashDirection = currentMoveInput;
                    RotateCharacter(currentMoveInput);
                }
                else
                {
                    dashDirection = -transform.forward;
                }
            }
        }
    }

    private void HandleJumpInput()
    {
        if ((effectivelyGrounded || coyoteTimer > 0f) && !isSteepSlope)
        {
            if (Input.GetButtonDown("Jump") && !isDashing)
            {
                RecalculateJumpDynamics();

                verticalVelocity = dynamicJumpVelocity;
                moveVelocity.y = 0f;
                effectivelyGrounded = false;
                isGround = false;
                coyoteTimer = 0f;

                if (AudioManager.Instance != null && jumpSound != null)
                    AudioManager.Instance.PlaySFX(jumpSound);
            }
        }
        else if (!effectivelyGrounded)
        {
            if (Input.GetButtonUp("Jump") && verticalVelocity > 0f && !isDashing)
                verticalVelocity *= 0.5f;
        }
    }

    private void RecalculateJumpDynamics()
    {
        float g_mag = Mathf.Abs(gravity);
        float h = maxJumpHeight;

        if (fallGravityMult > 1f)
        {
            float t_up_old = Mathf.Sqrt((2f * h) / g_mag);
            float t_total = t_up_old * 2f;

            float g_fall = g_mag * fallGravityMult;
            float t_down_new = Mathf.Sqrt((2f * h) / g_fall);

            float t_up_new = t_total - t_down_new;

            float g_up_new = (2f * h) / (t_up_new * t_up_new);
            dynamicUpGravityMult = g_up_new / g_mag;
            dynamicJumpVelocity = g_up_new * t_up_new;
        }
        else
        {
            dynamicUpGravityMult = 1f;
            dynamicJumpVelocity = Mathf.Sqrt(h * 2f * g_mag);
        }
    }

    private void ProcessMovement()
    {
        if (isAttacking && !attackCancelByDash) return;
        bool isCurrentlyMoving = currentMoveInput.sqrMagnitude > 0.01f;
        float currentSpeed = moveSpeed;

        if (anim != null && effectivelyGrounded && !isDashing)
        {
            if (isCurrentlyMoving)
            {
                anim.ResetTrigger("idle");
                anim.SetTrigger("move");
            }
            else
            {
                anim.ResetTrigger("move");
                anim.SetTrigger("idle");
            }
        }

        bool wantSprint = wantSprintInput && !sprintLocked && effectivelyGrounded && !isSteepSlope && isCurrentlyMoving;

        if (isDashing)
            currentSpeed = dashSpeed;
        else if (wantSprint)
            currentSpeed = sprintSpeed;

        if (anim != null)
            anim.SetBool("isRunning", currentSpeed == sprintSpeed || isDashing);

        if (effectivelyGrounded)
        {
            if (isSteepSlope)
            {
                Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;
                Vector3 slideRight = Vector3.Cross(Vector3.up, slideDirection).normalized;
                float controlFactor = Vector3.Dot(currentMoveInput, slideRight);

                if (isDashing)
                {
                    moveVelocity = dashDirection * dashSpeed;
                }
                else
                {
                    Vector3 targetSlideVelocity = (slideDirection * slideSpeed) + (slideRight * controlFactor * slideControlSpeed);
                    moveVelocity = Vector3.Lerp(moveVelocity, targetSlideVelocity, 5f * Time.deltaTime);

                    float dotForward = Vector3.Dot(transform.forward, slideDirection);
                    Vector3 lookDir = dotForward >= 0f ? slideDirection : -slideDirection;
                    RotateCharacter(lookDir);
                }
                verticalVelocity = -2f;
            }
            else
            {
                Vector3 targetDir = isDashing ? dashDirection : currentMoveInput;
                Vector3 targetMove = targetDir * currentSpeed;

                moveVelocity = Vector3.ProjectOnPlane(targetMove, groundNormal);

                if (!isDashing && currentMoveInput != Vector3.zero)
                    RotateCharacter(currentMoveInput);

                verticalVelocity = -2f;
            }
        }
        else
        {
            moveVelocity.y = 0f;

            if (isDashing)
            {
                moveVelocity = dashDirection * currentSpeed;
            }
            else
            {
                Vector3 targetAirMove = currentMoveInput * currentSpeed;
                moveVelocity = Vector3.Lerp(moveVelocity, targetAirMove, airAcceleration * Time.deltaTime);

                if (currentMoveInput != Vector3.zero)
                    RotateCharacter(currentMoveInput);
            }
        }

        if (isDashing && !effectivelyGrounded)
        {
            verticalVelocity = 0f;
        }
        else
        {
            float appliedGravity = gravity;
            if (verticalVelocity > 0f)
            {
                appliedGravity *= dynamicUpGravityMult;
            }
            else if (verticalVelocity < 0f)
            {
                appliedGravity *= fallGravityMult;
            }
            verticalVelocity += appliedGravity * Time.deltaTime;
        }

        Vector3 finalVelocity = moveVelocity + platformVelocity + (Vector3.up * verticalVelocity);
        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void RotateCharacter(Vector3 lookDirection)
    {
        lookDirection.y = 0f;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


    private void UsingSkill()
    {
        if (Input.GetButtonDown("Skill") && !isDashing && !SkillCooling && effectivelyGrounded)
        {
            if (playerState == null || playerState.selectedSkill == 0 || !playerState.skillActive[playerState.selectedSkill - 1]) return;
            if (AudioManager.Instance != null && skillUseSound != null)
                AudioManager.Instance.PlaySFX(skillUseSound);

            SkillCooling = true;
            isUsingSkill = true;

            switch (playerState.selectedSkill)
            {
                case 1: StartCoroutine("DimChangeCor"); break;
                case 2: StartCoroutine("FlyChangeCor"); break;
                case 3: StartCoroutine("TransformCor"); break;
            }
        }
    }

    private void ChangeSkill()
    {
        if (is2D || playerState == null) return;

        if (skillSwitchCooldownTimer > 0f)
        {
            skillSwitchCooldownTimer -= Time.deltaTime;
            return;
        }

        float skillInput = Input.GetAxisRaw("SkillChange");

        if (Mathf.Abs(skillInput) > 0.5f && !isSkillAxisInUse)
        {
            isSkillAxisInUse = true;
            int direction = skillInput > 0f ? 1 : -1;
            SwitchToNextValidSkill(direction);
        }
        else if (Mathf.Abs(skillInput) < 0.1f)
        {
            isSkillAxisInUse = false;
        }
    }

    private void SwitchToNextValidSkill(int direction)
    {
        int originalSkill = playerState.selectedSkill;
        int nextSkill = originalSkill;

        if (nextSkill < 1 || nextSkill > 3)
            nextSkill = direction > 0 ? 0 : 4;

        bool foundValid = false;
        for (int i = 0; i < 3; i++)
        {
            nextSkill = (nextSkill - 1 + direction + 3) % 3 + 1;
            if (playerState.skillActive.Length > (nextSkill - 1) && playerState.skillActive[nextSkill - 1])
            {
                foundValid = true;
                break;
            }
        }

        if (!foundValid) nextSkill = 0;

        if (nextSkill != originalSkill)
        {
            playerState.selectedSkill = nextSkill;
            StartCoroutine(GearUIRotationCor(direction, nextSkill));
        }
    }

    private IEnumerator GearUIRotationCor(int direction, int targetSkill)
    {
        skillSwitchCooldownTimer = 0.2f;
        if (gearUI == null) yield break;

        float duration = 0.1f;
        float elapsed = 0f;
        float targetAngle = direction > 0 ? -360f : 360f;
        Quaternion startRot = gearUI.localRotation;
        bool isIconChanged = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            gearUI.localRotation = startRot * Quaternion.Euler(0f, 0f, targetAngle * t);

            if (t >= 0.5f && !isIconChanged)
            {
                isIconChanged = true;
                if (skillIconUI != null && skillSprites.Length > targetSkill)
                    skillIconUI.sprite = skillSprites[targetSkill];
            }
            yield return null;
        }
        gearUI.localRotation = startRot;
    }

    private void TryAttack()
    {
        if (!Input.GetButton("Attack")) return;
        if (playerState == null || !playerState.attackActive) return;
        if (!effectivelyGrounded) return;
        if (isDashing) return;
        if (isAttacking) return;

        StartCoroutine(AttackCor());
    }

    private void TryUseQuickSlot()
    {
        if (quickSlotCooldownTimer > 0f) return;
        if (Input.GetButtonDown("ItemQuick"))
        {
            if (playerState != null && playerState.quickSlotItemCode != -1)
            {
                quickSlotCooldownTimer = 0.1f;
                
                if (transformUI != null)
                {
                    transformUI.UseQuickSlotItem();
                }
            }
        }
    }
    private IEnumerator AttackCor()
    {
        isAttacking = true;
        attackCancelByDash = false;

        if (anim != null)
        {
            anim.SetBool("IsAttack", true);
            anim.SetTrigger("StateChange");
        }

        float[] durations = { attack1Duration, attack2Duration };

        while (true)
        {
            foreach (float duration in durations)
            {
                float elapsed = 0f;
                bool wentAirborne = false;

                while (elapsed < duration)
                {
                    if (attackCancelByDash) goto BreakLoop;

                    float t = elapsed / duration;
                    float speed = attackMoveCurve.Evaluate(t) * (attackMoveDistance / duration) * 2f;

                    controller.Move((transform.forward * speed + platformVelocity) * Time.deltaTime);

                    if (!effectivelyGrounded) wentAirborne = true;
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (wentAirborne && !effectivelyGrounded) goto BreakLoop;
                if (!Input.GetButton("Attack")) goto BreakLoop;
            }
        }

    BreakLoop:
        EndAttackState();
    }

    private void EndAttackState()
    {
        isAttacking = false;
        attackCancelByDash = false;
        if (anim != null)
        {
            anim.SetBool("IsAttack", false);
            anim.SetTrigger("StateChange");
        }
    }

    private void Die()
    {
        isDead = true;
        isAttacking = false;
        isUsingSkill = false;
        isDashing = false;
        isFlyingMode = false;
        moveVelocity = Vector3.zero;
        verticalVelocity = 0f;
        currentMoveInput = Vector3.zero;

        StopAllCoroutines();

        if (anim != null)
        {
            anim.SetBool("IsAttack", false);
            anim.SetBool("IsDie", true);
            anim.SetTrigger("StateChange");
        }

        StartCoroutine(GameOverRoutine());
    }


    private IEnumerator GameOverRoutine()
    {

        yield return new WaitForSecondsRealtime(2f);
        if (gameOverFadeImage != null)
        {
            gameOverFadeImage.gameObject.SetActive(true);
            Color color = gameOverFadeImage.color;
            color.a = 0f;
            gameOverFadeImage.color = color;

            float fadeDuration = 1.0f;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                gameOverFadeImage.color = color;
                yield return null;
            }

            color.a = 1f;
            gameOverFadeImage.color = color;
        }

        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
    private void UpdateUI()
    {
        if (playerState == null) return;

        if (HPBar != null)
        {
            HPBar.maxValue = playerState.maxHP;
            HPBar.value = playerState.currentHP;
        }
        if (Stamina != null)
        {
            Stamina.maxValue = playerState.maxStamina;
            Stamina.value = playerState.currentStamina;
        }
    }

    private void AnimationRenewal()
    {
        if (anim == null) return;

        bool animGrounded = effectivelyGrounded;
        if (isFlyingMode) animGrounded = false;

        if (anim.GetBool("OnGround") != animGrounded)
            anim.SetTrigger("StateChange");

        anim.SetBool("OnGround", animGrounded);
        anim.SetFloat("YVelocity", verticalVelocity);
    }
    #endregion

    #region 카메라 기본 상태 업데이트 및 스킬 강제 종료

    private void ApplyCameraSettings()
    {
        if (is2D)
        {
            if (quarterViewVirtual != null) quarterViewVirtual.SetActive(false);
            if (sideViewVirtual != null) sideViewVirtual.SetActive(false);
            if (orthoViewVirtual != null) orthoViewVirtual.SetActive(true);
            if (mainCam != null) mainCam.orthographic = true;
        }
        else
        {
            if (orthoViewVirtual != null) orthoViewVirtual.SetActive(false);
            if (mainCam != null) mainCam.orthographic = false;

            if (sideViewed)
            {
                if (quarterViewVirtual != null) quarterViewVirtual.SetActive(false);
                if (sideViewVirtual != null) sideViewVirtual.SetActive(true);
            }
            else
            {
                if (quarterViewVirtual != null) quarterViewVirtual.SetActive(true);
                if (sideViewVirtual != null) sideViewVirtual.SetActive(false);
            }
        }
    }

    [ContextMenu("Force Cancel Skill")]
    public void ForceCancelSkill()
    {
        StopAllCoroutines();

        if (is2D)
        {
            is2D = false;

            float targetZ = transform.position.z;

            float radius = controller.radius * castRadiusMultiplier;
            Vector3 bottomSphereCenter = transform.position + controller.center + Vector3.down * (controller.height / 2f - controller.radius);
            float castDistance = controller.stepOffset + groundCheckOffset;

            bool isRealGroundBeneath = false;
            RaycastHit[] hits = Physics.SphereCastAll(bottomSphereCenter, radius, Vector3.down, castDistance, groundMask, QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (!(hit.collider is MeshCollider)) 
                {
                    isRealGroundBeneath = true;
                    break;
                }
            }

            if (!isRealGroundBeneath && lastestGround != null)
            {
                Platforms p = lastestGround.GetComponent<Platforms>();
                if (p != null)
                {
                    targetZ = p.GetOriginalZDepth();
                }
                else if (lastestGround.gameObject.layer == LayerMask.NameToLayer("Ground") || lastestGround.gameObject.layer == 6)
                {
                    targetZ = lastestGround.position.z;
                }
            }

            if (platforms != null)
            {
                foreach (Platforms p in platforms)
                {
                    if (p != null) p.SwitchTo3D();
                }
            }

            if (controller != null) controller.enabled = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
            if (controller != null) controller.enabled = true;
        }

        isUsingSkill = false;
        SkillCooling = false;
        isFlyingMode = false;
        isDashing = false;
        isAttacking = false;
        attackCancelByDash = false;
        dashTimer = 0f;
        moveVelocity = Vector3.zero;
        verticalVelocity = 0f;
        currentMoveInput = Vector3.zero;

        if (controller != null) controller.enabled = true;
        if (fade != null) fade.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (anim != null)
        {
            anim.SetBool("IsAttack", false);
            anim.SetBool("isRunning", false);
            anim.ResetTrigger("move");
            anim.SetTrigger("idle");
            anim.SetTrigger("StateChange");
        }

        if (visualBoundary != null) Destroy(visualBoundary);

        if (currentTargetObj != null)
        {
            currentTargetObj.transform.position = originalTargetWorldPos;
            currentTargetObj.SetActive(true);
            currentTargetObj = null;
        }

        if (originalPlayerParent != null && transform.parent == mainCam.transform)
            transform.SetParent(originalPlayerParent);

        if (mapManager != null && mapManager.HasSavedState)
        {
            mapManager.RestoreState();
            transform.position = originalPlayerPos;
        }

        CinemachineBrain brain = mainCam.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            brain.enabled = true;
            StartCoroutine(RestoreCameraBlend(brain));
        }

        if (sideViewVirtual != null && oldFollow != null)
        {
            CinemachineVirtualCamera vcam = sideViewVirtual.GetComponent<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                vcam.Follow = oldFollow;
                vcam.LookAt = oldLookAt;
            }
            oldFollow = null;
            oldLookAt = null;
        }

        if (transformUI != null && transformUI.mainPanel != null)
            transformUI.mainPanel.SetActive(false);

        ApplyCameraSettings();
    }

    #endregion

    #region 코루틴

    private IEnumerator RestoreCameraBlend(CinemachineBrain brain)
    {
        CinemachineBlendDefinition oldBlend = brain.m_DefaultBlend;
        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);

        yield return null;

        brain.m_DefaultBlend = oldBlend;
    }

    IEnumerator DimChangeCor()
    {
        bool targetIs2D = !is2D;

        var orthoTransposer = orthoViewVirtual?.GetComponent<CinemachineVirtualCamera>()?.GetCinemachineComponent<CinemachineTransposer>();

        if (targetIs2D && orthoTransposer != null)
            orthoTransposer.m_FollowOffset = new Vector3(0, 5, -20f);

        if (sideViewed)
        {
            if (quarterViewVirtual != null) quarterViewVirtual.SetActive(true);
            if (sideViewVirtual != null) sideViewVirtual.SetActive(false);
            if (orthoViewVirtual != null) orthoViewVirtual.SetActive(false);
            yield return new WaitForSeconds(0.4f);
        }

        if (targetIs2D)
        {
            if (quarterViewVirtual != null) quarterViewVirtual.SetActive(false);
            if (sideViewVirtual != null) sideViewVirtual.SetActive(false);
            if (orthoViewVirtual != null) orthoViewVirtual.SetActive(true);
        }
        else
        {
            if (orthoViewVirtual != null) orthoViewVirtual.SetActive(false);
            if (sideViewed)
            {
                if (quarterViewVirtual != null) quarterViewVirtual.SetActive(false);
                if (sideViewVirtual != null) sideViewVirtual.SetActive(true);
            }
            else
            {
                if (quarterViewVirtual != null) quarterViewVirtual.SetActive(true);
                if (sideViewVirtual != null) sideViewVirtual.SetActive(false);
            }
        }

        is2D = targetIs2D;

        float targetZ = transform.position.z;

        if (targetIs2D)
        {
            if (lastestGround != null && currentSlopeAngle > 0.1f && Mathf.Abs(groundNormal.z) > 0.05f)
            {
                Collider groundCollider = lastestGround.GetComponent<Collider>();
                if (groundCollider != null)
                {
                    float outermostZ = groundCollider.bounds.min.z;
                    targetZ = outermostZ - controller.radius - 0.05f;
                }
            }
        }
        else
        {
            foreach (Platforms p in platforms) p.SwitchTo3D();

            if (lastestGround != null)
            {
                float radius = controller.radius * castRadiusMultiplier;
                Vector3 bottomSphereCenter = transform.position + controller.center + Vector3.down * (controller.height / 2f - controller.radius);
                float castDistance = controller.stepOffset + groundCheckOffset;

                if (!Physics.SphereCast(bottomSphereCenter, radius, Vector3.down, out RaycastHit hit, castDistance, groundMask, QueryTriggerInteraction.Ignore))
                {
                    if (lastestGround.gameObject.layer == 6)
                        transform.position = new Vector3(transform.position.x, transform.position.y, lastestGround.position.z);
                }
            }
        }

        yield return new WaitForSeconds(0.25f);

        if (mainCam != null) mainCam.orthographic = targetIs2D;

        if (targetIs2D && orthoTransposer != null)
            orthoTransposer.m_FollowOffset = new Vector3(0, 5, -250f);
        else if (!targetIs2D && orthoTransposer != null)
            orthoTransposer.m_FollowOffset = new Vector3(0, 5, -20f);

        yield return new WaitForSeconds(0.25f);

        if (targetIs2D)
        {
            controller.enabled = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
            controller.enabled = true;

            foreach (Platforms p in platforms) p.SwitchTo2D(targetZ, controller);
        }

        isUsingSkill = false;
        yield return new WaitForSeconds(0.1f);
        SkillCooling = false;
    }

    IEnumerator FlyChangeCor()
    {
        controller.enabled = false;
        moveVelocity = Vector3.zero;
        verticalVelocity = 0f;

        if (fade != null) fade.SetActive(true);

        originalPlayerPos = transform.position;

        GameObject targetObj = null;
        FlythroughCamera flyCamInfo = null;
        yield return null;
        while (true)
        {
            if (Input.GetButtonDown("Skill") || Input.GetButtonDown("Cancel")) goto FinishCoroutine;

            if (Input.GetButtonDown("Attack"))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide);

                foreach (RaycastHit hit in hits)
                {
                    GameObject candidate = null;
                    if (hit.collider.CompareTag("CameraFreeObj")) candidate = hit.collider.gameObject;
                    else if (hit.collider.transform.parent != null && hit.collider.transform.parent.CompareTag("CameraFreeObj"))
                        candidate = hit.collider.transform.parent.gameObject;

                    if (candidate != null)
                    {
                        targetObj = candidate;
                        currentTargetObj = targetObj;
                        originalTargetWorldPos = targetObj.transform.position;
                        originalTargetParent = targetObj.transform.parent;
                        flyCamInfo = targetObj.GetComponent<FlythroughCamera>();
                        break;
                    }
                }

                if (targetObj != null) break;
            }
            yield return null;
        }

        transform.position = originalTargetWorldPos;
        targetObj.SetActive(false);

        if (quarterViewVirtual != null) quarterViewVirtual.SetActive(false);
        if (orthoViewVirtual != null) orthoViewVirtual.SetActive(false);
        if (sideViewVirtual != null) sideViewVirtual.SetActive(true);

        CinemachineVirtualCamera vcam = sideViewVirtual.GetComponent<CinemachineVirtualCamera>();
        oldFollow = vcam.Follow;
        oldLookAt = vcam.LookAt;
        vcam.Follow = transform;
        vcam.LookAt = transform;

        yield return new WaitForSeconds(0.5f);

        if (fade != null) fade.SetActive(false);

        CinemachineBrain brain = mainCam.GetComponent<CinemachineBrain>();
        if (brain != null) brain.enabled = false;

        transform.SetParent(mainCam.transform, true);

        if (mapManager != null) mapManager.SaveState();

        Transform pivotAnchor = (flyCamInfo != null && flyCamInfo.ownPoint != null) ? flyCamInfo.ownPoint : null;
        float limitDist = (flyCamInfo != null) ? flyCamInfo.limitRange : 30;

        if (pivotAnchor != null)
        {
            visualBoundary = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visualBoundary.name = "FlyBoundaryGuide";
            visualBoundary.transform.SetParent(pivotAnchor, false);
            visualBoundary.transform.localPosition = Vector3.zero;

            float worldScaleX = pivotAnchor.lossyScale.x != 0 ? (limitDist * 2f) / pivotAnchor.lossyScale.x : limitDist * 2f;
            float worldScaleY = pivotAnchor.lossyScale.y != 0 ? (limitDist * 2f) / pivotAnchor.lossyScale.y : limitDist * 2f;
            float worldScaleZ = pivotAnchor.lossyScale.z != 0 ? (limitDist * 2f) / pivotAnchor.lossyScale.z : limitDist * 2f;
            visualBoundary.transform.localScale = new Vector3(worldScaleX, worldScaleY, worldScaleZ);

            Destroy(visualBoundary.GetComponent<Collider>());
            MeshRenderer renderer = visualBoundary.GetComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));
            renderer.material.color = new Color(0f, 0.5f, 1f, 0.1f);
        }

        bool canLand = false;
        Vector3 landingPos = Vector3.zero;
        Vector3 landingNormal = Vector3.up;
        Transform landingTransform = null;
        bool isCanceled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isFlyingMode = true;

        while (true)
        {
            if (Input.GetButtonDown("Skill") || Input.GetButtonDown("Cancel"))
            {
                isCanceled = true;
                break;
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 mapMoveDelta = (mainCam.transform.right * h + mainCam.transform.forward * v) * (moveSpeed * 3f) * Time.deltaTime;

            if (pivotAnchor != null)
            {
                Vector3 nextAnchorWorldPos = pivotAnchor.position - mapMoveDelta;
                float distFromPlayer = Vector3.Distance(transform.position, nextAnchorWorldPos);
                float currDist = Vector3.Distance(transform.position, pivotAnchor.position);

                if (distFromPlayer <= limitDist || distFromPlayer < currDist)
                    map.position -= mapMoveDelta;
            }
            else
            {
                map.position -= mapMoveDelta;
            }

            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * 0.15f;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * 0.15f;

            Vector3 pivotPos = transform.position;
            map.RotateAround(pivotPos, Vector3.up, -mouseX);
            map.RotateAround(pivotPos, mainCam.transform.right, mouseY);

            canLand = false;
            float searchHeight = 1.2f;
            Vector3 searchOrigin = transform.position + Vector3.up * searchHeight;
            float searchDistance = searchHeight + 0.5f;

            if (Physics.SphereCast(searchOrigin, controller.radius, Vector3.down, out RaycastHit snapHit, searchDistance, groundMask, QueryTriggerInteraction.Ignore))
            {
                float slopeAngle = Vector3.Angle(Vector3.up, snapHit.normal);
                float floatFeetY = transform.position.y - (controller.height / 2f);
                float hitRelativeY = snapHit.point.y - floatFeetY;

                if (slopeAngle <= controller.slopeLimit && hitRelativeY < (controller.height * 0.6f))
                {
                    canLand = true;
                    landingPos = snapHit.point;
                    landingNormal = snapHit.normal;
                    landingTransform = snapHit.transform;
                }
            }

            if (Input.GetButtonDown("Attack") && canLand)
            {
                List<Vector3> validNormals = new List<Vector3>();
                float[] offsets = { -0.2f, 0f, 0.2f };

                foreach (float ox in offsets)
                {
                    foreach (float oz in offsets)
                    {
                        Vector3 gridOrigin = transform.position + Vector3.up * searchHeight + mainCam.transform.right * ox + mainCam.transform.forward * oz;
                        if (Physics.SphereCast(gridOrigin, controller.radius, Vector3.down, out RaycastHit hit, searchDistance, groundMask, QueryTriggerInteraction.Ignore))
                        {
                            if (Vector3.Angle(Vector3.up, hit.normal) <= controller.slopeLimit)
                                validNormals.Add(hit.normal);
                        }
                    }
                }

                if (validNormals.Count > 0)
                {
                    Vector3 bestNormal = landingNormal;
                    int maxCount = -1;

                    foreach (Vector3 n in validNormals)
                    {
                        int count = 0;
                        foreach (Vector3 other in validNormals)
                        {
                            if (Vector3.Angle(n, other) < 1.0f) count++;
                        }
                        if (count > maxCount)
                        {
                            maxCount = count;
                            bestNormal = n;
                        }
                    }
                    landingNormal = bestNormal;
                }
                break;
            }
            yield return null;
        }

        isFlyingMode = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        transform.SetParent(originalPlayerParent);
        if (visualBoundary != null) Destroy(visualBoundary);

        CinemachineBlendDefinition oldBlend = new CinemachineBlendDefinition();
        if (brain != null)
        {
            oldBlend = brain.m_DefaultBlend;
            if (isCanceled)
                brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
        }

        if (isCanceled)
        {
            if (mapManager != null) mapManager.RestoreState();
            transform.position = originalPlayerPos;
            targetObj.transform.position = originalTargetWorldPos;
            targetObj.SetActive(true);
        }
        else
        {
            Vector3 localLandingNormal = map.InverseTransformDirection(landingNormal);
            Vector3 localLandingPos = map.InverseTransformPoint(landingPos);
            Vector3 localForward = Vector3.forward;

            if (landingTransform != null)
                localForward = map.InverseTransformDirection(landingTransform.forward);

            Quaternion alignUpRot = Quaternion.FromToRotation(localLandingNormal, Vector3.up);
            Vector3 leveledLocalForward = alignUpRot * localForward;
            leveledLocalForward.y = 0f;

            Quaternion alignForwardRot = Quaternion.identity;
            if (leveledLocalForward.sqrMagnitude > 0.001f)
            {
                alignForwardRot = Quaternion.FromToRotation(leveledLocalForward.normalized, Vector3.forward);
            }
            else
            {
                Vector3 backupForward = alignUpRot * Vector3.forward;
                backupForward.y = 0f;
                if (backupForward.sqrMagnitude > 0.001f)
                    alignForwardRot = Quaternion.FromToRotation(backupForward.normalized, Vector3.forward);
            }

            Quaternion absoluteTargetMapRot = alignForwardRot * alignUpRot;

            Vector3 euler = absoluteTargetMapRot.eulerAngles;
            if (Mathf.Abs(Mathf.DeltaAngle(euler.x, Mathf.Round(euler.x))) < 1.0f) euler.x = Mathf.Round(euler.x);
            if (Mathf.Abs(Mathf.DeltaAngle(euler.y, Mathf.Round(euler.y))) < 1.0f) euler.y = Mathf.Round(euler.y);
            if (Mathf.Abs(Mathf.DeltaAngle(euler.z, Mathf.Round(euler.z))) < 1.0f) euler.z = Mathf.Round(euler.z);
            absoluteTargetMapRot = Quaternion.Euler(euler);

            Vector3 newWorldOffset = absoluteTargetMapRot * localLandingPos;
            Vector3 lastPos = newWorldOffset - landingPos;
            if (mapManager != null)
                mapManager.ApplyState(Vector3.zero, absoluteTargetMapRot);
            else
            {
                map.position = Vector3.zero;
                map.rotation = absoluteTargetMapRot;
            }

            if (landingTransform != null)
                transform.rotation = Quaternion.LookRotation(Vector3.forward);

            transform.position = landingPos + lastPos + Vector3.up * (controller.height / 2f);
            targetObj.transform.position = landingPos + lastPos;
            targetObj.SetActive(true);
        }

        if (brain != null) brain.enabled = true;
        vcam.Follow = oldFollow;
        vcam.LookAt = oldLookAt;

        ApplyCameraSettings();

        if (isCanceled && brain != null)
        {
            yield return null;
            brain.m_DefaultBlend = oldBlend;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

    FinishCoroutine:
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        controller.enabled = true;
        if (visualBoundary != null) Destroy(visualBoundary);
        if (fade != null) fade.SetActive(false);

        isFlyingMode = false;
        currentTargetObj = null;
        oldFollow = null;
        oldLookAt = null;

        isDashing = false;
        dashTimer = 0f;
        moveVelocity = Vector3.zero;
        verticalVelocity = 0f;

        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.ResetTrigger("move");
            anim.SetTrigger("idle");
            anim.SetTrigger("StateChange");
        }

        isUsingSkill = false;
        yield return new WaitForSeconds(0.1f);
        SkillCooling = false;
    }

    IEnumerator TransformCor()
    {
        controller.enabled = false;
        moveVelocity = Vector3.zero;
        verticalVelocity = 0f;

        if (fade != null) fade.SetActive(true);
        ComponentChangingObj targetObj = null;
        yield return null;
        while (true)
        {
            if (Input.GetButtonDown("Skill") || Input.GetButtonDown("Cancel")) goto FinishCoroutine;

            if (Input.GetButtonDown("Attack"))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide);

                foreach (RaycastHit hit in hits)
                {
                    targetObj = hit.collider.GetComponentInParent<ComponentChangingObj>();
                    if (targetObj == null && hit.collider.transform.parent != null)
                        targetObj = hit.collider.transform.parent.GetComponentInParent<ComponentChangingObj>();

                    if (targetObj != null) break;
                }
                if (targetObj != null) break;
            }
            yield return null;
        }

        if (fade != null) fade.SetActive(false);
        if (transformUI != null)
        {
            transformUI.OpenUI(targetObj);
            yield return new WaitUntil(() => !transformUI.mainPanel.activeSelf);
        }

    FinishCoroutine:
        if (fade != null) fade.SetActive(false);
        controller.enabled = true;
        isUsingSkill = false;
        yield return new WaitForSeconds(0.1f);
        SkillCooling = false;
    }

    #endregion
    private void OnDrawGizmosSelected()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            float radius = controller.radius * castRadiusMultiplier;
            Vector3 bottomSphereCenter = transform.position + controller.center + Vector3.down * (controller.height / 2f - controller.radius);
            float castDistance = controller.stepOffset + groundCheckOffset;

            Gizmos.color = isGround ? Color.green : Color.red;
            Gizmos.DrawWireSphere(bottomSphereCenter, radius);

            Vector3 castEndPosition = bottomSphereCenter + Vector3.down * castDistance;
            Gizmos.DrawWireSphere(castEndPosition, radius);
            Gizmos.DrawLine(bottomSphereCenter + Vector3.right * radius, castEndPosition + Vector3.right * radius);
            Gizmos.DrawLine(bottomSphereCenter + Vector3.left * radius, castEndPosition + Vector3.left * radius);
            Gizmos.DrawLine(bottomSphereCenter + Vector3.forward * radius, castEndPosition + Vector3.forward * radius);
            Gizmos.DrawLine(bottomSphereCenter + Vector3.back * radius, castEndPosition + Vector3.back * radius);

            Gizmos.color = Color.yellow;
            Vector3 interactOrigin = transform.position + Vector3.up * (controller.height / 2f);
            Gizmos.DrawWireSphere(interactOrigin, interactRadius);
            Gizmos.DrawLine(interactOrigin, interactOrigin + transform.forward * interactRange);
            Gizmos.DrawWireSphere(interactOrigin + transform.forward * interactRange, interactRadius);
        }
    }
}