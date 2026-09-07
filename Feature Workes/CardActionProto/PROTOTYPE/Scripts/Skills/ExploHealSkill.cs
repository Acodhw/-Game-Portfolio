using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploHealSkill : MonoBehaviour
{

     PlayerState_Proto pstate;

    public float Physics_Mult; //°ø°Ý·Â ºñ·Ê
    public float Magic_Mult; //Áö·Â ºñ·Ê
    public float HP_Mult; //Ã¼·Â ºñ·Ê
    public float Damage_Plus;
    float realHeal;

    // Start is called before the first frame update
    void Awake()
    {
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        realHeal = (pstate.getPower() * Physics_Mult) + (pstate.getIntellect() * Magic_Mult) + (pstate.getMaxHP() * HP_Mult) + Damage_Plus;
    }

    private void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        realHeal = (pstate.getPower() * Physics_Mult) + (pstate.getIntellect() * Magic_Mult) + (pstate.getMaxHP() * HP_Mult) + Damage_Plus;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            pstate.Heal(realHeal);
        }
    }
}
