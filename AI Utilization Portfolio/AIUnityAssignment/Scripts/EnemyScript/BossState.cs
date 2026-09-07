using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class BossState : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHP = 1000;
    public int currentHP;

    [Tooltip("화면에 띄워줄 보스 전용 체력바 UI")]
    public Slider hpSlider;

    [Header("References")]
    public BossMove bossMove;

    [Header("사망 컷신 타임라인")]
    [Tooltip("이 보스 처치 이벤트의 고유 ID")]
    public int deathEventID;

    [Tooltip("씬 내에 존재하는 PlayableDirector")]
    public PlayableDirector timelineDirector;

    [Tooltip("보스 사망 시 실행할 타임라인 에셋")]
    public PlayableAsset timelineAsset;

    private bool isDead = false;

    private float invincibilityTimer = 0f;
    private int currentPhaseIndex = 0;

    private void Start()
    {
        currentHP = maxHP;
        if (bossMove == null) bossMove = GetComponent<BossMove>();
        UpdateUI();
    }

    private void Update()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isDead || invincibilityTimer > 0f) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerAttack")) return;
        var pa = other.GetComponent<PlayerAttack>();
        if (pa == null) return;
        invincibilityTimer = 0.1f;
        TakeDamage((int)pa.Damage, pa.hitEffectPrefab);
    }
    public void TakeDamage(int damage, GameObject effectPrefab = null)
    {
        if (isDead) return;

        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        if (effectPrefab != null)
        {
            Vector3 centerPos = transform.position + Vector3.up * 1.0f;
            Vector2 rand = Random.insideUnitCircle * 1.0f;

            Vector3 randomPos = centerPos + new Vector3(rand.x, rand.y, 0f);
            if (Camera.main != null)
            {
                randomPos -= Camera.main.transform.forward * 1.5f;
            }

            GameObject effect = Instantiate(effectPrefab, randomPos, effectPrefab.transform.rotation);
            Destroy(effect, 2f);
        }

        UpdateUI();
        CheckPhase();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void CheckPhase()
    {
        float hpPercent = (float)currentHP / maxHP;
        int nextPhase = 0;

        if (hpPercent <= 0.75f && hpPercent > 0.5f)
        {
            nextPhase = 1;
        }
        else if (hpPercent <= 0.5f && hpPercent > 0.25f)
        {
            nextPhase = 2;
        }
        else if (hpPercent <= 0.25f && hpPercent > 0f)
        {
            nextPhase = 3;
        }
        else if (hpPercent > 0.75f)
        {
            nextPhase = 0;
        }

        if (nextPhase != currentPhaseIndex)
        {
            currentPhaseIndex = nextPhase;
            bossMove.ChangePhase(nextPhase);
        }
    }

    private void Die()
    {
        isDead = true;

        if (hpSlider != null)
        {
            if (hpSlider.transform.parent != null) hpSlider.transform.parent.gameObject.SetActive(false);
            else hpSlider.gameObject.SetActive(false);
        }

        if (bossMove != null)
        {
            bossMove.StopAllActionsForTimeline();
        }
        PlayDeathCutscene();
    }

    private void PlayDeathCutscene()
    {
        if (timelineDirector != null && timelineAsset != null)
        {
            timelineDirector.playableAsset = timelineAsset;
            timelineDirector.Play();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.eventCountDict[deathEventID] = 1;
        }
    }

    private void UpdateUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }
    }
}