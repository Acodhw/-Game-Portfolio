using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHitManager : MonoBehaviour
{
    private PlayerState_Proto pstate;
    public Slider HPBar;
    public Slider Barriorbar;
    public Slider MPBar;

    private bool barriorOn = false;
    // Start is called before the first frame update
    void Start()
    {
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
    }

    // Update is called once per frame
    void Update()
    {
        HPBar.maxValue = pstate.MaxHP;
        MPBar.maxValue = pstate.MaxMp;
        
        if (pstate.barrior > 0 && !barriorOn)
        {
            Barriorbar.maxValue = pstate.barrior;
            barriorOn = true;
        }
        else if (pstate.barrior <= 0)
        {
            barriorOn = false;
        }
        else if (pstate.barrior > 0 && barriorOn && pstate.barrior > Barriorbar.maxValue)
        {
            Barriorbar.maxValue = pstate.barrior;
        }
        Barriorbar.value = pstate.barrior;
        HPBar.value = pstate.getHP();
        MPBar.value = pstate.MP;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyAttack")
        {
            EnemyAttack_Proto pp;
            if (collision.GetComponent<EnemyAttack_Proto>() != null)
            {
                pp = collision.GetComponent<EnemyAttack_Proto>();


                if (pp.ccCode == 5)
                {
                    if (transform.position.x >= pp.transform.position.x)
                        pstate.ccPower = pp.ccPower;
                    else
                        pstate.ccPower = -pp.ccPower;
                }
                else
                {
                    pstate.ccPower = pp.ccPower;
                }
                pstate.ccTime = pp.ccTime;
                pstate.ccCode = pp.ccCode;
                if (pp.CriticalPoint >= Random.Range(1, 101))
                    pstate.Damage(pp.realDamage + pstate.getMaxHP() * (0.01f * pp.MaxHP_pro_Damage) + pstate.getHP() * (0.01f * pp.HP_pro_Damage), pp.dt, pp.pire_Point, 1.5f);
                else
                    pstate.Damage(pp.realDamage + pstate.getMaxHP() * (0.01f * pp.MaxHP_pro_Damage) + pstate.getHP() * (0.01f * pp.HP_pro_Damage), pp.dt, pp.pire_Point, 1);

            }
        }

    }
}
