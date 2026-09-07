using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class WaypointData
{
    public Transform point;
    [Tooltip("웨이포인트 도달 시 대기 시간")]
    public float waitTime = 0f;
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBasicMove : MonoBehaviour
{
    public enum AIState { Patrol, Chase, Attack, Knockback, Dead, Falling }

    [Header("Core Reference")]
    public Transform playerTarget;
    public Transform mapTransform;
    public EnemyState enemyState;
    public Animator anim;

    [Header("UI & Effect")]
    [Tooltip("플레이어 발견 시 머리 위에 띄울 ! 표식 오브젝트")]
    public GameObject alertMarkUI;

    [Header("Detection & Combat (시야 및 전투)")]
    [Tooltip("적의 시야각 (도 단위, 예: 120도)")]
    [Range(0f, 360f)] public float sightAngle = 120f;
    [Tooltip("시야가 닿는 최대 거리")]
    public float sightRange = 10f;
    [Tooltip("시야를 가리는 장애물 레이어 (벽 등)")]
    public LayerMask obstacleMask;

    [Tooltip("공격을 시도할 거리")]
    public float attackRange = 2f;
    [Tooltip("이 거리 밖으로 나가면 무조건 추적을 포기함")]
    public float loseSightRange = 15f;
    [Tooltip("공격 쿨타임 (초) - 모션이 끝난 뒤부터 이 시간이 지나야 다시 공격 판정")]
    public float attackCooldown = 1f;

    [Header("Standoff Distances (대치 거리)")]
    [Tooltip("플레이어가 이 거리 이하로 가까워지면 이동을 멈춥니다. (0 = 비활성)")]
    public float stayRange = 0f;
    [Tooltip("플레이어가 이 거리 이하로 가까워지면 플레이어를 바라보며 뒤로 후퇴. (0 = 비활성)")]
    public float retreatRange = 0f;

    [Header("Waypoints")]
    [Tooltip("웨이포인트 오브젝트와 대기 시간을 함께 설정합니다.")]
    public WaypointData[] waypoints; 

    private int currentWaypointIdx = 0;
    private float _patrolWaitTimer = 0f;
    private bool _isPatrolWaiting = false;

    [Header("Movement & Gravity")]
    public float moveSpeed = 3f;
    [Tooltip("후퇴 속도 (retreatRange 이하일 때)")]
    public float retreatSpeed = 2f;
    [Tooltip("회전 속도. 이동과 동시에 처리됩니다.")]
    public float rotationSpeed = 15f;
    public Vector3 baseGravityEuler = Vector3.zero;
    public float customGravityForce = 20f;

    [Header("Chase Path Settings")]
    [Tooltip("Chase 중 경로 재계산 주기 (초)")]
    public float chasePathRefreshInterval = 0.15f;
    [Tooltip("Chase 중 플레이어 직선 방향 혼합 비율 (0=경로만, 1=직선만)")]
    [Range(0f, 1f)] public float chaseDirectBlend = 0.4f;

    [Header("Knockback")]
    [Tooltip("넉백 힘 (클수록 멀리 날아감)")]
    public float knockbackForce = 10f;

    [Header("Physics & Ground")]
    [Tooltip("바닥으로 인식할 레이어 (아무것도 안고르면 자동설정)")]
    public LayerMask groundMask;

    [Header("Ground Check")]
    [Tooltip("바닥 체크 SphereCast 반지름")]
    public float groundCheckRadius = 0.3f;
    [Tooltip("바닥 체크 캐스트 시작 높이 오프셋 (중심에서 위로)")]
    public float groundCheckOriginOffset = 0.3f;
    [Tooltip("바닥 체크 캐스트 총 길이")]
    public float groundCheckDistance = 0.6f;

    [Header("2D Mode Settings")]
    [Tooltip("2D 모드: 피격용 하위 오브젝트가 플레이어의 Z로 이동합니다.")]
    public bool is2DMode = false;
    [Tooltip("2D 모드 시 Z축으로 끌려갈 하위 투명 콜라이더 오브젝트")]
    public Transform zSnapObj;

    [Header("Network")]
    [Tooltip("서로 시야/어그로를 공유할 다른 적들")]
    public List<EnemyBasicMove> linkedEnemies = new List<EnemyBasicMove>();

    private Rigidbody rb;
    private NavMeshAgent agent;
    private AIState currentState = AIState.Patrol;

    private float stateTimer = 0f;

    private float lastAttackMotionEndTime = -999f;

    private readonly float attackMotionTime = 0.417f;
    private readonly float knockbackMotionTime = 0.417f;
    private readonly float deadMotionTime = 0.583f;

    private bool cachedIsGrounded = true;
    private bool _gizmoGroundHit;

    private float _chasePathTimer = 0f;
    private float _patrolPathTimer = 0f;
    private const float PATROL_PATH_REFRESH = 0.5f;

    private Collider[] _selfColliders;

    private float invincibilityTimer = 0f;

    private float pivotToGroundOffset = 0f;
    private bool isOffsetCalculated = false;

    private MovePlatforms _currentPlatform;

    private bool _isJumpingLink = false;
    private Vector3 _linkJumpStart = Vector3.zero;
    private Vector3 _linkJumpTarget = Vector3.zero;
    private float _linkJumpTimer = 0f;

    private float _notGroundedTimer = 0f;

    private PlayerControl playerCtrl;
    private CharacterController _playerCC; 

    private float _watchdogTimer = 0f;
    private const float WATCHDOG_INTERVAL = 0.5f;
    private const float WATCHDOG_STUCK_DIST = 0.05f;
    private Vector3 _watchdogLastPos = Vector3.zero;

    private bool _wasPlayerUsingSkill = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        agent = GetComponent<NavMeshAgent>();

        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.updatePosition = false;
        agent.autoTraverseOffMeshLink = false;

        if (mapTransform != null && !transform.IsChildOf(mapTransform))
        {
            transform.SetParent(mapTransform, true);
        }

        if (enemyState == null) enemyState = GetComponent<EnemyState>();
        if (playerTarget != null)
        {
            playerCtrl = playerTarget.GetComponent<PlayerControl>();
            _playerCC = playerTarget.GetComponent<CharacterController>();
        }

        UpdateAgentDestination();
    }
    private bool IsPlayerUsingSkill()
    {
        return _playerCC != null && !_playerCC.enabled;
    }
    void Update()
    {
        if (currentState == AIState.Dead) return;
        if (IsPlayerUsingSkill()) return;

        if (invincibilityTimer > 0f) invincibilityTimer -= Time.deltaTime;

        if (alertMarkUI != null)
            alertMarkUI.SetActive(currentState == AIState.Chase || currentState == AIState.Attack);

        if (enemyState.currentHP <= 0 && currentState != AIState.Dead)
        {
            Die();
            return;
        }
        if (stateTimer > 0f)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                if (currentState == AIState.Attack)
                {
                    lastAttackMotionEndTime = Time.time;
                    currentState = AIState.Chase;
                }
                else if (currentState == AIState.Knockback)
                {
                    ExecuteNavMeshRecovery();
                    currentState = AIState.Chase;
                }
            }
        }
        if (currentState == AIState.Chase)
        {
            _chasePathTimer -= Time.deltaTime;
            if (_chasePathTimer <= 0f)
            {
                _chasePathTimer = chasePathRefreshInterval;
                UpdateAgentDestination();
            }
        }
        else if (currentState == AIState.Patrol && !_isPatrolWaiting)
        {
            _patrolPathTimer -= Time.deltaTime;
            if (_patrolPathTimer <= 0f)
            {
                _patrolPathTimer = PATROL_PATH_REFRESH;
                UpdateAgentDestination();
            }
        }
        if (currentState == AIState.Chase || currentState == AIState.Patrol)
        {
            _watchdogTimer -= Time.deltaTime;
            if (_watchdogTimer <= 0f)
            {
                _watchdogTimer = WATCHDOG_INTERVAL;

                float movedDist = Vector3.Distance(transform.position, _watchdogLastPos);
                bool shouldBeMoving = (currentState == AIState.Chase && playerTarget != null)
                                   || (currentState == AIState.Patrol && waypoints.Length > 0 && !_isPatrolWaiting);

                bool farFromDestination = false;
                if (currentState == AIState.Chase)
                {
                    farFromDestination = Vector3.Distance(transform.position, playerTarget.position) > attackRange + 0.5f;
                }
                else if (waypoints.Length > 0 && waypoints[currentWaypointIdx].point != null)
                {
                    farFromDestination = Vector3.Distance(transform.position, waypoints[currentWaypointIdx].point.position) > 1.5f;
                }

                if (shouldBeMoving && farFromDestination && movedDist < WATCHDOG_STUCK_DIST)
                {
                    ExecuteNavMeshRecovery();
                }

                _watchdogLastPos = transform.position;
            }
        }
        else
        {
            _watchdogLastPos = transform.position;
            _watchdogTimer = WATCHDOG_INTERVAL;
        }

        CheckPlayerDetection();
        ProcessStateMachine();
    }
    void FixedUpdate()
    {
        if (currentState == AIState.Dead) return;
        bool isUsingSkill = IsPlayerUsingSkill();
        if (isUsingSkill)
        {
            if (!_wasPlayerUsingSkill)
            {
                _wasPlayerUsingSkill = true;
                rb.isKinematic = true;            
                rb.velocity = Vector3.zero;       
                if (anim != null) anim.speed = 0f;
            }
            return; 
        }
        else if (_wasPlayerUsingSkill)
        {
            _wasPlayerUsingSkill = false;
            if (anim != null) anim.speed = 1f;

            if (currentState != AIState.Knockback)
                ExecuteNavMeshRecovery();
            else
                rb.isKinematic = false; 
        }

        bool currentGrounded = IsGrounded();
        if (!currentGrounded && !_isJumpingLink)
            _notGroundedTimer += Time.fixedDeltaTime;
        else
            _notGroundedTimer = 0f;

        cachedIsGrounded = _notGroundedTimer < 0.2f;

        if (agent != null && agent.enabled && agent.isOnOffMeshLink && !_isJumpingLink)
        {
            OffMeshLinkData linkData = agent.currentOffMeshLinkData;
            if (linkData.valid)
            {
                _isJumpingLink = true;
                _linkJumpStart = rb.position;
                _linkJumpTarget = linkData.endPos;
                _linkJumpTimer = 0f;
            }
            else
            {
                agent.ResetPath();
            }
        }

        if (currentState == AIState.Knockback || (!cachedIsGrounded && !_isJumpingLink))
        {
            if (agent != null && agent.enabled)
                agent.enabled = false;

            rb.isKinematic = false;
            ApplyCustomGravity();
            return;
        }

        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(transform.position);
                UpdateAgentDestination();
            }
        }

        if (agent != null && agent.enabled && !_isJumpingLink)
        {
            if (!agent.isOnNavMesh)
            {
                agent.Warp(transform.position);
            }
            else
            {
                agent.nextPosition = rb.position;
            }
        }

        MoveAlongPath();
    }

    void LateUpdate()
    {
        if (IsPlayerUsingSkill()) return;

        if (playerCtrl == null && playerTarget != null)
        {
            playerCtrl = playerTarget.GetComponent<PlayerControl>();
        }

        if (playerCtrl != null)
        {
            is2DMode = playerCtrl.is2D;
        }

        if (zSnapObj == null) return;

        if (is2DMode && playerTarget != null)
        {
            Vector3 targetPos = transform.position;
            targetPos.z = playerTarget.position.z;
            zSnapObj.position = Vector3.Lerp(zSnapObj.position, targetPos, 20f * Time.deltaTime);
        }
        else
        {
            zSnapObj.localPosition = Vector3.Lerp(zSnapObj.localPosition, Vector3.zero, 20f * Time.deltaTime);
        }
    }

    bool IsGrounded()
    {
        if (_selfColliders == null)
            _selfColliders = GetComponentsInChildren<Collider>(true);

        Vector3 upDir = -(mapTransform.rotation * (Quaternion.Euler(baseGravityEuler) * Vector3.down));
        Vector3 downDir = -upDir;
        Vector3 origin = transform.position + upDir * groundCheckOriginOffset;
        LayerMask mask = groundMask.value == 0 ? Physics.AllLayers : groundMask;

        _gizmoGroundHit = false;
        _currentPlatform = null;

        float actualCheckDist = Mathf.Max(groundCheckDistance, GetPivotOffset() + 0.2f);

        RaycastHit[] hits = Physics.SphereCastAll(
            origin, groundCheckRadius, downDir, actualCheckDist, mask, QueryTriggerInteraction.Ignore);

        foreach (var hit in hits)
        {
            bool isSelf = false;
            foreach (var sc in _selfColliders)
                if (sc == hit.collider) { isSelf = true; break; }
            if (isSelf) continue;

            _gizmoGroundHit = true;

            if (!isOffsetCalculated)
            {
                pivotToGroundOffset = Vector3.Project(transform.position - hit.point, upDir).magnitude;
                isOffsetCalculated = true;
            }

            _currentPlatform = hit.transform.GetComponent<MovePlatforms>();

            if (mapTransform != null && transform.parent != mapTransform)
            {
                transform.SetParent(mapTransform, true);
            }

            return true;
        }
        return false;
    }

    private float GetPivotOffset()
    {
        return agent != null ? agent.baseOffset : 1.0f;
    }

    void ApplyCustomGravity()
    {
        Vector3 dir = mapTransform.rotation * (Quaternion.Euler(baseGravityEuler) * Vector3.down);
        rb.AddForce(dir * customGravityForce, ForceMode.Acceleration);
    }
    void ProcessStateMachine()
    {
        float dist = playerTarget != null ? Vector3.Distance(transform.position, playerTarget.position) : 999f;

        switch (currentState)
        {
            case AIState.Patrol:
                if (waypoints.Length > 0 && waypoints[currentWaypointIdx].point != null)
                {
                    if (_isPatrolWaiting)
                    {
                        _patrolWaitTimer -= Time.deltaTime;
                        if (_patrolWaitTimer <= 0f)
                        {
                            _isPatrolWaiting = false;
                            currentWaypointIdx = (currentWaypointIdx + 1) % waypoints.Length;
                            UpdateAgentDestination();
                        }
                    }
                    else
                    {
                        Vector3 wp = mapTransform.TransformPoint(
                            mapTransform.InverseTransformPoint(waypoints[currentWaypointIdx].point.position));

                        Vector3 upDir = -(mapTransform.rotation * (Quaternion.Euler(baseGravityEuler) * Vector3.down));
                        float flatDist = Vector3.ProjectOnPlane(wp - transform.position, upDir).magnitude;

                        if (flatDist < 1.0f)
                        {
                            float currentWaitTime = waypoints[currentWaypointIdx].waitTime;

                            if (currentWaitTime > 0f)
                            {
                                _isPatrolWaiting = true;
                                _patrolWaitTimer = currentWaitTime;
                                if (agent != null && agent.enabled) agent.ResetPath(); // 정지
                            }
                            else
                            {
                                currentWaypointIdx = (currentWaypointIdx + 1) % waypoints.Length;
                                UpdateAgentDestination();
                            }
                        }
                    }
                }
                break;

            case AIState.Chase:
                if (dist > loseSightRange)
                {
                    currentState = AIState.Patrol;
                    _isPatrolWaiting = false;
                }
                else if (dist <= attackRange)
                {
                    if (Time.time >= lastAttackMotionEndTime + attackCooldown)
                        PerformAttack();
                }
                break;
        }
    }

    void CheckPlayerDetection()
    {
        if (playerTarget == null || currentState == AIState.Chase) return;

        Vector3 dirToPlayer = playerTarget.position - transform.position;
        float dist = dirToPlayer.magnitude;

        if (dist <= sightRange)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer) <= sightAngle / 2f)
            {
                Vector3 rayOrigin = transform.position + transform.up * 1f;
                Vector3 rayTarget = playerTarget.position + playerTarget.up * 1f;

                if (!Physics.Raycast(rayOrigin, rayTarget - rayOrigin, dist, obstacleMask))
                    NotifyFoundPlayer();
            }
        }
    }

    public void NotifyFoundPlayer()
    {
        if (currentState == AIState.Dead || currentState == AIState.Knockback || currentState == AIState.Attack) return;

        if (currentState != AIState.Chase)
        {
            currentState = AIState.Chase;
            _isPatrolWaiting = false;
            _chasePathTimer = 0f;
            UpdateAgentDestination();

            foreach (var enemy in linkedEnemies)
                if (enemy != null) enemy.NotifyFoundPlayer();
        }
    }

    void PerformAttack()
    {
        currentState = AIState.Attack;
        stateTimer = attackMotionTime;

        if (anim != null)
        {
            anim.SetBool("IsAttack", true);
            Invoke(nameof(ResetAttackAnim), attackMotionTime);
        }
    }

    void ResetAttackAnim()
    {
        if (anim != null) anim.SetBool("IsAttack", false);
    }

    void Die()
    {
        currentState = AIState.Dead;

        rb.isKinematic = false;
        Vector3 platformVel = _currentPlatform != null ? _currentPlatform.GetVelocity() : Vector3.zero;
        rb.velocity = platformVel;

        if (agent != null) agent.enabled = false;

        if (anim != null) anim.SetBool("IsDie", true);
        Destroy(gameObject, 5f + deadMotionTime);
    }

    private void ExecuteNavMeshRecovery()
    {
        _isJumpingLink = false;
        _isPatrolWaiting = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
        }

        _chasePathTimer = 0f;
        UpdateAgentDestination();
    }

    public void TeleportRecovery()
    {
        if (currentState == AIState.Dead) return;
        StartCoroutine(DelayedNavMeshRecoveryCor());
    }

    private IEnumerator DelayedNavMeshRecoveryCor()
    {
        yield return null;
        yield return null;

        if (currentState == AIState.Dead) yield break;

        if (currentState == AIState.Knockback)
            stateTimer = 0f;

        if (currentState == AIState.Attack)
        {
            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(transform.position);
            }
            yield break;
        }

        ExecuteNavMeshRecovery();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState == AIState.Dead || invincibilityTimer > 0f) return;
        if (IsPlayerUsingSkill()) return;

        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerAttack")) return;

        PlayerAttack pa = other.GetComponent<PlayerAttack>();
        if (pa == null) return;

        invincibilityTimer = 0.1f;
        enemyState.TakeDamage((int)pa.Damage, pa.hitEffectPrefab);
        NotifyFoundPlayer();

        if (enemyState.currentHP <= 0) { Die(); return; }
        if (currentState == AIState.Attack) return;

        currentState = AIState.Knockback;
        stateTimer = knockbackMotionTime;
        _isJumpingLink = false;

        if (anim != null) anim.SetTrigger("knock");

        if (agent != null && agent.enabled) agent.enabled = false;

        rb.isKinematic = false;

        Vector3 platformVel = _currentPlatform != null ? _currentPlatform.GetVelocity() : Vector3.zero;
        rb.velocity = platformVel;

        Vector3 upDir = -(mapTransform.rotation * (Quaternion.Euler(baseGravityEuler) * Vector3.down));
        Vector3 knockDir = Vector3.ProjectOnPlane(transform.position - playerTarget.position, upDir).normalized;
        rb.AddForce(knockDir * knockbackForce, ForceMode.Impulse);
    }

    void UpdateAgentDestination()
    {
        if (currentState == AIState.Dead || currentState == AIState.Knockback || agent == null || !agent.enabled) return;
        if (currentState == AIState.Patrol && _isPatrolWaiting) return;

        if (!agent.isOnNavMesh)
        {
            agent.Warp(transform.position);
        }

        if (!agent.isOnNavMesh) return;

        Vector3 targetWorld = transform.position;
        if (currentState == AIState.Chase && playerTarget != null)
            targetWorld = playerTarget.position;
        else if (currentState == AIState.Patrol && waypoints.Length > 0 && waypoints[currentWaypointIdx].point != null)
            targetWorld = waypoints[currentWaypointIdx].point.position;

        agent.SetDestination(targetWorld);
    }
    private Vector3 ClampToSurface(Vector3 targetPos, Vector3 upDir)
    {
        float offset = GetPivotOffset();
        Vector3 rayOrigin = targetPos + upDir * (offset + 1.0f);

        if (Physics.Raycast(rayOrigin, -upDir, out RaycastHit hit, offset + 3.0f, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point + upDir * offset;
        }
        return targetPos;
    }

    void MoveAlongPath()
    {
        if (playerTarget == null) return;

        Vector3 upDir = -(mapTransform.rotation * (Quaternion.Euler(baseGravityEuler) * Vector3.down));
        Vector3 platformOffset = _currentPlatform != null ? _currentPlatform.GetVelocity() * Time.fixedDeltaTime : Vector3.zero;

        if (currentState == AIState.Attack)
        {
            if (platformOffset != Vector3.zero)
            {
                Vector3 attackTargetPos = rb.position + platformOffset;
                attackTargetPos = ClampToSurface(attackTargetPos, upDir);
                rb.MovePosition(attackTargetPos);
            }
            if (anim != null) anim.SetBool("Move", false);
            return;
        }

        if (_isJumpingLink)
        {
            _linkJumpTimer += Time.fixedDeltaTime;

            float totalDist = Vector3.Distance(_linkJumpStart, _linkJumpTarget);
            float expectedDuration = Mathf.Max(0.1f, totalDist / moveSpeed);
            float jumpProgress = Mathf.Clamp01(_linkJumpTimer / expectedDuration);

            Vector3 nextJumpPos = Vector3.Lerp(_linkJumpStart, _linkJumpTarget, jumpProgress);

            nextJumpPos = ClampToSurface(nextJumpPos, upDir);
            rb.MovePosition(nextJumpPos);

            Vector3 flatLinkDir = Vector3.ProjectOnPlane(_linkJumpTarget - _linkJumpStart, upDir).normalized;
            if (flatLinkDir.sqrMagnitude > 0.001f)
            {
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(flatLinkDir, upDir), rotationSpeed * Time.fixedDeltaTime));
            }

            if (jumpProgress >= 1.0f || _linkJumpTimer > 2.0f)
            {
                _isJumpingLink = false;
                _linkJumpTimer = 0f;

                if (agent != null && agent.enabled && agent.isOnNavMesh)
                {
                    agent.CompleteOffMeshLink();
                    agent.Warp(transform.position);
                }
            }
            if (anim != null) anim.SetBool("Move", true);
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        Vector3 finalDir = Vector3.zero;
        Vector3 lookDir = Vector3.zero;
        Vector3 pathDir = Vector3.zero;

        if (retreatRange > 0f && distToPlayer <= retreatRange)
        {
            Vector3 toPlayerFlat = Vector3.ProjectOnPlane(playerTarget.position - transform.position, upDir).normalized;
            finalDir = -toPlayerFlat;
            lookDir = toPlayerFlat;
        }
        else if (stayRange > 0f && distToPlayer <= stayRange)
        {
            Vector3 toPlayerFlat = Vector3.ProjectOnPlane(playerTarget.position - transform.position, upDir).normalized;
            finalDir = Vector3.zero;
            lookDir = toPlayerFlat;
        }
        else if (agent != null && (agent.hasPath || agent.pathPending))
        {
            Vector3 steeringTarget = agent.steeringTarget;
            pathDir = Vector3.ProjectOnPlane(steeringTarget - transform.position, upDir).normalized;
            finalDir = pathDir;

            if (currentState == AIState.Chase && chaseDirectBlend > 0f)
            {
                Vector3 directDir = Vector3.ProjectOnPlane(playerTarget.position - transform.position, upDir).normalized;
                if (Vector3.Angle(pathDir, directDir) < 45f)
                {
                    finalDir = Vector3.Lerp(pathDir, directDir, chaseDirectBlend).normalized;
                }
            }

            finalDir = (finalDir + GetSeparationVector()).normalized;

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                finalDir = Vector3.zero;
            }
            lookDir = finalDir;
        }

        // rb.MovePosition 
        if (finalDir != Vector3.zero || platformOffset != Vector3.zero)
        {
            Vector3 nextPos = rb.position + finalDir * moveSpeed * Time.fixedDeltaTime + platformOffset;

            if (finalDir != Vector3.zero && !Physics.Raycast(nextPos + upDir * 1.0f, -upDir, 3.0f, groundMask, QueryTriggerInteraction.Ignore))
            {
                if (pathDir != Vector3.zero)
                {
                    nextPos = rb.position + pathDir * moveSpeed * Time.fixedDeltaTime + platformOffset;
                }

                if (pathDir == Vector3.zero || !Physics.Raycast(nextPos + upDir * 1.0f, -upDir, 3.0f, groundMask, QueryTriggerInteraction.Ignore))
                {
                    nextPos = rb.position + platformOffset;
                    if (platformOffset != Vector3.zero && !Physics.Raycast(nextPos + upDir * 1.0f, -upDir, 3.0f, groundMask, QueryTriggerInteraction.Ignore))
                    {
                        finalDir = Vector3.zero;
                    }
                }
            }

            if (finalDir != Vector3.zero || platformOffset != Vector3.zero)
            {
                nextPos = ClampToSurface(nextPos, upDir);
                rb.MovePosition(nextPos);
            }
        }

        if (lookDir.sqrMagnitude > 0.001f)
        {
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(lookDir, upDir), rotationSpeed * Time.fixedDeltaTime));
        }

        if (anim != null)
        {
            anim.SetBool("Move", finalDir.sqrMagnitude > 0.001f);
        }
    }

    Vector3 GetSeparationVector()
    {
        Vector3 separation = Vector3.zero;
        Vector3 upDir = -(mapTransform.rotation * (Quaternion.Euler(baseGravityEuler) * Vector3.down));

        foreach (var hit in Physics.OverlapSphere(transform.position, 1.5f))
        {
            if (hit.gameObject != gameObject && hit.CompareTag("Enemy"))
            {
                Vector3 repel = Vector3.ProjectOnPlane(transform.position - hit.transform.position, upDir);
                separation += repel.normalized / Mathf.Max(repel.magnitude, 0.1f);
            }
        }
        return separation.normalized * 0.6f;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 upDir = mapTransform != null
            ? -(mapTransform.rotation * (Quaternion.Euler(baseGravityEuler) * Vector3.down))
            : transform.up;

        Gizmos.color = Color.yellow;
        Vector3 fwd = transform.forward;
        Vector3 left = Quaternion.AngleAxis(-sightAngle / 2f, upDir) * fwd;
        Vector3 right = Quaternion.AngleAxis(sightAngle / 2f, upDir) * fwd;
        Gizmos.DrawRay(transform.position, left * sightRange);
        Gizmos.DrawRay(transform.position, right * sightRange);
        Vector3 prev = left;
        for (int i = 1; i <= 20; i++)
        {
            Vector3 next = Quaternion.AngleAxis(-sightAngle / 2f + (sightAngle / 20f) * i, upDir) * fwd;
            Gizmos.DrawLine(transform.position + prev * sightRange, transform.position + next * sightRange);
            prev = next;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseSightRange);

        if (stayRange > 0f)
        {
            Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.9f);
            Gizmos.DrawWireSphere(transform.position, stayRange);
        }

        if (retreatRange > 0f)
        {
            Gizmos.color = new Color(0.8f, 0.2f, 1f, 0.9f);
            Gizmos.DrawWireSphere(transform.position, retreatRange);
        }

        Vector3 downDir = -upDir;
        Vector3 gizmoOrigin = transform.position + upDir * groundCheckOriginOffset;
        Vector3 gizmoEnd = gizmoOrigin + downDir * groundCheckDistance;

#if UNITY_EDITOR
        bool grounded = Application.isPlaying ? _gizmoGroundHit : false;
#else
        bool grounded = false;
#endif

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(gizmoOrigin, groundCheckRadius);

        Gizmos.color = grounded ? Color.green : Color.magenta;
        Gizmos.DrawWireSphere(gizmoEnd, groundCheckRadius);

        Gizmos.color = grounded ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 1f, 0.5f);
        Gizmos.DrawLine(gizmoOrigin, gizmoEnd);
    }
}