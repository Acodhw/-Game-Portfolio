using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleAttack : MonoBehaviour
{
    public GameObject bullet;
    public Transform whalepoint;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("attack");
    }

    IEnumerator attack() {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 100; i++) {
            GameObject g = Instantiate(bullet, whalepoint.position, transform.rotation);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f)), ForceMode2D.Impulse);
            g = Instantiate(bullet, whalepoint.position, transform.rotation);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f)), ForceMode2D.Impulse);
            g = Instantiate(bullet, whalepoint.position, transform.rotation);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f)), ForceMode2D.Impulse);
            g = Instantiate(bullet, whalepoint.position, transform.rotation);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f)), ForceMode2D.Impulse);
            g = Instantiate(bullet, whalepoint.position, transform.rotation);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f)), ForceMode2D.Impulse);
            
            yield return new WaitForSeconds(0.03f);
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
