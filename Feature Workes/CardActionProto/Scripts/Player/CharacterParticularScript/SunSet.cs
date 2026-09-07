using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SunSet : MonoBehaviour
{
    [SerializeField]
    private GameObject nextObj;

    private PlayerManager playerManager;
    private uint nextDamage;
    private UnityEvent passiveEvent;
    private UnityEvent passiveBoom;
    private UnityEvent<int, float> Change;
    private float cool;

    ConditionCheckwithString[] conditioncheck = new ConditionCheckwithString[1];
    Func<States, uint, uint> ultimatePlus = (s, dam) =>
    {
        if (s.isinConditionList("_StigmaoftheSun"))
        {
            return (uint)(dam * 2.1f);
        }
        return dam;
    };

    private Transform enemy;
    private Transform player;

    private bool Ultimate = false;
    private bool alreadyDoing = false;
    private bool hitCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
        ConditionCheckwithString tmp;
        tmp.con = "_StigmaoftheSun";
        tmp.point = 1;
        tmp.time = 2;
        conditioncheck[0] = tmp;
        StartCoroutine("NextSkill");
    }

    public void Setting(uint nextDamage, UnityEvent nextEvent, UnityEvent passive, UnityEvent<int, float> c, float cooltime, Transform player, bool ult)
    {
        this.nextDamage = nextDamage;
        passiveEvent = nextEvent;
        passiveBoom = passive;
        Change = c;
        cool = cooltime;
        this.player = player;
        Ultimate = ult;
    }

    private void Update()
    {
        transform.right = GetComponent<Rigidbody2D>().velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hitCheck && collision.tag.Equals("Enemy")) {
            hitCheck = true;
            enemy = collision.transform;
            StartCoroutine("Teleport");
        }
    }

    IEnumerator NextSkill()
    {
        yield return new WaitForSeconds(1.75f);
        if(!alreadyDoing && hitCheck) Change.Invoke(9, cool);
        if (!hitCheck)
        {
            playerManager.UsingMP(-30);
            Change.Invoke(9, 0);
        }
        Destroy(gameObject);
    }

    IEnumerator Teleport()
    {
        float time = 0;
        while (time < 0.06f)
        {
            if (Input.GetButtonDown("Skill" + (playerManager.GetSettingSkillCode(3, 0) == 9 ? 1 : (playerManager.GetSettingSkillCode(3, 1) == 9 ? 2 : 3))))
            {
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }

        if (time < 0.06f)
        {
            alreadyDoing = true;
            player.transform.position = (Vector3)((Vector2)enemy.transform.position) + player.transform.position.z * Vector3.forward;
            passiveBoom.Invoke();
            GameObject g = Instantiate(nextObj, player.transform.position, player.transform.rotation);
            g.transform.localScale = new Vector3(GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1, 1);
            g.GetComponent<PlayerAttacks>().SetHitEvent(passiveEvent);
            DamageInfo di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = transform;
            di.damage = nextDamage;
            di.damageChangeFunc.Add(ultimatePlus);
            di.conditionCheck = Ultimate ? conditioncheck : null;
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
            Change.Invoke(9, cool);
        }
    }
}