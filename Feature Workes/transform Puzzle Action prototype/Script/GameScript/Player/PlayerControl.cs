using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerControl : MonoBehaviour
{
    private PlayerState pstate;
    private GameManager manager;

    private Rigidbody2D rigid;// 강체 물리엔진
    private Animator anim; // 플레이어 애니메이터
    private SpriteRenderer sprite; // 플레이어 스프라이트렌더
    private Material ownMaterial; // 본래 머터리얼

    private GameObject pushingObj; // 밀고 있는 오브젝트 체크
    private GameObject summonedFlail; // 철퇴 머리 소환 오브젝트
    private InteractObjs[] formChangingObj; // 폼을 변경한 오브젝트 목록

    private IEnumerator avoidingCoroutine; // 회피 코루틴을 저장
    private IEnumerator attackCoroutine; // 공격의 코루틴을 저장
    private IEnumerator[] skillkCoroutine; // 스킬의 코루틴을 저장

    private int nowForm = 0; // 지금 플레이어의 형태
    private int avoidCount = 1; // 회피 카운트를 제시합니다.

    private float movement; // 현재 x축 움직임
    private float nowGravitySpeed; // 현재 y축 움직임
    private float nowSurfaceAngle; // 현재 표면의 각도
    private float jumpSpeed; // 점프 가속
    private float fluidSteminaCheck; // 유체 상태 대쉬 스테미나 감소
    private float formChangeCooltime; // 폼을 바꾸기 위한 쿨타임 체크
    private float chargedPower = 0; // 탄성 상태에서 저장되어있는 힘입니다.
    private float[] EnergyTime = { 0, 0 }; // 전도체 상태에서 에너지가 보관되는 시간
    private float airborneInitTime; // 에어본 체크시간
    private float stunedTime; // 플레이어가 스턴 상태인지
    private float damEffectTime; // 데미지 쿨타임 생성

    private bool sprint; // 스프린트 중인지
    private bool isDead = false; // 사망 상태인지
    private bool isOnGround = false; // 플레이어가 땅 위에 있는지   
    private bool isGuard = false; // 현재 가드 상태인지
    private bool isInWater = false; // 플레이어가 물 안에 있는지
    private bool isInWaterAir = false; // 플레이어가 물 안에 있는지
    private bool isOnRoof = false; // 천장에 머리가 박았는지 체크
    private bool inJump = false; // 플레이어가 점프 중인지 체크
    private bool isJumpKeyActive = true; // 점프 키가 엑티브 중인지 확인
    private bool isSelecting = false; // 플레이어가 선택 관련 액션을 하고있는지 확인
    private bool isOnAttacking = false; // 공격 중인지 확인
    private bool isUsingSkill = false; // 스킬 사용중인지 확인
    private bool isAvoiding = false; // 회피 중인지 확인
    private bool freezeGravity = false; // 중력 고정
    private bool freezeMovement = false; // 이동 고정
    private bool wallhit = true; // 벽에 붙은 상태인지
    private bool avoidingCool = true; // 회피가 쿨타임인지

    [Header("Ground Check Settings")]
    [SerializeField][Tooltip("땅을 어디부터 체크할지 정합니다")]
    private Vector3 groundCheckPoint;
    [SerializeField][Tooltip("땅 체크 반지름을 설정합니다")]
    private float groundCheckRadious = 0.5f;
    [SerializeField][Tooltip("점프 키 누르기 시간 계수를 설정합니다")]
    private float jumpKeyValue = 1f;
    [SerializeField][Tooltip("땅의 레이어를 지정합니다")]
    private LayerMask gLayer;
    [SerializeField][Tooltip("물의 레이어를 지정합니다")]
    private LayerMask wLayer;

    [Header("Movement Settings")] 
    [SerializeField][Tooltip("공중 회피의 최대치를 지정합니다")]
    private int maxAvoid = 1;
    [SerializeField][Tooltip("플레이어의 기본 이동속도를 지정합니다")]
    private float moveSpeed = 7f;
    [SerializeField][Tooltip("달리기 계수를 지정합니다")]
    private float sprintValue = 1.5f;
    [SerializeField][Tooltip("중력 계수를 지정합니다")]
    private float gravityScale = 3f;
    [SerializeField][Tooltip("점프의 힘을 지정합니다")]
    private float jumpPower = 15f; 
    [SerializeField][Tooltip("플레이어의 표면 움직임 세팅을 지정합니다")]
    private Rigidbody2D.SlideMovement slideMovenent;
    [SerializeField][Tooltip("물에서 움직일 때 저항성을 지정합니다(1의 경우 물에서 평소와 같은 속도로 움직입니다)")]
    private float moveOnWaterResi = 3;

    [Header("Attack/Skill Settings")]
    [SerializeField][Tooltip("플레이어 공격 쿨타임을 지정합니다")]
    private Vector2[] attackTime;
    [SerializeField][Tooltip("플레이어 공격 시 이동하는 정도를 지정합니다")]
    private float[] attackMovePower;
    [SerializeField][Tooltip("플레이어 스킬의 쿨타임을 지정합니다")]
    private float[] skillCooltime;
    [SerializeField][Tooltip("플레이어 스킬의 스테미나 소모값을 지정합니다")]
    private int[] skillStemina;
    [SerializeField][Tooltip("플레이어의 공격 피격 판정 오브젝트입니다. 0번은 부모 오브젝트입니다.")]
    private GameObject[] attackColliders;
    [SerializeField][Tooltip("도리깨 헤드 오브젝트")]
    private GameObject flail;
    [SerializeField][Tooltip("탄성 파워 어택 오브젝트")]
    private GameObject elastPowerEff;
    [SerializeField][Tooltip("피격 당했을 때의 머터리얼")]
    private Material flashMaterial;

    [Header("Game Logic Settings")]
    [SerializeField][Tooltip("플레이어의 메인 카메라를 지정합니다")]
    private Camera mainCam;
    [SerializeField][Tooltip("플레이어가 선택할 오브젝트를 보여주는 아이콘 이미지")]
    private Image selectImg;
    [SerializeField][Tooltip("플레이어가 지정할 때 그 상태를 나타내도록 화면에 띄우는 이미지")]
    private Image slowedImg;
    [SerializeField][Tooltip("플레이어가 속성을 부여한 오브젝트의 최대 유지 거리를 지정합니다")]
    private float maxFormDist;
    [SerializeField][Tooltip("에너지 오브젝트를 지정합니다")]
    private GameObject[] EnergyObjs;
    [SerializeField][Tooltip("열 에너지 보관 쿨타임을 지정합니다")]
    private Image fireCoolImg;
    [SerializeField][Tooltip("전기 에너지 보관 타임을 지정합니다.")]
    private Image elecCoolImg;
    [SerializeField][Tooltip("플레이어가 보관한 물리적 힘을 지정합니다.")]
    private Image powerImg;

    void Start()
    {
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState>();
        manager = pstate.GetComponent<GameManager>();
        pstate.SetPlayer(this);
        formChangingObj = new InteractObjs[4];
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        ownMaterial = sprite.material;
        skillkCoroutine = new IEnumerator[4];

    }

    void Update()
    {
        SetAnimator();
        ReturnFormToObj();
        CheckCooltimes();
        DeadEventCheck();
        PlayerDamageSet();
        if (!isDead && !(stunedTime > 0) && !manager.GetOnCutScene())
        {
            AvoidSprint();
            Jump();
            Attack();
            UseSkill();
            FormChange();
            GiveFormToObj();
        }
    }

    void FixedUpdate()
    {
        Move();  
        GroundCheck();
        SetGravity();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("DeadZone"))
        {
            DamageInfo d = new DamageInfo();
            d.damage = 999999999;
            pstate.Damage(d, false, collision.transform);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != EnergyObjs[0] && collision.gameObject != EnergyObjs[1])
        {
            if (collision.gameObject.layer == 9 && nowForm == 2)
            {
                if (collision.tag.Equals("Fire")) EnergyTime[0] = 3f;
                if (collision.tag.Equals("Electric")) EnergyTime[1] = 3f;
            }
            
            if (collision.gameObject.layer == 9 && !(nowForm == 2 || nowForm == 4) && !isAvoiding)
            {
                if(collision.GetComponent<DamageZone>() != null)
                {
                    pstate.Damage(collision.GetComponent<DamageZone>().GetDamage(), (stunedTime > 0), collision.transform);
                }
            }
            if (collision.gameObject.layer == 12) {
                if (collision.GetComponent<DamageZone>() != null && !isGuard && !isAvoiding)
                {
                    pstate.Damage(collision.GetComponent<DamageZone>().GetDamage(), (stunedTime > 0), collision.transform);
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null) {
            Rigidbody2D rg = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rg.bodyType == RigidbodyType2D.Dynamic && !isOnAttacking && !isUsingSkill)
            {
                Vector2 dir = rg.position - (Vector2)transform.position;
                dir.Normalize();
                Vector2 realPowDir = dir;
                RaycastHit2D ray;
                if (collision.gameObject.layer == 8)
                {
                    ray = Physics2D.Raycast(transform.position, dir, Vector2.Distance(rg.position, transform.position), LayerMask.GetMask("Interaction"));
                    if (ray.collider != null) realPowDir = -ray.normal;
                    realPowDir.y = 0;
                    realPowDir.Normalize();
                }
                rg.AddForce(realPowDir * 25, ForceMode2D.Force);
                ray = Physics2D.Raycast(transform.position, sprite.flipX ? Vector2.left : Vector2.right, 1.2f, LayerMask.GetMask("Interaction"));
                if (ray.collider != null && Mathf.Abs(ray.normal.x) > 0.5f && (sprite.flipX ? (0 > movement) : (0 < movement)))
                    pushingObj = rg.gameObject;
                else pushingObj = null;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject == pushingObj) pushingObj = null;
    }

    // Outside using

    // 플레이어를 기절시킵니다
    public void Stun(CCInfo info, float power, bool pushLeft) {
        if (isDead) return;
        AllStopCoroutine();
        switch (info) {
            case CCInfo.stun:
                stunedTime = power;
                break;
            case CCInfo.push:
                stunedTime = power * 0.1f;
                movement = (pushLeft ? -1 : 1) * power * 3;
                nowGravitySpeed = power * 1.5f;
                airborneInitTime = 0.05f;
                break;
            case CCInfo.airborne:
                stunedTime = power * 0.2f;
                movement = 0;
                nowGravitySpeed = power * 3;
                airborneInitTime = 0.05f;
                break;
        }
    }

    public void FlashPlayer()
    {
        damEffectTime = 0.1f;
    }

    void AllStopCoroutine() {
        for (int i = 1; i < 9; i++)
            attackColliders[i].SetActive(false);
        Destroy(summonedFlail);
        if(attackCoroutine != null) StopCoroutine(attackCoroutine);
        if (avoidingCoroutine != null) StopCoroutine(avoidingCoroutine);
        for (int i = 0; i < 4; i++)
        {
            if (skillkCoroutine[i] != null) StopCoroutine(skillkCoroutine[i]);
        }
    }

    // using Update
    // 키에 작동하는 bool들을 세팅합니다.
    void AvoidSprint() {
        
        if (isOnGround || isInWater) avoidCount = maxAvoid;
        if (nowForm > 0 && !isAvoiding && avoidCount > 0)
        {
            if (!sprint && avoidingCool && InputSystem.actions.FindAction("Sprint").WasPressedThisFrame() && avoidCount > 0 && !isSelecting)
            {              
                isAvoiding = true;
                isOnAttacking = false;
                sprint = true;
                avoidingCool = false;
                if (!isOnGround && !isInWater) avoidCount -= 1;

                if (attackCoroutine != null)
                {
                    foreach (GameObject g in attackColliders)
                        g.SetActive(false);
                    attackColliders[0].SetActive(true);
                    StateCheckChangeValue("isRun", sprint);
                    anim.SetBool("isAttacking", isOnAttacking);
                    StopCoroutine(attackCoroutine);
                }
                avoidingCoroutine = Avoding();
                StartCoroutine(avoidingCoroutine);
            }
            if (sprint && !InputSystem.actions.FindAction("Sprint").IsPressed())
            {
                sprint = false;
            }
        }
        else 
            sprint = InputSystem.actions.FindAction("Sprint").IsPressed();
        Collider2D[] col = Physics2D.OverlapCircleAll(
            (Vector2)(transform.position),
            5, LayerMask.GetMask("FluidWall"));

       
        if (nowForm == 3 && sprint && !isAvoiding && pstate.GetStemina() > 0 && damEffectTime <= 0)
        {
            foreach (Collider2D col2d in col)
            {
                col2d.isTrigger = true;
            }
            gLayer &= ~(1 << 10);
            slideMovenent.layerMask = gLayer;
            sprite.color = new Color(0, 1, 1, 1);
        }
        else
        {
            foreach (Collider2D col2d in col)
            {
                col2d.isTrigger = false;
            }
            gLayer |= 1 << 10;
            slideMovenent.layerMask = gLayer;
            sprite.color = new Color(1, 1, 1, 1);
        }
    }

    // 애니메이터를 세팅합니다.
    void SetAnimator() {
        if (!isSelecting && !isDead)
        {
            if (InputSystem.actions.FindAction("Horizental").IsPressed() && movement != 0 && !wallhit) anim.SetTrigger("move");
            else anim.SetTrigger("idle");

            anim.SetBool("Push", pushingObj != null);
            anim.SetBool("isRun", sprint);
            StateCheckChangeValue("isOnGround", isOnGround && !isInWater, !isOnAttacking && !isUsingSkill);
            StateCheckChangeValue("stuned", stunedTime > 0);
            anim.SetFloat("YVelocity", nowGravitySpeed);
        }
    }

    // 점프 함수
    void Jump() {
        if (wallhit && nowForm == 5 && !isInWater && !isOnGround && !isOnAttacking && !isUsingSkill && !isSelecting) {
            if (InputSystem.actions.FindAction("Jump").WasPressedThisFrame()) {
                if (pstate.UseStemina(1))
                {
                    nowGravitySpeed = jumpPower * 0.9f / (isInWater ? moveOnWaterResi : 1);
                    movement = (sprite.flipX ? 1 : -1) * moveSpeed * 1.7f / (isInWater ? moveOnWaterResi : 1);
                    sprite.flipX = !sprite.flipX;
                }
            }
        }

        if (InputSystem.actions.FindAction("Jump").WasPressedThisFrame() && !isOnAttacking && !isUsingSkill && !isSelecting) {

            if (isInWater) {
                nowGravitySpeed = jumpPower / moveOnWaterResi;
            }
            else if (isJumpKeyActive)
            {
                inJump = true;
                jumpSpeed = jumpPower / 2;
            }
        }
        if (inJump) {
            if (InputSystem.actions.FindAction("Jump").IsPressed() && jumpSpeed < jumpPower) {
                jumpSpeed = Mathf.Clamp(jumpSpeed + Time.deltaTime * jumpSpeed * 10, 0, jumpPower);
            }
            else {
                inJump = false;
                isJumpKeyActive = false;
            }
        }
    }
    //공격 함수
    void Attack()
    {
        if (InputSystem.actions.FindAction("Attack").IsPressed() && !isAvoiding && !isUsingSkill && !isOnAttacking && nowForm >= 2 && !isSelecting)
        {      
            isOnAttacking = true;
            sprint = false;
            attackCoroutine = AttackCourutine();       
            StateCheckChangeValue("isAttacking", isOnAttacking);
            StartCoroutine(attackCoroutine);
        }
    }
    // 스킬 사용
    void UseSkill() {
        if (InputSystem.actions.FindAction("Skill").WasPressedThisFrame() && !isAvoiding && !isUsingSkill && nowForm >= 2 && pstate.GetSkillCooltime(nowForm) <= 0 && !isSelecting) {
            isUsingSkill = true;
            if (!pstate.UseStemina(skillStemina[nowForm - 2])) {
                isUsingSkill = false;
                return; 
            }
            isOnAttacking = false;
            sprint = false;

            if (attackCoroutine != null)
            {
                foreach (GameObject g in attackColliders)
                    g.SetActive(false);
                attackColliders[0].SetActive(true);
                anim.SetBool("isAttacking", isOnAttacking);
                StopCoroutine(attackCoroutine);
            }

            skillkCoroutine[0] = HammerSpin();
            skillkCoroutine[1] = SwordDash();
            skillkCoroutine[3] = FlailGrap();
            skillkCoroutine[2] = Parry();
            
            StateCheckChangeValue("isSkillUsing", isUsingSkill);
            StartCoroutine(skillkCoroutine[nowForm - 2]);
        }
    }

    //공격 데미지 변경
    void PlayerDamageSet() {
        int atk = pstate.GetAttack();
        DamageInfo d = new DamageInfo();

        d.critRate = pstate.GetCriticalRate();
        d.isCrit = (pstate.GetCritical() > Random.Range(0f, 100f));
        d.attackOwner = transform;
        d.ccinfo = CCInfo.push;
        d.stunPower = 2.6f;
        d.attackDir = sprite.flipX ? Vector2.left : Vector2.right;
        d.damage = Mathf.RoundToInt(atk * 1.1f);
        attackColliders[1].GetComponent<PlayerAttack>().setDamage(d);

        d.stunPower = 2.4f;
        d.damage = Mathf.RoundToInt(atk * 1f);
        attackColliders[2].GetComponent<PlayerAttack>().setDamage(d);

        d.stunPower = 1.4f;
        d.damage = Mathf.RoundToInt(atk * 0.8f);
        attackColliders[3].GetComponent<PlayerAttack>().setDamage(d);

        d.stunPower = 3.0f;
        d.damage = Mathf.RoundToInt(atk * 1f);
        attackColliders[4].GetComponent<PlayerAttack>().setDamage(d);

        d.ccinfo = CCInfo.none;
        d.stunPower = 0;
        d.damage = Mathf.RoundToInt(atk * 0.4f);
        d.isContinousdamage = true;
        d.nexthitTime = 0.25f;
        attackColliders[5].GetComponent<PlayerAttack>().setDamage(d);

        d.ccinfo = CCInfo.push;
        d.stunPower = 10;
        d.damage = Mathf.RoundToInt(atk * 1.9f);
        d.isContinousdamage = false;
        d.nexthitTime = 0f;
        attackColliders[6].GetComponent<PlayerAttack>().setDamage(d);

    }


    // 폼을 바꾸는 과정
    void FormChange() {
        if (anim.GetInteger("Form") != nowForm)
        {
            anim.SetInteger("Form", nowForm);
            anim.SetTrigger("changeForm");
        }
        nowForm = pstate.GetForm();
        if (InputSystem.actions.FindAction("Change").WasPressedThisFrame() && !isOnAttacking && !isUsingSkill && formChangeCooltime <= 0 && !isSelecting)
        {
            formChangeCooltime = 0.75f;
            pstate.AddForm(InputSystem.actions.FindAction("Change").ReadValue<float>() < 0 ? -1 : 1);
            anim.SetInteger("Form", pstate.GetForm());
        }

        for (int i = 0; i < 4; i++)
            pstate.SetIsFormActive(i + 2, formChangingObj[i] == null);
    }

    // 오브젝트에 폼을 수여
    void GiveFormToObj() 
    {
        if (InputSystem.actions.FindAction("Interact").WasPressedThisFrame() && !isOnAttacking && !isUsingSkill && !isSelecting && nowForm >= 2) 
        {       
            isSelecting = true;
            StartCoroutine("ObjSelecting");
        }
    }

    // 오브젝트에서 폼을 가져옴
    void ReturnFormToObj()
    {
     
        if (InputSystem.actions.FindAction("Interact").WasPressedThisFrame() && !isOnAttacking && !isUsingSkill && !isSelecting && nowForm == 1)
        {
            for (int i = 0; i < formChangingObj.Length; i++)
            {
                if (formChangingObj[i] != null)
                {
                    pstate.SetIsFormActive(formChangingObj[i].GetObjForm() + 1, true);
                    formChangingObj[i].SettingObjForm(0);
                    formChangingObj[i] = null;
                }
            }
        }
    }

    void CheckCooltimes() { 
        
        if(nowForm == 3 && sprint && !isAvoiding && pstate.GetStemina() > 0)
        {
            if(fluidSteminaCheck <= 0)
            {
                pstate.UseStemina(1);
                fluidSteminaCheck = 1;
            }
            else fluidSteminaCheck -= Time.deltaTime;
        }

        for (int i = 0; i < 2; i++)
        {

            if (EnergyTime[i] <= 0 || nowForm != 2)
            {
                EnergyTime[i] = 0;
                EnergyObjs[i].SetActive(false);
            }
            else
            {
                EnergyTime[i] -= Time.deltaTime;
                EnergyObjs[i].SetActive(true);
            }
        }

        if (nowForm == 2)
        {
            fireCoolImg.gameObject.SetActive(true);
            elecCoolImg.gameObject.SetActive(true);
            powerImg.gameObject.SetActive(false);
        }
        else if (nowForm == 4)
        {
            fireCoolImg.gameObject.SetActive(false);
            elecCoolImg.gameObject.SetActive(false);
            powerImg.gameObject.SetActive(true);
        }
        else
        {
            fireCoolImg.gameObject.SetActive(false);
            elecCoolImg.gameObject.SetActive(false);
            powerImg.gameObject.SetActive(false);
        }

        fireCoolImg.fillAmount = EnergyTime[0] / 3;
        elecCoolImg.fillAmount = EnergyTime[1] / 3;
        powerImg.fillAmount = chargedPower * 0.01f;

        if (formChangeCooltime > 0) formChangeCooltime -= Time.deltaTime;
        else formChangeCooltime = 0;

        if (airborneInitTime > 0) airborneInitTime -= Time.deltaTime;
        else airborneInitTime = 0;

        if (stunedTime > 0) stunedTime -= Time.deltaTime;
        else stunedTime = 0;

        if (damEffectTime > 0)
        {
            damEffectTime -= Time.deltaTime;
            sprite.material = flashMaterial;
        }
        else
        {
            damEffectTime = 0;
            sprite.material = ownMaterial;
        }
    }

    void DeadEventCheck() {
        if(pstate.GetHP() <= 0 && !isDead)
        {
            isDead = true;
            AllStopCoroutine();
            movement = 0;       
            StartCoroutine("DieEvent");
        }
    }

    // using FixedUpdate
    // 움직임을 관리하는 함수
    void Move(){
        float moveInput = 0;
        if (!isDead && !(stunedTime > 0) && !manager.GetOnCutScene())
            moveInput = InputSystem.actions.FindAction("Horizental").ReadValue<float>() * moveSpeed * (sprint ? sprintValue : 1);
        if (!isOnAttacking && !isUsingSkill && !isAvoiding && !isSelecting && !isDead && !(stunedTime > 0))
        {
            if (isOnGround) movement = moveInput / (isInWater ? moveOnWaterResi : 1);
            else movement = Mathf.Clamp(movement + moveInput * Time.fixedDeltaTime * (isInWater ? 3 : 1),
                    -moveSpeed * (sprint ? sprintValue : 1) / (isInWater ? moveOnWaterResi : 1), moveSpeed * (sprint ? sprintValue : 1) / (isInWater ? moveOnWaterResi : 1));
        }
        if(!freezeMovement && !manager.GetOnCutScene()) nowSurfaceAngle = Vector2.Angle(
                rigid.Slide(movement * Vector2.right, Time.fixedDeltaTime, slideMovenent).surfaceHit.normal,
                Vector2.up);


        if (InputSystem.actions.FindAction("Horizental").IsPressed()
            && !freezeMovement 
            && !isUsingSkill 
            && !isOnAttacking 
            && (isOnGround || (wallhit && nowForm == 5 && moveInput * movement >= 0) || isInWater)
            && movement != 0)
            sprite.flipX = moveInput < 0;

        attackColliders[0].transform.localScale = new Vector3(sprite.flipX ? -1 : 1, 1, 1);
    }
    // 중력 가속 만들기
    void SetGravity() {
        if (freezeGravity) nowGravitySpeed = 0;
        else if(airborneInitTime <= 0)
        {
            if (inJump)
            {
                nowGravitySpeed = jumpSpeed;
            }
            else if (isInWater)
            {
                if (nowGravitySpeed > 0 && isOnRoof) nowGravitySpeed = 0;
                else nowGravitySpeed -= gravityScale * 9.81f * Time.fixedDeltaTime / moveOnWaterResi;

                if (isInWaterAir && nowGravitySpeed < 3) nowGravitySpeed = 3;
                else if (nowGravitySpeed < -8f) nowGravitySpeed = -8f;
            }
            else if (isOnGround)
            {
                if (nowSurfaceAngle < 10 && nowSurfaceAngle > slideMovenent.gravitySlipAngle) nowGravitySpeed = 0;
                else nowGravitySpeed = -13;
            }
            else
            {
                if (nowGravitySpeed > 0 && isOnRoof) nowGravitySpeed = 0;

                if (wallhit && nowForm == 5)
                {
                    pstate.SteminaHold();
                    nowGravitySpeed = -3;
                }
                else nowGravitySpeed -= gravityScale * 9.81f * Time.fixedDeltaTime;


            }
        }
        slideMovenent.gravity = Vector2.up * nowGravitySpeed;
    }
    // 땅에 닿은 상태인지 체크
    void GroundCheck() {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, groundCheckRadious, groundCheckPoint, groundCheckPoint.magnitude, gLayer);
        isOnGround = hit.collider != null;
        
        if(manager.GetOnCutScene()) transform.parent = null;
        else transform.parent = hit.transform;
        transform.rotation = Quaternion.identity;

        hit = Physics2D.CapsuleCast(transform.position, new Vector2(1f, 2f), CapsuleDirection2D.Vertical, 0f, Vector2.right, 0, wLayer);
        isInWater = hit.collider != null;

        hit = Physics2D.CapsuleCast(transform.position, new Vector2(1f, 2f), CapsuleDirection2D.Vertical, 0f, Vector2.right, 0, LayerMask.GetMask("Energy"));
        if (hit.collider != null) isInWaterAir = (hit.collider.tag.Equals("FluidAir"));
        else isInWaterAir = false;

            hit = Physics2D.CircleCast(transform.position, groundCheckRadious, groundCheckPoint * 1.2f, groundCheckPoint.magnitude * jumpKeyValue, gLayer);
        isJumpKeyActive = (hit.collider != null) && nowGravitySpeed < 0.1f;

        float inp = InputSystem.actions.FindAction("Horizental").ReadValue<float>();
        RaycastHit2D rhit = Physics2D.CircleCast(transform.position, 0.5f, Vector2.right, 0.2f, gLayer);
        RaycastHit2D lhit = Physics2D.CircleCast(transform.position, 0.5f, Vector2.left, 0.2f, gLayer);
        bool lhitTagCheck = false, rhitTagCheck = false;
        if (lhit.collider != null) lhitTagCheck = (lhit.collider.tag != "PushEventObj");
        if (rhit.collider != null) rhitTagCheck = (rhit.collider.tag != "PushEventObj");
        wallhit = (inp < 0 && movement < 0 && lhitTagCheck) || (inp > 0 && movement > 0 && rhitTagCheck);

        hit = Physics2D.CircleCast(transform.position, groundCheckRadious, -groundCheckPoint, groundCheckPoint.magnitude, gLayer);
        isOnRoof = hit.collider != null;
    }

    // 코루틴

    // 사망 처리
    IEnumerator DieEvent()
    {
        if (!isOnGround)
        {
            StateCheckChangeValue("stuned", true);
            float t = 0;
            while(t < 2)
            {
                t += Time.deltaTime;
                if (isOnGround) break;
                yield return null;
            }
        }
        anim.SetBool("retire", true);
        anim.SetTrigger("stateSet");
        yield return YieldCache.WaitForSeconds(1.5f);
        pstate.StatusReset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // 회피 및 달리기
    IEnumerator Avoding(){
        float t = 0;
        if (!isOnGround)
        {
            if(InputSystem.actions.FindAction("Horizental").IsPressed()) sprite.flipX = InputSystem.actions.FindAction("Horizental").ReadValue<float>() < 0;
            freezeGravity = true;
        }
        while (t < 0.2f)
        {            
            if (t < 0.05f) 
                sprite.color = new Color(1, 1, 1, (0.05f - t) * 20);
            else if(t > 0.15f)
                sprite.color = new Color(1, 1, 1, (t - 0.15f) * 20);
            else
                sprite.color = new Color(1, 1, 1, 0);
            yield return null;
            t += Time.deltaTime;
            movement = (sprite.flipX ? -1 : 1) * 25 / (isInWater ? moveOnWaterResi : 1);
        }
        freezeGravity = false;
        sprite.color = new Color(1, 1, 1, 1);
        isAvoiding = false;
        yield return YieldCache.WaitForSeconds(0.3f);
        avoidingCool = true;
    }

    // 공격하는 모션의 코루틴
    IEnumerator AttackCourutine() {
        float t = 0;
        while (t < attackTime[nowForm].x)
        {  
            yield return null;
            t += Time.deltaTime;
            attackColliders[nowForm - 1].gameObject.SetActive((t > attackTime[nowForm].x * 0.3f && t < attackTime[nowForm].x * 0.8f));
            if (t > 0.1 && isOnGround) movement = (sprite.flipX ? -1 : 1) * attackMovePower[nowForm] * 
                    (attackTime[nowForm].x - t) / (attackTime[nowForm].x - 0.1f) / (!isInWater ? 1 : moveOnWaterResi);          
        }
        int inh = (int)InputSystem.actions.FindAction("Horizental").ReadValue<float>();
        if (InputSystem.actions.FindAction("Attack").IsPressed())
        {
            if (inh != 0 && isOnGround) sprite.flipX = inh < 0;
            t = 0;
            while (t < attackTime[nowForm].y)
            {
                yield return null;
                t += Time.deltaTime;
                attackColliders[nowForm - 1].gameObject.SetActive((t > attackTime[nowForm].y * 0.3f && t < attackTime[nowForm].y * 0.8f));
                if (t > 0.1 && isOnGround) movement = (sprite.flipX ? -1 : 1) * attackMovePower[nowForm] *
                        (attackTime[nowForm].y - t) / (attackTime[nowForm].y - 0.1f) / (!isInWater ? 1 : moveOnWaterResi);
            }
            inh = (int)InputSystem.actions.FindAction("Horizental").ReadValue<float>();
        }
        if (inh != 0 && isOnGround) sprite.flipX = inh < 0;
        isOnAttacking = false;
        StateCheckChangeValue("isAttacking", isOnAttacking);
    }

    // 스킬 1
    IEnumerator HammerSpin() {
        sprite.flipX = false;
        float t = 0;
        attackColliders[5].gameObject.SetActive(true);
        while (t < 3)
        {
            t += Time.deltaTime;
            float moveInput = InputSystem.actions.FindAction("Horizental").ReadValue<float>() * moveSpeed * (sprint ? sprintValue : 1);
            if (isInWater) movement = moveInput / (!isInWater ? 1 : moveOnWaterResi);
            else if (isOnGround) movement = moveInput;
            else movement = Mathf.Clamp(movement + moveInput * Time.fixedDeltaTime,
                    -moveSpeed * (sprint ? sprintValue : 1), moveSpeed * (sprint ? sprintValue : 1));
            yield return null;
        }
        attackColliders[5].gameObject.SetActive(false);
        pstate.SetSkillCooltime(2,skillCooltime[0]);
        isUsingSkill = false;
        StateCheckChangeValue("isSkillUsing", isUsingSkill);
    }

    // 스킬 2
    IEnumerator SwordDash()
    {
        if (InputSystem.actions.FindAction("Horizental").IsPressed()) sprite.flipX = InputSystem.actions.FindAction("Horizental").ReadValue<float>() < 0;
        freezeGravity = true;
        freezeMovement = true;
        movement = 0;
        yield return YieldCache.WaitForSeconds(0.5f);
        attackColliders[6].gameObject.SetActive(true);
        float t = 0;
        while (t < 0.02)
        {
            t += Time.deltaTime;
            yield return null;
            rigid.Slide(550 * (sprite.flipX ? Vector2.left : Vector2.right), Time.deltaTime, slideMovenent);
        }
        yield return YieldCache.WaitForSeconds(0.28f);
        attackColliders[6].gameObject.SetActive(false);
        freezeGravity = false;
        freezeMovement = false;
        isUsingSkill = false;
        pstate.SetSkillCooltime(3,skillCooltime[1]);
        StateCheckChangeValue("isSkillUsing", isUsingSkill);
    }

    // 스킬 3
    IEnumerator Parry()
    {
        if (InputSystem.actions.FindAction("Horizental").ReadValue<float>() != 0) sprite.flipX = InputSystem.actions.FindAction("Horizental").ReadValue<float>() < 0;
        movement = 0;
        anim.speed = 0;
        
        for (chargedPower = 0; chargedPower <= 100; chargedPower += Time.deltaTime * 100)
        {
            sprite.color = new Color(1, 1, 1 - 0.01f * chargedPower);
            if (InputSystem.actions.FindAction("Skill").IsPressed()) break;
            yield return null;
        }
        DamageInfo d = new DamageInfo();
        d.attackOwner = transform;
        d.damage = Mathf.RoundToInt(pstate.GetAttack() * 0.18f * chargedPower);
        d.stunPower = chargedPower > 70 ? 10 : 4;
        d.ccinfo = CCInfo.push;
        d.attackDir = sprite.flipX ? Vector2.left : Vector2.right;

        attackColliders[7].GetComponent<PlayerAttack>().setDamage(d);
        attackColliders[8].GetComponent<PlayerAttack>().setDamage(d);
        bool powered = false;
        if (chargedPower > 70) powered = true;

        anim.speed = 1;
        sprite.color = Color.white;
        int power = (int)chargedPower;
        for (; chargedPower > 0; chargedPower -= Time.deltaTime * 100 * 10)
            yield return null;

        isGuard = true;
        yield return YieldCache.WaitForSeconds(0.05f);

        Collider2D[] col = Physics2D.OverlapCircleAll(
            (Vector2)(transform.position + (sprite.flipX ? Vector3.left : Vector3.right) * 2.122829f),
            2.166189f, LayerMask.GetMask("Interact"));
        attackColliders[7].gameObject.SetActive(true);
        if (powered)
        {
            attackColliders[8].gameObject.SetActive(true);
            attackColliders[7].gameObject.SetActive(false);
            col = Physics2D.OverlapCircleAll(
            (Vector2)(transform.position + (sprite.flipX ? Vector3.left : Vector3.right) * 3.337997f),
            3.397162f, LayerMask.GetMask("Interact"));
            elastPowerEff.SetActive(true);
            elastPowerEff.transform.localScale = new Vector3((sprite.flipX ? -1 : 1), 1, 1);
        }

        foreach(Collider2D c in col){
            if(c.GetComponent<Rigidbody2D>() != null)
            {
                Rigidbody2D r = c.GetComponent<Rigidbody2D>();
                if (r.bodyType == RigidbodyType2D.Dynamic) 
                    r.AddForce(power * 0.45f * (sprite.flipX ? Vector2.left : Vector2.right), ForceMode2D.Impulse);
            }
        }
        
        yield return YieldCache.WaitForSeconds(0.23f);
        isGuard = false;
        attackColliders[8].gameObject.SetActive(false);
        attackColliders[7].gameObject.SetActive(false);
        elastPowerEff.SetActive(false);
        isUsingSkill = false;
        chargedPower = 0;
        pstate.SetSkillCooltime(4,skillCooltime[2]);
        StateCheckChangeValue("isSkillUsing", isUsingSkill);
    }

    // 스킬 4
    IEnumerator FlailGrap()
    {
        float h = InputSystem.actions.FindAction("Horizental").ReadValue<float>();
        float v = InputSystem.actions.FindAction("Vertical").ReadValue<float>();

        Vector3 vec;
        if (h != 0 || v != 0)
        {
            vec = new Vector3(h, v, 0).normalized;
            if (h != 0) sprite.flipX = h < 0;
        }
        else
            vec = (sprite.flipX ? Vector3.left : Vector3.right);

        float t = 0;
        freezeGravity = true;
        freezeMovement = true;
        movement = 0;
        yield return YieldCache.WaitForSeconds(0.15f);
        LineRenderer lr = Instantiate(flail, transform.position +
            (sprite.flipX ? Vector3.left : Vector3.right) * 1.6f + Vector3.down * 0.1f, transform.rotation)
            .GetComponent<LineRenderer>();
        summonedFlail = lr.gameObject;
        DamageInfo d = new DamageInfo();
        d.attackOwner = transform;
        d.damage = Mathf.RoundToInt(pstate.GetAttack() * 1.2f);
        d.stunPower = 1f;
        lr.GetComponent<PlayerAttack>().setDamage(d);

        while (t < 0.45f)
        {
            t += Time.fixedDeltaTime;
            yield return YieldCache.WaitForFixedUpdate;
            lr.SetPosition(0, transform.position + (sprite.flipX ? Vector3.left : Vector3.right) * 1.6f + Vector3.down * 0.1f);
            lr.SetPosition(1, lr.transform.position);
            lr.transform.Translate(vec * Time.fixedDeltaTime * 50);
            RaycastHit2D hit = Physics2D.CircleCast(lr.transform.position, 0.5f, (sprite.flipX ? Vector3.left : Vector3.right), 0.01f, (1 << 3 | 1 << 7));
            if (hit.collider != null) break;

        }
        
        if (t < 0.45f)
        {
            while (Vector3.Distance(transform.position, lr.transform.position) > 1f)
            {

                float a = Vector2.Angle(
                    rigid.Slide((lr.transform.position - transform.position).normalized * 30, Time.deltaTime, slideMovenent).slideHit.normal,
                    Vector2.up);
                if (a > 60) break;
                lr.SetPosition(0, transform.position + (sprite.flipX ? Vector3.left : Vector3.right) * 1.6f + Vector3.down * 0.1f);
                lr.SetPosition(1, lr.transform.position);
                yield return null;
            }
        }
        Destroy(summonedFlail);
        freezeGravity = false;
        freezeMovement = false;
        isUsingSkill = false;
        pstate.SetSkillCooltime(5, skillCooltime[3]);
        StateCheckChangeValue("isSkillUsing", isUsingSkill);
    }

    // 오브젝트 고르는 코루틴
    IEnumerator ObjSelecting()
    {
        Time.timeScale = 0;
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("PushEventObj");

        List<GameObject> ableObjList = new List<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            Vector3 viewPos = mainCam.WorldToViewportPoint(obj.transform.position);
            if (viewPos.x >= 0f && viewPos.x <= 1f && viewPos.y >= 0f && viewPos.y <= 1f && viewPos.z > 0)
            {
                ableObjList.Add(obj);
            }
        }
        if (ableObjList.Count <= 0) goto FinishSearching;

        ableObjList.Sort(delegate (GameObject A, GameObject B)
        {
            return A.transform.position.x < B.transform.position.x ? -1 : 1;
        });
        int index = 0;
        for (int i = 0; i < ableObjList.Count; i++)
        {
            if (Vector2.Distance(ableObjList[index].transform.position, transform.position)
                > Vector2.Distance(ableObjList[i].transform.position, transform.position)) index = i;
        }
        Vector3 UIPos = mainCam.WorldToViewportPoint(ableObjList[index].transform.position);
        if (selectImg != null && slowedImg != null)
        {
            selectImg.rectTransform.anchoredPosition = new Vector3((603 * UIPos.x) - 301.5f, (339 * UIPos.y) - 169.5f, 0);
            slowedImg.gameObject.SetActive(true);
            selectImg.gameObject.SetActive(true);
        }
        yield return new WaitUntil(() => InputSystem.actions.FindAction("Interact").IsPressed());
        while (true)
        {
            UIPos = mainCam.WorldToViewportPoint(ableObjList[index].transform.position);
            selectImg.rectTransform.anchoredPosition = new Vector3((603 * UIPos.x) - 301.5f, (339 * UIPos.y) - 169.5f, 0);

            if (InputSystem.actions.FindAction("CancelSelect").IsPressed()) goto FinishSearching;

            if (InputSystem.actions.FindAction("Horizental").IsPressed())
            {
                index = (index + (int)InputSystem.actions.FindAction("Horizental").ReadValue<float>()) % ableObjList.Count;
            }

            if (InputSystem.actions.FindAction("Interact").IsPressed())
            {
                InteractObjs iob = ableObjList[index].GetComponent<InteractObjs>();
                if (iob.GetObjForm() != 0)
                {
                    pstate.SetIsFormActive(iob.GetObjForm() + 1, true);
                    formChangingObj[iob.GetObjForm() - 1] = null;
                    iob.SettingObjForm(0);                 
                }
                formChangingObj[nowForm - 2] = iob;
                formChangingObj[nowForm - 2].SettingObjForm(nowForm - 1);
                
                pstate.SetIsFormActive(nowForm, false);
                pstate.AddForm(1 - nowForm);
                anim.SetInteger("Form", pstate.GetForm());
                anim.SetTrigger("changeForm");
                
                break;
            }
            yield return null;
        }
    FinishSearching:
        if (selectImg != null && slowedImg != null)
        {
            selectImg.gameObject.SetActive(false);
            slowedImg.gameObject.SetActive(false);
        }
        Time.timeScale = 1;
        yield return new WaitUntil(() => InputSystem.actions.FindAction("Interact").IsPressed() || InputSystem.actions.FindAction("Cancel").IsPressed());
        isSelecting = false;
    }

    // 디버그
    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + groundCheckPoint, groundCheckRadious);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + groundCheckPoint * jumpKeyValue, groundCheckRadious * 1.2f);
    }
    // 애니메이션 스테이트 확인용
    void StateCheckChangeValue(string name, bool value, bool changeState = true) {
        if (anim.GetBool(name) != value && changeState) anim.SetTrigger("stateSet");
        anim.SetBool(name, value);
    }
}

