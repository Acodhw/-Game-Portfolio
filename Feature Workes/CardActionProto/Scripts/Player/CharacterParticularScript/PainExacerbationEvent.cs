using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainExacerbationEvent : MonoBehaviour
{
    [SerializeField] private GameObject bloodExacerbation;
    private PlayerManager playerManager;
    private Transform player;

    private void Start()
    {
        playerManager = GameObject.Find("GameManager").GetComponent<PlayerManager>();
    }

    public void SetTransform(Transform trans) {
        player = trans;
    }

    public void SummonObj(States state, int damage) {
        if (state.GetState("HP") < state.GetState("MaxHP") * 0.5f)
        {
            GameObject g = Instantiate(bloodExacerbation, transform.position, transform.rotation);
            DamageInfo di = g.GetComponent<PlayerAttacks>().GetDamage();
            di.order = player;
            di.damage = (uint)Mathf.RoundToInt((playerManager.GetEditedState("Intellect") * 0.75f + playerManager.GetEditedState("Power") * 0.75f) * playerManager.GetEditedState("SkillPercent") * 0.01f);
            g.GetComponent<PlayerAttacks>().SetDamageInfo(di);
        }
    }
}
