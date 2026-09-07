using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowUpper : MonoBehaviour
{
    public int howManyShot = 1;
    public GameObject bullet;
    public float cooltime;
    public float findRange;
    public float power;
    bool canshot = true;
    private Transform player;
    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (canshot && Vector2.Distance(player.position, transform.position) <= findRange)
        {
            canshot = false;
            StartCoroutine("shot");
        }
    }

    IEnumerator shot()
    {
        for (int i = 0; i < howManyShot; i++)
        {
            GameObject g = Instantiate(bullet,new Vector3(transform.position.x, transform.position.y + 0.12f, transform.position.z), transform.rotation);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-power, power), Random.Range(power, power + 2f), 0), ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(cooltime);
        canshot = true;
    }
}
