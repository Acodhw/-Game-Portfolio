using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotsEvent : MonoBehaviour
{
    Rigidbody2D rb;

    public float liveTime = 1;
    public Vector2 shotDirection;
    public float shotSpeed;
    public bool canPenetration = false;
    public bool ShotofEnemy = false;

    void Awake() 
    { 
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {     
        StartCoroutine("RemoveOBJ");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canPenetration) {
            if (collision.gameObject.layer == 3 || (collision.tag == "Enemy" && !ShotofEnemy)) Destroy(gameObject);
         }
    }

    public void ShotObj() {
        rb.linearVelocity = shotDirection.normalized * shotSpeed;
    }

    IEnumerator RemoveOBJ() {
        yield return new WaitForSeconds(liveTime);
        Destroy(gameObject);
    }
}
