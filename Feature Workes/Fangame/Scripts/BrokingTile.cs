using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokingTile : MonoBehaviour
{
    public GameObject tile;
    bool isbroking = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isbroking) {
            StartCoroutine("broking");
        }
    }

    IEnumerator broking() {
        yield return new WaitForSeconds(1f);
        tile.SetActive(false);
        isbroking = true;
        yield return new WaitForSeconds(5f);
        tile.SetActive(true);
        isbroking = false;
    }
}
