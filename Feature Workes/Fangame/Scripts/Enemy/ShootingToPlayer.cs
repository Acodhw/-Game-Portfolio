using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingToPlayer : MonoBehaviour
{
    public int howManyShot = 1;
    public GameObject bullet;
    public float cooltime;
    public float findRange;
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
        if (canshot && Vector2.Distance(player.position, transform.position) <= findRange) {
            canshot = false;
            StartCoroutine("shot");
        }
    }

    IEnumerator shot() {        
        Vector2 v = player.position - transform.position;
        float angle = -(Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg + 90) + 180;
        float angle_Difference = 45;
        for (int i = 0; i < howManyShot; i++)
        {
            GameObject b = Instantiate(bullet, transform.position, transform.rotation);
            if (i == 0)
            {
                b.GetComponent<bulletTo>().angle = angle;
            }
            else
            {
                b.GetComponent<bulletTo>().angle = angle + i * angle_Difference;
                b = Instantiate(bullet, transform.position, transform.rotation);
                b.GetComponent<bulletTo>().angle = angle - i * angle_Difference;
            }
        }
        yield return new WaitForSeconds(cooltime);
        canshot = true;
    }
}
