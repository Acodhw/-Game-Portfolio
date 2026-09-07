using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaiver_Proto : MonoBehaviour
{
    public bool isEpic;
    public float speed = 2;
    public float followingSpeed = 1.5f;
    public GameObject findPlayerMark;        

    public GameObject LHitbox; public GameObject RHitbox;

    private PlayerState_Proto pstate;
    public EnemyState_Proto estate;
    public GameObject[] SkillOBJ;
    private Transform player;
    private Rigidbody2D rigid;
    private SpriteRenderer spr;
    public bool playerFinding;
    public bool canFlip = true;
    public bool canMove = true;
    public bool canAttack = true;
    public bool canSkill = true;
    private float cantMoveTime;
    private float cantFlipTime;
    private float cantAttackTime;
    private float cantSkillTime;
    public float slowTime;
    private float fs_Mult = 1;
    private float at_Mult = 1;
    private float sk_Mult = 1;
    private float cc_Mult = 1;
    private int MovingAxis;
    private bool isattack;
    private bool isskill;
    private bool isSkillOn;

    public float finding_time;    
    // Start is called before the first frame update
    void Start()
    {        
        spr = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        player = GameObject.Find("mainChar_proto").GetComponent<Transform>();
        Invoke("Moving", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (slowTime <= 0)
        {
            slowTime = 0;
            cc_Mult = 1;
        }
        else
        {
            slowTime -= Time.deltaTime;
        }
        estate.finding = playerFinding;
        if (!canSkill)
        {
            if (cantSkillTime <= 0)
            {
                canSkill = true;
            }
            else
            {
                cantSkillTime -= Time.deltaTime;
            }
        }                  
        else
        {
            if (playerFinding && !isskill && isEpic)
            {
                StartCoroutine("skill");
            }
        }
        if (!canAttack)
        {
            if (cantAttackTime <= 0)
            {
                canAttack = true;
            }
            else
            {
                cantAttackTime -= Time.deltaTime;
            }
        }
        else {
            if (Vector2.Distance(player.position, transform.position) < 1f && playerFinding && !isattack && !isSkillOn)
            {
                StartCoroutine("attack");

            }
        }
        if (canFlip)
        {
            if (playerFinding)
            {
                if (player.position.x < transform.position.x)
                    spr.flipX = true;
                else if (player.position.x > transform.position.x)
                    spr.flipX = false;
            }
            else
            {
                if (MovingAxis == -1)
                    spr.flipX = true;
                else if (MovingAxis == 1)
                    spr.flipX = false;
            }
        }
        else
        {
            if (cantFlipTime <= 0)
            {
                canFlip = true;
            }
            else
            {
                cantFlipTime -= Time.deltaTime;
            }
        }

        CC(estate.GetccCode(), estate.GetccPower(), estate.GetccTime());
    }

    void FixedUpdate()
    {
        FindPlayerCheck();

        if (canMove)
        {
            rigid.velocity = new Vector2(MovingAxis * speed * fs_Mult * sk_Mult * at_Mult * cc_Mult, rigid.velocity.y);


            if (playerFinding)
            {
                if (player.position.x < transform.position.x)
                    MovingAxis = -1;
                else if (player.position.x > transform.position.x)
                    MovingAxis = 1;
            }

            Debug.DrawRay(new Vector2(rigid.position.x + (0.45f * MovingAxis), rigid.position.y), Vector2.down * 0.7f, new Color(0, 1, 0));
            Debug.DrawRay(new Vector2(rigid.position.x + (0.9f * MovingAxis), rigid.position.y), Vector2.down * 3f, new Color(0, 1, 0));

            int layerMask = 1 << LayerMask.NameToLayer("Ground");

            RaycastHit2D rayhit = Physics2D.Raycast(new Vector2(rigid.position.x + (0.45f * MovingAxis), rigid.position.y), Vector2.down, 0.7f, layerMask);
            RaycastHit2D rayhit3 = Physics2D.Raycast(rigid.position, Vector2.right * MovingAxis, 0.3f, layerMask);
            if (rayhit.collider == null || rayhit3.collider != null)
            {
                if (playerFinding)
                {
                    RaycastHit2D rayhit2 = Physics2D.Raycast(new Vector2(rigid.position.x + (0.9f * MovingAxis), rigid.position.y), Vector2.down, 3f, layerMask);
                    if (rayhit2.collider == null)
                        MovingAxis = 0;
                }
                else
                    MovingAxis *= -1;
            }
        }
        else {
            if (cantMoveTime <= 0)
            {
                canMove = true;
            }
            else {
                cantMoveTime -= Time.deltaTime;
            }
        }
    }

    private void FindPlayerCheck()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        int layerMask2 = (1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D rayhit;
        if (spr.flipX)
            rayhit = Physics2D.BoxCast(rigid.position, new Vector2(4.5f, 2.5f), 0f , new Vector2(-1f, 0f), 2f, layerMask);
        else
            rayhit = Physics2D.BoxCast(rigid.position, new Vector2(4.5f, 2.5f), 0f, new Vector2(1f, 0f), 2f, layerMask);
        Debug.DrawRay(rigid.position, (player.position - transform.position).normalized * 6f, new Color(0, 1, 0));
        if (rayhit.collider != null && pstate.getCharactorKey() != 5)
        {
            RaycastHit2D rayhit2 = Physics2D.Raycast(rigid.position, (player.position - transform.position).normalized, 6f, layerMask2);
            if (rayhit2.collider == null)
            {
                playerFinding = true;
                finding_time = 5;
            }

        }

        if (estate.getIsGetDamage())
        {
            playerFinding = true;
            finding_time = 5;
        }
        

        if (playerFinding)
        {
            findPlayerMark.SetActive(true);
            if (Vector2.Distance(player.position, transform.position) > 1f)
                fs_Mult = followingSpeed;
            else
                fs_Mult = 0;
            if (Vector2.Distance(player.position, transform.position) > 5f)
            {
                if (finding_time <= 0)
                {
                    findPlayerMark.SetActive(false);
                    finding_time = 0;
                    playerFinding = false;
                    fs_Mult = 1;
                    Invoke("Moving", 0.5f);
                }
                else
                    finding_time -= Time.deltaTime;
            }
            else
            {
                finding_time = 5;

            }

        }

    }

    void Moving()
    {
        if (!playerFinding)
        {
            MovingAxis = Random.Range(-1, 2);
            Invoke("Moving", Random.Range(1.5f, 4.5f));
        }
    }

    void CC(int ccCode, float ccPower, float ccTime)
    {
        if (ccCode != -1 && ccCode != 2)
        {
            estate.ccReset();
            switch (ccCode) {
                case 0:
                    if (cantMoveTime < ccTime)
                    {
                        cantMoveTime = ccTime;
                        
                    }

                    if (cantFlipTime < ccTime)
                    {
                        cantFlipTime = ccTime;
                        
                    }
                    if (cantAttackTime < ccTime)
                    {
                        cantAttackTime = ccTime;
                        
                    }
                    canAttack = false;
                    canFlip = false;
                    canMove = false;
                    rigid.velocity = Vector2.zero;
                    break;
                case 3:
                    if (slowTime < ccTime)
                    {
                        slowTime = ccTime;
                        cc_Mult = (100 - ccPower) / 100;
                    }
                    else {
                        cc_Mult = (100 - ccPower) / 100;
                    }
                    break;
                case 5:
                    if (cantMoveTime < 0.5f + ccPower * 0.02f)
                    {
                        cantMoveTime = 0.5f + ccPower * 0.02f;
                        canMove = false;
                        rigid.velocity = Vector2.zero;
                    }                   
                    rigid.AddForce(new Vector2((transform.position - player.position).x, (transform.position - player.position).x * 0.5f).normalized * ccPower * 0.5f, ForceMode2D.Impulse);
                    break;
            }
            /*cc코드
             * 0 : 기절. 행동불능 (ccPower 필요없음)
             * 1 : 수면. 행동불능, 맞으면 없어짐 (ccPower 필요없음)
             * 2 : 독. 점진적 체력 소모 (ccPower에 따라 감소(%))
             * 3 : 마비. 느려짐 (ccPower에 따라 감소(%))
             * 4 : 경직. 이동불가 (ccPower 필요없음)
             * 5 : 밀쳐짐. 이동불가 (ccPower에 addforce, ccTime 필요없음)
             * 6 : 침묵. 기술 사용 불가 (ccPower 필요없음)
             * 7 : 실명. 명중률 감소 (ccPower에 따라 감소(%))
             * 8 : 쇠약. 공격력 감소 (ccPower에 따라 감소(%))
             * 9 : 치명 상태. 방어력 감소 (ccPower에 따라 감소(%))
             * 10 : 출혈 상태. 방어력 감소 + 지속 피해 (ccPower에 따라 감소(%))
             */
        }
    }

    IEnumerator attack()
    {
        isattack = true;
        if (spr.flipX)
        {
            LHitbox.SetActive(true);
            at_Mult = 0;
            yield return new WaitForSeconds(0.5f);
            LHitbox.SetActive(false);
            at_Mult = 1;
        }
        else 
        {
            RHitbox.SetActive(true);
            at_Mult = 0;
            yield return new WaitForSeconds(0.5f);
            RHitbox.SetActive(false);
            at_Mult = 1;
        }
        yield return new WaitForSeconds(1f);
        isattack = false;
    }

    IEnumerator skill()
    {
        isskill = true;
        int i = Random.Range(1, 3);
        yield return new WaitForSeconds(3f);
        isSkillOn = true;
        if (i == 1)
        {
            sk_Mult = 0;
            yield return new WaitForSeconds(0.5f);
            Instantiate(SkillOBJ[0], transform.position, transform.rotation);
            yield return new WaitForSeconds(1f);
            sk_Mult = 1;
        }
        else
        {
            sk_Mult = -1;
            yield return new WaitForSeconds(0.5f);
            sk_Mult = 1;
            estate.barrior += 10;
        }
        isSkillOn = false;
        isskill = false;
    }
}
