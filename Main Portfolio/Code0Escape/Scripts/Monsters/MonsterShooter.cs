using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterShooter : MonoBehaviour
{
    public Transform Player;

    public float FindDistance;
    public float AttackCooltime;
    public GameObject bullet;

    bool attackCool = true;

    int m_CurrentWaypointIndex;
    int maxhp;

    void Start()
    {
        Player = GameObject.Find("PlayerPrefeb").transform.GetChild(1);
    }


    void Update()
    {
        if (Vector3.Distance(Player.position, transform.position) <= FindDistance && attackCool)
        {
            attackCool = false;
            Vector3 l_vector = Player.transform.position - transform.position;
            Instantiate(bullet, transform.position, Quaternion.LookRotation(l_vector).normalized);
            StartCoroutine("AttackCooling");
        }
    }

    IEnumerator AttackCooling()
    {
        yield return new WaitForSeconds(AttackCooltime);
        attackCool = true;
    }

}
