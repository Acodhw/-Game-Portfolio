using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RansomWare : MonoBehaviour
{
    public Rigidbody bullet;
    public Transform player;

    public GameObject boomEffect;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Shot", 10, 0.25f);
    }

    void Shot() {
        Vector3 shotPoint = new Vector3(21.51f, Random.Range(80, 20), Random.Range(80, 20));
        Vector3 l_vector = player.transform.position - shotPoint;

        Instantiate(boomEffect, shotPoint, Quaternion.identity);
        Rigidbody bul = Instantiate(bullet, shotPoint, Quaternion.LookRotation(l_vector).normalized);
        bul.velocity = bul.transform.forward * 30f;

        shotPoint = new Vector3(21.51f, Random.Range(80, 20), Random.Range(80, 20));
        l_vector = player.transform.position - shotPoint;

        Instantiate(boomEffect, shotPoint, Quaternion.identity);
        bul = Instantiate(bullet, shotPoint, Quaternion.LookRotation(l_vector).normalized);
        bul.velocity = bul.transform.forward * 30f;

        shotPoint = new Vector3(21.51f, Random.Range(80, 20), Random.Range(80, 20));
        l_vector = player.transform.position - shotPoint;

        Instantiate(boomEffect, shotPoint, Quaternion.identity);
        bul = Instantiate(bullet, shotPoint, Quaternion.LookRotation(l_vector).normalized);
        bul.velocity = bul.transform.forward * 30f;

    }
}
