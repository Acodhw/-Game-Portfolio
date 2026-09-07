using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFrozenSummonIce : MonoBehaviour
{
    private DamageInfo damag;
    [SerializeField]
    private GameObject lastIce;

    private void OnDestroy()
    {
        GameObject g = Instantiate(lastIce, transform.position + Vector3.up * 7.5f, transform.rotation);
        g.GetComponent<Rigidbody2D>().velocity = Vector2.up * 10f;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(damag);
    }

    public void SetDamageInfo(DamageInfo damag) {
        this.damag = damag;
    }
}
