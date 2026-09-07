using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyState : MonoBehaviour
{
    public bool StaticMonster = false;
    public int hp;
    public int giveExp;
    public int giveMoney;
    public float removeTime;
    Slider hpbar;
    SpriteRenderer sr;
    public Animator motion;
    private PlayerState ps;
    private Transform player;
    bool isBonusGived = false;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player").transform;
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        hpbar = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>();
        hpbar.maxValue = hp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player Attack") {
            hp -= collision.GetComponent<Bullet_Damage>().damage * collision.GetComponent<Bullet_Damage>().StrongerNotTOBoss;
            Destroy(collision.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (StaticMonster)
        {
            if (player.position.x > transform.position.x)
                sr.flipX = true;
            else
                sr.flipX = false;
        }
        hpbar.value = hp;
        if (hp <= 0 && !isBonusGived) {
            isBonusGived = true;
            ps.ExpUp(giveExp);
            ps.SetMoney(ps.GetMoney() + giveMoney);
            StartCoroutine("die");
        }
    }

    IEnumerator die()
    {
        motion.SetTrigger("die");
        yield return new WaitForSeconds(removeTime);
        Destroy(transform.root.gameObject);
    }
}
