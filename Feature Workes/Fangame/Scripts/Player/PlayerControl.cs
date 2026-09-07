using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    private GameManager gmm;
    private float speed = 2.5f;
    private float jpower = 4.75f;
    private SpriteRenderer sr;
    private bool shotcool = true;
    private Rigidbody2D rigid;
    public GameObject apple_seed;
    public Animator motion;
    public bool isonground;
    public bool ispause = true;
    public bool isinWater = false;
    bool isotherInter = false;
    bool isinven = false;
    bool isSetskill = false;
    bool hitcool = true;
    bool canMove = true;
    bool manafill = false;
    bool die = false;

    private InventorySystem inven;
    public GameObject inventory_menu;
    public GameObject Player_interface;
    public GameObject Pause_interface;
    public GameObject SkillSet_interface;
    private PlayerState ps;

    public Text hps;
    public Text money_tx;
    public Text inven_money_tx;
    public Slider hpbar;
    public Slider EXPbar;
    public Slider invenHpbar;
    public Text invenHps;
    public Image mana_Bar;
    public Image MaxMana_Bar;

    public GameObject[] attackItemObj;

    public Sprite[] skillIcons;
    public int skill1code;
    public int skill2code;
    public Image Wskill;
    public Image Eskill;

    public Image Inter_Wskill;
    public Image Inter_Eskill;

    public GameObject[] skill_obj;
    bool[] cooling = new bool[21];
    public float[] cooltime = new float[21];
    public Image fade;

    public GameObject After_inter;

    private void Awake()
    {
        StartCoroutine("fadeout");

    }

    IEnumerator fadeout()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gmm.returnMovingPoint() != Vector3.zero)
        {
            transform.position = gmm.returnMovingPoint();
            gmm.setMovingPoint(Vector3.zero);
        }
        ispause = true;
        for (float i = 1; i >= 0; i -= 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        fade.gameObject.SetActive(false);
        ispause = false;
    }

    // Start is called before the first frame update
    void Start()
    {

        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        inven = GameObject.Find("GameManager").GetComponent<InventorySystem>();
        inven.pc = this;

        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        if (ps.savedPos != Vector3.zero)
        {
            transform.position = ps.savedPos;
            ps.savedPos = Vector3.zero;
        }
        ps.pc = this;
        skill1code = ps.getskill1();
        skill2code = ps.getskill2();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && hitcool && collision.GetComponent<Enemy_Attack_Damage>() != null) {
            hitcool = false;
            canMove = false;
            if (ps.getHP() > collision.GetComponent<Enemy_Attack_Damage>().damage)
                ps.damage(collision.GetComponent<Enemy_Attack_Damage>().damage);
            else
                ps.damage(ps.getHP());
            if (sr.flipX)
                rigid.velocity = new Vector3(1, 2);
            else
                rigid.velocity = new Vector3(-1, 2);
            StartCoroutine("invincibilityTime");
        }

        if (collision.tag == "Water")
        {
            isinWater = true;
            rigid.mass = 1.5f;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Water")
        {
            isinWater = false;
            rigid.mass = 1f;
        }
    }


    IEnumerator invincibilityTime() {
        sr.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        if (ps.getHP() > 0)
        {
            canMove = true;
        }
        else {
            die = true;
            StartCoroutine("dieEvent");
        }
        yield return new WaitForSeconds(0.25f);
        if (!die)
        {
            sr.color = new Color(1, 1, 1, 1);
            hitcool = true;
        }
    }

    IEnumerator dieEvent() {
        sr.color = new Color(1, 1, 1, 1);
        rigid.velocity = new Vector3(0, 0);
        motion.SetTrigger("die");
        yield return new WaitForSeconds(3f);
        fade.gameObject.SetActive(true);
        for (float i = 0; i < 1; i += 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }

    private void FixedUpdate()
    {
        float h;
        if (ispause || isotherInter)
        {
            h = 0;
            rigid.velocity = new Vector3(h * speed, rigid.velocity.y);
        }
        else if (canMove && !die)
        {

            h = Input.GetAxisRaw("Horizontal");
            if (isonground)
            {
                motion.SetBool("OnGround", true);
            }
            else
            {
                motion.SetBool("OnGround", false);
            }

            if (h != 0)
            {
                motion.SetTrigger("walk");
                if (h < 0)
                    sr.flipX = true;
                else
                    sr.flipX = false;
            }
            else
            {
                motion.SetTrigger("idle");
            }

            rigid.velocity = (new Vector3(h * speed / rigid.mass, rigid.velocity.y));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.GetCan_Shot())
        {
            After_inter.SetActive(true);
        }
        else {
            After_inter.SetActive(false);
        }
        ps.setskills(skill1code, skill2code);
        mana_Bar.fillAmount = ps.getMana() * 0.1f;
        MaxMana_Bar.fillAmount = ps.getMaxMana() * 0.1f;

        hpbar.maxValue = ps.getMaxHP();
        hpbar.value = ps.getHP();
        hps.text = ps.getHP() + "";

        EXPbar.maxValue = ps.getMaxEXP(ps.getLevel());
        EXPbar.value = ps.getEXP();

        invenHpbar.maxValue = ps.getMaxHP();
        invenHpbar.value = ps.getHP();
        invenHps.text = ps.getHP() + "";

        money_tx.text = "돈 : " + ps.GetMoney();
        inven_money_tx.text = "돈 : " + ps.GetMoney();

        Inter_Eskill.sprite = skillIcons[skill2code];
        Inter_Wskill.sprite = skillIcons[skill1code];

        Eskill.sprite = skillIcons[skill2code];
        Wskill.sprite = skillIcons[skill1code];

        if (!ispause && !die)
        {
            if (!manafill && ps.getMaxMana() > ps.getMana())
            {
                StartCoroutine("fillMana");
            }

            if (Input.GetButton("Jump"))
            {
                jump();
            }

            if (Input.GetButtonDown("Fire") && shotcool && ps.GetCan_Shot())
            {
                shot();
            }

            dash();

            if (!isotherInter && Input.GetButtonDown("Inventory"))
            {
                onInventory();
            }

            if (Input.GetButtonDown("pause"))
            {
                onPause();
            }

            if (!isotherInter && Input.GetButtonDown("SkillSet") && ps.GetCan_Shot())
            {
                onSkillSet();
            }
            if (Input.GetButton("Skill1") || Input.GetButton("Skill2") && ps.GetCan_Shot())
            {
                UsingSkill();
            }

        }


    }

    IEnumerator fillMana() {
        manafill = true;
        yield return new WaitForSeconds(1f);
        if (Input.GetButton("Skill1") || Input.GetButton("Skill2"))
            yield return null;
        else
        {
            ps.setMana(ps.getMana() + 1);
        }
        manafill = false;
    }

    private void dash()
    {
        if (Input.GetButton("Dash"))
        {
            speed = 4f;
        }
        if (Input.GetButtonUp("Dash"))
        {
            speed = 2.5f;
        }
    }

    private void shot()
    {
        shotcool = false;
        GameObject seed = Instantiate(apple_seed, transform.position, transform.rotation);
        if (Input.GetButton("Vertical"))
        {
            seed.GetComponent<bulletTo>().angle = 90;
        }
        else {
            if (sr.flipX) { seed.GetComponent<bulletTo>().angle = 180; }
            else { seed.GetComponent<bulletTo>().angle = 0; }
        }
        StartCoroutine("shootingcooltime");
    }

    IEnumerator shootingcooltime() {
        yield return new WaitForSeconds(0.1f);
        shotcool = true;
    }

    private void jump()
    {
        if (isonground)
        {
            rigid.velocity = (new Vector3(rigid.velocity.x, jpower / rigid.mass));
        }

    }

    private void onInventory()
    {
        isinven = true;
        isotherInter = true;
        inventory_menu.SetActive(true);
        Player_interface.SetActive(false);
    }

    public void offInventory()
    {
        inventory_menu.SetActive(false);
        Player_interface.SetActive(true);
        isotherInter = false;
        isinven = false;
    }

    public void attackItem(int attackcode)
    {
        offInventory();
        Instantiate(attackItemObj[attackcode], transform.position, transform.rotation);
    }

    private void onPause()
    {
        motion.SetBool("OnGround", true);
        motion.SetTrigger("idle");
        isotherInter = true;
        Pause_interface.SetActive(true);
        Player_interface.SetActive(false);
    }

    public void offPause()
    {
        Pause_interface.SetActive(false);
        Player_interface.SetActive(true);
        isotherInter = false;
    }

    private void onSkillSet()
    {
        motion.SetBool("OnGround", true);
        motion.SetTrigger("idle");
        ispause = true;
        isSetskill = true;
        SkillSet_interface.SetActive(true);
        Player_interface.SetActive(false);
    }

    public void offSkillSet()
    {
        SkillSet_interface.SetActive(false);
        Player_interface.SetActive(true);
        isSetskill = false;
        ispause = false;
    }

    private void UsingSkill() {
        int i = 0;
        if (Input.GetButton("Skill1") && !cooling[skill1code] && skill1code != 0)
        {
            i = skill1code;
            StartCoroutine("skill1Cooltime");
        }

        else if (Input.GetButton("Skill2") && !cooling[skill2code] && skill2code != 0)
        {
            i = skill2code;
            StartCoroutine("skill2Cooltime");
        }
        StartCoroutine("SkillEff", i);
    }

    IEnumerator SkillEff(int i)
    { 
        switch (i)
        {
            case 0:
                break;
            case 1:
                if (ps.getMana() >= 1)
                {
                    shotObj(skill_obj[0], 0, true);
                    ps.setMana(ps.getMana() - 1);
                }
                break;
            case 2:
                if (ps.getMana() >= 1)
                {
                    GameObject seed = Instantiate(apple_seed, transform.position, transform.rotation);
                    seed.GetComponent<bulletTo>().angle = 0;
                    seed = Instantiate(apple_seed, transform.position, transform.rotation);
                    seed.GetComponent<bulletTo>().angle = 45;
                    seed = Instantiate(apple_seed, transform.position, transform.rotation);
                    seed.GetComponent<bulletTo>().angle = 90;
                    seed = Instantiate(apple_seed, transform.position, transform.rotation);
                    seed.GetComponent<bulletTo>().angle = 135;
                    seed = Instantiate(apple_seed, transform.position, transform.rotation);
                    seed.GetComponent<bulletTo>().angle = 180;
                    seed = Instantiate(apple_seed, transform.position, transform.rotation);
                    seed.GetComponent<bulletTo>().angle = 225;
                    seed = Instantiate(apple_seed, transform.position, transform.rotation);
                    seed.GetComponent<bulletTo>().angle = 270;
                    seed = Instantiate(apple_seed, transform.position, transform.rotation);
                    seed.GetComponent<bulletTo>().angle = 315;
                    ps.setMana(ps.getMana() - 1);
                }
                break;
            case 3:
                if (ps.getMana() >= 2)
                {
                    shotObj(skill_obj[2], 0.16f, false);
                    ps.setMana(ps.getMana() - 2);
                }
                break;           
            case 4:
                if (ps.getMana() >= 2)
                {
                    canMove = false;
                    ps.setMana(ps.getMana() - 2);

                    yield return new WaitForSeconds(2f);
                    ps.heal((int)(ps.getMaxHP() * 0.1f));
                    canMove = true;
                }
                break;
            case 5:
                if (ps.getMana() >= 2)
                {
                    shotObj(skill_obj[4], 0, false);
                    ps.setMana(ps.getMana() - 2);
                }
                break;
            case 6:
                if (ps.getMana() >= 3)
                {                    
                    ps.setMana(ps.getMana() - 3);
                    Instantiate(skill_obj[5], new Vector3(transform.position.x, transform.position.y + 0.64f, transform.position.z), transform.rotation);
                    Instantiate(skill_obj[5], new Vector3(transform.position.x + 0.64f, transform.position.y, transform.position.z), transform.rotation);
                    Instantiate(skill_obj[5], new Vector3(transform.position.x - 0.64f, transform.position.y, transform.position.z), transform.rotation);
                    Instantiate(skill_obj[5], new Vector3(transform.position.x, transform.position.y - 0.64f, transform.position.z), transform.rotation);
                    Instantiate(skill_obj[5], new Vector3(transform.position.x + 0.32f, transform.position.y + 0.32f, transform.position.z), transform.rotation);
                    Instantiate(skill_obj[5], new Vector3(transform.position.x - 0.32f, transform.position.y - 0.32f, transform.position.z), transform.rotation);
                    Instantiate(skill_obj[5], new Vector3(transform.position.x - 0.32f, transform.position.y + 0.32f, transform.position.z), transform.rotation);
                    Instantiate(skill_obj[5], new Vector3(transform.position.x + 0.32f, transform.position.y - 0.32f, transform.position.z), transform.rotation);
                }
                break;
            case 7:
                if (ps.getMana() >= 3)
                {
                    transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    ps.setMana(ps.getMana() - 3);
                    yield return new WaitForSeconds(3f);
                    transform.localScale = new Vector3(1, 1, 1);
                    
                }
                break;
            case 8:
                if (ps.getMana() >= 3)
                {
                    rigid.velocity = (new Vector3(rigid.velocity.x, jpower * 1.5f));
                    ps.setMana(ps.getMana() - 3);
                    yield return new WaitForSeconds(0.1f);
                    for (int k = 0; k < 6; k++) {
                        GameObject seed = Instantiate(apple_seed, transform.position, transform.rotation);
                        seed.GetComponent<bulletTo>().angle = 0;
                        seed = Instantiate(apple_seed, transform.position, transform.rotation);
                        seed.GetComponent<bulletTo>().angle = 180;
                        yield return new WaitForSeconds(0.1f);
                    }
                    transform.localScale = new Vector3(1, 1, 1);

                }
                break;
            case 9:
                if (ps.getMana() >= 2)
                {
                    if (sr.flipX) { Instantiate(skill_obj[8], new Vector3(transform.position.x - 0.64f, transform.position.y, transform.position.z), transform.rotation); }
                    else { Instantiate(skill_obj[8], new Vector3(transform.position.x + 0.64f, transform.position.y, transform.position.z), transform.rotation); }
                    ps.setMana(ps.getMana() - 2);
                }
                break;
            case 10:
                if (ps.getMana() >= 3)
                {
                    canMove = false;
                    if (sr.flipX)
                        rigid.velocity = new Vector3(4, 4);
                    else
                        rigid.velocity = new Vector3(-4, 4);
                    ps.setMana(ps.getMana() - 3);
                    yield return new WaitForSeconds(0.1f);
                    for (int k = 0; k < 4; k++)
                    {
                        GameObject seed = Instantiate(apple_seed, transform.position, transform.rotation);
                        seed.GetComponent<bulletTo>().angle = 90;
                        seed = Instantiate(apple_seed, transform.position, transform.rotation);
                        seed.GetComponent<bulletTo>().angle = 45;
                        seed = Instantiate(apple_seed, transform.position, transform.rotation);
                        seed.GetComponent<bulletTo>().angle = 135;
                        yield return new WaitForSeconds(0.125f);
                    }
                    transform.localScale = new Vector3(1, 1, 1);
                    canMove = true;
                }
                break;

            case 17:
                if (ps.getMana() >= 3)
                {
                    Instantiate(skill_obj[16], new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f), transform.rotation);                 
                    ps.setMana(ps.getMana() - 3);
                }
                break;

            case 20:
                if (ps.getMana() >= 9)
                {
                    Instantiate(skill_obj[19], transform.position, transform.rotation);
                    ps.setMana(ps.getMana() - 0);
                }
                break;
            default:
                yield break;

        }
    }

    private void shotObj(GameObject go, float upper, bool canShotUp) {
        GameObject seed = Instantiate(go, new Vector3(transform.position.x, transform.position.y + upper, transform.position.z), transform.rotation);
        if (Input.GetButton("Vertical") && canShotUp)
        {
            seed.GetComponent<bulletTo>().angle = 90;
        }
        else
        {
            if (sr.flipX) { seed.GetComponent<bulletTo>().angle = 180; }
            else { seed.GetComponent<bulletTo>().angle = 0; }
        }
    }

    IEnumerator skill1Cooltime()
    {
        int skill1code_ = skill1code;
        cooling[skill1code_] = true;
        yield return new WaitForSeconds(cooltime[skill1code]);
        cooling[skill1code_] = false;
    }
    IEnumerator skill2Cooltime()
    {
        int skill2code_ = skill2code;
        cooling[skill2code_] = true;
        yield return new WaitForSeconds(cooltime[skill2code]);
        cooling[skill2code_] = false;
    }
}
