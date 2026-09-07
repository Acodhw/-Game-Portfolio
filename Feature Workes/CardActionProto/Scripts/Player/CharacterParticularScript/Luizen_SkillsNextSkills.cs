using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Luizen_SkillsNextSkills : MonoBehaviour
{
    [SerializeField]
    private GameObject nextObj;
    [SerializeField]
    private float removeTime;
    [SerializeField]
    private int skillcode;

    ConditionCheckwithString[] conditioncheck = new ConditionCheckwithString[1];
    private PlayerManager playerManager;
    private uint nextDamage;
    private UnityEvent<bool> changeBool;
    private UnityEvent passiveEvent;
    private UnityEvent passiveBoom;
    private UnityEvent<int, float> Change;
    private bool Ultimate = false;

    private float cool;

    Func<States, uint, uint> ultimatePlus = (s, dam) =>
    {
        if (s.isinConditionList("_StigmaoftheSun"))
        {
            return (uint)(dam * 2.1f);
        }
        return dam;
    };

    void Start()
    {
        playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        ConditionCheckwithString tmp;
        tmp.con = "_StigmaoftheSun";
        tmp.point = 1;
        tmp.time = 2;
        conditioncheck[0] = tmp;
        switch (skillcode) {
            case 2:
                StartCoroutine("HandofPull");
                break;
            case 8:
                StartCoroutine("Sunshine");
                break;
            case 11:
                StartCoroutine("SolarSpot");
                break;
            case 12:
                StartCoroutine("NuclerFusion");
                break;

        }
    }

    public void Setting(uint nextDamage, UnityEvent<bool> changebool, UnityEvent nextEvent, UnityEvent passive, UnityEvent<int, float> c, float cooltime, bool ult)
    {
        this.nextDamage = nextDamage;
        changeBool = changebool;
        passiveEvent = nextEvent;
        passiveBoom = passive;
        Change = c;
        cool = cooltime;
        Ultimate = ult;
    }

    IEnumerator NuclerFusion()
    {
        Transform[] elecball = { transform.GetChild(0).GetChild(0), transform.GetChild(0).GetChild(1), transform.GetChild(0).GetChild(2), transform.GetChild(0).GetChild(3) };

        int j = 0;
        for (float i = 1; i > 0; i -= 0.025f)
        {
            elecball[0].transform.localPosition = new Vector3(Mathf.Cos(j * 9 * Mathf.Deg2Rad) * (j - 40) * 0.35f, Mathf.Sin(j * 9 * Mathf.Deg2Rad) * (j - 40) * 0.35f, elecball[0].transform.localPosition.z);
            elecball[1].transform.localPosition = new Vector3(Mathf.Cos((j + 90) * 9 * Mathf.Deg2Rad) * (j - 40) * 0.35f, Mathf.Sin((j + 90) * 9 * Mathf.Deg2Rad) * (j - 40) * 0.35f, elecball[1].transform.localPosition.z);
            elecball[2].transform.localPosition = new Vector3(Mathf.Cos((j + 180) * 9 * Mathf.Deg2Rad) * (j - 40) * 0.35f, Mathf.Sin((j + 180) * 9 * Mathf.Deg2Rad) * (j - 40) * 0.35f, elecball[2].transform.localPosition.z);
            elecball[3].transform.localPosition = new Vector3(Mathf.Cos((j + 270) * 9 * Mathf.Deg2Rad) * (j - 40) * 0.35f, Mathf.Sin((j + 270) * 9 * Mathf.Deg2Rad) * (j - 40) * 0.35f, elecball[3].transform.localPosition.z);
            j++;
            yield return new WaitForSeconds((removeTime - 0.75f) * 0.025f);
        }

        float time = 0;
        changeBool.Invoke(true);
        while (time < 0.09f)
        {
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 12 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 12 ? 2 : 3))))
            {
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }
        changeBool.Invoke(false);
        if (time < 0.09f)
        {
            passiveBoom.Invoke();
            GameObject g = Instantiate(nextObj, transform.position + Vector3.forward * 0.1f, transform.rotation);
            g.GetComponent<PlayerAttacks>().SetHitEvent(passiveEvent);
            DamageInfo di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = transform;
            di.damage = nextDamage;
            di.damageChangeFunc.Add(ultimatePlus);
            di.conditionCheck = Ultimate ? conditioncheck : null;
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);

        }
        Change.Invoke(12, cool);
        Destroy(gameObject);
    }

    IEnumerator SolarSpot()
    {
        for (float i = 1; i > 0; i -= 0.05f)
        {
            transform.localScale = Vector3.one * (i + 0.071922f) * 1.5f;
            yield return new WaitForSeconds((removeTime - 0.75f) * 0.05f);
        }
        float time = 0;
        bool check = false;
        changeBool.Invoke(true);
        while (time < 0.09f)
        {
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 11 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 11 ? 2 : 3))))
            {
                check = true;
            }
            time += Time.deltaTime;
            yield return null;
        }
        changeBool.Invoke(false);
        GameObject g = Instantiate(nextObj, transform.position, transform.rotation);
        g.transform.localScale = new Vector3(GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<PlayerAttacks>().SetHitEvent(passiveEvent);
        DamageInfo di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = transform;
        di.damage = nextDamage;
        di.damageChangeFunc.Add(ultimatePlus);
        if (check)
        {
            passiveBoom.Invoke();
            di.damage = (uint)Mathf.RoundToInt(di.damage *1.25f);
            di.damageType = DamageType.True;
            di.elemental = Elemental.None;
            di.conditionCheck = Ultimate ? conditioncheck : null;
            g.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 1, 0) * 75f);
        }
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);


        Change.Invoke(11, cool);
        Destroy(gameObject);
    }

    IEnumerator Sunshine()
    {
        for (float i = 1; i > 0; i -= 0.05f)
        {
            transform.localScale = Vector3.one * (i + 0.071922f);
            yield return new WaitForSeconds((removeTime - 0.75f) * 0.05f);
        }
        float time = 0;
        bool check = false;
        changeBool.Invoke(true);
        while (time < 0.09f)
        {
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 8 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 8 ? 2 : 3))))
            {
                check = true;
            }
            time += Time.deltaTime;
            yield return null;
        }
        changeBool.Invoke(false);
        GameObject g = Instantiate(nextObj, transform.position, transform.rotation);
        g.transform.localScale = new Vector3(GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<PlayerAttacks>().SetHitEvent(passiveEvent);
        DamageInfo di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = transform;
        di.damage = nextDamage;
        di.damageChangeFunc.Add(ultimatePlus);
        di.conditionCheck = Ultimate ? conditioncheck : null;
        if (check)
        {
            passiveBoom.Invoke();
            di.damage = (uint)Mathf.RoundToInt((di.damage / 3f) * 4f);
            di.cc = ClowdControl.Blind;
            di.ccTime = 0.75f;
            g.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 1, 1) * 50f);
        }
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);


        Change.Invoke(8, cool);
        Destroy(gameObject);
    }

    IEnumerator HandofPull()
    {
        yield return new WaitForSeconds(removeTime - 0.075f);
        float time = 0;
        changeBool.Invoke(true);
        while (time < 0.09f)
        {
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 2 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 2 ? 2 : 3))))
            {
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }
        changeBool.Invoke(false);
        if (time < 0.09f)
        {
            passiveBoom.Invoke();
            GameObject g = Instantiate(nextObj, transform.position, transform.rotation);
            g.transform.localScale = new Vector3(GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<PlayerAttacks>().SetHitEvent(passiveEvent);
            DamageInfo di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = transform;
            di.damage = nextDamage;
            di.damageChangeFunc.Add(ultimatePlus);
            di.conditionCheck = Ultimate ? conditioncheck : null;
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);

        }
        Change.Invoke(2, cool);
        Destroy(gameObject);
    }

    
}
