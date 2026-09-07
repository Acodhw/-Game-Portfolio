using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class MainCharacter : CharacterBase
{
    bool isOnGround = true;
    int[] ChargedSlash = { 0, 0 };
    float chargeTime = 10;    
    private GameObject g;
    private DamageInfo di;

    // Start is called before the first frame update
    void Awake()
    {
        skillCourutine = new IEnumerator[13];
        skillCourutine[0] = UltimateSkill();
        skillCourutine[1] = DoubleAttack();
        skillCourutine[2] = SwordStance();
        skillCourutine[3] = SpinAttack();
        skillCourutine[4] = Raise();
        skillCourutine[5] = SwordAuraStep();
        skillCourutine[6] = AbsoluteAttack();
        skillCourutine[7] = TornadoSlash();
        skillCourutine[8] = RedDance();
        skillCourutine[9] = ThunderboltSword();
        skillCourutine[10] = Counter();
        skillCourutine[11] = StormAssult();
        skillCourutine[12] = ElementalCombo();
    }

    // Update is called once per frame
    void Update()
    {
        skillRank = playerManager.GetEditedState("SkillPercent") * 0.01f;
        isOnGround = playerControl.GetIsGround();
        playerControl.ChangeMotionCheck("Weapon", isArmorOn);

        if (playerManager.GetCharacter() == 0)
        {
            playerControl.transform.GetChild(0).localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            playerControl.transform.GetChild(0).GetChild(11).localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            playerControl.transform.GetChild(0).GetChild(11).GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = 0.1f * (10 - chargeTime);

            playerControl.transform.GetChild(0).GetChild(11).GetChild(1).GetComponent<UnityEngine.UI.Image>().color = (ChargedSlash[0] == 1 ? new Color(0, 1, 1) : ChargedSlash[0] == 2 ? new Color(1, 0, 0) : new Color(1, 1, 0));
            playerControl.transform.GetChild(0).GetChild(11).GetChild(1).GetComponent<UnityEngine.UI.Image>().fillAmount = ChargedSlash[1] * 0.333f;
            playerControl.transform.GetChild(0).GetChild(11).GetChild(1).localPosition = new Vector3((3 - ChargedSlash[1]) * 0.125f, 1.725f, -1);

            playerControl.transform.GetChild(0).GetChild(0).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, playerManager.GetEditedState("Power"), 0,
                0, 0, 0, ClowdControl.Push, 0, 1, null, Random.Range(0, 100) < playerManager.GetEditedState("CriticalProb"), playerManager.GetEditedState("CriticalPoint"),
                playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));;

            playerControl.transform.GetChild(0).GetChild(1).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, playerManager.GetEditedState("Power"), 0,
                0, 0, 0, ClowdControl.Push, 0, 1, null, Random.Range(0, 100) < playerManager.GetEditedState("CriticalProb"), playerManager.GetEditedState("CriticalPoint"),
                playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));

            if (passiveOn)
            {
                playerControl.transform.GetChild(0).GetChild(2).GetComponent<PlayerAttacks>().SetDamageInfo(
                    new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.5f), 0,
                    0, 0, 0, ClowdControl.Push, 0, 1.5f, null, Random.Range(0, 100) < playerManager.GetEditedState("CriticalProb"), playerManager.GetEditedState("CriticalPoint"),
                    playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));
            }
            else
            {
                playerControl.transform.GetChild(0).GetChild(2).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, playerManager.GetEditedState("Power"), 0,
                0, 0, 0, ClowdControl.Push, 0, 1, null, Random.Range(0, 100) < playerManager.GetEditedState("CriticalProb"), playerManager.GetEditedState("CriticalPoint"),
                playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>  >)null, null, false));

            }

            playerControl.transform.GetChild(0).GetChild(3).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, playerManager.GetEditedState("Power"), 0,
                0, 0, 0, ClowdControl.Push, 0, 1, null, Random.Range(0, 100) < playerManager.GetEditedState("CriticalProb"), playerManager.GetEditedState("CriticalPoint"),
                playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));

            playerControl.transform.GetChild(0).GetChild(4).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.25f * skillRank), 0,
                0, 0, 0, ClowdControl.Slow, 0.75f, 25, null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));

            playerControl.transform.GetChild(0).GetChild(5).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.5f * skillRank), 0,
                0, 0, 0, ClowdControl.Airborne, 0, 5f, null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));

            playerControl.transform.GetChild(0).GetChild(6).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.Wind, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.8f * skillRank), 0,
                0, 0, 0, ClowdControl.Airborne, 0, 4.5f, null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));

            playerControl.transform.GetChild(0).GetChild(7).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.None, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 3.5f * skillRank), 0,
                0, 0, 0, ClowdControl.None, 0, 0, null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));

            playerControl.transform.GetChild(0).GetChild(8).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.Wind, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.15f * skillRank), 0,
                0, 0, 0, ClowdControl.None, 0, 0, null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, true, 0.1f));

            playerControl.transform.GetChild(0).GetChild(9).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.Multi, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.95f * skillRank), 0,
                0, 0, 0, ClowdControl.None, 0, 0, null, false, 0, playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));

            playerControl.transform.GetChild(0).GetChild(10).GetComponent<PlayerAttacks>().SetDamageInfo(
                new DamageInfo(DamageType.Physics, Elemental.Multi, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 85f * skillRank), 0,
                0, 0, 0, ClowdControl.Stun, 5, 0, null, true, 100, playerManager.GetEditedState("Luck"), playerControl.transform, (List<System.Func<States, uint, uint>>)null, null, false));
        }
        if (ChargedSlash[0] != 0 && ChargedSlash[1] > 0)
        {
            chargeTime += Time.deltaTime;
            if (chargeTime >= 10) { ChargedSlash[0] = 0; ChargedSlash[1] = 0; }
        }
        else {
            chargeTime = 10;           
        }
    }

    public override void ChildObjectSetting()
    {
        base.ChildObjectSetting();
        PlayerOBJ = Instantiate(characterObjects[0], playerControl.transform).transform;
        PlayerOBJ.SetAsFirstSibling();
    }

    protected override void ChildObjectSetDisactive()
    {
        if (playerManager.GetCharacter() == 0)
        {
            playerControl.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(7).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(8).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(9).gameObject.SetActive(false);
            playerControl.transform.GetChild(0).GetChild(10).gameObject.SetActive(false);
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
            if (isOnGround)
            {
                i = (i == 2 ? 0 : i + 1);
                if(cc.cc != ClowdControl.Bound) playerControl.ChangeVelocity(new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 9f * CCMoveValue, 0));
                yield return new WaitForSeconds(0.1075f);
                if (playerControl.getCCInfo(1).cc != ClowdControl.Blind) playerControl.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                if (playerControl.getCCInfo(1).cc != ClowdControl.Blind) useChargeAttack();
                yield return new WaitForSeconds(0.02f);
                playerControl.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                yield return new WaitForSeconds(attackSpeedTime[i] * 0.75f - 0.1275f);
                if (!isOnGround) continue;
            }
            else
            {
                i = 2;
                yield return new WaitForSeconds(0.1075f);
                if (playerControl.getCCInfo(1).cc != ClowdControl.Blind) playerControl.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                if (playerControl.getCCInfo(1).cc != ClowdControl.Blind) useChargeAttack();
                yield return new WaitForSeconds(0.02f);
                playerControl.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
                yield return new WaitForSeconds(0.31f * 0.75f - 0.1275f);
                if (!playerManager.GetIsUsingMoveSkill() || SkillCooling) break;
                if (isOnGround) playerAnimation.SetBool("Attack", playerControl.ChangeMotionCheck("Attack", false));
            }
        }
        playerAnimation.SetBool("Attack", playerControl.ChangeMotionCheck("Attack", false));
        attackCooling = true;
    }

    protected override IEnumerator AttackSkillUsing(int code)
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

        CCinfo cc = playerControl.getCCInfo(0);
        if (!CostCheckForUsingSkill(skills[code].costKey, skills[code].cost) || skillCoolingInfo[code, 0] > 0 || ((code == 5 || code == 7 || code == 12) && (cc.cc == ClowdControl.Bound || playerManager.GetIsFlying())))
            goto Finish2;
        playerManager.UsingMP(skills[code].cost);

        skillCourutine[1] = DoubleAttack();
        skillCourutine[2] = SwordStance();
        skillCourutine[3] = SpinAttack();
        skillCourutine[4] = Raise();
        skillCourutine[5] = SwordAuraStep();
        skillCourutine[6] = AbsoluteAttack();
        skillCourutine[7] = TornadoSlash();
        skillCourutine[8] = RedDance();
        skillCourutine[9] = ThunderboltSword();
        skillCourutine[10] = Counter();
        skillCourutine[11] = StormAssult();
        skillCourutine[12] = ElementalCombo();

        yield return StartCoroutine(skillCourutine[code]);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", false));
    Finish2:
        
        SkillCooling = true;
        usingCanMoveSkill = false;
    Finish:
        skillDoingInfo[code] = false;
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


        if(!CostCheckForUsingSkill(skills[0].costKey, skills[0].cost)) goto Finish1;
        SkillCooling = false;

        
        playerManager.UsingUT();
        playerManager.ConditionChange(particular.FindCondition("_Invincible"), 1, 1.7f);
        Light2D GloberLight = GameObject.Find("Light 2D").GetComponent<Light2D>();
        playerRigid.gravityScale = 0;
        isMovingBySkill = true;
        playerRigid.velocity = Vector2.zero;
        float ins = GloberLight.intensity;
        float j = 1;
        for (float i = ins; i > (ins * 0.5f); i -= (ins * 0.05f))
        {
            playerControl.GetComponent<SpriteRenderer>().color = new Color(1, j, j);
            GloberLight.intensity = i;
            j -= 0.1f;
            yield return new WaitForSeconds(0.02f);
        }

        float tmp = (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        playerAnimation.SetInteger("Skills", 15);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        for (float i = 0; i < 18; i += (18 * 0.125f))
        {
            playerControl.GetComponent<SpriteRenderer>().color = new Color(1, j, j);
            j += 0.062f;
            playerRigid.velocity = new Vector2((18f - i) * tmp, (24f - ((i / 3) * 4)));
            yield return new WaitForSeconds(0.05f);
        }
        playerControl.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0, 0) * 60f);
        playerRigid.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.45f);
        playerControl.transform.GetChild(0).GetChild(10).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        playerControl.transform.GetChild(0).GetChild(10).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        for (float i = (ins * 0.5f); i < ins; i += (ins * 0.1f))
        {
            GloberLight.intensity = i;
            yield return new WaitForSeconds(0.02f);
        }        
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", false));
        playerRigid.gravityScale = 6;
        isMovingBySkill = false;
        GloberLight.intensity = ins;
        SkillCooling = true;
    Finish1:
        yield return null;
    }

    IEnumerator DoubleAttack() 
    {
        playerAnimation.SetInteger("Skills", 0);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.12f);
        useChargeAttack();
        g = Instantiate(characterObjects[1], playerControl.transform.position, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.criticalPercent = Random.Range(1, 101) < playerManager.GetEditedState("CriticalProb");
        di.criticalPoint = playerManager.GetEditedState("CriticalPoint");
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.75f);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(0.12f);
        useChargeAttack();
        g = Instantiate(characterObjects[2], playerControl.transform.position, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        di.criticalPercent = Random.Range(1, 101) < playerManager.GetEditedState("CriticalProb");
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(0.06f);
        StartCoroutine(SkillCooltime(1, skills[1].skillCooltime));
    }

    IEnumerator SwordStance() 
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        g = Instantiate(characterObjects[3], playerControl.transform.position - Vector3.forward + (Vector3.up * 0.96f), playerControl.transform.rotation);
        g.GetComponent<ShotsEvent>().Shot();
        yield return new WaitForSeconds(0.2f);
        chargeTime = 0;
        ChargedSlash[0] = 1;
        ChargedSlash[1] = 3;
        StartCoroutine(SkillCooltime(2, skills[2].skillCooltime));
    }

    IEnumerator SpinAttack() 
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        if (!isOnGround && playerRigid.velocity.y < 7)
            playerRigid.velocity = new Vector2(playerRigid.velocity.x, 7f);
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.075f);
        playerControl.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.025f);
        playerControl.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.22f);
        StartCoroutine(SkillCooltime(3, skills[3].skillCooltime));
    }

    IEnumerator Raise() 
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.3f, playerRigid.velocity.y);
        usingCanMoveSkill = true;
        playerAnimation.SetInteger("Skills", 2);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.225f);
        playerControl.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.025f);
        playerControl.transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(SkillCooltime(4, skills[4].skillCooltime));
    }

    IEnumerator SwordAuraStep() 
    {            
        playerAnimation.SetInteger("Skills", 3);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        float time = 0;
        while (time < 0.4f) {
            playerControl.ChangeVelocity(new Vector2(17f * (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1), playerRigid.velocity.y));
            time += Time.deltaTime;
            yield return null;
        }
        playerControl.ChangeVelocity(new Vector2(0, playerRigid.velocity.y));
        g = Instantiate(characterObjects[5], playerControl.transform.position, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt((playerManager.GetEditedState("Intellect") + playerManager.GetEditedState("Power") * 0.1f) * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();
        yield return new WaitForSeconds(0.09f);
        StartCoroutine(SkillCooltime(5, skills[5].skillCooltime));
    }

    IEnumerator AbsoluteAttack()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        g = Instantiate(characterObjects[6], playerControl.transform.position - Vector3.forward, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        yield return new WaitForSeconds(0.2f);
        chargeTime = 0;
        ChargedSlash[0] = 2;
        ChargedSlash[1] = 2;
        StartCoroutine(SkillCooltime(6, skills[6].skillCooltime));
    }

    IEnumerator TornadoSlash()
    {
        isMovingBySkill = true;
        playerRigid.gravityScale = 0;
        playerRigid.drag = 0;
        playerRigid.velocity = new Vector2(playerRigid.velocity.x, 17.5f);
        playerAnimation.SetInteger("Skills", 4);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        playerControl.transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.075f);
        playerControl.transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.225f);

        playerControl.transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.075f);
        playerControl.transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.225f);
        isMovingBySkill = false;
        playerRigid.gravityScale = 6;
        playerRigid.velocity = new Vector2(playerRigid.velocity.x, 20f);
        playerControl.transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.025f);
        playerControl.transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(SkillCooltime(7, skills[7].skillCooltime));
    }

    IEnumerator RedDance() 
    {
        if (isOnGround)
        {
            playerRigid.velocity = new Vector2(playerRigid.velocity.x, 0);
            playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
            playerAnimation.SetInteger("Skills", 5);
            playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
            yield return new WaitForSeconds(0.6f);
            g = Instantiate(characterObjects[8], playerControl.transform.position + new Vector3((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 1f, 0, -1), playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 2 + playerManager.GetEditedState("Power") * 1.5f);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            yield return new WaitForSeconds(0.09f);
            StartCoroutine(SkillCooltime(8, skills[8].skillCooltime));

        }
        else
        {
            playerManager.GetComponent<ParticularEvent>().SummonFlatText(playerControl.transform.position + Vector3.up, playerControl.transform).Changetext(1, 5, FontStyle.Italic, Color.white);
            playerManager.UsingMP(-skills[8].cost);
            StartCoroutine(SkillCooltime(8, 0.2f)); 
        }
    }

    IEnumerator ThunderboltSword()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.75f, playerRigid.velocity.y);
        g = Instantiate(characterObjects[9], playerControl.transform.position - Vector3.forward, playerControl.transform.rotation, playerControl.transform);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.75f);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(0.24f);
        chargeTime = 0;
        ChargedSlash[0] = 3;
        ChargedSlash[1] = 3;
        StartCoroutine(SkillCooltime(9, skills[9].skillCooltime));
    }

    IEnumerator Counter() 
    {
        float tmptime = 0;
        bool hit = false;
        playerManager.ConditionChange(particular.FindCondition("_Protection"), 1, 0.24f);
        playerControl.resetMatatrial();
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(0, 1, 1) * 15f);
        playerAnimation.SetInteger("Skills", 6);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        while (tmptime < 0.24f)
        {
            if (playerManager.GetIsHit()) hit = true;
            tmptime += Time.deltaTime;
            yield return null;
        }
        if (hit)
        {
            playerAnimation.SetTrigger("Counter");
            playerControl.transform.GetChild(0).GetChild(7).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.075f);
            playerControl.transform.GetChild(0).GetChild(7).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.225f);
        }
        StartCoroutine(SkillCooltime(10, skills[10].skillCooltime));
    }

    IEnumerator StormAssult()
    {
        playerAnimation.SetInteger("Skills", 7);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        playerControl.transform.GetChild(0).GetChild(8).gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        float t = 1;
        while ((!(Input.GetButton("Horizontal") || Input.GetButton("Jump")) && CostCheckForUsingSkill(skills[11].costKey, skills[11].cost))) {        
            yield return null;
            t += Time.deltaTime;
            if (t >= 1) { 
                playerManager.UsingMP(skills[11].cost);
                t -= 1;
            }
        }
        playerControl.transform.GetChild(0).GetChild(8).gameObject.SetActive(false);
        StartCoroutine(SkillCooltime(11, skills[11].skillCooltime));
    }

    IEnumerator ElementalCombo() 
    {
        playerRigid.drag = 0;
        playerControl.ChangeVelocity(new Vector2(35 * (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1), playerRigid.velocity.y));
        playerAnimation.SetInteger("Skills", 8);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.075f);
        playerControl.transform.GetChild(0).GetChild(9).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.025f);
        playerControl.transform.GetChild(0).GetChild(9).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.075f);
        playerControl.transform.GetChild(0).GetChild(9).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.025f);
        playerControl.transform.GetChild(0).GetChild(9).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.075f);
        playerControl.transform.GetChild(0).GetChild(9).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.025f);
        playerControl.transform.GetChild(0).GetChild(9).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(SkillCooltime(12, skills[12].skillCooltime));
    }

    void useChargeAttack()
    {
        GameObject g;
        DamageInfo di;
        if (!(ChargedSlash[0] != 0 && ChargedSlash[1] > 0))
            return;

        switch (ChargedSlash[0])
        {
            case 1:
                g = Instantiate(characterObjects[4], playerControl.transform.position + Vector3.right * (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 0.9f + Vector3.back, playerControl.transform.rotation);
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.5f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().Shot();
                break;
            case 2:
                g = Instantiate(characterObjects[7], playerControl.transform.position + Vector3.right * (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 1.1f + Vector3.back, playerControl.transform.rotation);
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().Shot();
                break;
            case 3:
                g = Instantiate(characterObjects[10], playerControl.transform.position + Vector3.right * (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 1.1f + Vector3.back, playerControl.transform.rotation);
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.8f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().Shot();
                break;
        }
        ChargedSlash[1]--;

    }

}
