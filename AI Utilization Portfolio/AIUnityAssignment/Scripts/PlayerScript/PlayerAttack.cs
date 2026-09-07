using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int Damage;
    public Vector3 AttackVector;

    [Tooltip("이 공격에 맞았을 때 출력될 피격 이펙트 프리팹")]
    public GameObject hitEffectPrefab;

    void Start()
    {

    }

    void Update()
    {

    }
}