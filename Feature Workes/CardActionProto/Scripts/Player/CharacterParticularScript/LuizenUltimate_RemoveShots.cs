using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuizenUltimate_RemoveShots : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("EnemyAttack") && collision.GetComponent<ShotsEvent>() != null)
        {
            if(collision.GetComponent<ShotsEvent>().GetIsBullet()) Destroy(collision.gameObject);
        }
    }
}
