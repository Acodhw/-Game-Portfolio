using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Damage : MonoBehaviour
{
    public int damage;
    public bool DonRemoveOnWall = false;
    public int StrongerNotTOBoss = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if ((collision.gameObject.layer == 8 || (collision.tag == "enemy" && collision.gameObject.layer != 10)) && !DonRemoveOnWall && collision.tag != "Water")
        {
            Destroy(this.gameObject);
        }
    }
}
