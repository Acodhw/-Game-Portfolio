using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheChild : MonoBehaviour
{
    public BossState bs;
    public Animator anim;
    public GameObject bullet;
    public GameObject Bone;
    public Transform[] BulletPoint;
    public GameObject stone;
    private PlayerControl pc;

    Collider2D[] comp;

    public bool canUseSkill = false;
    public bool usingSkill = false;
    // Start is called before the first frame update
    void Start()
    {
        canUseSkill = false;
        comp = Bone.GetComponents<Collider2D>();        
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
        if (bs.hp > 3600)
        {
            comp[2].enabled = false;
            comp[1].enabled = false;
            comp[0].enabled = true;            
        }
        else if (bs.hp > 1800)
        {
            comp[2].enabled = false;
            comp[1].enabled = true;
            comp[0].enabled = false;
            anim.SetBool("Xleg", true);
        }
        else
        {
            comp[2].enabled = true;
            comp[1].enabled = false;
            comp[0].enabled = false;
            anim.SetBool("Xarm", true);
        }

        if (!pc.ispause && bs.hp > 0)
        {
            
            if (canUseSkill && (!anim.GetCurrentAnimatorStateInfo(0).IsName("BreakLeg") && !anim.GetCurrentAnimatorStateInfo(0).IsName("BreakBody")))
            {
                bs.Boss_inter.SetActive(true);
                StartCoroutine("PotiAttack");
            }
        }
    }
    IEnumerator PotiAttack()
    {
        canUseSkill = false;
        yield return new WaitForSeconds(2f);
        usingSkill = true;
        if (bs.hp > 3600)
        {
            int a = Random.Range(1, 4);

            switch (a)
            {
                case 1:
                    anim.SetTrigger("skill1");
                    yield return new WaitForSeconds(1f);
                    anim.SetTrigger("idle");
                    for (int i = 0; i < 35; i++)
                    {
                        GameObject g = Instantiate(bullet, BulletPoint[1].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(1.66f);
                    for (int i = 0; i < 35; i++)
                    {
                        GameObject g = Instantiate(bullet, BulletPoint[0].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.66f);
                    usingSkill = false;
                    break;
                case 2:
                    anim.SetTrigger("skill2");
                    yield return new WaitForSeconds(0.66f);
                    anim.SetTrigger("idle");
                    for (int i = 0; i < 35; i++)
                    {
                        GameObject g = Instantiate(bullet, BulletPoint[0].position, new Quaternion(0,0,0,0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                        g = Instantiate(bullet, BulletPoint[1].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.416f);
                    for (int i = 0; i < 35; i++)
                    {
                        GameObject g = Instantiate(bullet, BulletPoint[0].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                        g = Instantiate(bullet, BulletPoint[1].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.41f);
                    for (int i = 0; i < 35; i++)
                    {
                        GameObject g = Instantiate(bullet, BulletPoint[0].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                        g = Instantiate(bullet, BulletPoint[1].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    usingSkill = false;
                    break;
                case 3:
                    anim.SetTrigger("skill3");
                    yield return new WaitForSeconds(1f);
                    anim.SetTrigger("idle");
                    yield return new WaitForSeconds(3.3f);
                    usingSkill = false;
                    break;
            }
        }
        else if (bs.hp > 1800)
        {
            int a = Random.Range(1, 4);

            switch (a)
            {
                case 1:
                    anim.SetTrigger("skill1");
                    yield return new WaitForSeconds(0.167f);
                    anim.SetTrigger("idle");
                    for (int i = 0; i < 20; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            GameObject g = Instantiate(bullet, BulletPoint[2].position, transform.rotation);
                            g.GetComponent<bulletTo>().angle = j * 90 + 1.21f * i * i;
                            g.GetComponent<bulletTo>().speed = 3;
                            g.GetComponent<bulletTo>().time = 5;
                            g = Instantiate(bullet, BulletPoint[3].position, transform.rotation);
                            g.GetComponent<bulletTo>().angle = j * 90 + 1.21f * i * i;
                            g.GetComponent<bulletTo>().speed = 3;
                            g.GetComponent<bulletTo>().time = 5;
                        }
                        if (bs.hp <= 0)
                            break;
                        yield return new WaitForSeconds(0.16f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.167f);
                    usingSkill = false;
                    break;
                case 2:
                    anim.SetTrigger("skill2");
                    yield return new WaitForSeconds(1.5f);
                    anim.SetTrigger("idle");
                    for (int i = 0; i < 35; i++)
                    {
                        GameObject g = Instantiate(bullet, BulletPoint[2].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                        g = Instantiate(bullet, BulletPoint[3].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.66f);
                    for (int i = 0; i < 35; i++)
                    {
                        GameObject g = Instantiate(bullet, BulletPoint[2].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                        g = Instantiate(bullet, BulletPoint[3].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.66f);
                    for (int i = 0; i < 35; i++)
                    {
                        GameObject g = Instantiate(bullet, BulletPoint[2].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                        g = Instantiate(bullet, BulletPoint[3].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.833f);
                    usingSkill = false;
                    break;
                case 3:
                    anim.SetTrigger("skill3");
                    yield return new WaitForSeconds(0.416f * 2);
                    anim.SetTrigger("idle");
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            GameObject g = Instantiate(stone, new Vector3(BulletPoint[3].position.x, BulletPoint[3].position.y, -0.98f), transform.rotation);
                            g.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-7f, 7f), Random.Range(9f, 25f), 0), ForceMode2D.Impulse);
                        }
                        if (bs.hp <= 0)
                            break;
                        yield return new WaitForSeconds(0.0583f * 2);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.333f * 2);
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            GameObject g = Instantiate(stone, new Vector3(BulletPoint[2].position.x, BulletPoint[2].position.y, -0.98f), transform.rotation);
                            g.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-7f, 7f), Random.Range(9f, 15f), 0), ForceMode2D.Impulse);
                        }
                        if (bs.hp <= 0)
                            break;
                        yield return new WaitForSeconds(0.0583f * 2);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.583f * 2);
                    usingSkill = false;
                    break;
            }
        }
        else
        {
            int a = Random.Range(1, 3);

            switch (a)
            {
                case 1:
                    anim.SetTrigger("skill1");
                    yield return new WaitForSeconds(0.75f);
                    anim.SetTrigger("idle");
                    for (int j = 0; j < 35; j++)
                    {
                        GameObject g = Instantiate(stone, BulletPoint[4].position, transform.rotation);
                        g.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-7f, 7f), Random.Range(9f, 15f), 0), ForceMode2D.Impulse);
                        g = Instantiate(bullet, BulletPoint[4].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(1f);
                    for (int j = 0; j < 35; j++)
                    {
                        GameObject g = Instantiate(stone, BulletPoint[4].position, transform.rotation);
                        g.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-7f, 7f), Random.Range(9f, 15f), 0), ForceMode2D.Impulse);
                        g = Instantiate(bullet, BulletPoint[4].position, new Quaternion(0, 0, 0, 0));
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                        g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.25f);
                    usingSkill = false;
                    break;
                case 2:
                    for (int i = 0; i < 100; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            GameObject g = Instantiate(bullet, new Vector3(BulletPoint[4].position.x, BulletPoint[4].position.y, -0.98f), transform.rotation);
                            g.GetComponent<bulletTo>().angle = j * 90 + 1.21f * i * i;
                            g.GetComponent<bulletTo>().speed = 5;
                            g = Instantiate(bullet, new Vector3(BulletPoint[4].position.x, BulletPoint[4].position.y, -0.98f), transform.rotation);
                            g.GetComponent<bulletTo>().angle = i * 7;
                            g.GetComponent<bulletTo>().speed = 5;
                            g = Instantiate(bullet, new Vector3(BulletPoint[4].position.x, BulletPoint[4].position.y, -0.98f), transform.rotation);
                            g.GetComponent<bulletTo>().angle = 180 + i * 7;
                            g.GetComponent<bulletTo>().speed = 5;
                        }
                        if (bs.hp <= 0)
                            break;
                        yield return new WaitForSeconds(0.185f);
                    }
                    usingSkill = false;
                    break;                
            }
        }
        yield return new WaitWhile(() => usingSkill); ;
        canUseSkill = true;
    }
}

