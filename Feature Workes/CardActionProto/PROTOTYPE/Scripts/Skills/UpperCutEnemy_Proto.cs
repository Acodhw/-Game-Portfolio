using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperCutEnemy_Proto : MonoBehaviour
{
    Rigidbody2D rigid;

    private void Start()
    {
        rigid = transform.parent.GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttack" && collision.GetComponent<UpperAttack>() != null)
        {
            rigid.velocity = Vector2.up * collision.GetComponent<UpperAttack>().upperPower;
        }
    }
}
