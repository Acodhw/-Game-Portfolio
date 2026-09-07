using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rush : MonoBehaviour
{
    int direction = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("mainChar_proto").GetComponent<SpriteRenderer>().flipX)
        {
            direction = -1;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else {
            direction = 1;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        StartCoroutine("remove");
    }

    IEnumerator remove() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.right * direction * 18f;
    }
}
