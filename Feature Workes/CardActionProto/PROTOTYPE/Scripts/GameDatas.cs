using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Cardkind {
    action, //공격이나 방어를 함
    support,//자기 자신을 지원하는 역할
    util,   //다음 카드에 추가적인 도움을 줌
    Other, //위에꺼 이외에 다른걸함
}

[System.Serializable]
public class Card {
    [Tooltip("카드에 쓰일 이미지")]public Sprite CardImage;
    [Tooltip("카드의 종류 선택")] public Cardkind cardkind;
    [Tooltip("카드 사용 시 소모되는 토큰 수")] public int needToken;
    [Tooltip("카드의 이름")] public string CardName;
    [Tooltip("카드의 정보")] [TextArea] public string CardInfo;
    [Tooltip("카드가 선택되었을 시 발동될 이벤트")] public UnityEngine.Events.UnityEvent events;
}

public class GameDatas : MonoBehaviour
{
    public Card[] card;
    public GameObject[] summonObj;
    private PlayerState_Proto pstate;
    [HideInInspector]
    public bool isNextTokenUseless;
    [HideInInspector]
    public float tokenUpTime;
    [HideInInspector]
    public float strongerAttack;
    [HideInInspector]
    public bool execution = false;
    [HideInInspector]
    public float nowtimeScale = 1;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    

    private void Start()
    {
        pstate = GetComponent<PlayerState_Proto>();
    }
    private void Update()
    {
        if (tokenUpTime > 0)
        {
            tokenUpTime -= Time.deltaTime;
        }
    }
    private Vector2 DefaultSummonPoint;

    public void resetDefaultSummonPoint() { DefaultSummonPoint += Vector2.zero; }
    public void HORIsetDefaultSummonPoint(float horizontal) {
        {
            if (pstate.player != null)
            {
                if (pstate.player.GetComponent<SpriteRenderer>().flipX)
                {
                    DefaultSummonPoint += Vector2.left * horizontal;
                }
                else
                {
                    DefaultSummonPoint += Vector2.right * horizontal;
                }
            }
            else
            {
                DefaultSummonPoint += Vector2.right * horizontal;
            }
        }
    }
    public void VERCsetDefaultSummonPoint(float vertical) { DefaultSummonPoint += Vector2.up * vertical; }

    public void summonObjonPoint(int objCode)
    {       
        GameObject g = Instantiate(summonObj[objCode], pstate.player.position, pstate.player.rotation);
        if (strongerAttack > 0)
        {
            if (g.GetComponent<PlayerAttack_Proto>())
            {               
                g.GetComponent<PlayerAttack_Proto>().Damage_Plus *= (1 + strongerAttack * 0.01f);
                g.GetComponent<PlayerAttack_Proto>().MaxHP_pro_Damage *= (1 + strongerAttack * 0.01f);
                g.GetComponent<PlayerAttack_Proto>().Physics_Mult *= (1 + strongerAttack * 0.01f);
                g.GetComponent<PlayerAttack_Proto>().HP_Mult *= (1 + strongerAttack * 0.01f);
                g.GetComponent<PlayerAttack_Proto>().Magic_Mult *= (1 + strongerAttack * 0.01f);
                strongerAttack = 0;
            }
        }
    }

    public void tokenUpCard()
    {
        tokenUpTime = 1;
    }


    public void NextTokenFree()
    {
        isNextTokenUseless = true;
    }

    public void TurnOnExcution()
    {
        execution = true;
    }

    public void NextAttackStronger(float strongPoint)
    {
        strongerAttack = strongPoint;
    }

    public void RandomUsingCard()
    {
        int tmp = Random.Range(0, 9);
        while (pstate.SettingCardKind[tmp] > 0)
        {
            tmp = Random.Range(0, 9);
        }
        card[tmp].events.Invoke();
    }

    public void Slow()
    {
        StartCoroutine("timeSlow");
    }

    IEnumerator timeSlow() {
        nowtimeScale = 0.75f;
        Time.timeScale = 0.75f;
        yield return new WaitForSecondsRealtime(2);
        nowtimeScale = 1;
        Time.timeScale = 1;
    }

}
