using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    public GameObject[] ShotOBJs;
    public Transform[] movingPoints;
    public BossControl enemyControll;
    public AudioClip MoveSound;
    public AudioClip AttackSound;
    public AudioClip HitSound;
    public AudioClip DieSound;
    public BossTalkFirst btf;
    public Color[] eleColors;

    private List<GameObject> summonedObj;

    private AudioSource audioSource;
    private IEnumerator corutine;

    private int facingDirection = -1;

    private bool pattenDid;
    bool dying = false;

    private Vector2 newVelocity;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Transform player;
    BGMManager bm;

    // Start is called before the first frame update
    private void Start()
    {
        summonedObj = new List<GameObject>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("MainCharacter").transform;
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        bm = GameObject.Find("GameManager").GetComponent<BGMManager>();
        Invoke("RandomMove", 2f);
    }

    void RandomMove()
    {
        if (!dying && btf.finished)
        {
            audioSource.PlayOneShot(MoveSound);
            transform.position = movingPoints[Random.Range(0, 5)].position;
        }
        Invoke("RandomMove", 3.75f);
    }


    private void Update()
    {
        if (enemyControll.getDamage)
        {
            enemyControll.getDamage = false;
            audioSource.PlayOneShot(HitSound);
        }

        if (enemyControll.HP <= 0 && !dying) StartCoroutine("Die");
        if (!dying && btf.finished)
        {
            bm.StartBGM(7);
            if (player.position.x - transform.position.x > 0 && facingDirection == -1)
            {
                Flip();
            }
            else if (player.position.x - transform.position.x < 0 && facingDirection == 1)
            {
                Flip();
            }
            if (!pattenDid)
            {
                for (int i = 0; i < 2; i++)
                {
                    rb = Instantiate(enemyControll.HPItem, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
                    rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 4f)), ForceMode2D.Impulse);
                }
                for (int i = 0; i < 2; i++)
                {
                    rb = Instantiate(enemyControll.MPItem, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
                    rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(4f, 4f)), ForceMode2D.Impulse);
                }
                pattenDid = true;
                switch (Random.Range(0, 4))
                {
                    case 0:
                        corutine = FireAttack();
                        break;
                    case 1:
                        corutine = WaterAttack();
                        break;
                    case 2:
                        corutine = EarthAttack();
                        break;
                    case 3:
                        corutine = WindAttack();
                        break;
                }
                StartCoroutine(corutine);
            }
        }
    }

    IEnumerator FireAttack()
    {
        yield return new WaitForSeconds(0.5f);
        enemyControll.tolerance[1] = Elements.Fire;
        enemyControll.originalSpriteColor = eleColors[0];
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 90; i++)
        {
            Vector3 to = player.transform.position - transform.position;
            float deg = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;
            ShotsEvent se = Instantiate(ShotOBJs[0], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = to;
            se.ShotObj();
            se = Instantiate(ShotOBJs[0], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((deg + 45) * Mathf.Deg2Rad), Mathf.Sin((deg + 45) * Mathf.Deg2Rad));
            se.ShotObj();
            se = Instantiate(ShotOBJs[0], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((deg - 45) * Mathf.Deg2Rad), Mathf.Sin((deg - 45) * Mathf.Deg2Rad));
            se.ShotObj();
            se = Instantiate(ShotOBJs[0], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((deg + 90) * Mathf.Deg2Rad), Mathf.Sin((deg + 45) * Mathf.Deg2Rad));
            se.ShotObj();
            se = Instantiate(ShotOBJs[0], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((deg - 90) * Mathf.Deg2Rad), Mathf.Sin((deg - 45) * Mathf.Deg2Rad));
            yield return new WaitForSeconds(0.75f);
        }
        yield return new WaitForSeconds(5f);
        pattenDid = false;
    }
    IEnumerator WaterAttack()
    {
        yield return new WaitForSeconds(0.5f);
        enemyControll.tolerance[1] = Elements.Water;
        enemyControll.originalSpriteColor = eleColors[1];
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 150; i++)
        {
            audioSource.PlayOneShot(AttackSound);
            for (int j = 0; j < 8; j++)
            {
                ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                se.shotDirection = new Vector2(Mathf.Cos((j * 45 + i * 15) * Mathf.Deg2Rad), Mathf.Sin((j * 45 + i * 15) * Mathf.Deg2Rad));
                se.shotSpeed = Random.Range(8f,15f);
                se.ShotObj();

            }
            yield return new WaitForSeconds(0.5f);

        }
        yield return new WaitForSeconds(5f);
        pattenDid = false;
    }
    IEnumerator EarthAttack()
    {
        yield return new WaitForSeconds(0.5f);
        enemyControll.tolerance[1] = Elements.Ground;
        enemyControll.originalSpriteColor = eleColors[2];
        yield return new WaitForSeconds(0.5f);
        for (int k = 0; k < 15; k++)
        {
            for (int i = 0; i < 10; i++)
            {
                audioSource.PlayOneShot(AttackSound);
                for (int j = 0; j < 8; j++)
                {
                    ShotsEvent se = Instantiate(ShotOBJs[2], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    se.shotDirection = new Vector2(Mathf.Cos((45 * j + i * 4.5f + k * 3) * Mathf.Deg2Rad), Mathf.Sin((45 * j + i * 4.5f + k * 3) * Mathf.Deg2Rad));
                    se.shotSpeed = 5 * 0.7f * i;
                    se.ShotObj();

                }
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(5f);
        pattenDid = false;
    }
    IEnumerator WindAttack()
    {
        yield return new WaitForSeconds(0.5f);
        enemyControll.tolerance[1] = Elements.Wind;
        enemyControll.originalSpriteColor = eleColors[3];
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 45; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    ShotsEvent se = Instantiate(ShotOBJs[3], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    se.shotDirection = new Vector2(Mathf.Cos((120 * j + k * 8 + i * 2f) * Mathf.Deg2Rad), Mathf.Sin((120 * j + k * 8 + i * 2f) * Mathf.Deg2Rad));
                    se.shotSpeed = 12;
                    se.ShotObj();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(5f);
        pattenDid = false;
    }



        IEnumerator Die(){
        audioSource.PlayOneShot(DieSound);
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        StopCoroutine(corutine);
        dying = true;
        for (int i = summonedObj.Count - 1; i >= 0; i--)
        {
            GameObject g = summonedObj[i];
            summonedObj.RemoveAt(i);
            Destroy(g);
        }
        yield return new WaitForSeconds(0.25f);
        Rigidbody2D rb;
        for (int i = 0; i < enemyControll.AmountHPitem; i++)
        {
            rb = Instantiate(enemyControll.HPItem, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 4f)), ForceMode2D.Impulse);
        }
        for (int i = 0; i < enemyControll.AmountMPitem; i++)
        {
            rb = Instantiate(enemyControll.MPItem, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(4f, 4f)), ForceMode2D.Impulse);
        }
        btf.nextScenego();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
}

