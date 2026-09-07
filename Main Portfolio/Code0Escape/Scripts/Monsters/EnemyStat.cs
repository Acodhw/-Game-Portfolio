using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStat : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    [SerializeField] private int GiveMoney = 50;
    [SerializeField] private GameObject Particle;
    [SerializeField] private Image HPBar;
    [SerializeField] private bool isNeedCode = false;
    private int MaxHP;
    private PlayerState state;

    // Start is called before the first frame update
    void Start()
    {
        state = GameObject.Find("GameManager").GetComponent<PlayerState>();
        MaxHP = HP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Bullet"))
        {
            int damage = other.GetComponent<CodeBullet>().GetDamage();
            if (isNeedCode)
            {
                if (other.GetComponent<CodeBullet>().GetVaccineCode()) HP = (HP - damage > 0) ? HP - damage : 0;
            }
            else HP = (HP - damage > 0) ? HP - damage : 0;
        }
        else if (other.tag.Equals("Boom"))
        {
            int damage = other.GetComponent<Boom>().damage;
            if (isNeedCode)
            {
                if(other.GetComponent<Boom>().vaccine) HP = (HP - damage > 0) ? HP - damage : 0;
            }
            else HP = (HP - damage > 0) ? HP - damage : 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HPBar.fillAmount = (float)HP / (float)MaxHP;
        if (HP <= 0)
        {
            state.SetMoney(state.GetMoney() + GiveMoney);
            Instantiate(Particle,transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
