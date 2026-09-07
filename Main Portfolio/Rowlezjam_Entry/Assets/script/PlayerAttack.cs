using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum Elements {
    None,
    Fire,
    Water,
    Ground,
    Wind,
}
public class PlayerAttack : MonoBehaviour
{
    public int Damage = 0;
    public Elements attackElemental;
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
