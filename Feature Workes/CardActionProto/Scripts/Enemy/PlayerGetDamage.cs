using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGetDamage : MonoBehaviour
{
    [SerializeField] private DamageInfo damageInfo;
    [SerializeField] private GameObject[] hitEffects;
    [SerializeField] private UnityEvent events;
    private bool sendHitEvent;
    private bool hit;

    private void Awake()
    {
        if (damageInfo.damageChangeFunc == null) damageInfo.damageChangeFunc = new List<System.Func<States, uint, uint>>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        if (damageInfo.order == null) damageInfo.order = transform;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            if(hitEffects.Length > 0) Instantiate(hitEffects[Random.Range(0, hitEffects.Length)], collision.transform.position + (Vector3)(collision.GetComponent<Rigidbody2D>().velocity * 0.15f) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
            if (hit || !sendHitEvent) return;
            Debug.Log("Hitting Count");
            hit = true;
            events.Invoke();
        }
    }

    private void OnEnable()
    {
        hit = false;
    }

    public DamageInfo GetDamage()
    {
        return damageInfo;
    }

    public void SetDamageInfo(DamageInfo di)
    {
        damageInfo = di;
    }
    public void SetHitEvent(UnityEvent events)
    {
        sendHitEvent = true;
        this.events = events;
    }
}
