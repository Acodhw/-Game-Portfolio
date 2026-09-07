using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossSecond : MonoBehaviour
{

    public Animator anim;

    public Rigidbody bullet1;
    public Rigidbody bullet2;

    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Pattarn", 1, 10);
        InvokeRepeating("RandomShot", 1, 3f);
        InvokeRepeating("AttackShot", 1, 1f);
    }

    void Pattarn()
    {
        Vector3 l_vector = player.position - transform.position;
        transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(l_vector).eulerAngles.y, 0);
        switch (Random.Range(0, 2)) {
            case 0:
                anim.SetTrigger("p1");
                break;
            case 1:
                anim.SetTrigger("p2");
                break;
        }
    }

    void AttackShot()
    {
        Vector3 l_vector = player.transform.position - (transform.position + new Vector3(0, 15, 0));

        Rigidbody bul = Instantiate(bullet1, transform.position + new Vector3(0, 15, 0), Quaternion.LookRotation(l_vector).normalized);
        bul.GetComponent<BulletMove>().speed = 45f;

         bul = Instantiate(bullet1, transform.position + new Vector3(0, 15, 0), Quaternion.LookRotation(l_vector).normalized);
        bul.GetComponent<BulletMove>().speed = 60;

         bul = Instantiate(bullet1, transform.position + new Vector3(0, 15, 0), Quaternion.LookRotation(l_vector).normalized);
        bul.GetComponent<BulletMove>().speed = 75f;
    }

    void RandomShot()
    {
        Vector3 l_vector = player.transform.position - transform.position;

        Rigidbody bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 15, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
    }
}
