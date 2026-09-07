using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyControll : MonoBehaviour
{
    public int HP = 10;
    public int HPitemFallProbability;
    public int MPitemFallProbability;
    public GameObject HPItem;
    public GameObject MPItem;
    public bool getDamage;

    public Material HitMaterial;
    public SpriteRenderer sprite;
    public List<Elements> tolerance;

    public Color originalSpriteColor;
    private Material originMatarial;
    private float hitFlashTime;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttack")
        {
            if (!tolerance.Contains(collision.GetComponent<PlayerAttack>().attackElemental))
            {
                HP -= collision.GetComponent<PlayerAttack>().Damage;
                if (HP > 0)
                {
                    getDamage = true;
                    hitFlashTime = 0.3f;
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        originalSpriteColor = sprite.color;
        originMatarial = sprite.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitFlashTime > 0)
        {
            sprite.color = Color.white;
            hitFlashTime -= Time.deltaTime;
            sprite.material = HitMaterial;
        }
        else if (hitFlashTime < 0) hitFlashTime = 0;
        else
        {
            sprite.color = originalSpriteColor;
            sprite.material = originMatarial;
        }
    }
}
