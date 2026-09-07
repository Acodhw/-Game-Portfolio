using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOD2phase : MonoBehaviour
{
    public GameObject[] ShotOBJs;
    public Transform[] shotpoint;
    public BossControl enemyControll;
    public AudioClip AttackSound;
    public AudioClip HitSound;
    public AudioClip DieSound;
    public BossTalkFirst btf;

    private List<GameObject> summonedObj;

    private AudioSource audioSource;
    private IEnumerator corutine;

    private int facingDirection = -1;

    private bool[] pattenDid;
    bool dying = false;

    private Vector2 newVelocity;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Transform player;
    BGMManager bm;

    // Start is called before the first frame update
    private void Start()
    {
        pattenDid = new bool[4];
        summonedObj = new List<GameObject>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("MainCharacter").transform;
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        bm = GameObject.Find("GameManager").GetComponent<BGMManager>();
        Invoke("ItemFall", 15);
    }

    void ItemFall()
    {
        if (!dying && btf.finished)
        {
            for (int i = 0; i < 2; i++)
            {
                rb = Instantiate(enemyControll.HPItem, transform.position + Vector3.back, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
                rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 4f)), ForceMode2D.Impulse);
            }
            for (int i = 0; i < 2; i++)
            {
                rb = Instantiate(enemyControll.MPItem, transform.position + Vector3.back, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
                rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(4f, 4f)), ForceMode2D.Impulse);
            }
        }
        Invoke("ItemFall", 15f);
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
            bm.StartBGM(8);
            if (!pattenDid[0])
            {
                pattenDid[0] = true;
                StartCoroutine("FireAttack");
            }
            if (!pattenDid[1])
            {
                pattenDid[1] = true;
                StartCoroutine("WaterAttack");
            }
            if (!pattenDid[2])
            {
                pattenDid[2] = true;
                StartCoroutine("EarthAttack");
            }
            if (!pattenDid[3])
            {
                pattenDid[3] = true;
                StartCoroutine("WindAttack");
            }
        }
    }

    IEnumerator FireAttack()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 5; i++)
        {
            Vector3 to = player.transform.position - shotpoint[0].position;
            float deg = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;
            ShotsEvent se = Instantiate(ShotOBJs[0], shotpoint[0].position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = to;
            se.shotSpeed = 8f;
            se.ShotObj();
            se = Instantiate(ShotOBJs[0], shotpoint[0].position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((deg + 45) * Mathf.Deg2Rad), Mathf.Sin((deg + 45) * Mathf.Deg2Rad));
            se.shotSpeed = 8f;
            se.ShotObj();
            se = Instantiate(ShotOBJs[0], shotpoint[0].position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((deg - 45) * Mathf.Deg2Rad), Mathf.Sin((deg - 45) * Mathf.Deg2Rad));
            se.shotSpeed = 8f;
            se.ShotObj();
            se = Instantiate(ShotOBJs[0], shotpoint[0].position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((deg + 90) * Mathf.Deg2Rad), Mathf.Sin((deg + 45) * Mathf.Deg2Rad));
            se.shotSpeed = 8f;
            se.ShotObj();
            se = Instantiate(ShotOBJs[0], shotpoint[0].position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
            se.shotDirection = new Vector2(Mathf.Cos((deg - 90) * Mathf.Deg2Rad), Mathf.Sin((deg - 45) * Mathf.Deg2Rad));
            se.shotSpeed = 8f;
            se.ShotObj();
            yield return new WaitForSeconds(0.75f);
        }
        yield return new WaitForSeconds(2f);
        pattenDid[0] = false;
    }
    IEnumerator WaterAttack()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 20; i++)
        {
            audioSource.PlayOneShot(AttackSound);
            for (int j = 0; j < 4; j++)
            {
                ShotsEvent se = Instantiate(ShotOBJs[1], shotpoint[1].position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                se.shotDirection = new Vector2(Mathf.Cos((j * 90 + i * 15) * Mathf.Deg2Rad), Mathf.Sin((j * 90 + i * 15) * Mathf.Deg2Rad));
                se.shotSpeed = Random.Range(8f, 15f);
                se.ShotObj();

            }
            yield return new WaitForSeconds(0.5f);

        }
        yield return new WaitForSeconds(2f);
        pattenDid[1] = false;
    }
    IEnumerator EarthAttack()
    {
        yield return new WaitForSeconds(0.5f);
        for (int k = 0; k < 3; k++)
        {
            for (int i = 0; i < 3; i++)
            {
                audioSource.PlayOneShot(AttackSound);
                for (int j = 0; j < 3; j++)
                {
                    ShotsEvent se = Instantiate(ShotOBJs[2], shotpoint[2].position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    se.shotDirection = new Vector2(Mathf.Cos((120 * j + i * 8f + k * 30) * Mathf.Deg2Rad), Mathf.Sin((120 * j + i * 8f + k * 30) * Mathf.Deg2Rad));
                    se.shotSpeed = 5 * 1.4f * i;
                    se.ShotObj();

                }
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(2f);
        pattenDid[2] = false;
    }
    IEnumerator WindAttack()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    ShotsEvent se = Instantiate(ShotOBJs[3], shotpoint[3].position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
                    se.shotDirection = new Vector2(Mathf.Cos((180 * j + k * 8 + i * 45) * Mathf.Deg2Rad), Mathf.Sin((180 * j + k * 8 + i * 45) * Mathf.Deg2Rad));
                    se.shotSpeed = 10;
                    se.ShotObj();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(2f);
        pattenDid[3] = false;
    }



    IEnumerator Die()
    {
        dying = true;
        audioSource.PlayOneShot(DieSound);
        float j = 1;
        while (j > 0)
        {
            j -= Time.deltaTime / 2;
            enemyControll.originalSpriteColor = new Color(1, 1, 1, j);
            yield return null;
        }             
        for (int i = summonedObj.Count - 1; i >= 0; i--)
        {
            GameObject g = summonedObj[i];
            summonedObj.RemoveAt(i);
            Destroy(g);
        }
        yield return new WaitForSeconds(0.25f);
        btf.nextScenego();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}