using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class TmpBehaivor : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Light2D GloberLight;

    public Sprite[] skillIcon = new Sprite[13];
    public Image[] skillIconUI = new Image[3];
    public GameObject[] Skilleffects;

    bool isOnGround = false;
    bool rayCaston = true;
    bool attackCooltime = true;
    bool isSwordOn = true;
    bool skillon = false;
    bool moveskillcool = true;
    bool jumpKeyCheck = false;

    int[] ChargedSlash = { 0, 0 };

    int[] SkillcodeSaved = { 0, 0, 0 };

    float speedValue = 1f;
    float[] attacktime = { 0.32f, 0.32f, 0.57f };
    const float maxSpeed = 9f;

    float jumpT;
    float jumpP = 10f;
    float jumpTL = 0.125f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        GloberLight = GameObject.Find("Light 2D").GetComponent<Light2D>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (!skillon)
        {
            speedValue = Input.GetKey(KeyCode.LeftShift) ? 1.75f : 1f;
            Jump();
            Attack();
            Skills();
        }
        for (int i = 0; i < 3; i++)
            skillIconUI[i].sprite = skillIcon[SkillcodeSaved[i]];
    }

    private void FixedUpdate()
    {
        anim.SetFloat("Y Velocity", rigid.velocity.y);
        if (rayCaston)
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(rigid.transform.position, new Vector3(0.97f, 0.03f, 0), 0f, Vector2.down, 0.97f, LayerMask.GetMask("Ground"));
            isOnGround = (raycastHit.collider != null);
            if(!skillon)
                anim.SetBool("OnGround", ChangeMotionCheck("OnGround", isOnGround));
        }
        if ((attackCooltime || !isOnGround) && !skillon)
        {
            if (Input.GetButton("Horizontal"))
            {
                spriteRenderer.flipX = (Input.GetAxisRaw("Horizontal") < 0);
                rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * maxSpeed * speedValue, rigid.velocity.y);
                anim.SetTrigger((speedValue > 1 ? "Run" : "Walk"));
            }
            else
            {
                rigid.velocity = new Vector2(0, rigid.velocity.y);
                anim.SetTrigger("Idle");
            }
        }
    }

    private void Jump()
    {
        if (isOnGround && rigid.gravityScale != 0 && !skillon)
        {
            if (Input.GetKey(KeyCode.Space) && jumpKeyCheck)
            {
                rayCaston = false;
                isOnGround = false;
                jumpKeyCheck = false;
                anim.SetBool("OnGround", ChangeMotionCheck("OnGround", isOnGround));
            }
        }

        if (!Input.GetKey(KeyCode.Space) || jumpT >= jumpTL)
        {
            rayCaston = true;
            jumpT = 0;
        }

        RaycastHit2D raycastHit = Physics2D.BoxCast(rigid.transform.position, new Vector3(0.97f, 0.7f, 0), 0f, Vector2.down, 1.3f, LayerMask.GetMask("Ground"));
        if (raycastHit.collider != null && Input.GetKeyDown(KeyCode.Space)) jumpKeyCheck = true;

        if (!rayCaston)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0f);
            rigid.AddForce((Vector2.up * jumpP * ((jumpT * 10) + 1f)), ForceMode2D.Impulse);
            jumpT += Time.deltaTime;
        }
    }

    private bool ChangeMotionCheck(string paraName, bool checkValue)
    {
        if (anim.GetBool(paraName) != checkValue) anim.SetTrigger("Move Changed");
        return checkValue;
    }

    public void SwordChange()
    {
        if (!skillon)
        {
            isSwordOn = !isSwordOn;
            anim.SetBool("Sword", ChangeMotionCheck("Sword", isSwordOn));
        }
    }

    public void SkillsSet(int skillcode)
    {
        for (int i = 0; i < 3; i++)
        {
            if (SkillcodeSaved[i] == skillcode)
            {
                SkillcodeSaved[i] = 0;
                return;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (SkillcodeSaved[i] == 0)
            {
                SkillcodeSaved[i] = skillcode;
                return;
            }

        }
    }

    void Attack()
    {
        if (Input.GetKey(KeyCode.A) && attackCooltime && isSwordOn)
        {
            attackCooltime = false;
            anim.SetBool("Attack", ChangeMotionCheck("Attack", true));
            StartCoroutine("Attacks");
        }
    }

    private void Skills()
    {
        if (isSwordOn)
        {
            if (Input.GetKey(KeyCode.S))
            {
                skillon = true;
                StartCoroutine(AttackSkill(SkillcodeSaved[0]));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                skillon = true;
                StartCoroutine(AttackSkill(SkillcodeSaved[1]));
            }
            else if (Input.GetKey(KeyCode.F))
            {
                skillon = true;
                StartCoroutine(AttackSkill(SkillcodeSaved[2]));
            }
            else if (Input.GetKey(KeyCode.U))
            {
                skillon = true;
                StartCoroutine("UltimateSkill");
            }
            else if (Input.GetKey(KeyCode.C) && moveskillcool)
            {
                StartCoroutine("MoveSkill");
            }
        }
    }

    IEnumerator Attacks()
    {
        while (Input.GetKey(KeyCode.A) && isSwordOn)
        {
            if (isOnGround)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i == 3)
                        i = 0;
                    if (!Input.GetKey(KeyCode.A) || !isOnGround || !isSwordOn)
                        break;                                     
                    rigid.velocity = new Vector2((spriteRenderer.flipX ? -1 : 1) * 9f, rigid.velocity.y);
                    yield return new WaitForSeconds(0.15f);
                    useChargeAttack();
                    yield return new WaitForSeconds(attacktime[i] - 0.15f);


                }
            }
            else
            {
                while (true)
                {
                    if (!Input.GetKey(KeyCode.A) || isOnGround || !isSwordOn)
                        break;
                    useChargeAttack();
                    yield return new WaitForSeconds(0.333f);

                }
            }
        }
        anim.SetBool("Attack", ChangeMotionCheck("Attack", false));
        attackCooltime = true;



    }

    IEnumerator MoveSkill() {
        moveskillcool = false;
        skillon = true;
        anim.SetInteger("Skills", 275);
        anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
        rigid.gravityScale = 0;
        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
            rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 45f;
        else
        {
            if (spriteRenderer.flipX)
            {
                rigid.velocity = Vector2.left * 45f;
            }
            else
            {
                rigid.velocity = Vector2.right * 45f;
            }
        }
        yield return new WaitForSeconds(0.15f);

        rigid.gravityScale = 6;
        rigid.velocity *= 0.35f;
        skillon = false;
        anim.SetBool("Skill On", ChangeMotionCheck("Skill On", false));
        yield return new WaitForSeconds(0.5f);
        moveskillcool = true;
    }

    IEnumerator AttackSkill(int code)
    {
        GameObject g;
        switch (code)
        {
            case 1:
                anim.SetInteger("Skills", 0);
                anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
                yield return new WaitForSeconds(0.16f);
                useChargeAttack();
                g = Instantiate(Skilleffects[0], transform.position, transform.rotation);
                g.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
                yield return new WaitForSeconds(0.16f);
                useChargeAttack();
                g = Instantiate(Skilleffects[1], transform.position, transform.rotation);
                g.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
                yield return new WaitForSeconds(0.08f);
                break;
            case 2:
                rigid.velocity = new Vector2(rigid.velocity.x * 0.5f, rigid.velocity.y);
                g = Instantiate(Skilleffects[7], transform.position - Vector3.forward + (Vector3.up * 0.96f), transform.rotation, transform);
                g.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
                yield return new WaitForSeconds(0.2f);
                ChargedSlash[0] = 1;
                ChargedSlash[1] = 3;
                break;
            case 3:
                rigid.velocity = new Vector2(rigid.velocity.x * 0.5f, rigid.velocity.y);
                if (!isOnGround && rigid.velocity.y < 7)
                    rigid.velocity = new Vector2(rigid.velocity.x, 7f);
                anim.SetInteger("Skills", 1);
                anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
                yield return new WaitForSeconds(0.32f);
                break;
            case 4:
                anim.SetInteger("Skills", 2);
                anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
                yield return new WaitForSeconds(0.4f);
                break;
            case 5:
                rigid.velocity = new Vector2(19.5f * (spriteRenderer.flipX ? -1 : 1), rigid.velocity.y);
                anim.SetInteger("Skills", 3);
                anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
                yield return new WaitForSeconds(0.4f);
                rigid.velocity = new Vector2(0, rigid.velocity.y);
                g = Instantiate(Skilleffects[2], transform.position, transform.rotation);
                g.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
                g.GetComponent<TmpShot>().angle = (spriteRenderer.flipX ? 180 : 0);
                yield return new WaitForSeconds(0.09f);
                break;
            case 6:
                rigid.velocity = new Vector2(rigid.velocity.x * 0.5f, rigid.velocity.y);
                g = Instantiate(Skilleffects[8], transform.position - Vector3.forward, transform.rotation, transform);
                g.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
                yield return new WaitForSeconds(0.2f);
                ChargedSlash[0] = 2;
                ChargedSlash[1] = 2;
                break;
            case 7:
                rigid.gravityScale = 0;
                rigid.velocity = new Vector2(rigid.velocity.x, 19.5f);
                anim.SetInteger("Skills", 4);
                anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
                yield return new WaitForSeconds(0.60f);
                rigid.gravityScale = 6;
                rigid.velocity = new Vector2(rigid.velocity.x, 24f);
                yield return new WaitForSeconds(0.215f);
                break;
            case 8:
                if (isOnGround)
                {
                    spriteRenderer.material.SetColor("_Color", new Color(1, 0.5f, 0) * 15f);
                    anim.SetInteger("Skills", 5);
                    anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
                    yield return new WaitForSeconds(0.65f);
                    g = Instantiate(Skilleffects[9], transform.position + new Vector3((spriteRenderer.flipX ? -1 : 1) * 1f, 0, -1), transform.rotation);
                    g.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
                    yield return new WaitForSeconds(0.09f);

                }
                break;
            case 9:
                g = Instantiate(Skilleffects[5], transform.position - Vector3.forward, transform.rotation, transform);
                g.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
                yield return new WaitForSeconds(0.24f);
                ChargedSlash[0] = 3;
                ChargedSlash[1] = 3;
                break;
            case 10:
                spriteRenderer.material.SetColor("_Color", new Color(0, 1, 1) * 15f);
                anim.SetInteger("Skills", 6);
                anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
                yield return new WaitForSeconds(0.24f);
                break;
            case 11:
                anim.SetInteger("Skills", 7);
                anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
                yield return new WaitForSeconds(0.6f);
                yield return new WaitWhile(() => !Input.anyKey);
                break;
            case 12:
                rigid.velocity = new Vector2(24 * (spriteRenderer.flipX ? -1 : 1), rigid.velocity.y);
                anim.SetInteger("Skills", 8);
                anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
                yield return new WaitForSeconds(0.4f);
                break;
            default:
                break;
        }
        anim.SetBool("Skill On", ChangeMotionCheck("Skill On", false));
        skillon = false;
    }

    IEnumerator UltimateSkill()
    {
        rigid.gravityScale = 0;
        rigid.velocity = Vector2.zero;
        float ins = GloberLight.intensity;
        float j = 1;
        for (float i = ins; i > (ins * 0.5f); i -= (ins * 0.05f))
        {
            spriteRenderer.color = new Color(1 , j, j);
            GloberLight.intensity = i;
            j -= 0.1f;
            yield return new WaitForSeconds(0.02f);
        }
        
        float tmp = (spriteRenderer.flipX ? -1 : 1);        
        anim.SetInteger("Skills", 15);
        anim.SetBool("Skill On", ChangeMotionCheck("Skill On", true));
        for (float i = 0; i < 18; i += (18 * 0.125f)) {
            spriteRenderer.color = new Color(1, j, j);
            j += 0.062f;
            rigid.velocity = new Vector2((18f - i) * tmp, (24f - ((i / 3) * 4)));
            yield return new WaitForSeconds(0.05f);
        }
        spriteRenderer.color = new Color(1, 1, 1);
        spriteRenderer.material.SetColor("_Color", new Color(1, 0, 0) * 60f);
        rigid.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.79f);
        for (float i = (ins * 0.5f); i < ins; i += (ins * 0.1f))
        { 
            GloberLight.intensity = i;
            yield return new WaitForSeconds(0.02f);
        }
        anim.SetBool("Skill On", ChangeMotionCheck("Skill On", false));
        rigid.gravityScale = 6;
        GloberLight.intensity = ins;
        skillon = false;
    }

    void useChargeAttack()
    {
        int x = 0;
        if (ChargedSlash[0] != 0 && ChargedSlash[1] > 0) {            
            switch (ChargedSlash[0]) {
                case 1:
                    x = 3;
                    break;
                case 2:
                    x = 4;
                    break;
                case 3:
                    x = 6;
                    break;
            }
            GameObject g;
            if (x == 3)
                g = Instantiate(Skilleffects[x], transform.position + Vector3.right * (spriteRenderer.flipX ? -1 : 1) * 1.2f + Vector3.back, transform.rotation);
            else if(x == 4)
                g = Instantiate(Skilleffects[x], transform.position + Vector3.right * (spriteRenderer.flipX ? -1 : 1) * 2.25f + Vector3.back, transform.rotation);
            else
                g = Instantiate(Skilleffects[x], transform.position + Vector3.right * (spriteRenderer.flipX ? -1 : 1) * 2f + Vector3.back, transform.rotation);
            g.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
            g.GetComponent<TmpShot>().angle = (spriteRenderer.flipX ? 180 : 0);
            ChargedSlash[1]--;
        }
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.down * 0.97f, new Vector3(0.97f, 0.03f, 0));

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.down * 1.3f, new Vector3(0.97f, 0.7f, 0));

    }
}
