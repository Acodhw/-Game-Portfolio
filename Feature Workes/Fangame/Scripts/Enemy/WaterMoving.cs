using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMoving : MonoBehaviour
{
    public EnemyState es;
    Rigidbody2D rigid;
    Animator anim;
    public int moveSetX;
    public int moveSetY;
    public float speed;
    public float thinkingTime;
    public float ThinkRange;
    public float findRange;
    public SpriteRenderer spr;
    private Transform player;

    bool isThinked;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        Invoke("Think", thinkingTime + ThinkRange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (es.hp < 0) {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        else {
            if (Vector2.Distance(player.position, transform.position) <= findRange && player.GetComponent<PlayerControl>().isinWater)
            {
                Vector2 v = player.position - transform.position;
                rigid.velocity = v.normalized * speed * 2f;
                
            }
            else
            {
                rigid.velocity = new Vector2(moveSetX, moveSetY).normalized * speed;

                Debug.DrawRay(new Vector2((transform.position.x + moveSetX * 0.1f), transform.position.y), Vector3.down * 0.2f, new Color(1, 0, 0));
                RaycastHit2D rayhit = Physics2D.Raycast(new Vector2((transform.position.x + moveSetX * 0.1f), transform.position.y), Vector3.down, 0.2f, LayerMask.GetMask("Ground"));
                if (rayhit.collider == null)
                {
                    moveSetY = -1;
                    spr.flipX = moveSetX == 1;
                    CancelInvoke();
                    Invoke("Think", thinkingTime);
                }
                else
                {
                    if (rayhit.collider.tag != "Water" && isThinked)
                    {
                        moveSetX *= -1;
                        moveSetY *= -1;
                        spr.flipX = moveSetX == 1;
                        CancelInvoke();
                        isThinked = false;
                        Invoke("Think", thinkingTime);
                    }
                }
            }
        }
    }

    void Think()
    {
        isThinked = true;
        moveSetX = Random.Range(-1, 2);
        moveSetY = Random.Range(-1, 2);

        float nextThink = Random.Range(thinkingTime, thinkingTime + ThinkRange);
        if (es.hp > 0)
        {
            if (Vector2.Distance(player.position, transform.position) > findRange)
                spr.flipX = moveSetX == 1;
            else
                spr.flipX = player.position.x > transform.position.x;
        }
        Invoke("Think", nextThink);
    }
}
