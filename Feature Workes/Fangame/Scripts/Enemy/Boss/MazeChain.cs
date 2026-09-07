using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeChain : MonoBehaviour
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
            if (canUseSkill) {
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
        switch (a) {
            case 1:
                for (int i = 1; i <= 3; i++) {
                    for (int j = 0; j < 4; j++) {
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
                usingSkill = false;
                break;
            case 2:
                for (int i = 0; i < 40; i++)
                {
                    GameObject g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = i * 7;
                    g.GetComponent<bulletTo>().speed = 5;
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = 120 + i * 7;
                    g.GetComponent<bulletTo>().speed = 5;
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = -120 + i * 7;
                    g.GetComponent<bulletTo>().speed = 5;
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.15f);
                }
                usingSkill = false;
                break;
            case 3:
                for (int i = 0; i < 18; i++)
                {
                    GameObject g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                    g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                    g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    g = Instantiate(bullet, transform.position, transform.rotation);
                    g.GetComponent<bulletTo>().angle = Random.Range(0f, 360f);
                    g.GetComponent<bulletTo>().speed = Random.Range(2f, 5f);
                    if (bs.hp <= 0)
                        break;
                    yield return new WaitForSeconds(0.15f);
                }
                usingSkill = false;
                break;
        }
        yield return new WaitWhile(() => usingSkill);;
        canUseSkill = true;
    }
}
