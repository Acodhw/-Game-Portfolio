using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEvents : MonoBehaviour
{
    private PlayerState ps;
    bool canheal = true;
    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && canheal)
        {
            StartCoroutine("healing");
        }
    }

    IEnumerator healing()
    {
        canheal = false;
        ps.heal((int)(ps.getMaxHP() * 0.2f));
        yield return new WaitForSeconds(0.5f);
        canheal = true;
    }

}
