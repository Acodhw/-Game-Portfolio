using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_King : MonoBehaviour
{
    public BossState bs;
    public Animator anim;
    public GameObject bullet;
    public GameObject Slime;
    public SpriteRenderer poti;
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
        if (pc.transform.position.x >= transform.position.x)
        {
            poti.flipX = true;
        }
        else
        {
            poti.flipX = false;
        }
        if (!pc.ispause && bs.hp > 0)
        {
                        
            if (canUseSkill)
            {
                bs.Boss_inter.SetActive(true);
                StartCoroutine("PotiAttack");
            }
        }
    }
    IEnumerator PotiAttack()
    {
        canUseSkill = false;
        if (GameObject.FindObjectsOfType(typeof(EnemyState)).Length < 20)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    GameObject g = Instantiate(Slime, new Vector3(transform.position.x + Random.Range(-3f, -0f), transform.position.y + Random.Range(2f, 4f), -0.2f), transform.rotation);
                }
                if (bs.hp <= 0)
                    break;
                yield return new WaitForSeconds(1.1f);
            }
        }
        if (bs.hp <= 0)
        {
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 1; i <= 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = j * 90 + i * 45;
                    g.GetComponent<bulletTo>().speed = i * 3;
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = j * 90 + 20 + i * 45;
                    g.GetComponent<bulletTo>().speed = i * 3;
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = j * 90 - 20 + i * 45;
                    g.GetComponent<bulletTo>().speed = i * 3;
                }
                if (bs.hp <= 0)
                    break;
                yield return new WaitForSeconds(0.35f);
            }
            yield return new WaitForSeconds(0.2f);
        }
        canUseSkill = true;
    }
}
