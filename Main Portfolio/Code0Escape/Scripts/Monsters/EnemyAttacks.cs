using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    public int damage;
    private PlayerState state;
    public bool hitDelete = true;

    void Start()
    {
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            state.Damage(damage);
            if (hitDelete) Destroy(gameObject);
        }

    }
}
