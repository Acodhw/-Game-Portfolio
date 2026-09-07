using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gient_Plant : MonoBehaviour
{
    public BossState bs;
    public Animator anim;
    public GameObject bullet;
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
            yield return new WaitForSeconds(0.185f);
        }
        yield return new WaitForSeconds(0.2f);
        canUseSkill = true;
    }
}
