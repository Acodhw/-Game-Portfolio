using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class FireBat : MonoBehaviour
{
    public float movementSpeed;
    public float PlayerFindDistance;
    public float PlayerFollowDistance;
    public float PlayerKeepDistance;
    public float findTime = 5;
    public GameObject FindedPlayerIcon;
    public GameObject ShotOBJ;
    public Animator EnemyAnimator;
    public EnemyControll enemyControll;
    public AudioClip AttackSound;
    public AudioClip HitSound;
    public AudioClip DieSound;

    private AudioSource audioSource;
    private float findPlayerTime;

    private int facingDirection = 1;

    private bool AttakingPlayer;
    bool dying = false;

    private Vector2 newVelocity;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Transform player;

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("MainCharacter").transform;
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
    }


    private void Update()
    {
        if (enemyControll.getDamage)
        {
            enemyControll.getDamage = false;
            audioSource.PlayOneShot(HitSound);
            findPlayerTime = findTime;
        }

        if (enemyControll.HP <= 0 && !dying) StartCoroutine("Die");
        FindedPlayerIcon.SetActive(findPlayerTime > 0);

        if (player.position.x - transform.position.x > 0 && facingDirection == -1)
        {
            Flip();
        }
        else if (player.position.x - transform.position.x < 0 && facingDirection == 1)
        {
            Flip();
        }
        if (findPlayerTime > 0)
        {
            if (PlayerFollowDistance < Vector2.Distance(player.position, transform.position))
            {
                rb.linearVelocity = (player.position - transform.position) * movementSpeed;
            }
            else if (PlayerKeepDistance > Vector2.Distance(player.position, transform.position))
            {
                rb.linearVelocity = -(player.position - transform.position) * movementSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                if (!AttakingPlayer)
                {
                    AttakingPlayer = true;
                    StartCoroutine("Attack");
                }
            }
            findPlayerTime -= Time.deltaTime;
        }
        else if (findPlayerTime < 0) findPlayerTime = 0;
    }
    IEnumerator Attack()
    {
        audioSource.PlayOneShot(AttackSound);
        yield return new WaitForSeconds(0.15f);
        Vector3 to = player.transform.position - transform.position;
        float deg = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;
        ShotsEvent se = Instantiate(ShotOBJ, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();        
        se.shotDirection = to;
        se.ShotObj();
        se = Instantiate(ShotOBJ, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
        se.shotDirection = new Vector2(Mathf.Cos((deg + 45) * Mathf.Deg2Rad), Mathf.Sin((deg + 45) * Mathf.Deg2Rad));
        se.ShotObj();
        se = Instantiate(ShotOBJ, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<ShotsEvent>();
        se.shotDirection = new Vector2(Mathf.Cos((deg - 45) * Mathf.Deg2Rad), Mathf.Sin((deg - 45) * Mathf.Deg2Rad));
        se.ShotObj();
        yield return new WaitForSeconds(1.5f);
        AttakingPlayer = false;
    }

    IEnumerator Die()
    {
        audioSource.PlayOneShot(DieSound);
        dying = true;
        EnemyAnimator.SetTrigger("Die");

        yield return new WaitForSeconds(0.6f);
        Rigidbody2D rb;
        if (Random.Range(0, 100) < enemyControll.HPitemFallProbability)
        {
            rb = Instantiate(enemyControll.HPItem, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 4f)), ForceMode2D.Impulse);
        }
        if (Random.Range(0, 100) < enemyControll.MPitemFallProbability)
        {
            rb = Instantiate(enemyControll.MPItem, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(Random.Range(-4f, 4f), Random.Range(4f, 4f)), ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        PlayerFind();
    }

    void PlayerFind()
    {
        if (PlayerFindDistance >= Vector2.Distance(player.position, transform.position))
        {
            Debug.DrawRay(transform.position, (Vector2)(player.position - transform.position).normalized * PlayerFindDistance, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (Vector2)(player.position - transform.position).normalized, PlayerFindDistance, 1 << 6 | 1 << 3);
            if (hit.collider.gameObject.layer == 6) findPlayerTime = findTime;
        }
    }


    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
}
