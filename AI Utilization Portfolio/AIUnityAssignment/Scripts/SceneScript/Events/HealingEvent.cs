using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingEvent : MonoBehaviour
{
    PlayerState ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
    }

    public void Healing()
    {
        ps.Heal(ps.maxHP);
    }
}
