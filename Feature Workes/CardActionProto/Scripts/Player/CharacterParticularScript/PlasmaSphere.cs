using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Events;

public class PlasmaSphere : MonoBehaviour
{
    [SerializeField]
    private GameObject Plasma;
    [SerializeField]
    private float rePlasmaTime;

    private Transform playerTrans;
    private uint PlasmaDamage;
    private UnityEvent<States, uint> PlayerPassive;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnPlasma", rePlasmaTime, rePlasmaTime);
    }

    void SpawnPlasma() {
        GameObject g = Instantiate(Plasma, transform.position + Vector3.forward * 0.1f, transform.rotation);
        DamageInfo di = g.GetComponent<PlayerAttacks>().GetDamage();
        di.order = playerTrans;
        di.damage = PlasmaDamage;
        di.damageChangeEvent = PlayerPassive;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
    }

    public void setDamage(Transform player, uint damage, UnityEvent<States, uint> passive) {
        playerTrans = player;
        PlasmaDamage = damage;
        PlayerPassive = passive;
    }
}
