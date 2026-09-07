using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drowned_Elite : MonoBehaviour
{
    public BossState bs;
    public GameObject bullet;
    public GameObject spear;
    public GameObject Drowned;
    private PlayerControl pc;
    private Transform player;
    private Rigidbody2D rigid;
    float speed = 0f;

    public bool canUseSkill = false;
    public bool usingSkill = false;
    // Start is called before the first frame update
    void Start()
    {
        canUseSkill = false;
        rigid = GetComponent<Rigidbody2D>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        player = GameObject.FindWithTag("Player").transform;
        StartCoroutine("timing");
    }

    IEnumerator timing()
    {
        canUseSkill = false;
        yield return new WaitForSeconds(5f);
        canUseSkill = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (pc.transform.position.x >= transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        if (!pc.ispause && bs.hp > 0)
        {
            Vector2 v = player.position - transform.position;
            rigid.velocity = v.normalized * speed;
            if (canUseSkill)
            {
                bs.Boss_inter.SetActive(true);
                speed = 0.5f;
                StartCoroutine("Skills");
            }
        }
    }

    IEnumerator Skills()
    {
        canUseSkill = false;
        yield return new WaitForSeconds(2f);
        usingSkill = true;
        int a = Random.Range(1, 6);
        switch (a)
        {
            case 1:
                if (GameObject.FindObjectsOfType(typeof(EnemyState)).Length < 2)
                {

                    for (int j = 0; j < 3; j++)
                    {
                        GameObject g = Instantiate(Drowned, new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), -0.2f), transform.rotation);
                        g.GetComponent<WaterMoving>().speed = 0.3f;
                        g.GetComponent<EnemyState>().giveExp = 0;
                        g.GetComponent<EnemyState>().giveMoney = 0;
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(1.1f);

                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            GameObject g = Instantiate(spear, new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), -0.2f), transform.rotation);
                        }
                        if (bs.hp <= 0)
                            break;
                        yield return new WaitForSeconds(1.1f);
                    }
                }
                usingSkill = false;
                break;
            case 2:
                for (int i = 0; i < 10; i++)
                {
                    if (i % 2 == 0)
                    {
                        GameObject g = Instantiate(spear, new Vector3(transform.position.x + 1.4f, transform.position.y, transform.position.z), transform.rotation);
                        g = Instantiate(spear, new Vector3(transform.position.x - 1.4f, transform.position.y, transform.position.z), transform.rotation);
                        g = Instantiate(spear, new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z), transform.rotation);
                        g = Instantiate(spear, new Vector3(transform.position.x, transform.position.y - 1.4f, transform.position.z), transform.rotation);
                    }
                    else
                    {
                        GameObject g = Instantiate(spear, new Vector3(transform.position.x + 1, transform.position.y + 1, transform.position.z), transform.rotation);
                        g = Instantiate(spear, new Vector3(transform.position.x - 1, transform.position.y - 1, transform.position.z), transform.rotation);
                        g = Instantiate(spear, new Vector3(transform.position.x - 1, transform.position.y + 1, transform.position.z), transform.rotation);
                        g = Instantiate(spear, new Vector3(transform.position.x + 1, transform.position.y - 1, transform.position.z), transform.rotation);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.5f);
                }
                usingSkill = false;
                break;
            case 3:
                for (int i = 0; i < 60; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        GameObject g = Instantiate(bullet, transform.position, transform.rotation);
                        g.GetComponent<bulletTo>().angle = j * 90 + 1.21f * i * i;
                        g.GetComponent<bulletTo>().speed = 2.5f;
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.2f);
                }
                usingSkill = false;
                break;
            case 4:

                Vector2 v = player.position - transform.position;
                float angle = -(Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg + 90) + 180;
                GameObject g1 = Instantiate(bullet, transform.position, transform.rotation);
                g1.GetComponent<bulletTo>().angle = angle;
                g1.GetComponent<bulletTo>().speed = 1f;
                g1.GetComponent<bulletTo>().time = 10f;
                GameObject g2 = Instantiate(bullet, transform.position, transform.rotation);
                g2.GetComponent<bulletTo>().angle = angle + 60;
                g2.GetComponent<bulletTo>().speed = 1f;
                g2.GetComponent<bulletTo>().time = 10f;
                GameObject g3 = Instantiate(bullet, transform.position, transform.rotation);
                g3.GetComponent<bulletTo>().angle = angle - 60;
                g3.GetComponent<bulletTo>().speed = 1f;
                g3.GetComponent<bulletTo>().time = 10f;

                if (bs.hp <= 0)
                    break;
                yield return new WaitForSeconds(1f);
                for (int i = 0; i < 5; i++)
                {
                    GameObject g = Instantiate(spear, g1.transform.position, transform.rotation);
                    g = Instantiate(spear, g2.transform.position, transform.rotation);
                    g = Instantiate(spear, g3.transform.position, transform.rotation);
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(1.5f);
                }
                usingSkill = false;
                break;
            case 5:
                speed = 0;
                if (bs.hp <= 0)
                    break;
                yield return new WaitForSeconds(3f);
                if (bs.HPbar.maxValue < bs.hp + 75)
                    bs.hp = (int)bs.HPbar.maxValue;
                else
                    bs.hp += 75;
                speed = 0.5f;

                usingSkill = false;
                break;
        }
        yield return new WaitWhile(() => usingSkill); ;
        canUseSkill = true;
    }
}
