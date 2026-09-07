using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_Moving_enemy : MonoBehaviour
{
    public EnemyState es;
    Rigidbody2D rigid;
    Animator anim;
    public int moveSet;
    public float speed;
    public float thinkingTime;
    public float ThinkRange;
    public float checking_ray_length = 0.2f;
    public SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("Think", thinkingTime + ThinkRange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(es.hp > 0)
            rigid.velocity = new Vector2(moveSet * speed, rigid.velocity.y);
        else
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        Debug.DrawRay(new Vector2((transform.position.x + moveSet * 0.1f), transform.position.y), Vector3.down * checking_ray_length, new Color(1, 0, 0));
        RaycastHit2D rayhit = Physics2D.Raycast(new Vector2((transform.position.x + moveSet * 0.1f), transform.position.y), Vector3.down, checking_ray_length, LayerMask.GetMask("Ground"));
        
        if (rayhit.collider == null)
        {
            moveSet *= -1;
            spr.flipX = moveSet == 1;
            CancelInvoke();
            Invoke("Think", thinkingTime);
        }
    }

    void Think()
    {
        moveSet = Random.Range(-1, 2);

        float nextThink = Random.Range(thinkingTime, thinkingTime + ThinkRange);
        if (es.hp > 0) {
            spr.flipX = moveSet == 1;
        }
        Invoke("Think", nextThink);
    }
}
