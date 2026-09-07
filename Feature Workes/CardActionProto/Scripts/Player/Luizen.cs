using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Luizen : CharacterBase
{
    bool isOnGround = true;
    bool Ultimate = false;
    bool reSkill = false;
    bool CanChargeByNormalAttack = true;
    int powerStack = 0;
    float chargeTime = 0;

    UnityEvent passiveForObject = new UnityEvent();
    UnityEvent events = new UnityEvent();
    UnityEvent normalevents = new UnityEvent();
    ConditionCheckwithString[] conditioncheck = new ConditionCheckwithString[1];

    public void ChangeReskill(bool b) {
        reSkill = b;
    }
    UnityEvent<bool> ChangeBool = new UnityEvent<bool>();    

    Func<States, uint, uint> ultimatePlus = (s, dam) =>
    {
        if (s.isinConditionList("_StigmaoftheSun"))
        {
            return (uint)(dam * 2.1f);
        }
        return dam;
    };


    private bool[] Skillchecking = new bool[13] { true, true, true, true, true, true, true, true, true, true, true, true, true };

    private GameObject g;
    private DamageInfo di;
    private bool[] canUseSkill = new bool[13] { true, true, true, true, true, true, true, true, true, true, true, true, true };

    // Start is called before the first frame update
    void Awake()
    {
        ChangeBool.AddListener(ChangeReskill);
        skillCourutine = new IEnumerator[13];
        events.AddListener(LuizenPassiveEvent);
        normalevents.AddListener(LuizenPassiveEventNormalAttack);
        passiveForObject.AddListener(PassiveAttackSummon);
        skillcoolWhenCCed[2] = false;
        skillcoolWhenCCed[8] = false;
        skillcoolWhenCCed[9] = false;
        skillcoolWhenCCed[11] = false;
        skillcoolWhenCCed[12] = false;
        ConditionCheckwithString tmp;
        tmp.con = "_StigmaoftheSun";
        tmp.point = 1;
        tmp.time = 2;
        conditioncheck[0] = tmp;
    }

    // Update is called once per frame
    void Update()
    {
        skillRank = playerManager.GetEditedState("SkillPercent") * 0.01f;
        isOnGround = playerControl.GetIsGround();
        playerControl.ChangeMotionCheck("Weapon", isArmorOn);
        
        if (playerManager.GetCharacter() == 3)
        {
            playerControl.transform.GetChild(0).localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            playerControl.transform.GetChild(0).GetChild(0).GetComponent<PlayerAttacks>().SetDamageInfo(
               new DamageInfo(DamageType.Physics, Elemental.None, playerManager.GetEditedState("Power"), 0,
               0, 0, 0, ClowdControl.None, 0, 1, Ultimate?conditioncheck:null, UnityEngine.Random.Range(0, 100) < playerManager.GetEditedState("CriticalProb"), playerManager.GetEditedState("CriticalPoint"),
               playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(0).GetComponent<PlayerAttacks>().SetHitEvent(normalevents);

            playerControl.transform.GetChild(0).GetChild(1).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.4f * skillRank), 0,
                0, 0, 0, ClowdControl.Push, 0, 3.5f, Ultimate ? conditioncheck : null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(1).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(2).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.1f * skillRank), 0,
                0, 0, 0, ClowdControl.Airborne, 0, 3, Ultimate ? conditioncheck : null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(2).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(3).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.5f * skillRank), 0,
                0, 0, 0, ClowdControl.Airborne, 0, 5.5f, Ultimate ? conditioncheck : null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(3).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(4).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.3f * skillRank), 0,
                0, 0, 0, ClowdControl.Airborne, 0, 0.7f, Ultimate ? conditioncheck : null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(4).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(5).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.95f * skillRank), 0,
                0, 0, 0, ClowdControl.None, 0, 0, Ultimate ? conditioncheck : null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(5).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(6).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.95f * skillRank), 0,
                0, 0, 0, ClowdControl.None, 0, 0, Ultimate ? conditioncheck : null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(6).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(7).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.9f * skillRank), 0,
                0, 0, 0, ClowdControl.None, 0, 0, Ultimate ? conditioncheck : null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(7).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(8).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.1f * skillRank), 0,
                0, 0, 0, ClowdControl.None, 0, 0, Ultimate ? conditioncheck : null, false, 100, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(8).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(9).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.5f * skillRank), 0,
                0, 0, 0, ClowdControl.Push, 0, 2, Ultimate ? conditioncheck : null, false, 100, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(9).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(10).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.True, Elemental.None, 5, 0,
                0, 0, 0, ClowdControl.Airborne, 0, -10, Ultimate ? conditioncheck : null, false, 100, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(10).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(11).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.Ground, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.8f * skillRank), 0,
                0, 0, 0, ClowdControl.Push, 0, 3, Ultimate ? conditioncheck : null, false, 100, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(11).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(12).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.95f * skillRank), 0,
                0, 0, 0, ClowdControl.None, 0, 0, Ultimate ? conditioncheck : null, false, 100, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(12).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(13).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.Fire, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.9f * skillRank), 0,
                0, 0, 0, ClowdControl.None, 0, 0, Ultimate ? conditioncheck : null, false, 100, playerManager.GetEditedState("Luck"), playerControl.transform, ultimatePlus, null, false));
            playerControl.transform.GetChild(0).GetChild(13).GetComponent<PlayerAttacks>().SetHitEvent(events);

            playerControl.transform.GetChild(0).GetChild(15).gameObject.SetActive(reSkill);

            playerControl.transform.GetChild(0).GetChild(14).GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = 0.2f * powerStack;
            if (chargeTime > 0)
            {
                chargeTime -= Time.deltaTime;
            }
            else
            {
                chargeTime = 0;
                powerStack = 0;
            }
            if (powerStack == 0) CanChargeByNormalAttack = true;

        }


    }

    public override void ChildObjectSetting()
    {
        base.ChildObjectSetting();
        PlayerOBJ = Instantiate(characterObjects[0], playerControl.transform).transform;
        PlayerOBJ.SetAsFirstSibling();
    }

    public override void StoppingCourutineWhenCCed()
    {
        base.StoppingCourutineWhenCCed();
        for (int i = 0; i < 13; i++)
        {
            if (skillcoolWhenCCed[i]) canUseSkill[i] = true;
        }
    }

    protected override void ChildObjectSetDisactive()
    {
        if (playerManager.GetCharacter() == 2)
        {

        }
    }

    protected override IEnumerator Attacks()
    {
        CCinfo cc = playerControl.getCCInfo(0);
        float CCMoveValue = cc.cc == ClowdControl.Slow || cc.cc == ClowdControl.Weakleg ? 1 - (cc.ccStrength * 0.01f) : 1;
        int i = 2;
        while (Input.GetButton("Attack") && canAttack)
        {
            playerAnimation.SetBool("Attack", playerControl.ChangeMotionCheck("Attack", true));

            i = (i == 2 ? 0 : i + 1);
            if (cc.cc != ClowdControl.Bound && isOnGround) playerControl.ChangeVelocity(new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 15f * CCMoveValue, 0));
            yield return new WaitForSeconds(0.2f);
            playerControl.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            playerControl.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSeconds(attackSpeedTime[0] - 0.3f);
            if (!isOnGround) continue;

        }
        playerAnimation.SetBool("Attack", playerControl.ChangeMotionCheck("Attack", false));
        attackCooling = true;
    }

    protected override IEnumerator AttackSkillUsing(int code)
    {
        if (canUseSkill[code])
        {
            float t = 0.15f;
            bool tmp = (Input.GetButton("Attack") || playerManager.GetIsUsingMoveSkill() || !SkillCooling);
            while ((Input.GetButton("Attack") || playerManager.GetIsUsingMoveSkill() || !SkillCooling) && t > 0)
            {
                yield return null;
                t -= Time.deltaTime;
            }
            if (t <= 0) goto Finish;
            yield return new WaitWhile(() => (!attackCooling && (Input.GetButton("Attack") || playerManager.GetIsUsingMoveSkill() || !SkillCooling)));
            if ((Input.GetButton("Attack") || playerManager.GetIsUsingMoveSkill() || !SkillCooling)) goto Finish;
            SkillCooling = false;
            skillDoingInfo[code] = true;
            canUseSkill[code] = false;

            CCinfo cc = playerControl.getCCInfo(0);
            if (!CostCheckForUsingSkill(skills[code].costKey, skills[code].cost) || skillCoolingInfo[code, 0] > 0 || ((code == 1 || code == 3 || code == 6 || code == 7) && (cc.cc == ClowdControl.Bound || playerManager.GetIsFlying())))
                goto Finish2;
            playerManager.UsingMP(skills[code].cost);

            skillCourutine[1] = SolarStab();
            skillCourutine[2] = HandofPull();
            skillCourutine[3] = Sunrise();
            skillCourutine[4] = SideAttack();
            skillCourutine[5] = SpearCombo();
            skillCourutine[6] = SpearDescent();
            skillCourutine[7] = HighNoon();
            skillCourutine[8] = Sunshine();
            skillCourutine[9] = Sunset();
            skillCourutine[10] = SolarFlare();
            skillCourutine[11] = SunspotExplosion();
            skillCourutine[12] = NuclearFusion();

            yield return StartCoroutine(skillCourutine[code]);
            playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", false));
        Finish2:

            SkillCooling = true;
            usingCanMoveSkill = false;
        Finish:
            if (Skillchecking[code])
            {
                skillDoingInfo[code] = false;
                canUseSkill[code] = true;
            }
        }
    }

    protected override IEnumerator UltimateSkill()
    {
        float t = 0.15f;
        while ((Input.GetButton("Attack") || playerManager.GetIsUsingMoveSkill() || !SkillCooling) && t > 0)
        {
            yield return null;
            t -= Time.deltaTime;
        }
        if (t <= 0) goto Finish1;
        yield return new WaitWhile(() => !attackCooling || (Input.GetButton("Attack") || playerManager.GetIsUsingMoveSkill() || !SkillCooling));
        if ((Input.GetButton("Attack") || playerManager.GetIsUsingMoveSkill() || !SkillCooling)) goto Finish1;


        if (!CostCheckForUsingSkill(skills[0].costKey, skills[0].cost)) goto Finish1;
        SkillCooling = false;

        playerManager.UsingUT();
        playerManager.ConditionChange(particular.FindCondition("_Invincible"), 1, 3.2f);     
        Light2D GloberLight = GameObject.Find("Light 2D").GetComponent<Light2D>();
        playerAnimation.SetInteger("Skills", 15);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        playerRigid.gravityScale = 0;
        isMovingBySkill = true;
        
        playerRigid.velocity = Vector2.zero;
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(0.9f, 0.9f, 0.8f) * 80f);
        float ins = GloberLight.intensity;
        float j = 1;
        for (float i = ins; i > (ins * 0.5f); i -= (ins * 0.05f))
        {
            GloberLight.intensity = i;
            j -= 0.1f;
            playerControl.GetComponent<SpriteRenderer>().color = new Color(1, 1, j);
            yield return new WaitForSeconds(0.02f);
        }

        g = Instantiate(characterObjects[10], playerControl.transform.position, playerControl.transform.rotation, playerControl.transform);
        Transform realAttacktrans = g.transform.GetChild(0);
        realAttacktrans.GetComponent<PlayerAttacks>().SetHitEvent(events);
        di = realAttacktrans.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 30f + playerManager.GetEditedState("Power") * 30f);
        realAttacktrans.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        playerAnimation.SetTrigger("NextStep");
        yield return new WaitForSeconds(0.3f);
        float k = GloberLight.intensity;
        for (int i = 0; i < 10; i++)
        {
            j += 0.1f;
            playerControl.GetComponent<SpriteRenderer>().color = new Color(j, 1, 1);
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.1f);
        GameObject g2 = Instantiate(characterObjects[11], playerControl.transform.position + (Vector3.forward * 0.6f) + (Vector3.up * 0.2f), playerControl.transform.rotation, playerControl.transform);
        GloberLight.intensity = ins + 1.5f;
        realAttacktrans.gameObject.SetActive(true);
        playerRigid.gravityScale = 6;
        isMovingBySkill = false;
        yield return new WaitForSeconds(0.1f);
        Destroy(g);
        Ultimate = true;
        float UltchargeTime = 10;

        yield return new WaitForSeconds(0.1f);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", false));
        playerControl.CameraPointSetting(false, playerControl.transform.GetChild(0).GetChild(1).position);       
        SkillCooling = true;

        while (UltchargeTime > 0)
        {
            UltchargeTime -= Time.deltaTime;
            if (GloberLight != null) GloberLight.intensity = ins + (0.15f * UltchargeTime);
            yield return null;
        }
        Ultimate = false;
        UltchargeTime = 0;
        if (GloberLight != null) GloberLight.intensity = ins;

    Finish1:
        yield return null;
    }

    public void SkillFinish(int code, float cooltime) {
        skillDoingInfo[code] = false;
        canUseSkill[code] = true;
        Skillchecking[code] = true;
        SkillCooling = true;
        StartCoroutine(SkillCooltime(code, cooltime));
    }

    IEnumerator SolarStab()
    {
        PassiveAttackSummon();
        playerRigid.gravityScale = 0;
        playerRigid.drag = 0;
        playerRigid.velocity = Vector2.zero;
        playerAnimation.SetInteger("SkillStep", 0);
        playerAnimation.SetInteger("Skills", 0);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.15f);
        float time = 0;
        isMovingBySkill = true;
        playerControl.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        while (time < 0.29f)
        {
            playerRigid.velocity = new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 30f, 0.1f);
            time += Time.deltaTime;
            yield return null;
        }
        playerControl.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        isMovingBySkill = false;
        playerRigid.gravityScale = 6;
        time = 0;
        reSkill = true;
        while (time < 0.09f)
        {           
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 1 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 1 ? 2 : 3))))
            {
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }
        reSkill = false;
        if (time < 0.08f)
        {
            reSkill = false;
            PassiveAttackSummon();
            playerAnimation.SetInteger("SkillStep", 1);
            playerAnimation.SetTrigger("NextStep");
            playerRigid.velocity = new Vector2(playerRigid.velocity.x*0.25f, playerRigid.velocity.y);
            playerControl.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            playerControl.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.12f);
        }    
        StartCoroutine(SkillCooltime(1, skills[1].skillCooltime));
    }

    IEnumerator HandofPull()
    {
        PassiveAttackSummon();
        Skillchecking[2] = false;
        g = Instantiate(characterObjects[3], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 9 + Vector3.back, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<PlayerAttacks>().GetComponent<PlayerAttacks>().SetHitEvent(events);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.1f);
        di.damageChangeFunc.Add(ultimatePlus);
        di.conditionCheck = Ultimate ? conditioncheck : null;
        UnityEvent<int, float> SetDoingbool = new UnityEvent<int, float>();
        SetDoingbool.AddListener(SkillFinish);
        g.GetComponent<Luizen_SkillsNextSkills>().Setting((uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 2.1f), ChangeBool, events, passiveForObject, SetDoingbool, skills[2].skillCooltime, Ultimate);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();
        yield return null;
    }

    IEnumerator Sunrise()
    {
        PassiveAttackSummon();
        playerAnimation.SetInteger("SkillStep", 0);
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        playerRigid.drag = 0;
        isMovingBySkill = true;
        playerControl.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        for (int i = 13; i > 0; i--) {
            playerRigid.velocity = new Vector2(playerRigid.velocity.x, i * 3f);
            yield return new WaitForSeconds(0.02f);
        }
        playerControl.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);

        float time = 0;
        reSkill = true;
        while (time < 0.105f)
        {
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 3 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 3 ? 2 : 3))))
            {
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }
        reSkill = false;
        if (time < 0.105f)
        {
            playerRigid.gravityScale = 0;
            playerRigid.velocity = new Vector2(0, 0);
            PassiveAttackSummon();
            playerAnimation.SetInteger("SkillStep", 1);
            playerAnimation.SetTrigger("NextStep");           
            yield return new WaitForSeconds(0.25f);
            playerControl.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            playerControl.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
        playerRigid.velocity = new Vector2(playerRigid.velocity.x, 0);
        isMovingBySkill = false;
        playerRigid.gravityScale = 6;
        StartCoroutine(SkillCooltime(3, skills[3].skillCooltime));
    }

    IEnumerator SideAttack()
    {
        PassiveAttackSummon();
        playerAnimation.SetInteger("SkillStep", 0);
        playerAnimation.SetInteger("Skills", 2);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        playerControl.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerControl.transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
        playerControl.transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerControl.transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.14f);
        
        float time = 0;
        reSkill = true;
        while (time < 0.09f)
        {
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 4 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 4 ? 2 : 3))))
            {
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }
        reSkill = false;
        if (time < 0.09f)
        {
            PassiveAttackSummon();
            playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(0.9f, 0.9f, 0) * 40f);
            playerAnimation.SetInteger("SkillStep", 1);
            playerAnimation.SetTrigger("NextStep");
            playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.25f, playerRigid.velocity.y);
            yield return new WaitForSeconds(0.25f);
            g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3.up * 0.2f) + (Vector3.forward * 0.1f), playerControl.transform.rotation);
            g.transform.localScale *= 1.5f;
            g.GetComponent<PlayerAttacks>().SetHitEvent(events);
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1.6f);
            di.damageChangeFunc.Add(ultimatePlus);
            di.conditionCheck = Ultimate ? conditioncheck : null;
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            yield return new WaitForSeconds(0.07f);
        }

        StartCoroutine(SkillCooltime(4, skills[4].skillCooltime));
    }

    IEnumerator SpearCombo()
    {
        PassiveAttackSummon();
        playerAnimation.SetInteger("SkillStep", 0);
        playerAnimation.SetInteger("Skills", 3);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.07f);
        playerControl.transform.GetChild(0).GetChild(7).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.14f);
        playerControl.transform.GetChild(0).GetChild(7).gameObject.SetActive(false);
        float time = 0;
        reSkill = true;
        while (time < 0.09f)
        {
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 5 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 5 ? 2 : 3))))
            {
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }
        reSkill = false;
        if (time < 0.09f)
        {
            time = 0;
            PassiveAttackSummon();
            playerAnimation.SetInteger("SkillStep", 1);
            playerAnimation.SetTrigger("NextStep");
            playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.25f, playerRigid.velocity.y);
            yield return new WaitForSeconds(0.07f);
            playerControl.transform.GetChild(0).GetChild(8).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.14f);
            playerControl.transform.GetChild(0).GetChild(8).gameObject.SetActive(false);
            reSkill = true;
            while (time < 0.09f)
            {
                if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 5 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 5 ? 2 : 3))))
                {
                    break;
                }
                time += Time.deltaTime;
                yield return null;
            }
            reSkill = false;
            if (time < 0.09f)
            {
                CCinfo cc = playerControl.getCCInfo(0);
                PassiveAttackSummon();
                playerAnimation.SetInteger("SkillStep", 2);
                playerAnimation.SetTrigger("NextStep");
                if (cc.cc != ClowdControl.Bound) playerRigid.velocity = new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 9f, 0);
                yield return new WaitForSeconds(0.1f);
                playerControl.transform.GetChild(0).GetChild(9).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.17f);
                playerControl.transform.GetChild(0).GetChild(9).gameObject.SetActive(false);
                yield return new WaitForSeconds(0.05f);
            }
        }

        StartCoroutine(SkillCooltime(5, skills[5].skillCooltime));
    }

    IEnumerator SpearDescent()
    {
        if (!isOnGround)
        {
            PassiveAttackSummon();
            playerAnimation.SetInteger("Skills", 4);
            playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
            playerRigid.gravityScale = 0;
            playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.1f, 0);
            yield return new WaitForSeconds(0.15f);
            playerRigid.gravityScale = 6;
            playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.1f, -30);
            playerControl.transform.GetChild(0).GetChild(10).gameObject.SetActive(true);
            yield return new WaitUntil(()=>isOnGround);
            playerControl.transform.GetChild(0).GetChild(10).gameObject.SetActive(false);
            float time = 0;
            reSkill = true;
            while (time < 0.09f)
            {
                if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 6 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 6 ? 2 : 3))))
                {
                    break;
                }
                time += Time.deltaTime;
                yield return null;
            }
            reSkill = false;
            if (time < 0.09f)
            {
                PassiveAttackSummon();
                playerAnimation.SetInteger("SkillStep", 1);
                playerAnimation.SetTrigger("NextStep");
                playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.25f, playerRigid.velocity.y);
                playerControl.transform.GetChild(0).GetChild(11).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                playerControl.transform.GetChild(0).GetChild(11).gameObject.SetActive(false);
            }
            StartCoroutine(SkillCooltime(6, skills[6].skillCooltime));

        }
        else
        {
            playerManager.GetComponent<ParticularEvent>().SummonFlatText(playerControl.transform.position + Vector3.up, playerControl.transform).Changetext(0, 5, FontStyle.Italic, Color.white);
            playerManager.UsingMP(-skills[6].cost);
            StartCoroutine(SkillCooltime(6, 0.2f));
        }
    }
    IEnumerator HighNoon()
    {
        if (!isOnGround)
        {
            while (!isOnGround)
            {
                PassiveAttackSummon();
                playerRigid.AddForce(new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 10f, 4f), ForceMode2D.Impulse);
                playerAnimation.SetInteger("Skills", 5);
                playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
                yield return new WaitForSeconds(0.1f);
                playerControl.transform.GetChild(0).GetChild(12).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                playerControl.transform.GetChild(0).GetChild(12).gameObject.SetActive(false);
                float time = 0;
                reSkill = true;
                while (time < 0.09f)
                {
                    if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 7 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 7 ? 2 : 3))))
                    {
                        break;
                    }
                    time += Time.deltaTime;
                    yield return null;
                }
                reSkill = false;
                if (time < 0.09f)
                {
                    playerAnimation.SetTrigger("NextStep");
                    continue;
                }
                break;
            }
            StartCoroutine(SkillCooltime(7, skills[7].skillCooltime));
        }
        else
        {
            playerManager.GetComponent<ParticularEvent>().SummonFlatText(playerControl.transform.position + Vector3.up, playerControl.transform).Changetext(0, 5, FontStyle.Italic, Color.white);
            playerManager.UsingMP(-skills[7].cost);
            StartCoroutine(SkillCooltime(7, 0.2f));
        }
        
    }

    IEnumerator Sunshine()
    {
        PassiveAttackSummon();
        Skillchecking[8] = false;
        g = Instantiate(characterObjects[5], playerControl.transform.position + Vector3.forward*0.1f, playerControl.transform.rotation, playerControl.transform);
        g.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(0.8f, 0.7f, 0) * 35f);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);       
        UnityEvent<int, float> SetDoingbool = new UnityEvent<int, float>();
        SetDoingbool.AddListener(SkillFinish);
        g.GetComponent<Luizen_SkillsNextSkills>().Setting((uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1.5f), ChangeBool, events, passiveForObject, SetDoingbool, skills[8].skillCooltime, Ultimate);
        yield return null;   
    }

    IEnumerator Sunset()
    {
        PassiveAttackSummon();
        Skillchecking[9] = false;
        g = Instantiate(characterObjects[6], playerControl.transform.position + Vector3.up * 15f+ Vector3.back, playerControl.transform.rotation);
        g.GetComponent<PlayerAttacks>().SetHitEvent(events);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1.75f);
        di.conditionCheck = Ultimate ? conditioncheck : null;
        di.damageChangeFunc.Add(ultimatePlus);
        UnityEvent<int, float> SetDoingbool = new UnityEvent<int, float>();
        SetDoingbool.AddListener(SkillFinish);
        g.GetComponent<SunSet>().Setting((uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 2.2f), events, passiveForObject, SetDoingbool, skills[9].skillCooltime, playerControl.transform, Ultimate);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<Rigidbody2D>().velocity = new Vector2(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, -0.7f).normalized * 31f;
        yield return null;
    }

    IEnumerator SolarFlare()
    {
        PassiveAttackSummon();
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        playerAnimation.SetInteger("SkillStep", 0);
        playerAnimation.SetInteger("Skills", 6);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.1f);
        playerControl.transform.GetChild(0).GetChild(13).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerControl.transform.GetChild(0).GetChild(13).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.04f);
        float time = 0;
        reSkill = true;
        while (time < 0.09f)
        {
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 10 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 10 ? 2 : 3))))
            {
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }
        reSkill = false;
        if (time < 0.09f)
        {
            PassiveAttackSummon();
            playerAnimation.SetInteger("SkillStep", 1);
            playerAnimation.SetTrigger("NextStep");
            playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.25f, playerRigid.velocity.y);
            yield return new WaitForSeconds(0.2f);
            g = Instantiate(characterObjects[7], playerControl.transform.position + (playerControl.GetComponent<SpriteRenderer>().flipX ? Vector3.left : Vector3.right) * 2.3f + Vector3.back, playerControl.transform.rotation);
            g.GetComponent<PlayerAttacks>().SetHitEvent(events);
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 2.5f);
            di.damageChangeFunc.Add(ultimatePlus);
            di.conditionCheck = Ultimate ? conditioncheck : null;
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            yield return new WaitForSeconds(0.12f);
        }
        StartCoroutine(SkillCooltime(10, skills[10].skillCooltime));
    }

    IEnumerator SunspotExplosion()
    {
        playerManager.ConditionChange(particular.FindCondition("_Protection"), 1, 1);
        PassiveAttackSummon();
        Skillchecking[11] = false;
        g = Instantiate(characterObjects[8], playerControl.transform.position + Vector3.back * 0.1f, playerControl.transform.rotation, playerControl.transform);
        g.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(0.8f, 0.7f, 0) * 40f);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        UnityEvent<int, float> SetDoingbool = new UnityEvent<int, float>();
        SetDoingbool.AddListener(SkillFinish);
        g.GetComponent<Luizen_SkillsNextSkills>().Setting((uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1.35f), ChangeBool, events, passiveForObject, SetDoingbool, skills[11].skillCooltime, Ultimate);
        yield return null;
    }

    IEnumerator NuclearFusion()
    {
        PassiveAttackSummon();
        Skillchecking[12] = false;
        g = Instantiate(characterObjects[9], playerControl.transform.position, playerControl.transform.rotation, playerControl.transform);
        Transform realAttacktrans = g.transform.GetChild(0);
        realAttacktrans.GetComponent<PlayerAttacks>().SetHitEvent(events);
        di = realAttacktrans.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.9f);
        di.damageChangeFunc.Add(ultimatePlus);
        di.conditionCheck = Ultimate ? conditioncheck : null;
        UnityEvent<int, float> SetDoingbool = new UnityEvent<int, float>();
        SetDoingbool.AddListener(SkillFinish);
        g.GetComponent<Luizen_SkillsNextSkills>().Setting((uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 2f), ChangeBool, events, passiveForObject, SetDoingbool, skills[12].skillCooltime, Ultimate);
        realAttacktrans.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return null;
    }


    public void PassiveAttackSummon() {
        g = Instantiate(characterObjects[1], playerControl.transform.position, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1) * 0.75f;
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.25f * (powerStack+1));
        di.damageChangeFunc.Add(ultimatePlus);
        di.conditionCheck = Ultimate ? conditioncheck : null;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
    }

    public void LuizenPassiveEvent() {
        if (powerStack >= 5)
        {
            chargeTime = 0;
            powerStack = 0;
            g = Instantiate(characterObjects[2], playerControl.transform.position, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 3f + playerManager.GetEditedState("Power") * 3f);
            di.damageChangeFunc.Add(ultimatePlus);
            di.conditionCheck = Ultimate ? conditioncheck : null;
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        }
        else
        {
            chargeTime = 0.75f;
            powerStack += 1;      
        }
        CanChargeByNormalAttack = true;
    }

    public void LuizenPassiveEventNormalAttack()
    {
        if (CanChargeByNormalAttack)
        {
            if (powerStack >= 5)
            {
                chargeTime = 0;
                powerStack = 0;
                g = Instantiate(characterObjects[2], playerControl.transform.position, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 3f + playerManager.GetEditedState("Power") * 3f);
                di.damageChangeFunc.Add(ultimatePlus);
                di.conditionCheck = Ultimate ? conditioncheck : null;
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            }
            else
            {
                chargeTime = 0.75f;
                powerStack += 1;
            }
        }
        else chargeTime = 0.75f;

        CanChargeByNormalAttack = false;
    }

    
}
