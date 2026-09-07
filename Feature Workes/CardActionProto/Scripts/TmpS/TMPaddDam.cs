using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMPaddDam : MonoBehaviour
{    
    PlayerGetDamage pgd;

    // Start is called before the first frame update
    void Start()
    {
        pgd = GetComponent<PlayerGetDamage>();
        DamageInfo di = pgd.GetDamage();
        di.damageChangeFunc.Add((s, dam) =>
        {
            if (s.GetState("HP") < s.GetState("MaxHP") * 0.5f)
            {
                return (uint)(dam * 1.5f);
            }
            return dam;
        });
        pgd.SetDamageInfo(di);
    }
}
