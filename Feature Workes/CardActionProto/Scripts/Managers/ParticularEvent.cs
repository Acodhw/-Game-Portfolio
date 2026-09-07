using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum DamageType {
    Physics,
    Magic,
    True,
}
public enum Elemental
{
    None,
    Fire,
    Water,
    Ground,
    Wind,
    Plant,
    Electric,
    Rock,
    Frozen,
    Esp,
    Spirit,
    Nature,
    Harmony,
    Light,
    Dark,
    Multi,
};

public enum ClowdControl
{
    None,
    Stun, 
    Slow, 
    Weakleg, 
    Bound,
    //분리
    Push, 
    Pull,
    Airborne, 
    //분리
    Blind, 
    Silance,
    //분리
    Fear, 
    Confusion, 
    Madness,   
};


[System.Serializable]
public struct Condition
{
    [Tooltip("상태의 아이콘을 지정합니다.")]
    [SerializeField] public Sprite conditionIcon;
    [Tooltip("상태의 이름을 지정합니다.")]
    [SerializeField] public string conditionName;
    [Tooltip("상태를 설명합니다.")]
    [TextArea(5, 10)]
    [SerializeField] public string conditionInfo;
    [Tooltip("상태의 최대 중첩 가능 횟수를 지정합니다.")]
    [SerializeField] public int MaxStack;
    [Tooltip("상태의 시리얼코드를 지정합니다.")]
    [SerializeField] public string SerialCode;   
}

[System.Serializable]
public struct ConditionCheck
{
    [Tooltip("상태의 시리얼 코드를 부여하십시오.")]
    public Condition con;
    [Tooltip("상태가 얼마나 강력할 지 부여")]
    public int point;
    [Tooltip("상태 유지 시간 부여")]
    public float time;
};

[System.Serializable]
public struct ConditionCheckwithString
{
    [Tooltip("상태의 시리얼 코드를 부여하십시오.")]
    public string con;
    [Tooltip("상태가 얼마나 강력할 지 부여")]
    public int point;
    [Tooltip("상태 유지 시간 부여")]
    public float time;
};


[System.Serializable]
public struct DamageInfo
{
    [Header("기본 데미지 설정")]
    [Tooltip("데미지의 타입(물리, 마법, 고정)을 설정합니다.")]
    [SerializeField] public DamageType damageType;
    [Tooltip("데미지의 속성을 지정합니다.")]
    [SerializeField] public Elemental elemental;
    [Tooltip("데미지의 고정값을 설정합니다.")]
    [SerializeField] public uint damage;
    [Tooltip("관통력을 지정합니다. l 수치만큼 적의 방어/저항 수치를 무시합니다.")]
    [SerializeField] public uint penetrate;
    [Space(5f)]
    [Header("세부 데미지 설정")]
    [Tooltip("데미지의 최대 체력 비례 데미지를 설정합니다.")]
    [SerializeField] public float maxHPDamage;
    [Tooltip("데미지의 현재 체력 비례 데미지를 설정합니다.")]
    [SerializeField] public float nowHPDamage;
    [Tooltip("데미지의 잃은 체력 비례 데미지를 설정합니다.")]
    [SerializeField] public float lostHPDamage;
    [Tooltip("데미지 처리와 함께 할 CC를 지정합니다.")]
    [SerializeField] public ClowdControl cc;
    [Tooltip("CC의 시간을 지정합니다.")]
    [SerializeField] public float ccTime;
    [Tooltip("CC의 강도를 지정합니다.")]
    [SerializeField] public float ccStrength;
    [Tooltip("추가될 상태를 지정합니다.")]
    [SerializeField] public ConditionCheckwithString[] conditionCheck;
    [Tooltip("크리티컬 여부입니다.")]
    [SerializeField] public bool criticalPercent;
    [Tooltip("크리티컬 계수입니다.")]
    [SerializeField] public float criticalPoint;
    [Space(5f)]
    [Header("특수 데미지 설정")]
    [Tooltip("데미지를 가한 자의 운 수치입니다.")]
    [SerializeField] public uint luck;
    [Tooltip("데미지를 가한 자의 Transform 정보입니다.")]
    [SerializeField] public Transform order;
    [Tooltip("데미지 피격 시 데미지를 변화시키는 이벤트를 설정합니다..")]
    [SerializeField] public List<Func<States, uint, uint>> damageChangeFunc;
    [Tooltip("데미지 피격 시 데미지가 변하지 않는 이벤트를 설정합니다.")]
    [SerializeField] public UnityEvent<States, uint> damageChangeEvent;
    [Tooltip("지속 데미지 여부를 설정합니다")]
    [SerializeField] public bool continuousDamage;
    [Tooltip("지속 데미지 여부가 활성화 되어 있을 경우, 이 시간마다 데미지가 들어가게 됩니다.")]
    [SerializeField] public float continuousTime;
    [Tooltip("피격 시 플래시 이펙트 삭제 여부입니다.")]
    [SerializeField] public bool noflash;


    public DamageInfo(DamageInfo di)
    {
        this = di;
    }
    public DamageInfo(DamageType dt, Elemental el, uint d, uint p, float mhd, float nhd, float lhd, ClowdControl c, float ct, float cs, ConditionCheckwithString[] cdc, bool cp, float cpo, uint lk, Transform or, Func<States, uint, uint> dcf, UnityEvent<States, uint> dce, bool cd, float ctm = 0f, bool fl = false)
    {
        damageType = dt;
        elemental = el;
        damage = d;
        penetrate = p;
        maxHPDamage = mhd;
        nowHPDamage = nhd;
        lostHPDamage = lhd;
        cc = c;
        ccTime = ct;
        ccStrength = cs;
        conditionCheck = cdc;
        criticalPercent = cp;
        criticalPoint = cpo;
        luck = lk;
        order = or;
        damageChangeFunc = new List<Func<States, uint, uint>>() { dcf };
        damageChangeEvent = dce;
        continuousDamage = cd;
        continuousTime = ctm;
        noflash = fl;
    }
    public DamageInfo(DamageType dt, Elemental el, uint d, uint p, int mhd, int nhd, int lhd, ClowdControl c, float ct, float cs, ConditionCheckwithString[] cdc, bool cp, float cpo, uint lk, Transform or, List<Func<States, uint, uint>> dcf, UnityEvent<States, uint> dce, bool cd, float ctm = 0f, bool fl = false)
    {
        damageType = dt;
        elemental = el;
        damage = d;
        penetrate = p;
        maxHPDamage = mhd;
        nowHPDamage = nhd;
        lostHPDamage = lhd;
        cc = c;
        ccTime = ct;
        ccStrength = cs;
        conditionCheck = cdc;
        criticalPercent = cp;
        criticalPoint = cpo;
        luck = lk;
        order = or;
        damageChangeFunc = dcf;
        damageChangeEvent = dce;
        continuousDamage = cd;
        continuousTime = ctm;
        noflash = fl;
    }

}

[System.Serializable]
public struct CCinfo
{
    [Tooltip("CC의 종류입니다.")]
    [SerializeField] public ClowdControl cc;
    [Tooltip("CC의 시간입니다.")]
    [SerializeField] public float ccTime;
    [Tooltip("CC의 강도입니다.")]
    [SerializeField] public float ccStrength;
    public CCinfo(ClowdControl c, float t, float s)
    {
        cc = c;
        ccTime = t;
        ccStrength = s;
    }
}

public class ParticularEvent : MonoBehaviour
{
    [Tooltip("CC기에 걸렸을 때 이펙트")]
    [SerializeField] private GameObject[] CCEffectObjects = new GameObject[9];

    [Tooltip("생성될 텍스트 오브젝트")]
    [SerializeField] private GameObject flatText;

    [Tooltip("버프")]
    [SerializeField] private List<Condition> buff;
    [Tooltip("디버프")]
    [SerializeField] private List<Condition> dibuff;
    [Tooltip("상태 확인")]
    [SerializeField] private List<Condition> condition;


    // 속성표
    private readonly List<Elemental>[,] elementalInteraction = new List<Elemental>[14, 2] {
     { // 불
            new List<Elemental> { Elemental.Plant, Elemental.Frozen, Elemental.Wind }, // 공격시 강점
            new List<Elemental> { Elemental.Water, Elemental.Ground, Elemental.Rock, Elemental.Nature }, // 공격시 약점
        },
     { // 물
            new List<Elemental> { Elemental.Fire, Elemental.Ground, Elemental.Rock }, // 공격시 강점
            new List<Elemental> { Elemental.Plant, Elemental.Water, Elemental.Nature }, // 공격시 약점
        },
     { // 땅
            new List<Elemental> { Elemental.Fire, Elemental.Ground, Elemental.Electric }, // 공격시 강점
            new List<Elemental> { Elemental.Plant, Elemental.Frozen, Elemental.Ground, Elemental.Rock, Elemental.Nature }, // 공격시 약점
        },
     { // 바람
            new List<Elemental> { Elemental.Electric, Elemental.Ground, Elemental.Frozen }, // 공격시 강점
            new List<Elemental> { Elemental.Rock, Elemental.Wind, Elemental.Nature}, // 공격시 약점
        },
     { // 풀
            new List<Elemental> { Elemental.Water, Elemental.Ground, Elemental.Rock }, // 공격시 강점
            new List<Elemental> { Elemental.Plant, Elemental.Electric, Elemental.Plant, Elemental.Nature }, // 공격시 약점
        },
     { // 전기
            new List<Elemental> { Elemental.Water, Elemental.Frozen }, // 공격시 강점
            new List<Elemental> { Elemental.Ground, Elemental.Rock, Elemental.Electric, Elemental.Nature }, // 공격시 약점
        },
     { // 바위
            new List<Elemental> { Elemental.Fire, Elemental.Frozen }, // 공격시 강점
            new List<Elemental> { Elemental.Ground, Elemental.Water }, // 공격시 약점
        },
     { // 냉동
            new List<Elemental> { Elemental.Esp, Elemental.Water, Elemental.Plant}, // 공격시 강점
            new List<Elemental> { Elemental.Electric, Elemental.Rock, Elemental.Wind }, // 공격시 약점
        },
     { // 초능력
            new List<Elemental> { Elemental.Wind, Elemental.Rock }, // 공격시 강점
            new List<Elemental> { Elemental.Electric, Elemental.Frozen }, // 공격시 약점
        },
     { // 영능력
            new List<Elemental> { Elemental.Esp }, // 공격시 강점
            new List<Elemental> { Elemental.Ground, Elemental.Rock }, // 공격시 약점
        },
     { // 자연
            new List<Elemental> { Elemental.Harmony, Elemental.Fire, Elemental.Water, Elemental.Ground, Elemental.Plant, Elemental.Electric }, // 공격시 강점
            new List<Elemental> { Elemental.Nature }, // 공격시 약점
        },
     { // 음양
            new List<Elemental> { Elemental.Nature, Elemental.Esp, Elemental.Spirit }, // 공격시 강점
            new List<Elemental> { Elemental.Harmony }, // 공격시 약점
        },
     { // 빛
            new List<Elemental> { Elemental.Dark }, // 공격시 강점
            new List<Elemental> { Elemental.Light }, // 공격시 약점
        },
     { // 어둠
            new List<Elemental> { Elemental.Light }, // 공격시 강점
            new List<Elemental> { Elemental.Dark }, // 공격시 약점
        },    
    };

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [ContextMenu("Set Condition")]
    void SetCondition()
    {
        condition = buff.Concat(dibuff).ToList();
    }
    [ContextMenu("Send String to LanguageManager")]
    public virtual void SaveString()
    {
        List<string> list = new List<string>() { };
        foreach (Condition i in condition)
        {
            list.Add(i.conditionName);
            list.Add(i.conditionInfo);
        }
        GameObject.Find("GameManager").GetComponent<LanguageManager>().SetTextList(("Particular Event"), list);
    }

    [ContextMenu("Get String to LanguageManager")]
    public void LoadString()
    {
        List<string> list = GameObject.Find("GameManager").GetComponent<LanguageManager>().GetTextList(("Playable Move Skills"));
        int j = 0;
        
        for (int i = 0; i < list.Count; i += 2)
        {
            Condition con = condition[j];
            con.conditionName = list[i];
            con.conditionInfo = list[i + 1];
            condition[j] = con;
            j++;
        }
    }

    public Condition FindCondition(string serial) {
        return condition.Find(x => x.SerialCode.Equals(serial));
    }

    public List<Elemental>[,] GetElementalInteraction()
    {
        return elementalInteraction;
    }
    
    public FlatText SummonFlatText(Vector3 position)
    {
        return Instantiate(flatText, position, Quaternion.Euler(0, 0, 0)).GetComponent<FlatText>();
    }

    public FlatText SummonFlatText(Vector3 position, Transform parant)
    {
        return Instantiate(flatText, position, Quaternion.Euler(0, 0, 0), parant).GetComponent<FlatText>();
    }

    public GameObject GetCCEffect(ClowdControl cc) 
    {
        switch (cc) {
            case ClowdControl.Stun:
                return CCEffectObjects[0];
            case ClowdControl.Slow:
                return CCEffectObjects[1];
            case ClowdControl.Weakleg:
                return CCEffectObjects[2];
            case ClowdControl.Bound:
                return CCEffectObjects[3];
            case ClowdControl.Blind:
                return CCEffectObjects[4];
            case ClowdControl.Silance:
                return CCEffectObjects[5];
            case ClowdControl.Fear:
                return CCEffectObjects[6];
            case ClowdControl.Confusion:
                return CCEffectObjects[7];
            case ClowdControl.Madness:
                return CCEffectObjects[8];
        }

        return null;
    }
}
