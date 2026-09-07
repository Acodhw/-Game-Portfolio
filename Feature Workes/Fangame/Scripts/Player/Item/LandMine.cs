using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    public GameObject boom;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && collision.gameObject.layer != 10)
        {
            Instantiate(boom, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
