using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Tooltip("플레이어에게 가할 데미지")]
    public int damage = 10;

    [Tooltip("이 공격에 맞았을 때 출력될 피격 이펙트 프리팹")]
    public GameObject hitEffectPrefab;
    public Vector3 AttackVector => transform.forward;
}