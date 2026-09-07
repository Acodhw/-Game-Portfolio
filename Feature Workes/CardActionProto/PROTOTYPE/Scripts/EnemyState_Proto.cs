using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyState_Proto : MonoBehaviour
{
    public Slider HPbar;
    public Slider Barriorbar;
    public float MaxHP;
    public float defense;
    public float barrior;
    public float magic_resistance;
    public float HP;

    public bool finding;

    public GameObject damageTx;
    public Image magicPassive;
    public Image gunPassive;
    public Image axePassive;
    public Sprite[] axePassiveImage;

    private int axePassiveStack;
    private int magicPassiveStack;
    private float bleedingDamageTime;
    private float magicPassiveTime;
    private float gunPassiveTime;
    private float axePassiveTime;

    private bool isGetDamage = false;
    private float DamageTime;

    private float poisonPower;
    private float poisonDamage;
    private float poisonTime;
    private bool canGetPoisonDamage;

    private int ccCode = -1;
    private float ccPower;
    private float ccTime;

    private PlayerState_Proto pstate;

    private bool barriorOn = false;
    bool sethp;
    private void Awake()
    {
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        HP = MaxHP;
        HPbar.maxValue = MaxHP;
        sethp = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (magicPassiveTime > 0)
        {
            magicPassive.color = new Color(1, 1, 1, 1);
            magicPassiveTime -= Time.deltaTime;
            if (magicPassiveStack == 2) 
            {
                magicPassiveStack = 0;
                magicPassiveTime = 0;
                Damage(pstate.intellect * 1.2f + HP * 0.05f, DamageType_Proto.Magic, 0, 1);
            }
        }
        else
        {
            magicPassiveStack = 0;
            magicPassiveTime = 0;
            magicPassive.color = new Color(1, 1, 1, 0);
        }

        if (gunPassiveTime > 0)
        {
            gunPassive.color = new Color(1, 1, 1, 1);
            gunPassiveTime -= Time.deltaTime;
            if (bleedingDamageTime > 0)
            {
                bleedingDamageTime -= Time.deltaTime;
            }
            else
            {
                Damage(MaxHP * 0.01f, DamageType_Proto.fix, 0, 1);
                bleedingDamageTime = 0.5f;
            }
        }
        else
        {
            bleedingDamageTime = 0;
            gunPassiveTime = 0;
            gunPassive.color = new Color(1, 1, 1, 0);
        }

        if (axePassiveTime > 0)
        {
            if (axePassiveStack < 4)
            {
                axePassive.color = new Color(1, 1, 1, 1);
                axePassive.sprite = axePassiveImage[axePassiveStack];
            }
            else
            {
                axePassiveStack = 0;
                axePassiveTime = 0;
                ccTime = 2f;
                ccCode = 0;
            }
        }
        else
        {
            axePassiveStack = 0;
            axePassiveTime = 0;
            axePassive.color = new Color(1, 1, 1, 0);
        }

        if (barrior > 0 && !barriorOn)
        {
            Barriorbar.maxValue = barrior;
            barriorOn = true;            
        }
        else if(barrior <= 0) {
            barriorOn = false; 
        }
        else if(barrior > 0 && barriorOn && barrior > Barriorbar.maxValue)
        {
            Barriorbar.maxValue = barrior;
        }
        Barriorbar.value = barrior;
        HPbar.gameObject.SetActive(isGetDamage);
        HPbar.value = HP;
        if (isGetDamage)
        {
            DamageTime -= Time.deltaTime;

            if (DamageTime <= 0)
            {
                isGetDamage = false;
                DamageTime = 0;
            }
        }

        if (sethp && HP <= 0) {
            pstate.CardToken++;
            if(pstate.GetComponent<GameDatas>().tokenUpTime > 0) 
                pstate.CardToken++;            
            Destroy(transform.parent.gameObject);
        }
    }

    private void AxePassiveUp()
    {
        axePassiveTime = 5;
        axePassiveStack++;
    }

    private void MagicPassiveUp()
    {
        magicPassiveTime = 3;
        magicPassiveStack++;
    }

    private void GunPassiveUp()
    {
        gunPassiveTime = 3;
        bleedingDamageTime = 0.5f;
    }

    public bool getIsGetDamage()
    {
        return isGetDamage;
    }


    public void ccReset()
    {
        ccCode = -1;
    }

    public int GetccCode()
    {
        return ccCode;
    }
    public float GetccPower()
    {
        return ccPower;
    }
    public float GetccTime()
    {
        return ccTime;
    }
    private void Damage(float damage, DamageType_Proto dt, float pire_Point, float numberSize)
    {
        isGetDamage = true;
        if (barrior <= 0)
        {
            if (dt == DamageType_Proto.Physics)
            {
                HP -= damage - damage * ((defense - defense * (pire_Point / (pire_Point + 128))) / (defense + 128));
                TextMesh tm = Instantiate(damageTx, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                if ((Mathf.RoundToInt(damage - damage * ((defense - defense * (pire_Point / (pire_Point + 128))) / (defense + 128)))) == 0)
                    tm.text = "-";
                else
                    tm.text = (Mathf.RoundToInt(damage - damage * ((defense - defense * (pire_Point / (pire_Point + 128))) / (defense + 128)))).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(1, 1, 1, 1);
            }
            else if (dt == DamageType_Proto.Magic)
            {
                HP -= damage - damage * ((magic_resistance - magic_resistance * (pire_Point / (pire_Point + 128))) / (magic_resistance + 128));
                TextMesh tm = Instantiate(damageTx, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                if ((Mathf.RoundToInt(damage - damage * ((magic_resistance - magic_resistance * (pire_Point / (pire_Point + 128))) / (magic_resistance + 128)))) == 0)
                    tm.text = "-";
                else
                    tm.text = (Mathf.RoundToInt(damage - damage * ((magic_resistance - magic_resistance * (pire_Point / (pire_Point + 128))) / (magic_resistance + 128)))).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(1, 1, 1, 1);
            }
            else
            {
                HP -= damage;                
                TextMesh tm = Instantiate(damageTx, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                if (Mathf.RoundToInt(damage) == 0)
                    tm.text = "-";
                else
                    tm.text = (Mathf.RoundToInt(damage)).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(1, 1, 1, 1);
            }
        }
        else
        {
            float dam;
            if (dt == DamageType_Proto.Physics)
                dam = damage - damage * ((defense - defense * (pire_Point / (pire_Point + 128))) / (defense + 128));
            else if (dt == DamageType_Proto.Magic)
                dam = damage - damage * ((magic_resistance - magic_resistance * (pire_Point / (pire_Point + 128))) / (magic_resistance + 128));
            else
                dam = damage;

            if (dam > barrior)
            {
                dam -= barrior;
                TextMesh tm = Instantiate(damageTx, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.8f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                if (Mathf.RoundToInt(barrior) == 0)
                    tm.text = (1).ToString();
                else
                    tm.text = (Mathf.RoundToInt(barrior)).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(0.2f, 0.2f, 1, 1);

                barrior = 0;
                HP -= dam;
                if (dam > 0)
                {
                    TextMesh tm2 = Instantiate(damageTx, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                    tm2.text = (Mathf.RoundToInt(dam)).ToString();
                    if (Mathf.RoundToInt(dam) == 0)
                        tm2.text = "1";
                        
                    tm2.fontSize = (int)(tm.fontSize * 1.75f);
                    tm2.color = new Color(1, 1, 1, 1);
                }
            }
            else
            {
                barrior -= dam;
                TextMesh tm = Instantiate(damageTx, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                tm.text = (Mathf.RoundToInt(dam)).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(0.2f, 0.2f, 1, 1);
            }
        }
        DamageTime = 5;
    }

    public void execution(float HPdown) {
        if (MaxHP * (HPdown * 0.01) < HP)
        {
            Damage(MaxHP * 2 + barrior * 2, DamageType_Proto.fix, 100, 1.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttack")
        {
            PlayerAttack_Proto pp;
            if (collision.GetComponent<PlayerAttack_Proto>() != null)
            {
                pp = collision.GetComponent<PlayerAttack_Proto>();
                ccCode = pp.ccCode;
                ccPower = pp.ccPower;
                ccTime = pp.ccTime;
                if (pp.isMoveSkills) { Damage(pp.realDamage + MaxHP * (0.01f * pp.MaxHP_pro_Damage) + HP * (0.01f * pp.HP_pro_Damage), pp.dt, pp.pire_Point, 1); }
                else
                {
                    if (pstate.getCharactorKey() == 5 && !finding)
                    {
                        Damage(pp.realDamage * pstate.criticalDamageMult * 1.5f + MaxHP * 0.1f, DamageType_Proto.fix, pp.pire_Point, 1.5f);
                    }

                    if (pstate.getCharactorKey() == 7 && MaxHP * 0.15f >= HP)
                    {
                        Damage(MaxHP * 2 + barrior * 2, DamageType_Proto.fix, pp.pire_Point, 1.5f);
                    }

                    if (pstate.getCharactorKey() == 1)
                    {
                        MagicPassiveUp();
                    }
                    if (pstate.getCharactorKey() == 2)
                    {
                        GunPassiveUp();
                    }
                    if (pstate.getCharactorKey() == 4)
                    {
                        AxePassiveUp();
                    }

                    if (gunPassiveTime > 0)
                    {
                        if (pp.criticalPoint >= Random.Range(1, 101))
                        {
                            Damage((pp.realDamage * pstate.criticalDamageMult + MaxHP * (0.01f * pp.MaxHP_pro_Damage) + HP * (0.01f * pp.HP_pro_Damage)) * 1.2f, pp.dt, pp.pire_Point, 1.5f);
                        }
                        else
                        {
                            Damage((pp.realDamage + MaxHP * (0.01f * pp.MaxHP_pro_Damage) + HP * (0.01f * pp.HP_pro_Damage)) * 1.2f, pp.dt, pp.pire_Point, 1);
                        }
                    }
                    else
                    {
                        if (pp.criticalPoint >= Random.Range(1, 101))
                        {
                            Damage(pp.realDamage * pstate.criticalDamageMult + MaxHP * (0.01f * pp.MaxHP_pro_Damage) + HP * (0.01f * pp.HP_pro_Damage), pp.dt, pp.pire_Point, 1.5f);
                        }
                        else
                        {
                            Damage(pp.realDamage + MaxHP * (0.01f * pp.MaxHP_pro_Damage) + HP * (0.01f * pp.HP_pro_Damage), pp.dt, pp.pire_Point, 1);
                        }
                    }

                    if (pstate.GetComponent<GameDatas>().execution) 
                    {
                        pstate.GetComponent<GameDatas>().execution = false;
                        int tmp = Random.Range(0, 100);
                        if (tmp < 25)
                        {
                            execution(25);
                            pstate.Heal(0.1f * (pstate.getMaxHP() - pstate.getHP()));
                        }
                    }
                }
            }
        }

    }
}
