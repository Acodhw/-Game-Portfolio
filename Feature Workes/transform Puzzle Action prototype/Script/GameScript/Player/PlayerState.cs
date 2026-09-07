using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniRx;
using UnityEngine;
using UnityEngine.Events;


public class PlayerState : MonoBehaviour
{
    private PlayerControl player;
    private GameManager manager;
    private Dictionary<Transform, float> continousDam;

    private float steminahealCool = 0;
    [SerializeField]
    DamageText damageText;
    [SerializeField]
    private int HP;
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private int stemina;
    [SerializeField]
    private int maxStemina;
    [SerializeField]
    private int attack;
    [SerializeField]
    private int defence;
    [SerializeField]
    private float critRate;
    [SerializeField]
    private int critPossi;
    [SerializeField]
    private int damageReduceRate;

    private Dictionary<EffectInfo, Vector2> effects;
    private int playerForm;
    private float[] skillCooltimeChecking = { 0, 0, 0, 0, 0, 0 }; 
    private float[] skillMaxCooltimeChecking = { 0, 0, 0, 0, 0, 0 };

    private bool[] formAble = { true, false, false, false, false, false }; // 현재 해금된 폼인지
    private bool[] formActive = { true, true, true, true, true, true }; // 폼이 특정 이유로 사용 불가능한지 여부

    public int GetHP() { return HP; }
    public int GetMaxHP() { return maxHP; }
    public int GetStemina() { return stemina; }
    public int GetMaxStemina() { return maxStemina; }
    public int GetAttack() { return attack; }
    public int GetDefence() { return defence; }
    public float GetCriticalRate() { return critRate; }
    public int GetCritical() { return critPossi; }
    public int GetForm() { return playerForm; }

    public float GetSkillCooltime(int index) { return skillCooltimeChecking[index]; }
    public float GetSkillMaxCooltime(int index) { return skillMaxCooltimeChecking[index]; }

    public bool GetIsFormActive(int index) { return formActive[index]; }
    public bool GetIsFormAble(int index) { return formAble[index]; }
    public void SetPlayer(PlayerControl p) {
        player = p;
    }
    public void AddForm(int add) {
        int nowForm = playerForm;
        do
        {
            nowForm = (nowForm + 6 + add) % 6;
        }
        while (!(formAble[nowForm] && formActive[nowForm]));
        playerForm = nowForm;
        
    }
    public void SetForm(int set)
    {
        playerForm = set;
    }
    public void SetSkillCooltime(int index, float time) {
        skillCooltimeChecking[index] = time;
        skillMaxCooltimeChecking[index] = time;
    }
    public void SetIsFormActive(int index, bool val) {
        formActive[index] = val;
    }
    public void SetIsFormAble(int index, bool val)
    {
        formAble[index] = val;
    }

    public void StatusReset(bool ignoreLevel = true) {
        if (!ignoreLevel)
        {
            
        }
        HP = maxHP;
        stemina = maxStemina;
    }
    public void Damage(DamageInfo dam, bool isStun, Transform owner)
    {
        int defenceRate = manager.GetDefenceRate();
        if (dam.isContinousdamage)
        {
            if (continousDam.ContainsKey(owner)) return;          
        }
        else
        {
            if (continousDam.ContainsKey(owner))
            {
                continousDam[owner] = 0.05f;
                return;
            }
        }

        float getDamage = dam.damage * (dam.isCrit ? dam.critRate : 1);
        if (!dam.isFixed)
        {
            getDamage *= 1 - ((float)defence / (defence + defenceRate));
            getDamage *= 1 - (damageReduceRate / 100);

            if (dam.damageChangeFunc != null)
                getDamage = dam.damageChangeFunc(getDamage,
                    new stateInfo(player.transform, HP, maxHP, attack, defence, damageReduceRate, isStun, new StatusEffect[0]));
        }
        int d = Mathf.RoundToInt(getDamage);      
        HP = (HP - d <= 0 ? 0 : HP - d);
        DamageText dtx = Instantiate<DamageText>(damageText,
            player.transform.position + Vector3.back * 0.1f
            + (player.GetComponent<SpriteRenderer>().flipX? Vector3.right : Vector3.left) * 0.75f,
            transform.rotation).GetComponent<DamageText>();
        dtx.SetText(d.ToString(), dam.isCrit);

        player.FlashPlayer();

        if (dam.ccinfo != CCInfo.none) player.Stun(dam.ccinfo, dam.stunPower, dam.attackOwner.position.x > player.transform.position.x);
        if(dam.damageAfterEvent != null) dam.damageAfterEvent.Invoke(getDamage,
            new stateInfo(player.transform, HP, maxHP, attack, defence, damageReduceRate, isStun, new StatusEffect[0]));

        if (dam.isContinousdamage)
        {
            continousDam.Add(owner, dam.nexthitTime);
        }
        else
        {
            continousDam.Add(owner, 0.05f);
        }
    }
    
    public void Heal(int h)
    {
        if (HP < maxHP - h) HP += h;
        else HP = maxHP;
    }
    public void HealStemina(int s)
    {
        if (stemina < maxStemina - s) stemina += s;
        else stemina = maxStemina;
    }
    public bool UseStemina(int s)
    {
        if (stemina < s) return false;
        stemina -= s;
        SteminaHold();
        return true;
    }
    public void SteminaHold() {
        steminahealCool = 1.8f;
    }


    private void Awake()
    {
        manager = GetComponent<GameManager>();
        continousDam = new Dictionary<Transform, float>();
        StatusReset(true);
    }
    private void Update()
    {
        
        if(HP > maxHP) HP = maxHP;
        if (HP < 0) HP = 0;
        if (stemina > maxStemina) stemina = maxStemina;
        if (stemina < 0) stemina = 0;

        if (steminahealCool > 0)
        {
            steminahealCool -= Time.deltaTime;
        }
        else { 
            HealStemina(1);
            steminahealCool = 1.8f;
        }

        for (int i = 0; i < 6; i++)
        {
            if (skillCooltimeChecking[i] > 0) skillCooltimeChecking[i] -= Time.deltaTime;
            else
            {
                skillCooltimeChecking[i] = 0;
                skillMaxCooltimeChecking[i] = 0;
            }
        }

        Transform[] ke = new Transform[continousDam.Keys.Count];
        continousDam.Keys.CopyTo(ke, 0);
        foreach (Transform k in ke)
        {
            continousDam[k] -= Time.deltaTime;
            if (continousDam[k] <= 0) continousDam.Remove(k);
        }
    }
}
