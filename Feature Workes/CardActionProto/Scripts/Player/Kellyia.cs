using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Kellyia : CharacterBase
{
    bool isOnGround = true;
    float chargeTime = 0;

    private GameObject g;
    private DamageInfo di;

    // Start is called before the first frame update
    void Awake()
    {
        skillCourutine = new IEnumerator[13];
    }

    // Update is called once per frame
    void Update()
    {
        skillRank = playerManager.GetEditedState("SkillPercent") * 0.01f;
        isOnGround = playerControl.GetIsGround();
        playerControl.ChangeMotionCheck("Weapon", isArmorOn);

        if (playerManager.GetCharacter() == 2)
        {
            playerControl.transform.GetChild(0).localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            playerControl.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = 0.333f * chargeTime;
        }
        
        if (chargeTime <= 0)
        {
            chargeTime = 0;
        }
        else
        {
            chargeTime -= Time.deltaTime;        
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
        if (playerManager.GetCharacter() == 2)
        {
            
        }
    }

    protected override IEnumerator Attacks()
    {
        CCinfo cc = playerControl.getCCInfo(0);
        float CCMoveValue = cc.cc == ClowdControl.Slow || cc.cc == ClowdControl.Weakleg ? 1 - (cc.ccStrength * 0.01f) : 1;
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        while (Input.GetButton("Attack") && canAttack)
        {
            canFlip = false;
            playerAnimation.SetBool("Attack", playerControl.ChangeMotionCheck("Attack", true));
            yield return new WaitForSeconds(attackSpeedTime[0] * 0.15f);
            g = Instantiate(characterObjects[1], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.75f);
            di.damageChangeFunc.Add((state, damage) => state.isinConditionList("_Bleeding") ? (uint)Mathf.RoundToInt(damage * 1.5f) : damage);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().Shot();
            yield return new WaitForSeconds(attackSpeedTime[0] * 0.34f);
            if (chargeTime > 0)
            {
                g = Instantiate(characterObjects[2], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.45f);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.GetComponent<ShotsEvent>().Shot();
            }
            canFlip = true;
            yield return new WaitForSeconds(attackSpeedTime[0] * 0.5f - 0.05f);
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
        if (!CostCheckForUsingSkill(skills[code].costKey, skills[code].cost + ((code == 9 || code == 10) ? Mathf.RoundToInt(playerManager.GetState("MaxMP")* 0.25f) : 0)) || skillCoolingInfo[code, 0] > 0 || ((code == 6 || code == 7 || code == 8) && (cc.cc == ClowdControl.Bound || playerManager.GetIsFlying())))
            goto Finish2;
        playerManager.UsingMP(skills[code].cost + ((code == 9 || code == 10) ? Mathf.RoundToInt(playerManager.GetState("MaxMP") * 0.25f) : 0));

        skillCourutine[1] = DualPistol();
        skillCourutine[2] = FireShot();
        skillCourutine[3] = PainExacerbation();
        skillCourutine[4] = FallingLights();
        skillCourutine[5] = ExplosionBullet();
        skillCourutine[6] = Phoenix();
        skillCourutine[7] = Firestorm();
        skillCourutine[8] = RunandGun();
        skillCourutine[9] = SplitShot();
        skillCourutine[10] = Saturation();
        skillCourutine[11] = FatalShot();
        skillCourutine[12] = Cyclone();

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


        if (!CostCheckForUsingSkill(skills[0].costKey, skills[0].cost)) goto Finish1;
        SkillCooling = false;

        playerManager.UsingUT();
        playerManager.ConditionChange(particular.FindCondition("_Invincible"), 1, 3.2f);
        Light2D GloberLight = GameObject.Find("Light 2D").GetComponent<Light2D>();
        playerAnimation.SetInteger("Skills", 15);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        playerRigid.velocity = Vector2.zero;
        float ins = GloberLight.intensity;
        float j = 1;
        for (float i = ins; i > (ins * 0.5f); i -= (ins * 0.05f))
        {
            GloberLight.intensity = i;
            j -= 0.1f;
            playerControl.GetComponent<SpriteRenderer>().color = new Color(j, 1, 1);
            yield return new WaitForSeconds(0.02f);
        }

        float tmp = (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        playerControl.CameraPointSetting(true, playerControl.transform.GetChild(0).GetChild(1).position);
        playerAnimation.SetTrigger("NextStep");
        playerRigid.velocity = Vector2.zero;
        g = Instantiate(characterObjects[14], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.2f + Vector3.up * 0.35f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);

        for (int i = 0; i < 10; i++)
        {
            j += 0.1f;
            playerControl.GetComponent<SpriteRenderer>().color = new Color(j, 1, 1);
            yield return new WaitForSeconds(0.02f);
        }
        playerControl.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        g = Instantiate(characterObjects[16], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.2f + Vector3.up * 0.35f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        
        g = Instantiate(characterObjects[17], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.2f + Vector3.up * 0.2f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1.25f : 1.25f, 1.25f, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)(uint)Mathf.RoundToInt((playerManager.GetEditedState("Power") * 12.5f + playerManager.GetEditedState("Intellect") * 12.5f) * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();
       
        yield return new WaitForSeconds(0.3f);
        g = Instantiate(characterObjects[14], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.2f + Vector3.up * 0.35f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        
        yield return new WaitForSeconds(0.5f);
        g = Instantiate(characterObjects[16], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.2f + Vector3.up * 0.35f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
       
        g = Instantiate(characterObjects[17], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.2f + Vector3.up * 0.2f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1.25f : 1.25f, 1.25f, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)(uint)Mathf.RoundToInt((playerManager.GetEditedState("Power") * 12.5f + playerManager.GetEditedState("Intellect") * 12.5f) * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();
        
        yield return new WaitForSeconds(0.3f);
        g = Instantiate(characterObjects[15], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.2f + Vector3.up * 0.35f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        
        yield return new WaitForSeconds(0.5f);
        g = Instantiate(characterObjects[16], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.2f + Vector3.up * 0.35f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
       
        g = Instantiate(characterObjects[18], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.2f + Vector3.up * 0.2f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1.25f : 1.25f, 1.25f, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt((playerManager.GetEditedState("Power") * 27.5f + playerManager.GetEditedState("Intellect") * 27.5f) * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();
        playerRigid.velocity = new Vector2(-10 * tmp, 10);
        for (float i = (ins * 0.5f); i < ins; i += (ins * 0.05f))
        {
            j += 0.1f;
            playerControl.GetComponent<SpriteRenderer>().color = new Color(j, 1, 1);
            GloberLight.intensity = i;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.1f);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", false));
        playerControl.CameraPointSetting(false, playerControl.transform.GetChild(0).GetChild(1).position);
        GloberLight.intensity = ins;
        SkillCooling = true;
        Finish1:
        yield return null;
    }

    IEnumerator DualPistol()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        g = Instantiate(characterObjects[3], playerControl.transform.position - Vector3.forward + (Vector3.up * 0.96f), playerControl.transform.rotation);
        g.GetComponent<ShotsEvent>().Shot();
        yield return new WaitForSeconds(0.2f);
        chargeTime = 3;
        StartCoroutine(SkillCooltime(1, skills[1].skillCooltime));
    }

    IEnumerator FireShot()
    {
        usingCanMoveSkill = true;
        canFlip = false;
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        g = Instantiate(characterObjects[8], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f, playerControl.transform.rotation, playerControl.transform);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.45f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(3f);
        Destroy(g);
        canFlip = true;
        usingCanMoveSkill = false;
        StartCoroutine(SkillCooltime(2, skills[2].skillCooltime));

    }

    IEnumerator PainExacerbation()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        playerAnimation.SetInteger("Skills",0);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.1f);
        g = Instantiate(characterObjects[7], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f + Vector3.back, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        g.GetComponent<PainExacerbationEvent>().SetTransform(playerControl.transform);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SkillCooltime(3, skills[3].skillCooltime));
    }

    IEnumerator FallingLights()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        playerAnimation.SetInteger("Skills", 2);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.1f);
        float degree;
        for (int i = 0; i <= 15; i++)
        {
            degree = Random.Range(100f, 80f);
            g = Instantiate(characterObjects[5], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 1.5f, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 37.5f));
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.50f * skillRank);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().Shot();
        }
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SkillCooltime(4, skills[4].skillCooltime));
    }

    IEnumerator ExplosionBullet()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        playerAnimation.SetInteger("Skills", 0);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.1f);
        g = Instantiate(characterObjects[9], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f + Vector3.back, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<BoomBullet>().SetPlayer(playerControl.transform);
        g.GetComponent<ShotsEvent>().Shot();
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SkillCooltime(5, skills[5].skillCooltime));
    }

    IEnumerator Phoenix()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.01f, 0) * 35f);
        playerAnimation.SetInteger("Skills", 7);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.41f);
        playerRigid.gravityScale = 0;
        playerRigid.drag = 0;
        playerRigid.velocity = new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? 1 : -1) * 12f, 16);
        yield return new WaitForSeconds(0.08f);
        g = Instantiate(characterObjects[6], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1.5f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();        
        yield return new WaitForSeconds(0.07f);
        playerRigid.gravityScale = 6;
        yield return new WaitForSeconds(0.41f);
        StartCoroutine(SkillCooltime(6, skills[6].skillCooltime));
    }

    IEnumerator Firestorm()
    {
        playerRigid.drag = 0;
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        playerAnimation.SetInteger("Skills", 4);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        playerRigid.velocity = new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 12f, 0);
        float time = 0;
        float count = 0.05f;
        while (time < 1.7f)
        {
            playerRigid.velocity = new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 12f, playerRigid.velocity.y);
            time += Time.deltaTime;
            if (time >= count) {
                count += 0.05f;
                float degree = Random.Range(190f, 120f);
                g = Instantiate(characterObjects[11], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 10));
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.1f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.GetComponent<ShotsEvent>().Shot();
                degree = Random.Range(190f, 120f);
                g = Instantiate(characterObjects[11], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 10));
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.1f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.GetComponent<ShotsEvent>().Shot();
            }
            yield return null;
        }
        StartCoroutine(SkillCooltime(7, skills[7].skillCooltime));
    }

    IEnumerator RunandGun()
    {
        playerRigid.drag = 0;
        
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        playerAnimation.SetInteger("Skills", 4);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        playerControl.ChangeVelocity(new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 25f, 0));
        float time = 0;      
        while (time < 0.3f)
        {
            playerControl.ChangeVelocity(new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * 25f, playerRigid.velocity.y));
            if (CostCheckForUsingSkill("MP", 25) && Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(2, 0) == 8 ? 1 : (playerManager.GetSettingSkillCode(2, 1) == 8 ? 2 : 3)))) {
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }

        if (time < 0.3f)
        {
            playerManager.UsingMP(25);
            playerAnimation.SetBool("RunandGun", true);
            playerAnimation.SetTrigger("NextStep");
            playerRigid.drag = 0;
            isMovingBySkill = true;
            playerRigid.gravityScale = 0;
            playerRigid.velocity = new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? 1 : -1) * 18f, 32);
            yield return new WaitForSeconds(0.2f);
            playerRigid.velocity = Vector2.zero;

            for (int i = 1; i <= 6; i++) {
                g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * (180 + 30 * i)), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * (180 + 30 * i))) * 0.75f, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * (190 + 30 * i)), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * (190 + 30 * i))), 45));
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.75f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.GetComponent<ShotsEvent>().Shot();
                g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * (180 + 30 * i)), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * (180 + 30 * i))) * 0.75f, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * (180 + 30 * i)), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * (180 + 30 * i))), 45));
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.75f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.GetComponent<ShotsEvent>().Shot();
                yield return new WaitForSeconds(0.04f);
            }
            playerRigid.gravityScale = 6;
            isMovingBySkill = false;
            playerRigid.velocity = new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? 1 : -1) * 25f, 0);
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(SkillCooltime(8, 15));
        }
        else {
            playerControl.ChangeVelocity(new Vector2(playerRigid.velocity.x * 0.25f, playerRigid.velocity.y * 0.1f));
            playerAnimation.SetBool("RunandGun", false);
            playerAnimation.SetTrigger("NextStep");
            yield return new WaitForSeconds(0.1f);
            g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, Vector2.right, 45));
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.75f * skillRank);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().Shot();
            yield return new WaitForSeconds(0.15f);
            g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, Vector2.right, 45));
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.75f * skillRank);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().Shot();
            yield return new WaitForSeconds(0.15f);
            StartCoroutine(SkillCooltime(8, 0.5f));
        }

    }

    IEnumerator SplitShot()
    {
        usingCanMoveSkill = true;
        canFlip = false;
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerAnimation.SetInteger("Skills", 6);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        float degree;
        for (int i = 0; i <= 40; i++)
        {
            degree = Random.Range(-25f, 40f);
            g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().Shot();

            degree = Random.Range(-25f, 40f);
            g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().Shot();

            degree = Random.Range(140f, 205f);
            g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().Shot();

            degree = Random.Range(140f, 205f);
            g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().Shot();

            yield return new WaitForSeconds(0.125f);
        }
        degree = 0;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 15;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 30;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 45;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 180;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 165;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 150;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 135;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * (uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 30));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();
        canFlip = true;
        usingCanMoveSkill = false;
        StartCoroutine(SkillCooltime(9, skills[9].skillCooltime));
    }

    IEnumerator Saturation()
    {
        usingCanMoveSkill = true;
        canFlip = false;
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerAnimation.SetInteger("Skills", 5);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        float degree;
        for (int i = 0; i <= 40; i++)
        {

            degree = isOnGround ? 0 : 270;
            if (degree == 0)
            {
                g = Instantiate(characterObjects[13], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.35f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.35f, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.GetComponent<ShotsEvent>().Shot();
            }
            else {
                g = Instantiate(characterObjects[13], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.3f + Vector3.down * 0.75f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.transform.right = Vector3.down;
                g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.3f + Vector3.down * 0.75f, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.GetComponent<ShotsEvent>().Shot();
            }
            yield return new WaitForSeconds(0.0625f);
            if (degree == 0)
            {
                g = Instantiate(characterObjects[13], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.15f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.15f, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.GetComponent<ShotsEvent>().Shot();
            }
            else
            {
                g = Instantiate(characterObjects[13], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.1f + Vector3.down * 0.75f + Vector3.back, playerControl.transform.rotation, playerControl.transform);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.transform.right = Vector3.down;
                g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.1f + Vector3.down * 0.75f, playerControl.transform.rotation);
                g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
                g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
                g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
                di = g.GetComponent<PlayerAttacks>().GetDamage();
                di.order = playerControl.transform;
                di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
                g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
                g.GetComponent<ShotsEvent>().Shot();
            }
            yield return new WaitForSeconds(0.0625f);
        }

        g = Instantiate(characterObjects[13], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.25f, playerControl.transform.rotation, playerControl.transform);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);

        degree = 5;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 15;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 25;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = 35;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = -5;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = -15;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = -25;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();

        degree = -35;
        g = Instantiate(characterObjects[4], playerControl.transform.position + (Vector3)new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)) * 0.75f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<ShotsEvent>().AddMoveSet(new MovementInfo(5, new Vector2((uint)Mathf.Cos((uint)Mathf.Deg2Rad * degree), (uint)Mathf.Sin((uint)Mathf.Deg2Rad * degree)), 40));
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 0.2f * skillRank);
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();
        canFlip = true;
        usingCanMoveSkill = false;
        StartCoroutine(SkillCooltime(10, skills[10].skillCooltime));
    }

    IEnumerator FatalShot()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        playerAnimation.SetInteger("Skills", 0);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.1f);
        g = Instantiate(characterObjects[10], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        if (Random.Range(1, 101) <= playerManager.GetEditedState("Luck") * 0.5f){
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.10f);
        }
        else {
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Power") * 1.80f);
            di.damageType = DamageType.True;
        }
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SkillCooltime(11, skills[11].skillCooltime));
    }

    IEnumerator Cyclone()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerControl.GetComponent<SpriteRenderer>().material.SetColor("_GlowColor", new Color(1, 0.5f, 0) * 15f);
        playerAnimation.SetInteger("Skills", 0);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.1f);
        g = Instantiate(characterObjects[12], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.2f + Vector3.back, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        g.GetComponent<CycloneBullet>().SetPlayer(playerControl.transform);
        g.GetComponent<ShotsEvent>().Shot();
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SkillCooltime(12, skills[12].skillCooltime));
    }

}
