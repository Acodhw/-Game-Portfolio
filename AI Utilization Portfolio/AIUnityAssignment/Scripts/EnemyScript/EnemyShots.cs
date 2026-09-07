using UnityEngine;

public class EnemyShots : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    private Vector3 moveDirection = Vector3.forward;

    [Header("Colliders")]
    public Collider col3D;
    public Collider col2D;

    [Header("2D Settings")]
    public float snapSpeed = 20f;

    private bool isCurrently2D = false;
    private Transform playerTarget;
    private PlayerControl player;

    void Awake()
    {
        if (col3D == null)
            col3D = GetComponent<Collider>();

        if (col2D != null)
            col2D.enabled = false;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerControl>();
        if (player != null && player.is2D)
        {
            SwitchTo2D(player.transform);
        }
    }

    public void Initialize(Vector3 direction, float speed, float lifeTime)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        if (player != null)
        {
            if (player.is2D && !isCurrently2D)
            {
                SwitchTo2D(player.transform);
            }
            else if (!player.is2D && isCurrently2D)
            {
                SwitchTo3D();
            }
        }

        if (!isCurrently2D || playerTarget == null || col2D == null)
            return;
        Vector3 targetPos = col3D.transform.position;
        targetPos.z = playerTarget.position.z;

        col2D.transform.position = targetPos;
    }

    public void SwitchTo2D(Transform playerTransform)
    {
        isCurrently2D = true;
        playerTarget = playerTransform;

        if (col3D != null)
            col3D.enabled = false;

        if (col2D != null)
        {
            col2D.transform.position = col3D.transform.position;
            col2D.enabled = true;
        }
    }

    public void SwitchTo3D()
    {
        isCurrently2D = false;
        playerTarget = null;

        if (col2D != null)
        {
            col2D.enabled = false;
            col2D.transform.localPosition = Vector3.zero;
        }

        if (col3D != null)
            col3D.enabled = true;
    }
}