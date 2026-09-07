using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipingShot : MonoBehaviour
{
    PlayerAttack_Proto pap;
    // Start is called before the first frame update
    void Start()
    {
        pap = GetComponent<PlayerAttack_Proto>();
        StartCoroutine("powerup");
    }

    IEnumerator powerup() 
    {
        for (float i = 0; i <= 6; i += 0.2f)
        {
            pap.Physics_Mult = i;
            yield return new WaitForSeconds(0.04f);
        }        
    }
}
