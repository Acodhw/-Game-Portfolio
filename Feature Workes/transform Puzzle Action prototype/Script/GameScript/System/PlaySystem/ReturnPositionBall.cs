using UnityEngine;

public class ReturnPositionBall : MonoBehaviour
{
    [SerializeField]
    private float deadY;

    private Vector3 firstPos;
    private Rigidbody2D rigid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        firstPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < deadY)
        {
            rigid.angularVelocity = 0;
            rigid.linearVelocity = Vector2.zero;
            transform.position = firstPos;
        }
    }
}
