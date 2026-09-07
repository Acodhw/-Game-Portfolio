using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curse : MonoBehaviour
{
    public GameObject CurseBul;
    private GameManager gmm;
    private PlayerState ps;
    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        ps.damage((int)(ps.getMaxHP() * 0.1f));
        StartCoroutine("remthis");
    }

    IEnumerator remthis() {
        int dam = (int)(0.1f * (((ps.getLevel() - 1) * 10) + 100 - ((3 - gmm.getDifficulty()) * 30 + ((ps.getLevel() - 1) * ((3 - gmm.getDifficulty()) + 5)))));
        GameObject g = Instantiate(CurseBul, transform.position, transform.rotation);
        g.GetComponent<bulletTo>().angle = 0;
        g.GetComponent<Bullet_Damage>().damage = dam;
        g = Instantiate(CurseBul, transform.position, transform.rotation);
        g.GetComponent<bulletTo>().angle = 45;
        g.GetComponent<Bullet_Damage>().damage = dam;
        g = Instantiate(CurseBul, transform.position, transform.rotation);
        g.GetComponent<bulletTo>().angle = 90;
        g.GetComponent<Bullet_Damage>().damage = dam;
        g = Instantiate(CurseBul, transform.position, transform.rotation);
        g.GetComponent<bulletTo>().angle = 135;
        g.GetComponent<Bullet_Damage>().damage = dam;
        g = Instantiate(CurseBul, transform.position, transform.rotation);
        g.GetComponent<bulletTo>().angle = 180;
        g.GetComponent<Bullet_Damage>().damage = dam;
        g = Instantiate(CurseBul, transform.position, transform.rotation);
        g.GetComponent<bulletTo>().angle = 225;
        g.GetComponent<Bullet_Damage>().damage = dam;
        g = Instantiate(CurseBul, transform.position, transform.rotation);
        g.GetComponent<bulletTo>().angle = 270;
        g.GetComponent<Bullet_Damage>().damage = dam;
        g = Instantiate(CurseBul, transform.position, transform.rotation);
        g.GetComponent<bulletTo>().angle = 315;
        g.GetComponent<Bullet_Damage>().damage = dam;
        yield return new WaitForSeconds(1.1f);
        Destroy(gameObject);
    }
}
