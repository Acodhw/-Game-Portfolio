using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl_Proto : MonoBehaviour
{
    Rigidbody2D rigid;
    PlayerCharactor_Proto pchar;
    PlayerState_Proto pstate;
    SpriteRenderer sr;
    public Sprite[] CharactorSprite;
    public Sprite[] PassiveSprite;
    private bool isPause = false;
    private bool canMove = true;
    private bool canFlip = true;
    private bool moveSkillOn = false;
    private bool usingStringSkillOn = false;
    public bool isgetStun = false;
    private bool isgetSilence = false;
    public float stuntime;
    private float silencetime;
    private bool isunstoppable = false;
    public bool isOnGround;
    private bool rayCaston = true;
    public bool canAttack = true;
    public bool punchbarrior = false;
    public float speed;
    private float sprintPoint = 1;
    public float jumpP;
    public float jumpTL;
    private float jumpT;
    public GameObject[] Rhitboxes;
    public GameObject[] Lhitboxes;
    public GameObject[] Reffects;
    public GameObject[] Leffects;
    public Image profile;
    public Image passive;
    public GameObject P_Passiveobj;
    public GameObject S_Passiveobj;
    public Image punchPassive;
    public Image SpearPassive;
    public GameObject SpearSun;
    public GameObject[] MoveSkillOBJ;
    public GameObject unstoppable;
    public GameObject slash;
    public GameObject cannon;
    public GameObject wire;
    public GameObject rope;

    public GameObject noTargetMassage;

    public GameObject SkillSelectInter;
    public GameObject MainInter;
    public GameObject CardActionInter;
    public GameObject CardSetInter;

    public int attackStack;

    public int spearStack;
    public int punchStack;
    private float spearStackOnTime;
    private float PunchStackOnTime;
    public float PunchPassiveTime;

    private bool canJinja = false;
    private Transform jinjaPoint;
    private float jinjaSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        pchar = GetComponent<PlayerCharactor_Proto>();
        sr = GetComponent<SpriteRenderer>();
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        pstate.player = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPause && isOnGround) {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        if (SkillSelectInter.activeSelf || CardActionInter.activeSelf || CardSetInter.activeSelf)
            isPause = true;
        else
            isPause = false;

        if (Input.GetButtonDown("SkillSelectMenu"))
            if (SkillSelectInter.activeSelf)
            {                
                SkillSelectInter.SetActive(false);
            }
            else
            {                
                if (CardSetInter.activeSelf)
                    CardSetInter.SetActive(false);
                SkillSelectInter.SetActive(true);
            }
        if (Input.GetButtonDown("CardSelectMenu")) 
            if (CardSetInter.activeSelf)
            {                
                CardSetInter.SetActive(false);
            }
            else
            {               
                if (SkillSelectInter.activeSelf)
                    SkillSelectInter.SetActive(false);
                CardSetInter.SetActive(true);
            }

        if (Input.GetButtonDown("CardAction")) 
            if (CardActionInter.activeSelf)
            {
                CardActionInter.SetActive(false);
                MainInter.SetActive(true);
                Time.timeScale = pstate.GetComponent<GameDatas>().nowtimeScale;
            }
            else
            {
                if (!isPause)
                {

                    Time.timeScale = 0.25f;
                    MainInter.SetActive(false);
                    CardActionInter.SetActive(true);
                }
            }


        if (Input.GetButtonDown("DrawCard"))
        {
            if (!SkillSelectInter.activeSelf || CardActionInter.activeSelf || CardSetInter.activeSelf)
            {
                if (pstate.OpenedCard.Count < 4 && pstate.SettingCard.Count > 0) {
                    pstate.OpenedCard.Add(pstate.SettingCard[0]);
                    pstate.SettingCardKind[pstate.SettingCard[0]] -= 1;
                    pstate.HaveCardKind[pstate.SettingCard[0]] -= 1;
                    pstate.SettingCard.RemoveAt(0);                    
                    pstate.DacMix();
                }                
            }
        }

        if (Input.GetButtonDown("CardSummon"))
        {
            if (!isPause)
            {
                if (pstate.OpenedCard.Count > 0)
                {
                    if (pstate.GetComponent<GameDatas>().card[pstate.OpenedCard[0]].needToken <= pstate.CardToken)
                    {
                        pstate.CardToken -= pstate.GetComponent<GameDatas>().card[pstate.OpenedCard[0]].needToken;
                        pstate.GetComponent<GameDatas>().card[pstate.OpenedCard[0]].events.Invoke();
                        pstate.OpenedCard.RemoveAt(0);
                    }
                    else 
                    {
                        if (pstate.GetComponent<GameDatas>().isNextTokenUseless)
                        {
                            pstate.GetComponent<GameDatas>().isNextTokenUseless = false;
                            pstate.GetComponent<GameDatas>().card[pstate.OpenedCard[0]].events.Invoke();
                            pstate.OpenedCard.RemoveAt(0);
                        }
                        else
                        {
                            TextMesh tm = Instantiate(pstate.damageTx, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, -0.5f), transform.rotation).GetComponent<TextMesh>();
                            tm.text = "토큰이 부족합니다.";
                        }
                    }
                }
            }
        }


        if (isunstoppable)
        {
            unstoppable.SetActive(true);
        }
        else
        {
            unstoppable.SetActive(false);
        }
        if (pstate.getCharactorKey() == 6)
        {
            S_Passiveobj.SetActive(false);
            P_Passiveobj.SetActive(true);
            if (PunchPassiveTime == 0)
            {
                punchPassive.fillAmount = (float)punchStack / 30;
                punchPassive.color = new Color(1, 1, 1);
            }
            else
            {
                punchPassive.fillAmount = PunchPassiveTime / 7;
                punchPassive.color = new Color(1, 0, 0);
            }
        }
        else if (pstate.getCharactorKey() == 3)
        {
            S_Passiveobj.SetActive(true);
            P_Passiveobj.SetActive(false);
            SpearPassive.fillAmount = spearStack * 0.333f;
            SpearPassive.color = new Color(spearStack * 0.333f, spearStack * 0.333f, spearStack * 0.333f);
        }
        else
        {
            S_Passiveobj.SetActive(false);
            P_Passiveobj.SetActive(false);
        }
        if (PunchPassiveTime <= 0)
        {
            PunchPassiveTime = 0;
            if (punchbarrior)
            {
                punchbarrior = false;
                if (pstate.barrior < pstate.getMaxHP() * 1.25f)
                    pstate.barrior = 0;
                else
                    pstate.barrior -= pstate.getMaxHP() * 1.25f;
            }
        }
        else
        {
            PunchPassiveTime -= Time.deltaTime;
        }

        if (isgetStun)
        {
            if (stuntime <= 0)
            {
                isgetStun = false;
            }
            else
            {
                stuntime -= Time.deltaTime;
            }
        }
        if (isgetSilence)
        {
            if (silencetime <= 0)
            {
                isgetSilence = false;
            }
            else
            {
                silencetime -= Time.deltaTime;
            }
        }
        if (spearStack != 0)
        {
            if (spearStackOnTime <= 0)
            {
                spearStack = 0;
            }
            else
            {
                spearStackOnTime -= Time.deltaTime;
            }
        }
        if (punchStack != 0)
        {
            if (PunchStackOnTime <= 0)
            {
                punchStack = 0;
            }
            else
            {
                PunchStackOnTime -= Time.deltaTime;
            }
        }
        sr.sprite = CharactorSprite[pstate.getCharactorKey()];
        profile.sprite = CharactorSprite[pstate.getCharactorKey()];
        passive.sprite = PassiveSprite[pstate.getCharactorKey()];
        if (!isPause)
        {
            if (canMove && !isgetStun && !moveSkillOn)
            {
                if (!usingStringSkillOn)
                {
                    Jump();
                    movingSkill();

                }
                if (!(pchar.AttackSkill1Act || pchar.AttackSkill1Act))
                {
                    if (!pchar.skillonbutcanAct)
                        characterChange();
                    attackSkill();
                    Attack();
                }
            }
            Dash();
        }
    }

    // FixedUpdate is called once per FixedTime Default is 0.02seconds
    private void FixedUpdate()
    {
        if (!isPause)
        {
            if (!moveSkillOn && !usingStringSkillOn && !pchar.skillonbutcantMove)
                Move();
        }
        CC(pstate.ccCode, pstate.ccPower, pstate.ccTime);

        if (canJinja && usingStringSkillOn && !isPause)
            jinjaMove();

        bool isg1 = true;
        bool isg2 = true;
        if (rayCaston)
        {

            int layerMask = 1 << LayerMask.NameToLayer("Ground");
            Debug.DrawRay(new Vector2(rigid.position.x - 0.3f, rigid.position.y), Vector2.down * 0.7f, new Color(0, 1, 0));
            Debug.DrawRay(new Vector2(rigid.position.x + 0.3f, rigid.position.y), Vector2.down * 0.7f, new Color(0, 1, 0));

            RaycastHit2D rayhit = Physics2D.Raycast(new Vector2(rigid.position.x - 0.3f, rigid.position.y), Vector2.down, 0.7f, layerMask);
            if (rayhit.collider != null)
                isg1 = true;
            else
                isg1 = false;

            rayhit = Physics2D.Raycast(new Vector2(rigid.position.x + 0.3f, rigid.position.y), Vector2.down, 0.7f, layerMask);
            if (rayhit.collider != null)
                isg2 = true;
            else
                isg2 = false;
            isOnGround = (isg2 || isg1);
        }

    }

    //캐릭터 무브

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (canFlip && !isgetStun)
        {
            if (h < 0)
                sr.flipX = true;
            else if (h > 0)
                sr.flipX = false;
        }

        if (canMove && !isgetStun)
        {
            if (!isOnGround)
            {
                if (h * rigid.velocity.x < 0)
                    rigid.AddForce(Vector2.right * h * speed * 3f, ForceMode2D.Force);
                else
                    rigid.AddForce(Vector2.right * h * speed * 1.5f, ForceMode2D.Force);
            }
            else
            {
                if (h * rigid.velocity.x < 0)
                    rigid.AddForce(Vector2.right * h * speed * sprintPoint * 8f, ForceMode2D.Force);
                else
                    rigid.AddForce(Vector2.right * h * speed * sprintPoint * 2, ForceMode2D.Force);
            }

            if (rigid.velocity.x >= speed * sprintPoint)
            {

                rigid.velocity = new Vector2(speed * sprintPoint, rigid.velocity.y);
            }
            else if (rigid.velocity.x <= -speed * sprintPoint)
            {

                rigid.velocity = new Vector2(-speed * sprintPoint, rigid.velocity.y);
            }

            if (h == 0 && isOnGround)
            {
                if (rigid.velocity.x < 0.01f && rigid.velocity.x > -0.01f)
                    rigid.velocity = new Vector2(0, rigid.velocity.y);
                else
                    rigid.AddForce(Vector2.left * speed * rigid.velocity.x * 4f, ForceMode2D.Force);
            }
        }
    }
    private void Jump()
    {
        if (isOnGround && !isgetStun && rigid.gravityScale != 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rayCaston = false;
                isOnGround = false;
            }
        }


        if (!Input.GetButton("Jump") || jumpT >= jumpTL)
        {
            rayCaston = true;
            jumpT = 0;
        }

        if (!rayCaston)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0f);
            if (Input.GetButton("Horizontal") && (rigid.velocity.x < speed * sprintPoint * 0.3f && rigid.velocity.x > -speed * sprintPoint * 0.3f))
                rigid.AddForce((Vector2.right * speed * 0.5f * Input.GetAxisRaw("Horizontal")), ForceMode2D.Impulse);

            rigid.AddForce((Vector2.up * jumpP * ((jumpT * 10) + 1f)), ForceMode2D.Impulse);

            jumpT += Time.deltaTime;
        }
    }
    private void Dash()
    {
        if (isOnGround)
        {
            if (Input.GetButton("Dash"))
                sprintPoint = 1.75f;
            else
                sprintPoint = 1f;
        }
    }
    private void jinjaMove()
    {
        /*Vector2 vec = transform.position - jinjaPoint.position;
        
        if (maxAngle >= 180) {
            maxAngle = 180;
        }
        if (angle > maxAngle) {
            maxAngle = angle;
        }
        rigid.AddForce(-vec.normalized * rigid.mass * rigid.gravityScale * 9.81f * (3 * Mathf.Cos(angle * Mathf.PI / 180) - 2 * Mathf.Cos(maxAngle * Mathf.PI / 180)), ForceMode2D.Force);*/
        Vector2 vec = transform.position - jinjaPoint.position;
        float angle = Vector2.SignedAngle(Vector2.down, vec);
        rigid.AddForce((-vec.normalized * jinjaSpeed * jinjaSpeed) / Vector3.Distance(transform.position, jinjaPoint.position), ForceMode2D.Force);

        if (jinjaSpeed < -12)
            jinjaSpeed = -12;
        if (jinjaSpeed > 12)
            jinjaSpeed = 12;

        if (Input.GetButton("Horizontal"))
        {
            jinjaSpeed += 4 * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        }
        if (vec.x < 0)
        {
            jinjaSpeed += 2.5f * Time.deltaTime;
        }
        else if(vec.x > 0)
        {
            jinjaSpeed -= 2.5f * Time.deltaTime;
        }
        rigid.velocity = new Vector2(-vec.y, vec.x).normalized * jinjaSpeed;        
    }

    //캐릭터 상태

    public void CC(int ccCode, float ccPower, float ccTime)
    {
        if (ccCode != -1 && ccCode != 2)
        {
            if (isunstoppable)
                pstate.remCC();
            else
            {
                pstate.remCC();
                moveSkillOn = false;
                switch (ccCode)
                {
                    case 0:
                        if (stuntime < ccTime)
                        {
                            stuntime = ccTime;

                        }
                        isgetStun = true;
                        rigid.velocity = Vector2.zero;
                        break;
                    case 5:
                        if (stuntime < 0.5f + ccPower * ccPower * 0.0004f)
                        {
                            stuntime = 0.5f + ccPower * ccPower * 0.0004f;

                        }
                        isgetStun = true;
                        rigid.velocity = Vector2.zero;
                        rigid.AddForce((Vector2.right + Vector2.up * 0.5f).normalized * ccPower * 0.5f, ForceMode2D.Impulse);
                        break;
                }
                /*cc코드
                 * 0 : 기절. 행동불능 (ccPower 필요없음)
                 * 1 : 수면. 행동불능, 맞으면 없어짐 (ccPower 필요없음)
                 * 2 : 독. 점진적 체력 소모 (ccPower에 따라 감소(%))
                 * 3 : 마비. 느려짐 (ccPower에 따라 감소(%))
                 * 4 : 경직. 이동불가 (ccPower 필요없음)
                 * 5 : 밀쳐짐. 이동불가 (ccPower에 addforceccTime 필요없음)
                 * 6 : 침묵. 기술 사용 불가 (ccPower 필요없음)
                 * 7 : 실명. 명중률 감소 (ccPower에 따라 감소(%))
                 * 8 : 쇠약. 공격력 감소 (ccPower에 따라 감소(%))
                 * 9 : 치명 상태. 방어력 감소 (ccPower에 따라 감소(%))
                 */
            }
        }
    }

    private void characterChange()
    {
        if (Input.GetButtonDown("Changing"))
        {
            if (Input.GetAxisRaw("Changing") == -1 && pstate.getCharactorKey() == 0)
                pstate.setCharactorKey(7);
            else if (Input.GetAxisRaw("Changing") == 1 && pstate.getCharactorKey() == 7)
                pstate.setCharactorKey(0);
            else
                pstate.setCharactorKey(pstate.getCharactorKey() + (int)(Input.GetAxisRaw("Changing")));
        }
    }
    public void spearStackUp()
    {
        if (spearStack < 3)
        {
            spearStack++;
            spearStackOnTime = 0.7f;
        }
        else
        {
            spearStack = 0;
            spearStackOnTime = 0;
            GameObject g = Instantiate(SpearSun, transform.position, transform.rotation);
            g.transform.parent = transform;
        }
    }
    public void PunchStackUp()
    {
        if (punchStack < 29)
        {
            if (PunchPassiveTime <= 0)
            {
                punchStack++;
                PunchStackOnTime = 10f;
            }
        }
        else
        {
            punchStack = 0;
            PunchPassiveTime = 7f;
            pstate.barrior += pstate.getMaxHP() * 1.25f;
            punchbarrior = true;
        }
    }

    //캐릭터 스킬

    private void Attack()
    {
        if (Input.GetButton("Attack") && canAttack)
        {
            canAttack = false;
            switch (pstate.getCharactorKey())
            {
                case 0:
                    StartCoroutine(attacking(0.25f * 1.333f, 0.15f, 0, 3, 4f));
                    break;
                case 1:
                    StartCoroutine(attacking(0.333f * 1.333f, 0.333f, 3, 1, -1));
                    break;
                case 2:
                    StartCoroutine(attacking(0.25f * 1.333f, 0.25f, 4, 1, -1));
                    break;
                case 3:
                    StartCoroutine(attacking(0.25f * 1.333f, 0.15f, 5, 1, 6f));
                    break;
                case 4:
                    StartCoroutine(attacking(0.333f * 1.333f, 0.2f, 6, 1, 2f));
                    break;
                case 5:
                    StartCoroutine(attacking(0.25f * 1.333f, 0.15f, 7, 3, -1));
                    break;
                case 6:
                    StartCoroutine(attacking(0.167f * 1.333f, 0.15f, 10, 4, -1));
                    break;
                case 7:
                    StartCoroutine(attacking(1f, 0, 14, 1, -1));
                    break;
            }
        }
    }

    private void movingSkill()
    {
        if (Input.GetButtonDown("Movingskill"))
        {
            if (Input.GetAxisRaw("Movingskill") == -1 && pchar.canMoveSkill1 && pstate.MoveSkill1 != -1)
            {
                pchar.canMoveSkill1 = false;
                StartCoroutine(MoveSkillCoroutine(pstate.MoveSkill1, 1));
            }
            else if (Input.GetAxisRaw("Movingskill") == 1 && pchar.canMoveSkill2 && pstate.MoveSkill2 != -1)
            {
                pchar.canMoveSkill2 = false;
                StartCoroutine(MoveSkillCoroutine(pstate.MoveSkill2, 2));
            }

        }


        /*움직임 스킬
         * 1. Slash : 앞으로 이동하며 전방에 적은데미지
         * 2. Flash : 보는 방향으로 순간이동
         * 3. Wire : 특정 위치로 자신을 끌어당기는 줄 생성
         * 4. String : 특정 위치를 기준으로 진자 운동을 하는 줄 생성
         * 5. Fly : 중력을 무시하고 잠시 이동
         * 6. Canon : 먼 거리를 빠르게 이동하며 큰 데미지
         * 7. Target : 가장 가까운 적에게 순간이동
         * 8. Unstoppable : 1초간 cc기에 면역
         */

    }
    private void attackSkill()
    {
        if (Input.GetButtonDown("AttackSkill1") && pstate.AttackSkill1[pstate.getCharactorKey()] != -1)
        {
            pchar.usingSkill(pstate.AttackSkill1[pstate.getCharactorKey()], 1);
        }
        else if (Input.GetButtonDown("AttackSkill2") && pstate.AttackSkill2[pstate.getCharactorKey()] != -1)
        {
            pchar.usingSkill(pstate.AttackSkill2[pstate.getCharactorKey()], 2);
        }
    }

    // 코루틴
    IEnumerator attacking(float attackCool, float PlusAttackTime, int attackEFFCode, int mult, float attackMove)
    {
        //attackMove가 -1이면 움직이면서 공격, 아닐경우 해당 addforce를한다.
        for (attackStack = 0; attackStack < mult; attackStack++)
        {
            if (Input.GetButton("Attack"))
            {
                canFlip = false;
                if (!sr.flipX)
                {

                    Reffects[attackEFFCode + attackStack].SetActive(true);
                    Rhitboxes[pstate.getCharactorKey()].SetActive(true);
                    Lhitboxes[pstate.getCharactorKey()].SetActive(false);
                    if (attackMove != -1 && isOnGround && !isgetStun)
                    {
                        canMove = false;
                        rigid.velocity = new Vector2(rigid.velocity.x * 0.35f, 0);
                        rigid.AddForce(new Vector2(attackMove, 0), ForceMode2D.Impulse);
                    }
                }
                else
                {
                    Leffects[attackEFFCode + attackStack].SetActive(true);
                    Lhitboxes[pstate.getCharactorKey()].SetActive(true);
                    Rhitboxes[pstate.getCharactorKey()].SetActive(false);
                    if (attackMove != -1 && isOnGround && !isgetStun)
                    {
                        canMove = false;
                        rigid.velocity = new Vector2(rigid.velocity.x * 0.35f, 0);
                        rigid.AddForce(new Vector2(-attackMove, 0), ForceMode2D.Impulse);
                    }
                }

                yield return new WaitForSeconds(attackCool * 0.5f);
                if (attackMove != -1 && isOnGround)
                    rigid.velocity = new Vector2(0, 0);

                if (!sr.flipX)
                    Rhitboxes[pstate.getCharactorKey()].SetActive(false);
                else
                    Lhitboxes[pstate.getCharactorKey()].SetActive(false);

                canFlip = true;
                yield return new WaitForSeconds(attackCool * 0.5f);
                if (Reffects[attackEFFCode + attackStack].activeSelf)
                {
                    Reffects[attackEFFCode + attackStack].SetActive(false);
                    if (attackMove != -1)
                    {
                        canMove = true;
                    }
                }
                else
                {
                    Leffects[attackEFFCode + attackStack].SetActive(false);
                    if (attackMove != -1)
                    {
                        canMove = true;
                    }
                }
            }
            else
            {
                break;
            }
        }
        yield return new WaitForSeconds(PlusAttackTime);
        canAttack = true;
    }
    IEnumerator skillCooltime(int code, float cooltime)
    {
        yield return new WaitForSeconds(cooltime);
        switch (code)
        {
            case 1:
                pchar.canMoveSkill1 = true;
                break;
            case 2:
                pchar.canMoveSkill2 = true;
                break;
        }
    }    
    IEnumerator MoveSkillCoroutine(int skillcode, int code)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        moveSkillOn = true;
        switch (skillcode)
        {
            case 0:

                if (code == 1)
                {
                    pchar.MoveSkill1Act = true;
                }
                else
                {
                    pchar.MoveSkill2Act = true;
                }
                rigid.gravityScale = 0;
                slash.SetActive(true);
                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 20;
                else
                {

                    if (sr.flipX)
                    {
                        rigid.velocity = Vector2.left * 30f;
                    }
                    else
                    {
                        rigid.velocity = Vector2.right * 30f;
                    }
                }
                yield return StartCoroutine(count(0.15f));
                if (code == 1)
                {
                    pchar.MoveSkill1Act = false;
                }
                else
                {
                    pchar.MoveSkill2Act = false;
                }
                rigid.gravityScale = 5;
                slash.SetActive(false);
                moveSkillOn = false;
                StartCoroutine(skillCooltime(code, 5));
                break;
            case 1:
                moveSkillOn = false;
                Vector2 a;
                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    a = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 20;
                else
                {

                    if (sr.flipX)
                    {
                        a = Vector2.left;
                    }
                    else
                    {
                        a = Vector2.right;
                    }
                }


                RaycastHit2D rayhit = Physics2D.Raycast(transform.position + (new Vector3(a.normalized.x, a.normalized.y, 0) * 5), a.normalized, 0.1f, layerMask);
                if (rayhit.collider != null)
                {
                    rayhit = Physics2D.Raycast(rigid.position, a.normalized, 5f, layerMask);
                    transform.position = rayhit.point - a.normalized * 0.5f;
                }
                else
                    transform.position = transform.position + (new Vector3(a.normalized.x, a.normalized.y, 0) * 5);
                StartCoroutine(skillCooltime(code, 5));
                break;
            case 2:
                moveSkillOn = false;
                if (code == 1)
                {
                    pchar.MoveSkill1Act = true;
                }
                else
                {
                    pchar.MoveSkill2Act = true;
                }
                GameObject summoned_wire = Instantiate(wire, transform.position, transform.rotation);
                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    summoned_wire.GetComponent<Wire_Proto>().foward = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 20).normalized;
                else
                {

                    if (sr.flipX)
                    {
                        summoned_wire.GetComponent<Wire_Proto>().foward = (Vector2.left * 30f).normalized;
                    }
                    else
                    {
                        summoned_wire.GetComponent<Wire_Proto>().foward = (Vector2.right * 30f).normalized;
                    }
                }
                yield return new WaitUntil(() => (summoned_wire.GetComponent<Wire_Proto>().isontheWall || !summoned_wire.activeSelf));                
                if (!summoned_wire.activeSelf)
                {
                    Destroy(summoned_wire);
                    if (code == 1)
                    {
                        pchar.canMoveSkill1 = true;
                    }
                    else
                    {
                        pchar.canMoveSkill2 = true;
                    }
                    if (code == 1)
                    {
                        pchar.MoveSkill1Act = false;
                    }
                    else
                    {
                        pchar.MoveSkill2Act = false;
                    }
                    break;
                }
                else {
                    usingStringSkillOn = true;
                    rigid.gravityScale = 0;
                    rigid.velocity = (summoned_wire.GetComponent<Wire_Proto>().point.position - transform.position) * 1.5f;
                    yield return new WaitUntil(() => (Vector3.Distance(summoned_wire.GetComponent<Wire_Proto>().point.position, transform.position) < 1 || Input.GetButton("Jump") || isgetSilence || isgetStun || !canMove));
                    if (Input.GetButton("Jump"))
                    {
                        
                        Jump();
                        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    }
                    usingStringSkillOn = false;
                    rigid.gravityScale = 5;
                    if (code == 1)
                    {
                        pchar.MoveSkill1Act = false;
                    }
                    else
                    {
                        pchar.MoveSkill2Act = false;
                    }
                    Destroy(summoned_wire);
                    StartCoroutine(skillCooltime(code, 6));
                }
                
                break;
            case 3:
                moveSkillOn = false;
                if (code == 1)
                {
                    pchar.MoveSkill1Act = true;
                }
                else
                {
                    pchar.MoveSkill2Act = true;
                }
                GameObject summoned_rope = Instantiate(rope, transform.position, transform.rotation);
                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    summoned_rope.GetComponent<Pendulum_Proto>().foward = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 20).normalized;
                else
                {

                    if (sr.flipX)
                    {
                        summoned_rope.GetComponent<Pendulum_Proto>().foward = (Vector2.left * 30f).normalized;
                    }
                    else
                    {
                        summoned_rope.GetComponent<Pendulum_Proto>().foward = (Vector2.right * 30f).normalized;
                    }
                }
                yield return new WaitUntil(() => (summoned_rope.GetComponent<Pendulum_Proto>().isontheWall || !summoned_rope.activeSelf));
                if (!summoned_rope.activeSelf)
                {
                    Destroy(summoned_rope);
                    if (code == 1)
                    {
                        pchar.canMoveSkill1 = true;
                    }
                    else
                    {
                        pchar.canMoveSkill2 = true;
                    }
                    if (code == 1)
                    {
                        pchar.MoveSkill1Act = false;
                    }
                    else
                    {
                        pchar.MoveSkill2Act = false;
                    }
                    break;
                }
                else
                {
                    /*
                    float length = Vector3.Distance(summoned_rope.GetComponent<Pendulum_Proto>().point.position, transform.position);
                    float angle = Vector2.SignedAngle(Vector2.down, vec) * Mathf.PI / 180;                    
                    
                    

                    float plusheight = 0.5f * Plusspeed * Plusspeed / (9.81f * rigid.gravityScale);
                    float height = length * Mathf.Cos(angle);
                    maxAngle = Mathf.Acos((height - plusheight) / (length));*/
                    Vector2 moveDir = rigid.velocity.normalized;
                    Vector2 vec = transform.position - summoned_rope.GetComponent<Pendulum_Proto>().point.position;
                    float Plusspeed = Mathf.Sin(Mathf.Abs(Vector2.SignedAngle(moveDir, vec)) * Mathf.PI / 180);
                    jinjaSpeed = Plusspeed;
                    if (vec.y > 0)
                        rigid.velocity = new Vector2(-vec.y, vec.x).normalized * Plusspeed;
                    else
                        rigid.velocity = new Vector2(vec.y, -vec.x).normalized * Plusspeed;

                    usingStringSkillOn = true;
                    canJinja = true;
                    jinjaPoint = summoned_rope.GetComponent<Pendulum_Proto>().point;
                    rigid.gravityScale = 0;
                    Debug.DrawRay(rigid.position, -((transform.position - jinjaPoint.position).normalized), new Color(1, 0, 0), Vector3.Distance(transform.position, jinjaPoint.position) * 0.9f);
                    yield return new WaitUntil(() => (Physics2D.Raycast(rigid.position, -((transform.position - jinjaPoint.position).normalized), Vector3.Distance(transform.position, jinjaPoint.position) * 0.85f, layerMask).collider != null || Vector3.Distance(jinjaPoint.position, transform.position) > 16f || Input.GetButton("Jump") || isgetSilence || isgetStun || !canMove));
                    rigid.gravityScale = 5;
                    jinjaPoint = null;
                    canJinja = false;
                    if (Input.GetButton("Jump"))
                    {
                        Jump();
                        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    }
                    usingStringSkillOn = false;
                    if (code == 1)
                    {
                        pchar.MoveSkill1Act = false;
                    }
                    else
                    {
                        pchar.MoveSkill2Act = false;
                    }
                    Destroy(summoned_rope);
                    StartCoroutine(skillCooltime(code, 6));
                }

                break;
            case 4:
                if (code == 1)
                {
                    pchar.MoveSkill1Act = true;
                }
                else
                {
                    pchar.MoveSkill2Act = true;
                }
                moveSkillOn = false;
                rigid.gravityScale = 0;
                yield return StartCoroutine(count(1));
                if (code == 1)
                {
                    pchar.MoveSkill1Act = false;
                }
                else
                {
                    pchar.MoveSkill2Act = false;
                }
                rigid.gravityScale = 5;
                StartCoroutine(skillCooltime(code, 10));
                break;
            case 5:
                if (code == 1)
                {
                    pchar.MoveSkill1Act = true;
                }
                else
                {
                    pchar.MoveSkill2Act = true;
                }
                rigid.gravityScale = 0;
                cannon.SetActive(true);
                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                    rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 10;
                else
                {

                    if (sr.flipX)
                    {
                        rigid.velocity = Vector2.left * 15f;
                    }
                    else
                    {
                        rigid.velocity = Vector2.right * 15f;
                    }
                }

                yield return new WaitUntil(() => ((Physics2D.Raycast(rigid.position + Vector2.down * 0.3f, rigid.velocity.normalized, 0.5f, layerMask)).collider != null) || ((Physics2D.Raycast(rigid.position + Vector2.up * 0.3f, rigid.velocity.normalized, 0.5f, layerMask)).collider != null) || (isgetStun || isgetSilence || !canMove) || rigid.velocity == Vector2.zero);
                if (code == 1)
                {
                    pchar.MoveSkill1Act = false;
                }
                else
                {
                    pchar.MoveSkill2Act = false;
                }
                rigid.gravityScale = 5;
                cannon.SetActive(false);
                moveSkillOn = false;
                StartCoroutine(skillCooltime(code, 15));
                break;
            case 6:
                moveSkillOn = false;
                var objects = GameObject.FindGameObjectsWithTag("Enemy").ToList();

                // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
                var neareastObject = objects
                    .OrderBy(obj =>
                    {
                        return Vector3.Distance(transform.position, obj.transform.position);
                    })
                .FirstOrDefault();
                if (neareastObject != null && Vector3.Distance(transform.position, neareastObject.transform.position) < 13f)
                {
                    transform.position = neareastObject.transform.position;
                    StartCoroutine(skillCooltime(code, 7));
                }
                else
                {
                    if (code == 1)
                    {
                        pchar.canMoveSkill1 = true;
                    }
                    else
                    {
                        pchar.canMoveSkill2 = true;
                    }
                    Instantiate(noTargetMassage, transform.position, transform.rotation);
                }
                break;
            case 7:
                if (code == 1)
                {
                    pchar.MoveSkill1Act = true;
                }
                else
                {
                    pchar.MoveSkill2Act = true;
                }
                isunstoppable = true;
                moveSkillOn = false;
                yield return new WaitForSeconds(2f);
                if (code == 1)
                {
                    pchar.MoveSkill1Act = false;
                }
                else
                {
                    pchar.MoveSkill2Act = false;
                }
                isunstoppable = false;
                skillCooltime(code, 25);
                break;
        }
    }

    public IEnumerator count(float time)
    {
        for (int i = 0; i < (int)(time * 100); i++)
        {
            if (isgetStun || isgetSilence || !canMove)
                break;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
