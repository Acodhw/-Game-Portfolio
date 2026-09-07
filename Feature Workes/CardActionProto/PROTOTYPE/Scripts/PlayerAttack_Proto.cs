using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType_Proto
{
    Physics,
    Magic,
    fix,
};
public class PlayerAttack_Proto : MonoBehaviour
{
    public bool isNormalAttack;
    public bool isMoveSkills;
    public PlayerState_Proto pstate;
    public PlayerControl_Proto pc;

    public DamageType_Proto dt;
    public float Physics_Mult; //공격력 비례
    public float Magic_Mult; //지력 비례
    public float HP_Mult; //체력 비례
    public float Damage_Plus;
    public float realDamage;
    public float MaxHP_pro_Damage; //적 최대체력 비례
    public float HP_pro_Damage; //적 현재체력 비례    

    public int criticalPoint; //크리티컬 확률
    public float pire_Point; //fix 데미지가 아닌 모든 데미지에 대해 방어/마법저항 관통 포인트

    public int ccCode; // -1은 cc없음
    public float ccPower;
    public float ccTime;

    bool stackup = false;
    // Start is called before the first frame update
    void Awake()
    {
        pc = GameObject.Find("mainChar_proto").GetComponent<PlayerControl_Proto>();
        pstate = GameObject.Find("GameManager").GetComponent<PlayerState_Proto>();
        realDamage = (pstate.getPower() * Physics_Mult) + (pstate.getIntellect() * Magic_Mult) + (pstate.getMaxHP() * HP_Mult) + Damage_Plus;        
    }

    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        if (isNormalAttack)
        {
            criticalPoint = pstate.criticalPoint;
            switch (pstate.getCharactorKey())
            {
                case 0:
                    if (pc.attackStack < 2)
                    {
                        ccCode = 5;
                        ccPower = 5;
                        Physics_Mult = 1;
                    }
                    else
                    {
                        ccCode = 5;
                        ccPower = 10;
                        Physics_Mult = 2;
                    }
                    break;

                case 6:
                    if (pc.PunchPassiveTime > 0)
                    {
                        Physics_Mult = 1.25f;
                    }
                    else
                    {
                        Physics_Mult = 0.5f;
                    }
                    break;
            }
        }
        if (!isMoveSkills)
        {
            switch (pstate.getCharactorKey())
            {
                case 3:
                    if (stackup)
                    {
                        stackup = false;
                        pc.spearStackUp();
                    }
                    break;
                case 6:
                    if (stackup)
                    {
                        stackup = false;
                        pc.PunchStackUp();
                    }
                    break;
            }
        }
        realDamage = (pstate.getPower() * Physics_Mult) + (pstate.getIntellect() * Magic_Mult) + (pstate.getMaxHP() * HP_Mult) + Damage_Plus;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponent<EnemyState_Proto>() != null)
        {
            switch (pstate.getCharactorKey())
            {
                case 3:
                    stackup = true;
                    break;

                case 6:
                    stackup = true;
                    break;

            }
        }
    }
}
