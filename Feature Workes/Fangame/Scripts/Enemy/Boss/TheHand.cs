using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHand : MonoBehaviour
{
    public BossState bs;
    public Animator anim;
    public GameObject bullet;
    public Transform BulletPoint;
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

        int a = Random.Range(1, 4);

        switch (a)
        {
            case 1:
                anim.SetTrigger("skill1");
                yield return new WaitForSeconds(0.5f);
                anim.SetTrigger("idle");
                for (int i = 0; i < 10; i++)
                {
                    Vector2 v = pc.transform.position - BulletPoint.transform.position;
                    float angle = -(Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg + 90) + 180;
                    GameObject g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle;
                    g.GetComponent<bulletTo>().speed = 4f;
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle + 45;
                    g.GetComponent<bulletTo>().speed = 4f;
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle + 90;
                    g.GetComponent<bulletTo>().speed = 4f;
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle - 45;
                    g.GetComponent<bulletTo>().speed = 4f;
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = angle - 90;
                    g.GetComponent<bulletTo>().speed = 4f;
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.0333f);
                }
                if (bs.hp <= 0)
                    break;
                yield return new WaitForSeconds(0.5f);
                usingSkill = false;
                break;
            case 2:
                anim.SetTrigger("skill2");
                yield return new WaitForSeconds(1f);
                anim.SetTrigger("idle");
                for (int i = 0; i < 20; i++)
                {
                    GameObject g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(180f,360f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f); 
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(180f, 360f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f); 
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(180f, 360f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f);
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(180f, 360f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f);
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(180f, 360f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f);
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.0333f);
                }
                if (bs.hp <= 0)
                    break;
                yield return new WaitForSeconds(0.333f);
                for (int i = 0; i < 20; i++)
                {
                    GameObject g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(0f, 180f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f);
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(0f, 180f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f);
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(0f, 180f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f);
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(0f, 180f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f);
                    g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(0f, 180f);
                    g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f);
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.0333f);
                }
                if (bs.hp <= 0)
                    break;
                yield return new WaitForSeconds(0.34f);
                usingSkill = false;
                break;
            case 3:
                anim.SetTrigger("skill3");
                yield return new WaitForSeconds(0.833f);
                anim.SetTrigger("idle");
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        GameObject g = Instantiate(bullet, BulletPoint.transform.position, transform.rotation);
                        g.GetComponent<bulletTo>().angle = Random.Range(0f, 180f);
                        g.GetComponent<bulletTo>().speed = Random.Range(3f, 6f);
                    }
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.0166f);
                }

                if (bs.hp <= 0)
                    break;
                yield return new WaitForSeconds(0.02f);
                usingSkill = false;
                break;

        }
        yield return new WaitWhile(() => usingSkill); ;
        canUseSkill = true;
    }
}
