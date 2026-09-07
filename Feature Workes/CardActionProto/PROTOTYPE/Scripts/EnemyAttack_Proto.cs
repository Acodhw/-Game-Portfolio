using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack_Proto : MonoBehaviour
{

    public DamageType_Proto dt;
    public float realDamage;
    public float MaxHP_pro_Damage; //적 최대체력 비례
    public float HP_pro_Damage; //적 현재체력 비례    

    public int CriticalPoint;   
    public float criticalDamageMult;
    public float pire_Point; //fix 데미지가 아닌 모든 데미지에 대해 방어/마법저항 관통 포인트

    public int ccCode; // -1은 cc없음
    public float ccPower;
    public float ccTime;
}
