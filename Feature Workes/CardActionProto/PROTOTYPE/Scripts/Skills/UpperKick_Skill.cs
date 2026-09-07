using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperKick_Skill : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("mainChar_proto").GetComponent<SpriteRenderer>().flipX)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
        StartCoroutine("remove");
    }

    IEnumerator remove()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
