using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Luciely : CharacterBase
{
    bool isOnGround = true;
    float chargeTime = 0;

    float plusNormalDamage = 1;
    int NormalDamageCount = 1;
    float plusNormalTime = 0;

    private GameObject g;
    private DamageInfo di;

    UnityEvent<States, uint> passiveNormalAttackEvent = new UnityEvent<States, uint>();
    UnityEvent<States, uint> passiveSkillEvent = new UnityEvent<States, uint>();

    public void LucielyPassiveEventSkills(States s, uint damag)
    {
        if (s.isinConditionList("_StarMagicFragment"))
        {
            s.RemoveCondition("_StarMagicFragment");
            g = Instantiate(characterObjects[7], s.transform.position, s.transform.rotation);
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.3f);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        }
        else
        {
            s.ConditionChange("_StarMagicFragment", 1, 3);
        }
    }

    public void LucielyPassiveEventNormalAttack(States s, uint damag)
    {
        if (s.isinConditionList("_StarMagicFragment"))
        {
            s.RemoveCondition("_StarMagicFragment");
            if (NormalDamageCount > 5) NormalDamageCount = 5;
            else NormalDamageCount++;
            plusNormalTime = 3;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        skillCourutine = new IEnumerator[13];

    }
    private void Start()
    {
        passiveNormalAttackEvent.AddListener(LucielyPassiveEventNormalAttack);
        passiveSkillEvent.AddListener(LucielyPassiveEventSkills);
    }

    // Update is called once per frame
    void Update()
    {
        skillRank = playerManager.GetEditedState("SkillPercent") * 0.01f;
        isOnGround = playerControl.GetIsGround();
        playerControl.ChangeMotionCheck("Weapon", isArmorOn);

        if (playerManager.GetCharacter() == 1)
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
        plusNormalDamage = 1 + 0.04f * NormalDamageCount;
        if (NormalDamageCount > 0)
        {
            if (plusNormalTime <= 0)
            {
                plusNormalTime = 0;
                NormalDamageCount = 0;
            }
            else
            {

                plusNormalTime -= Time.deltaTime;
            }
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
        while (Input.GetButton("Attack") && canAttack)
        {
            playerAnimation.SetBool("Attack", playerControl.ChangeMotionCheck("Attack", true));
            yield return new WaitForSeconds(attackSpeedTime[0] * 0.25f);
            g = Instantiate(characterObjects[5], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 0.75f + Vector3.up * 0.45f, playerControl.transform.rotation);
            g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1) * 0.65f;
            di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = playerControl.transform;
            di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.8f * plusNormalDamage);
            di.damageChangeEvent = passiveNormalAttackEvent;
            if (NormalDamageCount > 0) NormalDamageCount = 0;
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
            g.GetComponent<ShotsEvent>().Shot();
            yield return new WaitForSeconds(attackSpeedTime[0] * 0.7f);

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
        if (!CostCheckForUsingSkill(skills[code].costKey, skills[code].cost + ((code == 9 || code == 10) ? Mathf.RoundToInt(playerManager.GetState("MaxMP") * 0.25f) : 0)) || skillCoolingInfo[code, 0] > 0 || ((code == 6 || code == 7 || code == 8) && (cc.cc == ClowdControl.Bound || playerManager.GetIsFlying())))
            goto Finish2;
        playerManager.UsingMP(skills[code].cost + ((code == 10) ? Mathf.RoundToInt(playerManager.GetState("MaxMP") * 0.20f) : 0));

        skillCourutine[1] = SpotRay();
        skillCourutine[2] = Plasma();
        skillCourutine[3] = SatelliteGuard();
        skillCourutine[4] = StarlightTransform();
        skillCourutine[5] = GreatRedSpot();
        skillCourutine[6] = PhotonLeap();
        skillCourutine[7] = PlanetaryNebula();
        skillCourutine[8] = GravityExplosion();
        skillCourutine[9] = BigFrozen();
        skillCourutine[10] = GammaSaturation();
        skillCourutine[11] = MeteorShower();
        skillCourutine[12] = BirthOfStar();

        yield return StartCoroutine(skillCourutine[code]);
        if (NormalDamageCount > 0 && (code != 3 || code != 4)) NormalDamageCount = 0;
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
        playerManager.ConditionChange(particular.FindCondition("_Invincible"), 1, 12f);
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
            playerControl.GetComponent<SpriteRenderer>().color = new Color(1, 1, j);
            yield return new WaitForSeconds(0.02f);
        }

        float tmp = (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);

        playerRigid.velocity = Vector2.zero; 

        for (int i = 0; i < 10; i++)
        {
            j += 0.1f;
            playerControl.GetComponent<SpriteRenderer>().color = new Color(1, 1, j);
            yield return new WaitForSeconds(0.02f);
        }
        playerControl.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

        g = Instantiate(characterObjects[19], playerControl.transform.position, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        di = g.transform.GetChild(2).GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.5f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.transform.GetChild(2).GetComponent<PlayerAttacks>().SetDamageInfo(di);

        di = g.transform.GetChild(3).GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 65f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.transform.GetChild(3).GetComponent<PlayerAttacks>().SetDamageInfo(di);

        playerAnimation.SetTrigger("NextStep");
        yield return new WaitForSeconds(2f);
        playerControl.CameraPointSetting(true, playerControl.transform.GetChild(0).GetChild(1).position);
        playerAnimation.SetTrigger("NextStep");
        yield return new WaitForSeconds(5.5f);
        playerAnimation.SetTrigger("NextStep");
        yield return new WaitForSeconds(4.8f);
        //ult

        playerControl.CameraPointSetting(false, playerControl.transform.GetChild(0).GetChild(1).position);
        for (float i = (ins * 0.5f); i < ins; i += (ins * 0.05f))
        {
            j += 0.1f;
            playerControl.GetComponent<SpriteRenderer>().color = new Color(j, 1, 1);
            GloberLight.intensity = i;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(0.1f);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", false));
        
        GloberLight.intensity = ins;
        SkillCooling = true;
    Finish1:
        yield return null;
    }


    IEnumerator SpotRay()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.2f);
        g = Instantiate(characterObjects[1], playerControl.transform.position + (Vector3)(playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 6 + Vector3.up * 4f + Vector3.forward * 2, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1.9f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(0.28f);
        StartCoroutine(SkillCooltime(1, skills[1].skillCooltime));

    }
    IEnumerator Plasma()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.2f);
        g = Instantiate(characterObjects[2], playerControl.transform.position + Vector3.forward * 0.1f, playerControl.transform.rotation);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.7f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g = Instantiate(characterObjects[3], playerControl.transform.position + Vector3.up * 2.5f + Vector3.forward * 2, playerControl.transform.rotation);
        g.GetComponent<PlasmaSphere>().setDamage(transform, (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.7f), passiveSkillEvent);
        yield return new WaitForSeconds(0.28f);
        StartCoroutine(SkillCooltime(2, skills[2].skillCooltime));

    }

    IEnumerator SatelliteGuard()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.2f);
        g = Instantiate(characterObjects[4], playerControl.transform.position, playerControl.transform.rotation, playerControl.transform);
        playerManager.BarriorAdd("SateliteGuard", (uint)Mathf.RoundToInt(playerManager.GetState("MaxHP") * 0.1f), 7);
        yield return new WaitForSeconds(0.28f);
        StartCoroutine(SkillCooltime(3, skills[3].skillCooltime));

    }
    IEnumerator StarlightTransform()
    {
        isMovingBySkill = true;
        playerRigid.gravityScale = 0;
        playerRigid.velocity = Vector2.zero;
        playerRigid.drag = 0;
        playerAnimation.SetInteger("Skills", 275);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        Vector2 toShot = Vector2.zero;
        float leftTime = 0.06f;
        while (leftTime > 0)
        {
            if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)) toShot = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            leftTime -= Time.deltaTime;
            yield return null;
        }
        if (toShot == Vector2.zero) toShot = (playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right);
        playerRigid.velocity = toShot * 15;
        g = Instantiate(characterObjects[6], playerControl.transform.position + Vector3.forward * 0.1f, playerControl.transform.rotation, playerControl.transform);
        playerManager.ConditionChange("_Protection", 1, 1);
        yield return new WaitForSeconds(1f);
        isMovingBySkill = false;
        playerRigid.gravityScale = 6;
        playerRigid.velocity = Vector2.down * 0.01f;
        StartCoroutine(SkillCooltime(4, skills[4].skillCooltime));

    }

    IEnumerator GreatRedSpot()
    {
        usingCanMoveSkill = true;
        playerRigid.velocity = Vector2.zero;
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.2f);
        g = Instantiate(characterObjects[8], playerControl.transform.position + Vector3.back * 0.1f, playerControl.transform.rotation, playerControl.transform);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.375f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(3f);
        usingCanMoveSkill = false;
        StartCoroutine(SkillCooltime(5, skills[5].skillCooltime));
    }

    IEnumerator PhotonLeap()
    {
        playerRigid.velocity = Vector2.zero;
        playerAnimation.SetInteger("Skills", 275);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        Vector2 toFlash = Vector2.zero;
        float leftTime = 0.06f;
        while (leftTime > 0)
        {
            if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)) toFlash = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            leftTime -= Time.deltaTime;
            yield return null;
        }
        if (toFlash == Vector2.zero) toFlash = (playerControl.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right);
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerControl.transform.position + (Vector3)(toFlash * 6), new Vector3(0.5f, 1f, 0), 0f, Vector2.down, 0f, LayerMask.GetMask("Ground"));
        if (raycastHit.collider != null)
        {
            raycastHit = Physics2D.BoxCast(playerControl.transform.position, new Vector3(0.5f, 1f, 0), 0f, toFlash, 6f, LayerMask.GetMask("Ground"));
            playerControl.transform.position = new Vector3(raycastHit.point.x, raycastHit.point.y, -1.1f);
        }
        else playerControl.transform.position = playerControl.transform.position + (Vector3)(toFlash * 6);       
        g = Instantiate(characterObjects[9], playerControl.transform.position + Vector3.forward * 0.1f, playerControl.transform.rotation);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        StartCoroutine(SkillCooltime(6, skills[6].skillCooltime));
    }
    IEnumerator PlanetaryNebula()
    {
        playerRigid.velocity *= 0.25f;
        playerAnimation.SetInteger("Skills", 2);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.167f);
        GameObject g2 = Instantiate(characterObjects[10], playerControl.transform.position + Vector3.forward * 0.99f, playerControl.transform.rotation, playerControl.transform);
        DamageInfo di2 = g2.GetComponent<PlayerAttacks>().GetDamage();
        di2.order = playerControl.transform;
        di2.cc = ClowdControl.Slow;
        di2.ccStrength = 75;
        di2.ccTime = 1.5f;
        di2.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1.2f * plusNormalDamage * skillRank);
        di2.damageChangeEvent = passiveSkillEvent;
        g2.GetComponent<PlayerAttacks>().SetDamageInfo(di2);
        yield return new WaitForSeconds(3f);
        g2.transform.parent = null;
        playerAnimation.SetTrigger("NextStep");
        di2.elemental = Elemental.Fire;
        di2.cc = ClowdControl.None;
        di2.continuousDamage = true;
        di2.continuousTime = 1f;
        di2.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.4f * plusNormalDamage * skillRank);
        di2.maxHPDamage = 1f;
        g2.GetComponent<PlayerAttacks>().SetDamageInfo(di2);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(SkillCooltime(7, skills[7].skillCooltime));
    }
    IEnumerator GravityExplosion()
    {
        playerRigid.velocity *= 0.25f;
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.167f);
        GameObject g2 = Instantiate(characterObjects[12], playerControl.transform.position + Vector3.back * 0.1f + (playerControl.GetComponent<SpriteRenderer>().flipX ? Vector3.left : Vector3.right) * 1.5f + Vector3.up * 0.5f, playerControl.transform.rotation, playerControl.transform);
        DamageInfo di2 = g2.GetComponent<PlayerAttacks>().GetDamage();
        di2.order = g2.transform;
        di2.cc = ClowdControl.Pull;
        di2.ccStrength = 1.5f;
        di2.damageChangeEvent = passiveSkillEvent;
        g2.GetComponent<PlayerAttacks>().SetDamageInfo(di2);
        yield return new WaitForSeconds(0.167f);
        playerAnimation.SetInteger("Skills", 2);
        playerAnimation.SetTrigger("Move Changed");
        yield return new WaitForSeconds(0.333f);
        g = Instantiate(characterObjects[11], playerControl.transform.position + Vector3.forward * 0.1f + (playerControl.GetComponent<SpriteRenderer>().flipX ? Vector3.left : Vector3.right) * 1.5f + Vector3.down * 0.1f, playerControl.transform.rotation, playerControl.transform);
        yield return new WaitForSeconds(0.5f);
        di2.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1.8f * plusNormalDamage * skillRank);
        di2.cc = ClowdControl.None;
        g2.GetComponent<PlayerAttacks>().SetDamageInfo(di2);
        yield return new WaitForSeconds(1.3f);
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetTrigger("Move Changed");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SkillCooltime(8, skills[8].skillCooltime));
    }

    IEnumerator BigFrozen()
    {
        playerRigid.velocity *= 0.25f;
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.167f);
        g = Instantiate(characterObjects[13], playerControl.transform.position + Vector3.forward * 0.1f, playerControl.transform.rotation, playerControl.transform);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.65f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(0.333f);
        g = Instantiate(characterObjects[14], playerControl.transform.position + Vector3.forward * 0.9f, playerControl.transform.rotation, playerControl.transform.parent);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.3f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 1.6f * plusNormalDamage * skillRank);
        g.GetComponent<BigFrozenSummonIce>().SetDamageInfo(di);
        StartCoroutine(SkillCooltime(9, skills[9].skillCooltime));
    }

    IEnumerator GammaSaturation()
    {
        playerRigid.velocity *= 0.25f;
        playerAnimation.SetInteger("Skills", 2);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.167f);
        g = Instantiate(characterObjects[15], playerControl.transform.position + Vector3.forward * 0.9f + Vector3.up * 0.5f + (playerControl.GetComponent<SpriteRenderer>().flipX ? Vector3.left : Vector3.right) * 1.25f, playerControl.transform.rotation, playerControl.transform.parent);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.35f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(4f);     
        playerAnimation.SetTrigger("NextStep");       
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(SkillCooltime(10, skills[10].skillCooltime));
    }

    IEnumerator MeteorShower()
    {
        playerRigid.velocity = new Vector2(playerRigid.velocity.x * 0.5f, playerRigid.velocity.y);
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.2f);
        g = Instantiate(characterObjects[16], playerControl.transform.position + Vector3.up * 18f + Vector3.forward * -0.1f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -2 : 2, 2, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 4.5f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        g.GetComponent<ShotsEvent>().Shot();
        yield return new WaitForSeconds(0.28f);
        StartCoroutine(SkillCooltime(11, skills[11].skillCooltime));

    }

    IEnumerator BirthOfStar()
    {
        playerRigid.velocity *= 0.25f;
        playerAnimation.SetInteger("Skills", 2);
        playerAnimation.SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        yield return new WaitForSeconds(0.167f);
        g = Instantiate(characterObjects[17], playerControl.transform.position + Vector3.forward * 0.99f + (playerControl.GetComponent<SpriteRenderer>().flipX ? Vector3.left : Vector3.right) * 3f + Vector3.up * 0.5f, playerControl.transform.rotation, playerControl.transform);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 0.8f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(1.5f);
        playerAnimation.SetInteger("Skills", 1);
        playerAnimation.SetTrigger("Move Changed");
        g = Instantiate(characterObjects[18], playerControl.transform.position + (playerControl.GetComponent<SpriteRenderer>().flipX ? Vector3.left : Vector3.right) * 3f + Vector3.up * 0.5f + Vector3.forward * 0.1f, playerControl.transform.rotation);
        g.transform.localScale = new Vector3(playerControl.GetComponent<SpriteRenderer>().flipX ? -2 : 2, 2, 1);
        g.GetComponent<ShotsEvent>().SetVelocityFixValue(playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerControl.transform;
        di.damage = (uint)Mathf.RoundToInt(playerManager.GetEditedState("Intellect") * 3.8f * plusNormalDamage * skillRank);
        di.damageChangeEvent = passiveSkillEvent;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(SkillCooltime(12, skills[12].skillCooltime));
    }
}

