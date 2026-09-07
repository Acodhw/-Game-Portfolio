using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Proto : MonoBehaviour
{
    public float MaxHP = 100;
    public float HP = 100; 
    public float MaxMp = 250;
    public float MP = 250;
    public float barrior = 0;
    public float defense;
    public float magic_resistance;

    public float power;
    public float intellect;

    public int criticalPoint;
    public float criticalDamageMult;

    public int ccCode = -1;
    public float ccPower;
    public float ccTime;

    public int MoveSkill1;
    public int MoveSkill2;

    public int[] AttackSkill1;
    public int[] AttackSkill2;

    public GameObject damageTx;
    public Transform player;

    private int charactorKey = 0;
    private bool ManaAutoHeal = true;

    public int CardToken = 10;
    public int MaxCardToken = 10;
    public List<int> SettingCard; //이놈이 덱
    public int[] SettingCardKind; //이놈은 덱에 뭔카드가 몇개 있는지
    public int[] HaveCardKind; //이놈은 그냥 가지고있는거. 
    public List<int> OpenedCard; //나와있는 카드.

    private void Awake()
    {
        HP = MaxHP;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    IEnumerator AutoMana() 
    {
        yield return new WaitForSeconds(1f);
        if (MaxMp * 0.01f + MP > MaxMp)
        {
            MP = MaxMp;
        }
        else
        {
            MP += MaxMp * 0.01f;
        }
        ManaAutoHeal = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (CardToken > MaxCardToken)
            CardToken = MaxCardToken;
        if (ManaAutoHeal)
        {
            ManaAutoHeal = false;
            StartCoroutine("AutoMana");
        }
    }

    public void remCC()
    {
        ccCode = -1;
        ccPower = 0;
        ccTime = 0;
    }

    public void setCharactorKey(int key)
    {
        charactorKey = key;
    }

    public int getCharactorKey()
    {
        return charactorKey;
    }


    public float getPower()
    {
        return power;
    }


    public float getIntellect()
    {
        return intellect;
    }

    public float getMaxHP()
    {
        return MaxHP;
    }

    public float getHP()
    {
        return HP;
    }

    public void Heal(float healP)
    {
        TextMesh tm = Instantiate(damageTx, new Vector3(player.position.x + 0.5f, player.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
        tm.text = ((int)healP).ToString();
        tm.color = new Color(0.2f, 1, 0.2f, 1);
        if (healP + HP > MaxHP)
        {
            HP = MaxHP;
        }
        else
        {
            HP += healP;
        }
    }

    public void Damage(float damage, DamageType_Proto dt, float pire_Point, float numberSize)
    {
        if (barrior <= 0)
        {
            if (dt == DamageType_Proto.Physics)
            {
                HP -= damage - damage * ((defense - defense * (pire_Point / (pire_Point + 128))) / (defense + 128));
                TextMesh tm = Instantiate(damageTx, new Vector3(player.position.x + 0.5f, player.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                tm.text = (Mathf.RoundToInt(damage - damage * ((defense - defense * (pire_Point / (pire_Point + 128))) / (defense + 128)))).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(1, 1, 0.2f, 1);
            }
            else if (dt == DamageType_Proto.Magic)
            {
                HP -= damage - damage * ((magic_resistance - magic_resistance * (pire_Point / (pire_Point + 128))) / (magic_resistance + 128));
                TextMesh tm = Instantiate(damageTx, new Vector3(player.position.x + 0.5f, player.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                tm.text = (Mathf.RoundToInt(damage - damage * ((magic_resistance - magic_resistance * (pire_Point / (pire_Point + 128))) / (magic_resistance + 128)))).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(1, 1, 0.2f, 1);
            }
            else
            {
                HP -= damage;
                TextMesh tm = Instantiate(damageTx, new Vector3(player.position.x + 0.5f, player.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                tm.text = (Mathf.RoundToInt(damage)).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(1, 1, 0.2f, 1);
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
                TextMesh tm = Instantiate(damageTx, new Vector3(player.position.x + 0.5f, player.position.y + 0.8f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                if(Mathf.RoundToInt(barrior) == 0)
                    tm.text = (1).ToString();
                else
                    tm.text = (Mathf.RoundToInt(barrior)).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(0.2f, 1, 1, 1);

                barrior = 0;
                HP -= dam;
                if (dam > 0)
                {
                    TextMesh tm2 = Instantiate(damageTx, new Vector3(player.position.x + 0.5f, player.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                    tm2.text = (Mathf.RoundToInt(dam)).ToString();
                    if (Mathf.RoundToInt(dam) == 0)
                        tm2.text = "1";

                    tm2.fontSize = (int)(tm.fontSize * 2f);
                    tm2.color = new Color(1, 1, 0.2f, 1);
                }
            }
            else
            {
                barrior -= dam;
                TextMesh tm = Instantiate(damageTx, new Vector3(player.position.x + 0.5f, player.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                tm.text = (Mathf.RoundToInt(dam)).ToString();
                tm.fontSize = (int)(tm.fontSize * numberSize);
                tm.color = new Color(0.2f, 1, 1, 1);
            }
        }
    }

    public void ManaUp(float upM, bool texton = false)
    {
        if (texton)
        {
            TextMesh tm = Instantiate(damageTx, new Vector3(player.position.x + 0.5f, player.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
            tm.text = ((int)upM).ToString();
            tm.color = new Color(0.2f, 1, 0.2f, 1);
        }
        if (upM + MP > MaxMp)
        {
            MP = MaxMp;
        }
        else
        {
            MP += upM;
        }
    }

    public void DacMix()
    {
        int random1, random2;
        int temp;

        for (int i = 0; i < SettingCard.Count; ++i)
        {
            random1 = Random.Range(0, SettingCard.Count);
            random2 = Random.Range(0, SettingCard.Count);

            temp = SettingCard[random1];
            SettingCard[random1] = SettingCard[random2];
            SettingCard[random2] = temp;
        }
    }
}
