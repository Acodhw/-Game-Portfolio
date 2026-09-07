using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomShot : MonoBehaviour
{
    public GameObject boom;
    public GameObject pleces;
    public int boomshotcode;

    private void Start()
    {
        if (boomshotcode == 1)
        {
            Rigidbody2D a = Instantiate(pleces, transform.position + Vector3.up * 0.7f, transform.rotation).GetComponent<Rigidbody2D>();
            a.AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f)), ForceMode2D.Impulse);
            a = Instantiate(pleces, transform.position + Vector3.up * 0.7f, transform.rotation).GetComponent<Rigidbody2D>();
            a.AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f)), ForceMode2D.Impulse);
            a = Instantiate(pleces, transform.position + Vector3.up * 0.7f, transform.rotation).GetComponent<Rigidbody2D>();
            a.AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f)), ForceMode2D.Impulse);
            a = Instantiate(pleces, transform.position + Vector3.up * 0.7f, transform.rotation).GetComponent<Rigidbody2D>();
            a.AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f)), ForceMode2D.Impulse);

            StartCoroutine("remove", 0.5f);
        }
        else if(boomshotcode == 2)
            StartCoroutine("remove", 2f);
    }

    IEnumerator remove(float count)
    {
        yield return new WaitForSeconds(count);
        Destroy(gameObject);
    }


    void OnDestroy()
    {

        if (boomshotcode == 0)
        {
            Instantiate(boom, transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && boomshotcode == 2)
        {
            Destroy(gameObject);
        }
    }
}
