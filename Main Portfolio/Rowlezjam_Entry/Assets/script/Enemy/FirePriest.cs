using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class FirePriest : MonoBehaviour
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
        summonedObj = new List<GameObject> ();
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
                        corutine = Move4points();
                        break;
                    case 1:
                        corutine = batSummon();
                        break;
                    case 2:
                        corutine = roundShot();
                        break;
                    case 3:
                        corutine = Shot3Bulltets();
                        break;
                }
                StartCoroutine(corutine);
            }
        }
    }

    IEnumerator Move4points()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[0].position;
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[1].position;
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[2].position;
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[3].position;
        yield return new WaitForSeconds(2.5f);
        pattenDid = false;
    }
    IEnumerator batSummon()
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
        yield return new WaitForSeconds(4f);
        pattenDid = false;
    }

    IEnumerator roundShot() {
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[0].position;
        EnemyAnimator.SetTrigger("Attack");
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i <  8; i++) {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i) * Mathf.Deg2Rad), Mathf.Sin((45 * i) * Mathf.Deg2Rad));
            se.ShotObj();
        }
        
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[3].position;
        EnemyAnimator.SetTrigger("Attack");
        audioSource.PlayOneShot(AttackSound);
        for (int i = 0; i < 8; i++)
        {
            ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((45 * i) * Mathf.Deg2Rad), Mathf.Sin((45 * i) * Mathf.Deg2Rad));
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
        yield return new WaitForSeconds(3.5f);
        pattenDid = false;
    }

    IEnumerator Shot3Bulltets()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[1].position;
        yield return new WaitForSeconds(0.25f);
        EnemyAnimator.SetTrigger("Attack");
        audioSource.PlayOneShot(AttackSound);
        Vector3 to = player.transform.position - transform.position;
        float deg = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;
        ShotsEvent se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
        se.shotDirection = to;
        se.ShotObj();
        se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
        se.shotDirection = new Vector2(Mathf.Cos((deg + 45) * Mathf.Deg2Rad), Mathf.Sin((deg + 45) * Mathf.Deg2Rad));
        se.ShotObj();
        se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
        se.shotDirection = new Vector2(Mathf.Cos((deg - 45) * Mathf.Deg2Rad), Mathf.Sin((deg - 45) * Mathf.Deg2Rad));
        se.ShotObj();
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        yield return new WaitForSeconds(1.5f);
        audioSource.PlayOneShot(MoveSound);
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        transform.position = movingPoints[2].position;
        yield return new WaitForSeconds(0.25f);
        EnemyAnimator.SetTrigger("Attack");
        audioSource.PlayOneShot(AttackSound);
        to = player.transform.position - transform.position;
        deg = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;
        se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
        se.shotDirection = to;
        se.ShotObj();
        se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
        se.shotDirection = new Vector2(Mathf.Cos((deg + 45) * Mathf.Deg2Rad), Mathf.Sin((deg + 45) * Mathf.Deg2Rad));
        se.ShotObj();
        se = Instantiate(ShotOBJs[1], transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
        se.shotDirection = new Vector2(Mathf.Cos((deg - 45) * Mathf.Deg2Rad), Mathf.Sin((deg - 45) * Mathf.Deg2Rad));
        se.ShotObj();
        Instantiate(ShotOBJs[0], transform.position, transform.rotation);
        yield return new WaitForSeconds(3.5f);
        pattenDid = false;
    }

    IEnumerator Die()
    {
        Instantiate(ShotOBJs[0], transform.position, new Quaternion(0, 0, 0, 0));
        audioSource.PlayOneShot(DieSound);
        GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
        StopCoroutine(corutine);
        dying = true;
        for (int i = summonedObj.Count - 1; i >= 0; i--) {
            GameObject g = summonedObj[i];
            summonedObj.RemoveAt(i);
            Destroy(g);
        }
        yield return new WaitForSeconds(0.25f);
        Rigidbody2D rb;
        for(int i = 0; i < enemyControll.AmountHPitem; i++) {
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
