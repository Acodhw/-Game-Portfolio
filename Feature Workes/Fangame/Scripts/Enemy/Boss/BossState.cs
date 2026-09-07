using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossState : MonoBehaviour
{
    public int hp;
    public Animator anim;
    public GameObject AfterEventObject;
    public GameObject PL_inter;
    public GameObject Boss_inter;

    public Slider HPbar;
    public int giveExp;
    public int giveMoney;
    public int GiveItemCode;
    public float removeTime;

    private PlayerState ps;
    private InventorySystem inven;
    bool isBonusGived = false;
    // Start is called before the first frame update
    void Start()
    {
        inven = GameObject.Find("GameManager").GetComponent<InventorySystem>();
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        HPbar.maxValue = hp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player Attack")
        {
            hp -= collision.GetComponent<Bullet_Damage>().damage;
            Destroy(collision.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HPbar.value = hp;
        if (hp <= 0 && !isBonusGived)
        {
            isBonusGived = true;
            ps.ExpUp(giveExp);
            ps.SetMoney(ps.GetMoney() + giveMoney);
            if (GiveItemCode > 0) {
                inven.itemcodes.Add(GiveItemCode);
            }
            StartCoroutine("die");
        }
    }

    IEnumerator die()
    {
        PL_inter.SetActive(false);
        Boss_inter.SetActive(false);
        anim.SetTrigger("idle");
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("die");
        yield return new WaitForSeconds(removeTime);
        PL_inter.SetActive(true);
        AfterEventObject.SetActive(true);
        Destroy(transform.root.gameObject);
    }
}
