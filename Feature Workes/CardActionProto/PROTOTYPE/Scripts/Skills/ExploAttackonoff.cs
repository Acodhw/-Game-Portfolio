using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploAttackonoff : MonoBehaviour
{
    bool ison = false;
    public GameObject g;
    void Update()
    {
        if (!ison && (Physics2D.Raycast(transform.position, Vector2.down, 2.7f, 1 << LayerMask.NameToLayer("Ground")).collider != null)) 
        {
            ison = true;
            StartCoroutine("Onoff");
        }
    }

    IEnumerator Onoff()
    {
        for (int i = 0; i < 25; i++)
        {
            g.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            g.SetActive(false);
            yield return new WaitForSeconds(0.03f);
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
