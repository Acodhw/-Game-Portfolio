using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class tmphit : States
{
    public uint MaxHP = 100000000;
    public uint HP = 10000000;
    private SpriteRenderer player;
    private SpriteRenderer sprite;
    private Material SpriteMatarial;
    public Material HitMatarial;
    private Rigidbody2D rigid;
    public bool isFixed = true;
    public bool ishitting = false;
    private float hittime = 0;

    public int Defence;
    public int Resistance;
    public float[] damageTime = new float[10];

    private new void Awake()
    {
        player = GameObject.Find("Player").GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        particular = GameObject.Find("GameManager").GetComponent<ParticularEvent>();
        SpriteMatarial = sprite.material;

        state = new StringUIntDic() {
    {"MaxHP", 100000000},          
    {"HP", 100000000},             
    };
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("PlayerAttack")) {           
            DamageInfo d = collision.attachedRigidbody.GetComponent<PlayerAttacks>().GetDamage();
            if (!d.continuousDamage)
            {
                if (!d.noflash) ishitting = true;
                collision.attachedRigidbody.GetComponent<PlayerAttacks>().MakeHitEffect(transform);
                Damage(d, true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("PlayerAttack"))
        {           
            DamageInfo d = collision.attachedRigidbody.GetComponent<PlayerAttacks>().GetDamage();
            if (d.continuousDamage && !continueusTimeCheck.ContainsKey(collision.GetInstanceID()))
            {
                if (!d.noflash) ishitting = true;
                collision.attachedRigidbody.GetComponent<PlayerAttacks>().MakeHitEffect(transform);
                Damage(d, true);
                continueusTimeCheck.Add(collision.GetInstanceID(), d.continuousTime);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("PlayerAttack"))
        {
            ishitting = false;           
        }
    }

    public override void Damage(DamageInfo damageInfo, bool isSpawnDamageText)
    {
        hittime = 3;
        bool playerInLeft = ((player.transform.position.x - transform.position.x < 1.1f && player.transform.position.x - transform.position.x > -1.1f && player.flipX) || (player.transform.position.x - transform.position.x) > 1.1f);
        bool orderInLeft = playerInLeft;
        if (damageInfo.order != null) orderInLeft = (player.transform == damageInfo.order) ? playerInLeft : ((damageInfo.order.position.x == transform.position.x) ? player.flipX : (damageInfo.order.position.x > transform.position.x));
        uint damag = (uint)Mathf.RoundToInt((damageInfo.damage * (damageInfo.criticalPercent ? (damageInfo.criticalPoint * 0.01f) : 1)) + (MaxHP * damageInfo.maxHPDamage * 0.01f) + (MaxHP * damageInfo.nowHPDamage * 0.01f));
        if (damageInfo.damageChangeFunc != null && damageInfo.damageChangeFunc.Count > 0 && damageInfo.damageType != DamageType.True) {
            foreach(var i in damageInfo.damageChangeFunc)
                damag = i(this, damag);
        }
        if (damageInfo.conditionCheck != null && damageInfo.conditionCheck.Length > 0)
            foreach (ConditionCheckwithString i in damageInfo.conditionCheck)
            {
                Condition condition = particular.FindCondition(i.con);
                ConditionChange(condition, i.point, i.time);
            }
        Vector3 a = new Vector3(255, 255, 255);
        switch (damageInfo.elemental)
        {
            case Elemental.Fire:
                a = new Vector3(255, 102, 26);
                break;
            case Elemental.Water:
                a = new Vector3(20, 132, 243);
                break;
            case Elemental.Ground:
                a = new Vector3(225, 168, 36);
                break;
            case Elemental.Wind:
                a = new Vector3(112, 230, 243);
                break;
            case Elemental.Plant:
                a = new Vector3(144, 219, 79);
                break;
            case Elemental.Electric:
                a = new Vector3(238, 238, 0);
                break;
            case Elemental.Rock:
                a = new Vector3(161, 145, 131);
                break;
            case Elemental.Frozen:
                a = new Vector3(163, 196, 239);
                break;
            case Elemental.Esp:
                a = new Vector3(255, 63, 255);
                break;
            case Elemental.Spirit:
                a = new Vector3(87, 23, 151);
                break;
            case Elemental.Nature:
                a = new Vector3(58, 196, 150);
                break;
            case Elemental.Harmony:
                a = new Vector3(149, 149, 149);
                break;
            case Elemental.Light:
                a = new Vector3(252, 252, 252);
                break;
            case Elemental.Dark:
                a = new Vector3(21, 21, 21);
                break;
        }
        a = a / 225;
        Color c = new Color(a.x, a.y, a.z);
       
        switch (damageInfo.damageType)
        {
            case DamageType.Physics:
                damag = (uint)Mathf.RoundToInt(damag - (damag * Defence / (Defence + GuardConstant)));
                break;
            case DamageType.Magic:
                damag = (uint)Mathf.RoundToInt(damag - (damag * Resistance / (Resistance + GuardConstant)));
                break;
        }

        if (damageInfo.damageChangeEvent != null) damageInfo.damageChangeEvent.Invoke(this, damag);
        if (damag > 0)particular.SummonFlatText(transform.position + 1.2f * new Vector3(playerInLeft ? -0.75f : 0.75f + Random.Range(-0.1f, 0.1f), 0.3f + Random.Range(-0.1f, 0.1f), -2)).Changetext(damag.ToString(), damageInfo.criticalPercent ? 10 : 6, c);
        if (!damageInfo.noflash)
        {
            sprite.material = HitMatarial;
            StartCoroutine("ResetMatarial", 0.2f);
        }
        HP -= damag;
        if (!isFixed)
        {
            switch (damageInfo.cc)
            {
                case ClowdControl.Airborne:
                    rigid.velocity = new Vector2(0, 1) * damageInfo.ccStrength * 6;
                    break;
                case ClowdControl.Push:
                    rigid.velocity = new Vector2(orderInLeft ? -6 : 6, 5) * damageInfo.ccStrength;
                    break;
                case ClowdControl.Pull:
                    rigid.velocity = new Vector2(orderInLeft ? 6 : -6, 5) * damageInfo.ccStrength;
                    break;
            }
        }
    }

    private void Update()
    {
        DamagewithCondition();
        if (hittime >= 0)
        {
            hittime -= Time.deltaTime;
        }
        else
        {
            HP = MaxHP;

        }

        state["HP"] = HP;

        for (int i = continueusTimeCheck.Count - 1; i >= 0; i--)
        {
            var item = continueusTimeCheck.ElementAt(i);
            if (item.Value <= 0)
            {
                continueusTimeCheck.Remove(item.Key);
                continue;
            }
            continueusTimeCheck[item.Key] -= Time.deltaTime;
        }

    }

    IEnumerator ResetMatarial(float time) {
        yield return new WaitForSeconds(time);

        if (!ishitting)
        {
            sprite.material = SpriteMatarial;
        }
        else {
            ishitting = false;
            StartCoroutine("ResetMatarial", 0.1f);
        }
    }

    void DamagewithCondition() {
        if (isinConditionList("_Bleeding"))
        {
            if (damageTime[0] <= 0)
            {
                DamageInfo di = new DamageInfo();
                di.maxHPDamage = 0.5f;
                di.damageType = DamageType.True;
                Damage(di, true);
                damageTime[0] = 1;
            }
            else
            {
                damageTime[0] -= Time.deltaTime;
            }
        }
        else  damageTime[0] = 0;

        if (isinConditionList("_PainExacerbation"))
        {
            if (damageTime[1] <= 0)
            {
                DamageInfo di = new DamageInfo();
                di.maxHPDamage = 1f;
                di.damageType = DamageType.Physics;
                Damage(di, true);
                damageTime[1] = 1;
            }
            else
            {
                damageTime[1] -= Time.deltaTime;
            }
        }
        else damageTime[1] = 0;
    }
}
