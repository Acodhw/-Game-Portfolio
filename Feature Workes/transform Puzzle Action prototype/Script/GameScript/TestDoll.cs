using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;

public class TestDoll : MonoBehaviour
{
    [SerializeField]
    private int hp = 1000;
    [SerializeField]
    private int def = 25;
    [SerializeField]
    private Material flasgMaterial;
    [SerializeField]
    private GameObject damageTx;

    private int maxhp;
    private float flashTime;
    private Dictionary<Transform, float> continousDam;
    private GameManager manager;
    private Rigidbody2D rb;
    private Material ownMaterial;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            if (collision.GetComponent<PlayerAttack>() != null)
            {
                Damage(collision.GetComponent<PlayerAttack>().GetDamage(), collision.transform);
            }
        }
    }

    public void Stun(CCInfo info, float power, bool pushLeft)
    {
        switch (info)
        {
            case CCInfo.stun:
                break;
            case CCInfo.push:
                rb.linearVelocity = ((pushLeft ? Vector2.left : Vector2.right) + Vector2.up * 0.5f) * power * 3;
                break;
            case CCInfo.airborne:
                rb.linearVelocity =  Vector2.up * power * 3;
                break;
        }
    }

    public void Damage(DamageInfo dam, Transform owner)
    {
        int defenceRate = manager.GetDefenceRate();
        if (continousDam.ContainsKey(owner))
        {
            if (!dam.isContinousdamage)continousDam[owner] = 0.05f;
            return;
        }
        if (dam.isContinousdamage)
        {
            continousDam.Add(owner, dam.nexthitTime);
        }
        else
        {
            continousDam.Add(owner, 0.05f);
        }

        float getDamage = dam.damage * (dam.isCrit ? dam.critRate : 1);
        if (!dam.isFixed)
        {
            getDamage *= 1 - ((float)def / (def + defenceRate));

            if (dam.damageChangeFunc != null)
                getDamage = dam.damageChangeFunc(getDamage,
                    new stateInfo(transform, hp, maxhp, 0, def, 0, false, new StatusEffect()));
        }
        int d = Mathf.RoundToInt(getDamage);
        hp = (hp - d <= 0 ? 0 : hp - d);
        flashTime = 0.1f;
        DamageText dtx = Instantiate(damageTx,
            transform.position + Vector3.back * 0.1f
            + (dam.attackOwner.position.x < transform.position.x ? Vector3.right : Vector3.left) * 1.25f,
            transform.rotation).GetComponent<DamageText>();
        dtx.SetText(d.ToString(), dam.isCrit);

        if (dam.ccinfo != CCInfo.none) Stun(dam.ccinfo, dam.stunPower, dam.attackDir.x < 0);
        if (dam.damageAfterEvent != null) dam.damageAfterEvent.Invoke(getDamage,
            new stateInfo(transform, hp, maxhp, 0, def, 0, false, new StatusEffect()));

    }

    private void Awake()
    {
        continousDam = new Dictionary<Transform, float>();
        GameObject temp = Instantiate(damageTx);
        temp.SetActive(false);
        Destroy(temp, 0.02f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ownMaterial = GetComponent<SpriteRenderer>().material;
        rb = GetComponent<Rigidbody2D>();       
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        maxhp = hp;
    }

    // Update is called once per frame
    void Update()
    {
        Transform[] ke = new Transform[continousDam.Keys.Count];
        continousDam.Keys.CopyTo(ke, 0);
        foreach (Transform k in ke)
        {
            continousDam[k] -= Time.deltaTime;
            if (continousDam[k] <= 0) continousDam.Remove(k);
        }

        if (flashTime > 0)
        {
            flashTime -= Time.deltaTime;
            GetComponent<SpriteRenderer>().material = flasgMaterial;
        }
        else
        {
            flashTime = 0;
            GetComponent<SpriteRenderer>().material = ownMaterial;
        }
    }
}
