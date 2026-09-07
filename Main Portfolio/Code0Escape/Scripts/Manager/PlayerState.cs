using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField]
    private int maxHP = 100;
    private int HP;

    private int nowslot = 1;

    private int hpitem = 0;
    private int buffitem = 0;
    [SerializeField] private int money = 0;

    // Start is called before the first frame update
    void Awake()
    {
        StateReset();
    }

    public void StateReset()
    {
        HP = maxHP;
        hpitem = 0;
        buffitem = 0;
    }

    public void Damage(int d)
    {
        HP = (HP - d > 0) ? (HP - d) : 0;
    }

    public void Heal(int h)
    {
        HP = (HP + h < maxHP) ? (HP + h) : maxHP;
    }
    

    public void SetNowslot(int n)
    {
        nowslot = n;
    }

    public void SetMoney(int m)
    {
        money = m;
    }

    public void SetHpitem(int h)
    {
        hpitem = h;
    }

    public void SetBuffitem(int b)
    {
        buffitem = b;

    }

    public int GetHP()
    {
        return HP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public int GetNowslot()
    {
        return nowslot;
    }

    public int GetMoney()
    {
        return money;
    }

    public int GetHpitem()
    {
        return hpitem;
    }

    public int GetBuffitem()
    {
        return buffitem;

    }
}
