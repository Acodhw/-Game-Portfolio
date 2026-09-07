using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingZone : MonoBehaviour
{
    PlayerState_Proto pstate;
    bool a = true;
    // Start is called before the first frame update
    void Start()
    {
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        StartCoroutine("remove");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (a)
            {
                a = false;
                pstate.Heal(pstate.getMaxHP() * 0.02f);
                StartCoroutine("cooltime");
            }
        }
    }

    IEnumerator cooltime() 
    {
        yield return new WaitForSeconds(1);
        a = true;
    }
    IEnumerator remove()
    {
        yield return new WaitForSeconds(7);
        Destroy(gameObject);
    }
}
