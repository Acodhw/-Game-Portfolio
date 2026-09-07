using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoCut_Skill_Proto : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().flipX = GameObject.Find("mainChar_proto").GetComponent<SpriteRenderer>().flipX;
        StartCoroutine("remove");
    }

    IEnumerator remove()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
