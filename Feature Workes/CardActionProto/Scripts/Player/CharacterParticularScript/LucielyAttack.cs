using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class LucielyAttack : MonoBehaviour
{
    [SerializeField] private PlayerAttacks plAttack;
    [SerializeField] private GameObject boom;
    private void OnDestroy()
    {
        GameObject g = Instantiate(boom, transform.position, transform.rotation);
        g.transform.localScale = Vector3.one;
        g.GetComponent<PlayerAttacks>().SetDamageInfo(plAttack.GetDamage());
    }
}
