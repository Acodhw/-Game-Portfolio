using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroiWoodhorse : MonoBehaviour
{

    public Animator anim;

    public GameObject virusWarrior;
    public GameObject virusShooter;

    public Rigidbody bullet;

    public Transform player;
    public Transform SpawnParant;
    public Transform Eye1;
    public Transform Eye2;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SummonEnemy", 15, 30);
        InvokeRepeating("Attack", 10, 8);
        InvokeRepeating("Shot", 10, 0.5f);
    }

    void SummonEnemy() {
        EnemyMovement e = Instantiate(virusWarrior, transform.position + new Vector3(Random.Range(10f, 25f), 0, 0), Quaternion.identity, SpawnParant).GetComponent<EnemyMovement>();
        e.waypoints = new Transform[1] { player };

        e = Instantiate(virusShooter, transform.position + new Vector3(Random.Range(-25f, -10f), 0, 0), Quaternion.identity, SpawnParant).GetComponent<EnemyMovement>();
        e.waypoints = new Transform[1] { player };
    }

    void Attack()
    {
        Vector3 l_vector = player.position - transform.position;
        transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(l_vector).eulerAngles.y, 0);
        anim.SetTrigger("Attack");
    }

    void Shot()
    {
        Vector3 l_vector = player.transform.position - Eye1.position;

        Rigidbody bul = Instantiate(bullet, Eye1.position, Quaternion.LookRotation(l_vector).normalized);
        bul.velocity = bul.transform.forward * 30f;

        l_vector = player.transform.position - Eye2.position;

        bul = Instantiate(bullet, Eye2.position, Quaternion.LookRotation(l_vector).normalized);
        bul.velocity = bul.transform.forward * 30f;
    }

}
