using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isHPheal;


    private void Start()
    {
        StartCoroutine("ItemRemove");
    }

    IEnumerator ItemRemove() {
        yield return new WaitForSeconds(15f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            collision.GetComponent<MainCharacter>().GetItem(isHPheal);
            Destroy(gameObject); 
        }
    }
}
