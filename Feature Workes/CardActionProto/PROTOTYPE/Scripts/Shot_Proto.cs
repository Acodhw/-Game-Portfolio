using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot_Proto : MonoBehaviour
{
    public GameObject Shot;
    public bool shotToLeft = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        GameObject g = Instantiate(Shot, transform.position, transform.rotation);
        if (shotToLeft)
            g.GetComponent<PlayerAttackShot>().speed = -g.GetComponent<PlayerAttackShot>().speed;
    }
}
