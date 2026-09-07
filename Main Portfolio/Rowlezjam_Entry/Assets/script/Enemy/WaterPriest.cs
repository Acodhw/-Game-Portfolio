using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPriest : MonoBehaviour
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
        bm = GameObject.Find("GameManager").GetComponent<BGMManager>();
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
                switch (Random.Range(0, 4))
                {
                    case 0:
                        corutine = MoveFast();
                        break;
                    case 1:
                        corutine = SummonWaterMonster();
                        break;
                    case 2:
                        corutine = roundShot();
                        break;
                    case 3:
                        corutine = ShotFallingWater();
                        break;
                }
                StartCoroutine(corutine);
            }
        }
    }

    IEnumerator MoveFast()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 5; i++) {
            audioSource.PlayOneShot(MoveSound);
            Instantiate(ShotOBJs[0], transform.position, transform.rotation);
            transform.position = movingPoints[i].position;
            yield return new WaitForSeconds(0.45f);
        }      
        yield return new WaitForSeconds(0.75f);
        for (int i = 3; i >= 0; i--)
        {
            audioSource.PlayOneShot(MoveSound);
            Instantiate(ShotOBJs[0], transform.position, transform.rotation);
            transform.position = movingPoints[i].position;
            yield return new WaitForSeconds(0.45f);
        }
        yield return new WaitForSeconds(0.5f);
        pattenDid = false;
    }
    IEnumerator SummonWaterMonster()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[4].position;
        yield return new WaitForSeconds(1f);
        EnemyAnimator.SetTrigger("Attack");
        audioSource.PlayOneShot(AttackSound);
        summonedObj.Add(Instantiate(ShotOBJs[2], transform.position + Vector3.right, transform.rotation));
        yield return new WaitForSeconds(0.3f);
        summonedObj.Add(Instantiate(ShotOBJs[2], transform.position + Vector3.left, transform.rotation));
        yield return new WaitForSeconds(0.3f);
        summonedObj.Add(Instantiate(ShotOBJs[2], transform.position + Vector3.right, transform.rotation));      
        yield return new WaitForSeconds(6f);
        pattenDid = false;
    }
    IEnumerator roundShot()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[1].position;
        EnemyAnimator.SetTrigger("Attack");
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i) * Mathf.Deg2Rad), Mathf.Sin((45 * i) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i + 15) * Mathf.Deg2Rad), Mathf.Sin((45 * i + 15) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i + 30) * Mathf.Deg2Rad), Mathf.Sin((45 * i + 30) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[2].position;
        EnemyAnimator.SetTrigger("Attack");
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i) * Mathf.Deg2Rad), Mathf.Sin((45 * i) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i - 15) * Mathf.Deg2Rad), Mathf.Sin((45 * i - 15) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i - 30) * Mathf.Deg2Rad), Mathf.Sin((45 * i - 30) * Mathf.Deg2Rad));
            se.ShotObj();
        }

        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[4].position;
        EnemyAnimator.SetTrigger("Attack");
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i) * Mathf.Deg2Rad), Mathf.Sin((45 * i) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i - 10) * Mathf.Deg2Rad), Mathf.Sin((45 * i - 10) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i - 20) * Mathf.Deg2Rad), Mathf.Sin((45 * i - 20) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i) * Mathf.Deg2Rad), Mathf.Sin((45 * i) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i - 10) * Mathf.Deg2Rad), Mathf.Sin((45 * i - 10) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i - 20) * Mathf.Deg2Rad), Mathf.Sin((45 * i - 20) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        yield return new WaitForSeconds(3.5f);
        pattenDid = false;
    }
    IEnumerator ShotFallingWater()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[4].position;
        yield return new WaitForSeconds(0.25f);
        EnemyAnimator.SetTrigger("Attack");      
        for (int i = 0; i < 8; i++)
        {
            audioSource.PlayOneShot(AttackSound);
            ShotsEvent se = Instantiate(ShotOBJs[3], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Random.Range(-20f, 20f), Random.Range(25f, 35f));
            se.shotSpeed = Random.Range(15, 35);
            se.ShotObj();
            se = Instantiate(ShotOBJs[3], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Random.Range(-20f, 20f), Random.Range(25f, 35f));
            se.shotSpeed = Random.Range(15, 35);
            se.ShotObj();
            se = Instantiate(ShotOBJs[3], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Random.Range(-20f, 20f), Random.Range(25f, 35f));
            se.shotSpeed = Random.Range(15, 35);
            se.ShotObj();
            yield return new WaitForSeconds(0.1f);
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
