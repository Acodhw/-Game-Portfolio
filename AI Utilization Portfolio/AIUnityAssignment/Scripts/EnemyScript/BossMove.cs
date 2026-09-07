using System.Collections;
using UnityEngine;

/// <summary>
/// 플라이스루 맵 회전에 대응하며 4개의 페이즈 패턴을 수행하는 보스 이동 및 공격 스크립트입니다.
/// </summary>
public class BossMove : MonoBehaviour
{
    [Header("Core References")]
    public Transform playerTarget;
    public Transform mapTransform;
    public Animator anim;

    [Tooltip("보스 중지.")]
    public bool isCompletelyStopped = false;

    [Header("Audio Settings")]
    [Tooltip("1번, 3번 페이즈에서 일반 웨이포인트 이동 시 재생할")]
    public AudioClip normalMoveSfx;
    [Tooltip("페이즈 전환 시 데미지 연출과 함께 재생할 소리")]
    public AudioClip hitMoveSfx;

    [Header("Projectile Settings")]
    public EnemyShots projectilePrefab;
    [Tooltip("투사체 발사 시 보스 중심에서 떨어질 오프셋 거리")]
    public float fireOffset = 1.5f;

    [Header("2D Mode Settings")]
    [Tooltip("2D 모드 시 스냅할 보스의 캡슐 콜라이더")]
    public CapsuleCollider bossCollider;
    public float snapSpeed = 20f;

    [Header("Phase 1: Floor (100%~75%)")]
    [Tooltip("십자가 맵의 튀어나온 4개 지점 웨이포인트. 0과 2, 1과 3이 마주보도록 셋팅.")]
    public Transform[] phase1Waypoints;
    public float phase1DashSpeed = 20f;
    public float phase1MoveSpeed = 5f;

    [Header("Phase 2: Left Wall (75%~50%)")]
    [Tooltip("순차적으로 이동할 3개의 플랫폼 웨이포인트")]
    public Transform[] phase2Waypoints;
    public float phase2MoveSpeed = 8f;

    [Header("Phase 3: Ceiling (50%~25%)")]
    [Tooltip("가로 방향 4개의 포인트")]
    public Transform[] phase3Waypoints;
    public float phase3MoveSpeed = 8f;

    [Header("Phase 4: Right Wall (25%~0%)")]
    [Tooltip("8자 모양 맵의 중앙 머무는 지점")]
    public Transform phase4CenterPoint;
    public float phase4MoveSpeed = 5f;

    private int currentPhase = 0;
    private Coroutine currentPatternCoroutine;
    private Coroutine transitionCoroutine;
    private Coroutine phase2DropCoroutine;

    private bool isDashing = false;
    private PlayerControl playerCtrl;
    private Vector3 originalColliderCenter;

    private Vector3 currentPhaseUpDir = Vector3.up;
    private Vector3 targetPhaseUpDir = Vector3.up;
    private Vector3 fromPhaseUpDir = Vector3.up;
    private float phaseUpBlendT = 1f;
    private float phaseUpBlendDur = 5f; 


    private void Start()
    {
        if (mapTransform != null && !transform.IsChildOf(mapTransform))
            transform.SetParent(mapTransform, true);

        if (playerTarget != null)
            playerCtrl = playerTarget.GetComponent<PlayerControl>();

        if (bossCollider == null) bossCollider = GetComponent<CapsuleCollider>();
        if (bossCollider != null) originalColliderCenter = bossCollider.center;

        currentPhase = -1;
        ChangePhase(0);
    }

    private void Update()
    {
        if (isCompletelyStopped) return;

        if (phaseUpBlendT < 1f)
        {
            phaseUpBlendT = Mathf.Clamp01(phaseUpBlendT + Time.deltaTime / phaseUpBlendDur);
            currentPhaseUpDir = Vector3.Slerp(fromPhaseUpDir, targetPhaseUpDir, phaseUpBlendT).normalized;
        }

        if (!isDashing && playerTarget != null)
        {
            Vector3 toPlayerGlobal = playerTarget.position - transform.position;
            Vector3 toPlayerLocal = mapTransform.InverseTransformDirection(toPlayerGlobal);

            Vector3 flatLookDirLocal = Vector3.ProjectOnPlane(toPlayerLocal, currentPhaseUpDir).normalized;

            if (flatLookDirLocal.sqrMagnitude > 0.001f)
            {
                Quaternion targetLocalRot = Quaternion.LookRotation(flatLookDirLocal, currentPhaseUpDir);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLocalRot, 10f * Time.deltaTime);
            }
        }
    }

    private void LateUpdate()
    {
        if (isCompletelyStopped || bossCollider == null) return;

        if (playerCtrl != null && playerCtrl.is2D && playerTarget != null)
        {
            Vector3 targetGlobalPos = transform.position;
            targetGlobalPos.z = playerTarget.position.z;

            Vector3 targetLocalCenter = transform.InverseTransformPoint(targetGlobalPos);
            bossCollider.center = Vector3.Lerp(bossCollider.center, targetLocalCenter, snapSpeed * Time.deltaTime);
        }
        else
        {
            bossCollider.center = Vector3.Lerp(bossCollider.center, originalColliderCenter, snapSpeed * Time.deltaTime);
        }
    }

    public void ChangePhase(int newPhase)
    {
        if (isCompletelyStopped || currentPhase == newPhase) return;
        StopAllPhaseCoroutines();

        isDashing = false;

        if (currentPhase == -1)
        {
            currentPhase = newPhase;
            currentPhaseUpDir = GetPhaseUpDir(newPhase);
            targetPhaseUpDir = currentPhaseUpDir;
            fromPhaseUpDir = currentPhaseUpDir;
            phaseUpBlendT = 1f;
            StartPhasePattern();
        }
        else
        {
            currentPhase = newPhase;
            transitionCoroutine = StartCoroutine(PhaseTransitionRoutine());
        }
    }

    private void StopAllPhaseCoroutines()
    {
        if (currentPatternCoroutine != null) { StopCoroutine(currentPatternCoroutine); currentPatternCoroutine = null; }
        if (transitionCoroutine != null) { StopCoroutine(transitionCoroutine); transitionCoroutine = null; }
        if (phase2DropCoroutine != null) { StopCoroutine(phase2DropCoroutine); phase2DropCoroutine = null; }
    }

    private IEnumerator PhaseTransitionRoutine()
    {
        if (anim != null) anim.SetTrigger("Hit");

        if (AudioManager.Instance != null && hitMoveSfx != null)
            AudioManager.Instance.PlaySFX(hitMoveSfx, false);

        fromPhaseUpDir = currentPhaseUpDir;
        targetPhaseUpDir = GetPhaseUpDir(currentPhase);
        phaseUpBlendT = 0f;

        yield return new WaitForSeconds(1f);

        StartPhasePattern();
    }

    private Vector3 GetPhaseUpDir(int phase)
    {
        switch (phase)
        {
            case 0: return Vector3.up;  
            case 1: return Vector3.left; 
            case 2: return Vector3.down; 
            case 3: return Vector3.right;
            default: return Vector3.up;
        }
    }

    private void StartPhasePattern()
    {
        switch (currentPhase)
        {
            case 0: currentPatternCoroutine = StartCoroutine(Phase1Routine()); break;
            case 1: currentPatternCoroutine = StartCoroutine(Phase2Routine()); break;
            case 2: currentPatternCoroutine = StartCoroutine(Phase3Routine()); break;
            case 3: currentPatternCoroutine = StartCoroutine(Phase4Routine()); break;
        }
    }

    public void StopAllActionsForTimeline()
    {
        isCompletelyStopped = true;
        StopAllPhaseCoroutines();
        StopAllCoroutines();
        if (anim != null) anim.speed = 0;
    }

    private IEnumerator Phase1Routine()
    {
        while (!isCompletelyStopped)
        {
            if (phase1Waypoints.Length < 4) yield break;

            PlayMoveSfx();

            int randIndex = Random.Range(0, phase1Waypoints.Length);
            yield return StartCoroutine(MoveToPoint(phase1Waypoints[randIndex], phase1MoveSpeed));

            yield return new WaitForSeconds(0.5f);

            if (Random.value > 0.5f)
            {

                int oppIndex = (randIndex + 2) % phase1Waypoints.Length;

                if (anim != null) anim.SetTrigger("Dash");
                yield return new WaitForSeconds(0.5f);

                isDashing = true;
                yield return StartCoroutine(MoveToPoint(phase1Waypoints[oppIndex], phase1DashSpeed));
                isDashing = false;
            }
            else
            {
                if (anim != null) anim.SetTrigger("Magic");
                yield return new WaitForSeconds(0.5f);

                Vector3 toPlayer = (playerTarget.position - transform.position).normalized;
                FireFanProjectiles(toPlayer, 3, 15f);
            }

            yield return new WaitForSeconds(3f);
        }
    }

    private IEnumerator Phase2Routine()
    {
        phase2DropCoroutine = StartCoroutine(Phase2DropProjectilesRoutine());

        int wpIndex = 0;
        while (!isCompletelyStopped)
        {
            if (phase2Waypoints.Length == 0) yield break;

            yield return StartCoroutine(MoveToPoint(phase2Waypoints[wpIndex], phase2MoveSpeed));
            yield return new WaitForSeconds(2f);

            wpIndex = (wpIndex + 1) % phase2Waypoints.Length;
        }
        if (phase2DropCoroutine != null)
        {
            StopCoroutine(phase2DropCoroutine);
            phase2DropCoroutine = null;
        }
    }

    private IEnumerator Phase2DropProjectilesRoutine()
    {
        while (!isCompletelyStopped)
        {
            yield return new WaitForSeconds(2.0f);

            if (anim != null) anim.SetTrigger("Magic");
            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < 15; i++)
            {
                Vector3 dropOrigin = playerTarget.position + (Vector3.up * 15f);
                dropOrigin += Vector3.right * Random.Range(-20f, 20f)
                            + Vector3.forward * Random.Range(-20f, 20f);

                FireSingleProjectile(dropOrigin, Vector3.down, 10f);
            }
        }
    }

    private IEnumerator Phase3Routine()
    {
        while (!isCompletelyStopped)
        {
            if (phase3Waypoints.Length == 0) yield break;

            PlayMoveSfx();

            int randIndex = Random.Range(0, phase3Waypoints.Length);
            yield return StartCoroutine(MoveToPoint(phase3Waypoints[randIndex], phase3MoveSpeed));

            yield return new WaitForSeconds(0.5f);

            if (anim != null) anim.SetTrigger("Magic");
            yield return new WaitForSeconds(0.5f);

            Vector3 worldPhaseUp = mapTransform.rotation * currentPhaseUpDir;

            for (int i = 0; i < 25; i++)
            {
                Vector3 frontDir = transform.forward;
                Vector3 randomFrontDir = Quaternion.AngleAxis(Random.Range(-60f, 60f), worldPhaseUp) * frontDir;
                Vector3 sideAxis = Vector3.Cross(randomFrontDir, worldPhaseUp).normalized;
                randomFrontDir = Quaternion.AngleAxis(Random.Range(-20f, 20f), sideAxis) * randomFrontDir;

                FireSingleProjectile(transform.position + randomFrontDir * fireOffset, randomFrontDir, 12f);

                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(3f);
        }
    }

    private IEnumerator Phase4Routine()
    {
        if (phase4CenterPoint != null)
            yield return StartCoroutine(MoveToPoint(phase4CenterPoint, phase4MoveSpeed));

        while (!isCompletelyStopped)
        {
            yield return new WaitForSeconds(2f);

            int pattern = Random.Range(0, 3);

            if (pattern == 0)
            {
                if (anim != null) anim.SetTrigger("Magic");
                yield return new WaitForSeconds(0.3f);

                float radius = 30f;
                int bulletCount = 30;
                for (int i = 0; i < bulletCount; i++)
                {
                    float angle = (360f / bulletCount) * i * Mathf.Deg2Rad;
                    Vector3 spawnPos = transform.position
                                     + (mapTransform.up * Mathf.Cos(angle) * radius)
                                     + (mapTransform.forward * Mathf.Sin(angle) * radius);
                    Vector3 toBossDir = (transform.position - spawnPos).normalized;

                    FireSingleProjectile(spawnPos, toBossDir, 8f);
                }
            }
            else if (pattern == 1)
            {
                if (anim != null) anim.SetTrigger("Magic");
                yield return new WaitForSeconds(0.3f);

                Vector3 toPlayer = (playerTarget.position - transform.position).normalized;
                FireFanProjectiles(toPlayer, 5, 20f);
            }
            else if (pattern == 2)
            {
                if (anim != null) anim.SetTrigger("Magic");
                yield return new WaitForSeconds(0.3f);

                for (int i = 0; i < 15; i++)
                {
                    Vector3 dropOrigin = playerTarget.position + (Vector3.up * 15f);
                    dropOrigin += Vector3.right * Random.Range(-20f, 20f)
                                + Vector3.forward * Random.Range(-20f, 20f);
                    FireSingleProjectile(dropOrigin, Vector3.down, 10f);
                }
            }
        }
    }
    private IEnumerator MoveToPoint(Transform targetPoint, float speed)
    {
        while (Vector3.Distance(transform.localPosition, targetPoint.localPosition) > 0.1f)
        {
            if (isCompletelyStopped) yield break;

            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                targetPoint.localPosition,
                speed * Time.deltaTime);

            if (isDashing)
            {
                Vector3 toTargetGlobal = targetPoint.position - transform.position;
                Vector3 toTargetLocal = mapTransform.InverseTransformDirection(toTargetGlobal);
                Vector3 flatLookDirLocal = Vector3.ProjectOnPlane(toTargetLocal, currentPhaseUpDir).normalized;

                if (flatLookDirLocal.sqrMagnitude > 0.001f)
                {
                    Quaternion targetLocalRot = Quaternion.LookRotation(flatLookDirLocal, currentPhaseUpDir);
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLocalRot, 20f * Time.deltaTime);
                }
            }

            yield return null;
        }
        transform.localPosition = targetPoint.localPosition;
    }
    private void FireSingleProjectile(Vector3 pos, Vector3 dir, float speed)
    {
        if (projectilePrefab == null) return;

        Vector3 worldPhaseUp = mapTransform.rotation * currentPhaseUpDir;
        EnemyShots proj = Instantiate(projectilePrefab, pos, Quaternion.LookRotation(dir, worldPhaseUp));

        if (mapTransform != null)
            proj.transform.SetParent(mapTransform, true);

        proj.Initialize(dir, speed, 5f);

        PlayerControl pc = playerTarget.GetComponent<PlayerControl>();
        if (pc != null && pc.is2D)
            proj.SwitchTo2D(playerTarget);
    }

    private void FireFanProjectiles(Vector3 centerDir, int count, float angleStep)
    {
        if (projectilePrefab == null) return;

        float startAngle = -((count - 1) * angleStep) / 2f;
        Vector3 worldPhaseUp = mapTransform.rotation * currentPhaseUpDir;

        for (int i = 0; i < count; i++)
        {
            float currentAngle = startAngle + (i * angleStep);
            Vector3 shootDir = Quaternion.AngleAxis(currentAngle, worldPhaseUp) * centerDir;
            Vector3 spawnPos = transform.position + shootDir * fireOffset;
            FireSingleProjectile(spawnPos, shootDir, 10f);
        }
    }

    private void PlayMoveSfx()
    {
        if (AudioManager.Instance != null && normalMoveSfx != null)
            AudioManager.Instance.PlaySFX(normalMoveSfx, false);
    }
}