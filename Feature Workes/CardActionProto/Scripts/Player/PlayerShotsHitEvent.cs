using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DirectionV {
    right, up, down, left 
}
public class PlayerShotsHitEvent : MonoBehaviour
{
    
    [SerializeField] bool ignoreEnemyHit;
    [SerializeField] bool ignoreWallHit;
    [SerializeField] bool ignoreDirection;
    [SerializeField] int maxEnemy;
    int EnemyCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy"))
        {
            if (!ignoreEnemyHit) {
                EnemyCount++;
                if(EnemyCount >= maxEnemy) Destroy(gameObject);
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !ignoreWallHit)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(!ignoreDirection) transform.right = GetComponent<Rigidbody2D>().velocity;
    }
}
