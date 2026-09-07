using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageProto
{
    public int damage;
    public Vector3 damageVector;
    public Transform owner;
}
public class AttacksProto : MonoBehaviour
{
    [SerializeField] private DamageProto damage;

    public void SetDamage(DamageProto d) { damage = d; }
    public DamageProto GetDamage() { return damage; }
}
