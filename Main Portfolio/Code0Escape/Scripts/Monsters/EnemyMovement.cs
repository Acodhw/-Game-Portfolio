using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;

    public Transform Player;
    public Transform RayPoint;   
    public LayerMask RayMask;

    public bool FindedPlayer;

    public float FindDistance;

    public float AttackRange;
    public float MoveSpeed;
    public float AttackCooltime;

    bool attackCool = true;
    bool FindCool = true;

    int m_CurrentWaypointIndex;
    int maxhp;

    void Start()
    {
        Player = GameObject.Find("PlayerPrefeb").transform.GetChild(1);
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(waypoints[0].position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Bullet")) {
            FindedPlayer = true;
            FindCool = false;
            StartCoroutine("FindCooling");
        }
    }

    void Update()
    {
        if (!FindedPlayer)
        {
            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                navMeshAgent.velocity = navMeshAgent.velocity * 0.05f;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
            RaycastHit hit;
            bool col = Physics.Raycast(RayPoint.position, RayPoint.forward, out hit, FindDistance, RayMask);
            if (col) if (hit.collider.tag.Equals("Player"))
                {
                    FindedPlayer = true;
                    FindCool = false;
                    StartCoroutine("FindCooling");
                }
        }
        else {
            if (Vector3.Distance(Player.position, transform.position) <= AttackRange && attackCool)
            {
                attackCool = false;
                navMeshAgent.velocity = Vector3.zero;
                anim.SetTrigger("Attack");
                StartCoroutine("AttackCooling");
            }
            else {
                m_CurrentWaypointIndex = 0;
                navMeshAgent.SetDestination(Player.position);
            }

            if (Vector3.Distance(Player.position, transform.position) > FindDistance && FindCool) {
                FindedPlayer = true;
            }
        }
    }

    IEnumerator AttackCooling() {
        yield return new WaitForSeconds(AttackCooltime);
        attackCool = true;
    }

    IEnumerator FindCooling()
    {
        yield return new WaitForSeconds(3);
        FindCool = true;
    }
}
