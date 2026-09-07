using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotWantVector : MonoBehaviour
{
    public float angle;
    public float time;
    public int hitcountLimit = 1;
    public bool wallthrow;
    private Vector2 togo;
    private Rigidbody2D rigid;
    private int hitcount;
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        rigid = GetComponent<Rigidbody2D>();
        togo = new Vector2(Mathf.Cos(angle * Mathf.PI / 180), Mathf.Sin(angle * Mathf.PI / 180)).normalized;
        StartCoroutine("remove");
    }

    IEnumerator remove()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        rigid.velocity = togo * 18;
        if (hitcount >= hitcountLimit)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
            hitcount++;
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !wallthrow)
        {
            Destroy(gameObject);
        }
    }
}
