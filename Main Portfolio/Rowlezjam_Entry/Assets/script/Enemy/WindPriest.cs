using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindPriest : MonoBehaviour
{
    public GameObject[] ShotOBJs;
    public Transform[] movingPoints;
    public Animator EnemyAnimator;
    public BossControl enemyControll;
    public AudioClip MoveSound;
    public AudioClip AttackSound;
    public AudioClip HitSound;
    public AudioClip DieSound;
    public BossTalkFirst btf;

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
        Invoke("RandomMove", 2f);
        bm = GameObject.Find("GameManager").GetComponent<BGMManager>();
    }

    void RandomMove() {
        if (!dying && btf.finished)
        {
            audioSource.PlayOneShot(MoveSound);
            Instantiate(ShotOBJs[0], transform.position, transform.rotation);
            transform.position = movingPoints[Random.Range(0, 5)].position;          
        }
        Invoke("RandomMove", 2f);
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
                pattenDid = true;
                switch (Random.Range(0, 3))
                {
                    case 0:
                        corutine = roundShot();
                        break;
                    case 1:
                        corutine = SummonWindMonster();
                        break;
                    case 2:
                        corutine = RoundShot2();
                        break;
                }
                StartCoroutine(corutine);
            }
        }
    }
    IEnumerator SummonWindMonster()
    {
        yield return new WaitForSeconds(0.5f);
        EnemyAnimator.SetTrigger("Attack");
        audioSource.PlayOneShot(AttackSound);
        summonedObj.Add(Instantiate(ShotOBJs[2], transform.position + Vector3.right * 3, transform.rotation));
        yield return new WaitForSeconds(0.3f);
        summonedObj.Add(Instantiate(ShotOBJs[2], transform.position + Vector3.left * 3, transform.rotation));
        yield return new WaitForSeconds(3.5f);
        pattenDid = false;
    }
    IEnumerator roundShot()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 45; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    se.shotDirection = new Vector2(Mathf.Cos((120 * j + k * 8 + i * 2f) * Mathf.Deg2Rad), Mathf.Sin((120 * j + k * 8 + i * 2f) * Mathf.Deg2Rad));
                    se.shotSpeed = 12;
                    se.ShotObj();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(3f);
        pattenDid = false;
    }
    IEnumerator RoundShot2()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 90; i++)
        {

            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((i * 5) * Mathf.Deg2Rad), Mathf.Sin((i * 5) * Mathf.Deg2Rad));
            se.shotSpeed = 12;
            se.ShotObj();
            se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((-i * 5) * Mathf.Deg2Rad), Mathf.Sin((-i * 5) * Mathf.Deg2Rad));
            se.shotSpeed = 12;
            se.ShotObj();

            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(3.5f);
        pattenDid = false;
    }

    IEnumerator Die()
    {
        Instantiate(ShotOBJs[0], transform.position, new Quaternion(0, 0, 0, 0));
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
