using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MovementInfo
{
    [Tooltip("유지 시간을 나타냅니다.")]
    [SerializeField] public float time;
    [Tooltip("방향을 지정합니다.")]
    [SerializeField] public Vector2 velocity;
    [Tooltip("속력을 지정합니다.")]
    [SerializeField] public float speed;

    public MovementInfo(float t, Vector2 v, float s) {
        time = t; velocity = v; speed = s;
    }
}

public class ShotsEvent : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;

    [Tooltip("투사체와 이펙트가 남아있는 시간을 지정합니다.")]
    [SerializeField] private float leftTime;

    [Tooltip("움직임 정보를 입력합니다.")]
    [SerializeField] private List<MovementInfo> moveset;

    [Tooltip("속도의 보정치를 지정합니다.")]
    [SerializeField] private int velocityFixValue;

    [Tooltip("생성 후 발사 여부를 지정합니다.")]
    [SerializeField] private bool ShotInSummon = false;

    [Tooltip("투사체 여부를 지정합니다. 투사체가 아닐경우 투사체제거 효과가 통하지 않습니다.")]
    [SerializeField] private bool isBullet = true;


    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        if(ShotInSummon) StartCoroutine("TimeMovesetSetting");
        Destroy(gameObject, leftTime);
    }


    public void Shot() 
    {
        StartCoroutine("TimeMovesetSetting");
    }

    public void AddMoveSet(MovementInfo moveset)
    { 
        this.moveset.Add(moveset);
    }

    public void SetVelocityFixValue(int value)
    {
        velocityFixValue = value;
    }

    public bool GetIsBullet() {
        return isBullet;
    }
    IEnumerator TimeMovesetSetting() 
    {
        for (int i = 0; i < moveset.Count; i++) 
        {
            Vector2 shotSpeed = (moveset[i].velocity.normalized * moveset[i].speed);          
            rigid.velocity = new Vector2(shotSpeed.x * velocityFixValue, shotSpeed.y);     
            yield return new WaitForSeconds(moveset[i].time);
        }
    }
}

