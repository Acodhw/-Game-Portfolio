using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGround : MonoBehaviour
{
    private PlayerControl pc;
    // Start is called before the first frame update
    void Start()
    {
        pc = transform.parent.GetComponent<PlayerControl>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8) {
            pc.isonground = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            pc.isonground = false;
        }
    }
}
