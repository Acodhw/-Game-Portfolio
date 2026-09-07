using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.AI;

public class EnemyControlProto : MonoBehaviour
{
    private NavMeshAgent m_Agent; // 적 이동을 위한 네브매쉬 
    private Animator animator; // 적의 애니메이터
    private Rigidbody rigid; // 적의 물리엔진
    private Transform player; // 플레이어 위치
    private Transform spiningObj; // 회전 오브젝트

    private float objAngle = 0;

    private bool isFindedPlayer; // 플레이어 발견 여부
    private bool attackCool;
    private bool isAttack;
    private bool isPerried;

    [SerializeField] private int hp; // 적의 체력
    [SerializeField] private int atk = 5; // 적의 공격력
    [SerializeField] private int sightAngle; // 시야 각도
    [SerializeField] private float sightDist; // 시야 범위
    [SerializeField] private float findDist; // 플레이어 인식 범위
    [SerializeField] private float attackDist; // 공격 인식 범위
    [SerializeField] private float speed; // 이동 속도
    [SerializeField] private float objRotateSpeed; // 물체 회전 속도
    [SerializeField] private AttacksProto attack; // 공격


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 && other.tag.Equals("PlayerAttack")) {
            DamageProto d = other.GetComponent<AttacksProto>().GetDamage();
            hp -= d.damage;
            isFindedPlayer = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        spiningObj = transform.GetChild(2);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToPlayer();
        ObjectRotate();
        if (hp <= 0) Destroy(gameObject);
        m_Agent.speed = speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FindPlayer();
    }

    private void ObjectRotate()
    {
        objAngle += objRotateSpeed * Time.deltaTime;
        objAngle %= 360;
        spiningObj.localRotation = Quaternion.Euler(0, objAngle, 0);
    }

    private void FindPlayer() {
        if (isFindedPlayer && Vector3.Distance(player.position, transform.position) > findDist)
        {
            isFindedPlayer = false;
        }
        else if (!isFindedPlayer) {
            RaycastHit rh;
            for (int angle = -sightAngle; angle <= sightAngle; angle++) {
                Physics.Raycast(transform.position - transform.position.y * 0.5f * Vector3.up, Quaternion.Euler(0, angle, 0) * transform.forward,
                    out rh, sightDist);
                if (rh.collider != null)
                {
                    if (rh.transform.Equals(player))
                    {
                        isFindedPlayer = true;
                        break;
                    }
                }
            }
        }
    }

    private void MoveToPlayer()
    {
        if (isFindedPlayer)
        {
            if (Vector3.Distance(player.position, transform.position) > attackDist && !isAttack && !isPerried)
            {
                if (Vector3.Distance(m_Agent.destination, player.position) > 1.0f)
                {
                    m_Agent.destination = player.position;
                }
            }
            else {
                m_Agent.destination = transform.position;
                if (!isPerried)
                {
                    if (!attackCool)
                    {
                        isAttack = true;
                        attackCool = true;
                        Vector3 v = (player.position - transform.position).normalized;
                        v.y = 0;
                        transform.forward = v;
                        DamageProto d = new DamageProto();
                        d.damageVector = v;
                        d.damage = atk;
                        d.owner = transform;
                        attack.SetDamage(d);
                        animator.SetTrigger("attack");
                        Observable.FromCoroutine(x => Cooling(0.55f), publishEveryYield: false)
                        .Subscribe(_ => isAttack = false);
                        Observable.FromCoroutine(x => Cooling(1.25f), publishEveryYield: false)
                        .Subscribe(_ => attackCool = false);
                    }
                }
            }
        }
        else {
            transform.Rotate(Vector3.up * objRotateSpeed * Time.deltaTime);
        }
    }

    public void Perring()
    {
        isPerried = true;
        animator.SetTrigger("perried");
        Observable.FromCoroutine(x => Cooling(1.5f), publishEveryYield: false)
                    .Subscribe(_ => isPerried = false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, findDist);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDist);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightDist);
    }

    // unirx 기능을 이용해 쿨타임을 기다리는 함수
    private IEnumerator Cooling(float cooltime)
    {
        yield return YieldCache.WaitForSeconds(cooltime);
    }
}
