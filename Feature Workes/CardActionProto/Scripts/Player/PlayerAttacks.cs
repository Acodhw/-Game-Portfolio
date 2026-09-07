using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttacks : MonoBehaviour
{
    [Header("데미지정보 세팅")]
    [SerializeField] private DamageInfo damageInfo;
    [Header("피격 이펙트 세팅")]
    [SerializeField] private bool EffectRotated = true;
    [SerializeField] private GameObject[] hitEffects;
    [SerializeField] private GameObject fixedHitEffects;
    [SerializeField] private Vector2 EffectPosition;
    [Header("세부 정보 세팅")]
    [SerializeField] private bool isNormalAttack;
    [SerializeField] private UnityEvent events;
    [SerializeField] private int ultimitPointStack;

    private bool hit = false;
    private bool sendHitEvent = false;
    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        if(damageInfo.damageChangeFunc == null) damageInfo.damageChangeFunc = new List<System.Func<States, uint, uint>>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (damageInfo.order == null) damageInfo.order = transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy") && !damageInfo.continuousDamage)
        {
            if (hit) return;
            hit = true;
            if (ultimitPointStack > 0) playerManager.UltimatePointUp(ultimitPointStack, isNormalAttack);
            if (!sendHitEvent) return;
            events.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy") && damageInfo.continuousDamage && !hit)
        {
            hit = true;
            if (ultimitPointStack > 0) playerManager.UltimatePointUp(ultimitPointStack, isNormalAttack);
            if (!sendHitEvent) return;
            StartCoroutine("hitInit", damageInfo.continuousTime);
            events.Invoke();
        }
    }

    IEnumerator hitInit(float time) {
        yield return new WaitForSeconds(time);
        hit = false;
    }

    private void OnEnable()
    {
        if (playerManager == null) playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        hit = false;
    }

    public DamageInfo GetDamage() {
        return damageInfo;
    }

    public void SetDamageInfo(DamageInfo di) {
        damageInfo = di;
    }

    public void SetHitEvent(UnityEvent events)
    {
        sendHitEvent = true;
        this.events = events;
    }

    public void SetIsNormalAttack(bool normal) {
        isNormalAttack = normal;
    }

    public void MakeHitEffect(Transform trans)
    {
        if (hitEffects.Length > 0) Instantiate(hitEffects[Random.Range(0, hitEffects.Length)], trans.position + (Vector3)EffectPosition + (Vector3.back * 2) + (Vector3)(trans.GetComponent<Rigidbody2D>().velocity * 0.15f) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1), EffectRotated ? Quaternion.Euler(0, 0, Random.Range(0f, 360f)) : Quaternion.Euler(0, 0, 0));
        if (fixedHitEffects != null) Instantiate(fixedHitEffects, trans.position + (Vector3)EffectPosition + (Vector3.back * 2.1f), Quaternion.Euler(0, 0, 0));
    }
}
