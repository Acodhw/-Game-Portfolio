using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stronger_Strong : MonoBehaviour
{
    private PlayerState ps;
    private Bullet_Damage bd;
    private SpriteRenderer plsr;
    private SpriteRenderer sr;
    private bulletTo bt;
    public Sprite[] sprite;
    // Start is called before the first frame update
    void Start()
    {
        bt = GetComponent<bulletTo>();
        bd = GetComponent<Bullet_Damage>();
        sr = GetComponent<SpriteRenderer>();
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        plsr = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>();
        sr.flipX = plsr.flipX;
        if (plsr.flipX)
            bt.angle = 180;
        bd.damage = (int)(ps.getLevel() / 1.5f);
        if (ps.getLevel() < 3)
            sr.sprite = sprite[0];
        else if (ps.getLevel() < 7)
            sr.sprite = sprite[1];
        else if (ps.getLevel() < 12)
            sr.sprite = sprite[2];
        else if (ps.getLevel() < 20)
            sr.sprite = sprite[3];
        else if (ps.getLevel() < 27)
            sr.sprite = sprite[4];
        else if (ps.getLevel() < 34)
            sr.sprite = sprite[5];
        else
            sr.sprite = sprite[6];
    }
}
