using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpShot : MonoBehaviour
{
    public float angle;
    public float speed;
    public float time;
    public bool isRigid;
    Rigidbody2D rigid;
    Vector2 foword;
    // Start is called before the first frame update
    void Start()
    {
        if(isRigid)
            rigid = GetComponent<Rigidbody2D>();
        StartCoroutine("RemoveObj");       
    }

    // Update is called once per frame
    void Update()
    {
        foword = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        if (isRigid)
            rigid.velocity = foword * speed;
    }

    IEnumerator RemoveObj()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
