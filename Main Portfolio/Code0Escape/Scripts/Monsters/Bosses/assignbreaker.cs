using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class assignbreaker : MonoBehaviour
{
    public GameObject Boom;

    public Rigidbody bullet1;
    public Rigidbody bullet2;

    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Booming", 30.3f, 5);
        InvokeRepeating("AttackShot", 30.3f, 0.5f);
        InvokeRepeating("RandomShot", 30.3f, 3f);
    }

    void Booming()
    {
        Instantiate(Boom, transform.position + new Vector3(0, 1.992764f, 0), Quaternion.identity, transform);
    }

    void AttackShot()
    {
        Vector3 l_vector = player.transform.position - (transform.position + new Vector3(0, 1.992764f, 0));

        Rigidbody bul = Instantiate(bullet1, transform.position + new Vector3(0, 1.992764f, 0), Quaternion.LookRotation(l_vector).normalized);
        bul.velocity = bul.transform.forward * 45f;

    }

    void RandomShot()
    {
        Rigidbody bul = Instantiate(bullet2, transform.position + new Vector3(0, 1.992764f, 0), Quaternion.Euler(Random.Range(0,360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 1.992764f, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 1.992764f, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 1.992764f, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 1.992764f, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 1.992764f, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
        bul = Instantiate(bullet2, transform.position + new Vector3(0, 1.992764f, 0), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bul.velocity = bul.transform.forward * 30f;
    }
}
