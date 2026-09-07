using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackShot : MonoBehaviour
{
    public int hitcount = 0;
    float first;
    public float speed;
    public GameObject summonOBJ;
    public PlayerState_Proto pstate;

    public enum shotkind
    {
        magic,
        gun,
        m_e,
        explo,
    }
    public shotkind sk;

    private void Start()
    {
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        first = speed;
        if (sk == shotkind.m_e)
        {
            StartCoroutine("boom");
        }
        else if(sk == shotkind.explo)
        {
            StartCoroutine("remove_explo");
        }
        else
        {
            StartCoroutine("remove");
        }
    }

    IEnumerator boom() {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    IEnumerator remove_explo()
    {
        yield return new WaitForSeconds(0.45f);
        speed = -speed;
        tag = "Untagged";
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }
    IEnumerator remove()
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        if (hitcount >= 3 && sk == shotkind.gun)
        {
            Destroy(gameObject);
        }
        else if (sk == shotkind.magic && hitcount >= 1)
        {
            Instantiate(summonOBJ, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (sk == shotkind.explo)
        {
            if (collision.tag == "Player" && first != speed)
            {
                pstate.Heal((pstate.intellect * 0.1f + (pstate.getMaxHP() - pstate.getHP()) * 0.05f) * hitcount);
                Destroy(gameObject);
            }
        }
        if(!(sk == shotkind.m_e)) {
            if (collision.tag == "Enemy")
            {
                if(first == speed)
                hitcount++;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && sk != shotkind.explo) {
                Destroy(gameObject);
            }
        }

    }
}
