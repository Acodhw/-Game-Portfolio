using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlChain : MonoBehaviour
{
    public BossState bs;
    public GameObject bullet;
    private PlayerControl pc;

    public bool canUseSkill = false;
    public bool usingSkill = false;
    // Start is called before the first frame update
    void Start()
    {
        canUseSkill = false;
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
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
        if (!pc.ispause && bs.hp > 0)
        {
            if (canUseSkill)
            {
                StartCoroutine("ChainsSkills");
            }
        }
    }

    IEnumerator ChainsSkills()
    {
        canUseSkill = false;
        yield return new WaitForSeconds(2f);
        usingSkill = true;
        int a = Random.Range(1, 4);
        switch (a)
        {
            case 1:
                for (int i = 1; i <= 40; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        GameObject g = Instantiate(bullet, new Vector3(transform.position.x + (j - 4) * 1.5f, transform.position.y + 3, transform.position.z), transform.rotation);
                        g.GetComponent<bulletTo>().angle = Random.Range(225f, 315f);
                        g.GetComponent<bulletTo>().speed = 2;
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.25f);
                }
                usingSkill = false;
                break;
            case 2:
                for (int i = 0; i < 60; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        GameObject g = Instantiate(bullet, transform.position, transform.rotation);
                        g.GetComponent<bulletTo>().angle = j * 90 + 1.21f * i * i;
                        g.GetComponent<bulletTo>().speed = 5;
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.075f);
                }
                usingSkill = false;
                break;
            case 3:
                for (int i = 0; i < 36; i++)
                {
                    Vector2 v = pc.transform.position - transform.position;
                    float angle = -(Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg + 90) + 180;
                    GameObject g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle;
                    g.GetComponent<bulletTo>().speed = 6f;
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle + 45;
                    g.GetComponent<bulletTo>().speed = 6f;
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle + 90;
                    g.GetComponent<bulletTo>().speed = 6f;
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle - 45;
                    g.GetComponent<bulletTo>().speed = 6f;
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle - 90;
                    g.GetComponent<bulletTo>().speed = 6f;
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.03f);
                }
                usingSkill = false;
                break;
        }
        yield return new WaitWhile(() => usingSkill); ;
        canUseSkill = true;
    }
}
