using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class QuarterviewPlayerProto : MonoBehaviour
{
    private CharacterController controller; // 현재 캐릭터가 가지고있는 캐릭터 컨트롤러 콜라이더.
    private Transform characterModel; // 캐릭터 본체
    private Transform playerCamera; // 캐릭터를 바라보는 카메라입니다.
    private SphereCollider attackCollider; // 캐릭터 공격 콜라이더입니다.

    private Vector3 moveDir; // 캐릭터의 움직임 저장
    private Vector2 moveVec; // 이동 입력 받기

    private int nowWeapon = 0; // 현재 무기를 확인
    private int hp = 100; // 체력
    private int stemina = 15; // 스테미나

    private float electricSaveTime = 0; // 전기 저장중 시간
    private float fireSaveTime = 0; // 불 저장중 시간
    private float[] hitRange = { 0, 1.5f, 0.75f, 0, 2, 3.25f, 2.25f }; // 무기 범위 판정

    [SerializeField] private bool[] weaponAble = { true, false, false, false, false, false, false }; // 무기가 사용 가능한지 확인
    [SerializeField] private bool[] Gotweapon = { true, false, false, false, false, false, false }; // 무기를 얻었는지 확인 

    private bool isOnGround; // 땅에 닿았는지 체크
    private bool isInSwim; // 수영 가능한 유체 안에 있는지 확인
    private bool pushSprint; // 스프린트 눌렀는지 확인
    private bool sprintEvent; // 스프린트 눌렀는지 확인
    private bool avoid; // 피하는 중인지 확인
    private bool sprint; // 달리는 중인지 확인
    private bool jump; // 점프 눌렀는지 확인
    private bool canJumpKey; // 점프 키를 누를 수 있는지 확인(점프 딜레이)
    private bool changeCool = true; // 무기 바꾸는 쿨링 상태 확인
    private bool attackCool = true; // 공격 쿨링 상태 확인
    private bool isOnGipo; // 기포에 닿은 상태인지 확인
    private bool ishealedStemina; // 스테미나 회복 상태 확인  

    private const float gravityAcc = 9.81f; // 중력가속도
    private const float sprintValue = 1.75f; // 달리기 계수
    private const float avoidTime = 0.15f; // 회피 발동 시간을 지정합니다.
    private const float changeCooltime = 0.1f; // 무기 변경 쿨타임 지정
    private const float attackCooltime = 0.42f; // 공격 쿨타임 지정


    [Header("Player Arguement")]
    [SerializeField][Tooltip("캐릭터의 애니메이터")]
    private Animator anim;
    [SerializeField][Tooltip("무기 오브젝트를 설정합니다.")]
    private GameObject[] weapons;
    [SerializeField][Tooltip("보관중인 에너지 이펙트를 설정합니다")]
    private GameObject[] energyEffect;
    [SerializeField][Tooltip("투사체를 지정합니다")]
    private GameObject[] shots;
    [SerializeField][Tooltip("카메라가 물 속 일때, 띄우는 반투명 이미지를 설정합니다.")]
    private Image waterImage;
    [SerializeField][Tooltip("체력바를 지정합니다.")]
    private Slider HPBar;
    [SerializeField][Tooltip("스테미나 바를 지정합니다.")]
    private Slider steminaBar;
    [SerializeField][Tooltip("폼 변환 메테리얼을 지정합니다.")]
    private Material[] formMaterial;

    [Header("Player Setting Value")]
    [SerializeField][Tooltip("기본 이동 속도를 지정합니다")][Range(0, 100)]
    private float speed = 10;
    [SerializeField][Tooltip("땅부터 캐릭터 높이를 지정합니다")][Range(0, 3)]
    private float groundHeight = 1;
    [SerializeField][Tooltip("점프 세기를 지정합니다")][Range(0, 50)]
    private float jumpSpeed = 5;
    [SerializeField][Tooltip("중력 강도를 정합니다")]
    private float gravityScale = 1;
    [SerializeField][Tooltip("상호작용 거리를 지정합니다")][Range(0, 5)]
    private float interactionDistance = 1;
    [SerializeField][Tooltip("캐릭터가 미는 힘을 지정합니다")][Range(0, 100)]
    private float pushPower = 5;
    [SerializeField][Tooltip("금속 시 에너지를 보관하는 시간을 지정합니다")][Range(0, 8)]
    private float energyTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GameObject.Find("CinemachineBrain").transform;
        controller = GetComponent<CharacterController>();
        characterModel = transform.GetChild(0);
        attackCollider = characterModel.GetChild(1).GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        SourceManagement();
        MoveCharacter();
        WeaponChange();
        SprintCheck();
        EnergySave();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        OnCameraWater();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;

        // 캐릭터 크기의 capsuleCast를 이용, 캐릭터 이동 방향에 닿는 법선 벡터 방향으로 민다.
        if (rigidbody != null && hit.gameObject.layer != 7)
        {
            RaycastHit rh;
            Physics.CapsuleCast(transform.position + Vector3.up, transform.position + Vector3.down,
                0.5f, characterModel.forward, out rh, 1f);
            Vector3 forceDirection = -rh.normal.normalized; // 물리엔진의 힘은 표면의 노멀 벡터로 가함
            forceDirection.y = 0; // 탑뷰/쿼터뷰 게임이므로, y 이동 필요없음
            forceDirection.Normalize();           
            // 특정 위치에서 힘을 가하는 메서드
            rigidbody.AddForceAtPosition(forceDirection, transform.position, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.tag.Equals("EnemyAttack"))
        {
            DamageProto d = c.GetComponent<AttacksProto>().GetDamage();
            if (!attackCool && nowWeapon == 2 && Vector3.Angle(characterModel.forward, -d.damageVector) < 70)
                d.owner.GetComponent<EnemyControlProto>().Perring();
            else
                hp -= d.damage;
        }
    }

    //Move키 입력
    public void OnMove(InputAction.CallbackContext context)
    {
        // 입력 움직임 세팅. 
        moveVec = context.ReadValue<Vector2>().normalized;
    }
    //Jump키 입력
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && (canJumpKey || isInSwim)) jump = true;   
    }
    //Sprint키 입력
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            pushSprint = true;
        }
        else pushSprint = false;
        if (context.canceled) sprintEvent = false;
    }

    // 무기 변경 키
    public void OnChange(InputAction.CallbackContext context)
    {
        if (changeCool && attackCool && !avoid)
        {
            changeCool = false;
            sprint = false;
            int get = (int)(context.ReadValue<float>());
            do nowWeapon = (nowWeapon + get + 7) % 7;
            while (!weaponAble[nowWeapon] || !Gotweapon[nowWeapon]);

            Observable.FromCoroutine(x => Cooling(changeCooltime), publishEveryYield: false)
                .Subscribe(_ => changeCool = true);
        }
    }

    // 공격 키 입력
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (attackCool && nowWeapon != 0 && context.performed && !avoid)
        {
            attackCool = false;
            sprint = false;
            DamageProto d = new DamageProto();
            d.damageVector = transform.forward;
            d.damage = 5;
            d.owner = transform;
            attackCollider.GetComponent<AttacksProto>().SetDamage(d);
            anim.SetTrigger("Attack");
            Observable.FromCoroutine(x => Cooling(attackCooltime), publishEveryYield: false)
                .Subscribe(_ => attackCool = true);
            if(nowWeapon == 3)
            {
                Rigidbody r = Instantiate(shots[0], transform.position, transform.rotation).GetComponent<Rigidbody>();
                r.linearVelocity = moveDir - Vector3.up * moveDir.y + characterModel.forward * 12;
                r.GetComponent<AttacksProto>().SetDamage(d);
            }
                
        }
    }

    // 상호작용 키 입력
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (attackCool && context.started && !avoid)
        {
            RaycastHit rh;
            Physics.BoxCast(transform.position, new Vector3(0.5f, 0.5f, 0.1f), characterModel.forward,
                out rh, characterModel.rotation, interactionDistance);
            if (rh.collider != null)
                if (rh.collider.GetComponent<InterectionScriptProto>() != null)
                    rh.collider.GetComponent<InterectionScriptProto>().Interaction();
        }
    }

    // hp, 스테미나 관리 함수
    private void SourceManagement() {
        HPBar.value = hp;
        steminaBar.value = stemina;

        if (!ishealedStemina)
        {
            ishealedStemina = true;
            stemina = Mathf.Clamp(stemina + (sprint ? -1 : 1), 0, 15);
            Observable.FromCoroutine(x => Cooling(2), publishEveryYield: false)
                .Subscribe(_ => ishealedStemina = false);
        }
    }

    // unirx 기능을 이용해 쿨타임을 기다리는 함수
    private IEnumerator Cooling(float cooltime) {
        yield return YieldCache.WaitForSeconds(cooltime);
    }

    // 캐릭터 달리기 체크 함수
    private void SprintCheck() {
        if (isOnGround && !isInSwim)
        {
            // sprint키가 입력 상태인경우
            if (pushSprint && !avoid && !sprintEvent && stemina > 2)
            {
                stemina -= 2;
                sprintEvent = true;
                Observable.FromCoroutine(ASCoroutine).Subscribe(); // 코루틴 - 회피 및 대시
            }
            else if (sprint && !pushSprint) sprint = false;
        }
    }

    // 캐릭터 움직임 함수
    private void MoveCharacter()
    {
        if (isInSwim) { // 수영 시
            // 입력 - 움직임 보정 
            Vector3 vec = (Vector3.left * moveVec.y + Vector3.forward * moveVec.x).normalized;

            // 플레이어 회전
            if (vec.magnitude > 0 && !avoid) characterModel.rotation = Quaternion.Slerp(characterModel.rotation, Quaternion.LookRotation(vec), 0.175f);
            if (isOnGipo && nowWeapon != 6)
            {
                moveDir.y += (jumpSpeed * 0.3f - moveDir.y) * Time.deltaTime;
            }
            else
            {
                moveDir.y -= gravityAcc * 0.3f * gravityScale * Time.deltaTime;
                moveDir.y = Mathf.Clamp(moveDir.y, -8, 256);
            }
            // 캐릭터 수영
            if (jump)
            {
                //수영은 점프의 열화로 구현
                if (vec.magnitude > 0) characterModel.rotation = Quaternion.LookRotation(vec);
                moveDir.y = jumpSpeed * 0.65f * (nowWeapon == 6 ? 0.75f : 1);
                jump = false;
            }
            // 입력된 방향으로 움직인다
            moveDir = vec * speed * 0.9f * moveVec.magnitude + Vector3.up* moveDir.y;
            
        }
        else if (isOnGround)
        {
            // 입력 - 움직임 보정 
            Vector3 vec = (Vector3.left * moveVec.y + Vector3.forward * moveVec.x).normalized;
            //달리기 체크
            vec *= sprint ? sprintValue : 1;
            vec *= nowWeapon == 4 ? 1.75f : 1;
            // 플레이어 회전
            if (vec.magnitude > 0 && !avoid) characterModel.rotation = Quaternion.Slerp(characterModel.rotation, Quaternion.LookRotation(vec), 0.35f);
            // 경사면 대비 -y 이동
            moveDir.y = -gravityAcc;
            // 캐릭터 점프
            if (jump)
            {
                //점프로 바로 플레이어 움직이기
                if (vec.magnitude > 0) characterModel.rotation = Quaternion.LookRotation(vec);
                controller.Move(Vector3.up * 0.2f);
                moveDir.y = jumpSpeed * (nowWeapon == 6 ? 0.5f : 1);
                isOnGround = false;
                jump = false;               
            }
            // 입력된 방향으로 움직인다
            if (nowWeapon != 4)
                moveDir = vec * speed * moveVec.magnitude + Vector3.up * moveDir.y;
            else
                moveDir = Vector3.Lerp(new Vector3(moveDir.x, 0, moveDir.z), vec * speed * moveVec.magnitude, 0.005f) + Vector3.up * moveDir.y;           
        }
        else
        {
            moveDir.y -= gravityAcc * Time.deltaTime * gravityScale; //중력 처리
            if (avoid) moveDir.y = 0; // 회피중엔 안떨어지게
        }
        if (!avoid) controller.Move(moveDir * Time.deltaTime); // 캐릭터 움직임.
    }
    
    // 무기 변경 함수
    private void WeaponChange() {
        characterModel.GetComponent<MeshRenderer>().material = formMaterial[nowWeapon];
        attackCollider.radius = hitRange[nowWeapon];
        attackCollider.center = Vector3.forward * hitRange[nowWeapon];    
        if (!weapons[nowWeapon].activeSelf)
        {
            for (int i = 0; i < 7; i++)
            {
                if (i == nowWeapon) weapons[i].SetActive(true);
                else weapons[i].SetActive(false);
            }
        }
    }

    // 회피 코루틴
    private IEnumerator ASCoroutine() {
        avoid = true;
        Vector3 vec = (Vector3.left * moveVec.y + Vector3.forward * moveVec.x).normalized;
        if (vec.magnitude == 0) vec = -characterModel.forward;

        characterModel.gameObject.SetActive(false);
        float t = 0;
        while (t < avoidTime)
        {
            t += Time.fixedDeltaTime;
            controller.Move(vec * speed * 3f * Time.fixedDeltaTime);
            yield return YieldCache.WaitForFixedUpdate;
        }
        characterModel.gameObject.SetActive(true);
        avoid = false;
        if (pushSprint) sprint = true;
    }

    // 금속일 시, 열, 전기 에너지 보관 확인
    private void EnergySave() {
        Collider[] energys = Physics.OverlapCapsule(transform.position + Vector3.up,
           transform.position - Vector3.up, 0.5f, LayerMask.GetMask("Energy"));
        if (energys.Length > 0) {
            foreach(Collider c in energys)
            {
                if (c.gameObject.Equals(energyEffect[0]) || c.gameObject.Equals(energyEffect[1])) continue;
                if (c.tag == "Fire") fireSaveTime = energyTime;
                if (c.tag == "Electric") electricSaveTime = energyTime;
            }
                
        }

        if (electricSaveTime > 0 && nowWeapon == 1) 
            electricSaveTime -= Time.deltaTime;
        else        
            electricSaveTime = 0;

        if (fireSaveTime > 0 && nowWeapon == 1)        
            fireSaveTime -= Time.deltaTime;        
        else        
            fireSaveTime = 0;
        
        energyEffect[0].SetActive(electricSaveTime > 0);
        energyEffect[1].SetActive(fireSaveTime > 0);
    }

    // 캐릭터의 땅을 확인하는 함수
    private void GroundCheck()
    {
        RaycastHit rh;
        Physics.SphereCast(transform.position, 0.48f, Vector3.down, 
            out rh, groundHeight, LayerMask.GetMask("Ground")); // 땅 확인 
        isOnGround = (rh.collider != null);

        Physics.SphereCast(transform.position, 0.48f, Vector3.down,
            out rh, groundHeight * 2.5f, LayerMask.GetMask("Ground")); // 점프 제약 해제 확인
        canJumpKey = (rh.collider != null);

        isInSwim = Physics.OverlapCapsule(transform.position + Vector3.up,
            transform.position - Vector3.up, 0.5f, LayerMask.GetMask("Water")).Length > 0; // 수영 가능성 확인

        isOnGipo = Physics.OverlapCapsule(transform.position + Vector3.up,
            transform.position - Vector3.up, 0.5f, LayerMask.GetMask("MoveEnergy")).Length > 0; // 기포 여부 확인  
    }

    
  

    // 카메라가 물 속에 있을때, 효과를 지정합니다.
    private void OnCameraWater() {
        waterImage.gameObject.SetActive(Physics.OverlapSphere(playerCamera.position,
                 0.2f, LayerMask.GetMask("Water")).Length > 0 && isInSwim);
    }

    //무기 사용 가능을 변경
    public void SetWeaponAble(int index) {
        weaponAble[index] = !weaponAble[index];

        if (!weaponAble[index]) nowWeapon = 0;

    }
    public int GetWeapon()
    {
        return nowWeapon;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundHeight, 0.48f);
    }
}
