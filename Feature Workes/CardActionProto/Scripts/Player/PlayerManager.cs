using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;


public struct BarriorInfo {
    public float time;
    public uint barrior;
    public string code;
}

[System.Serializable]
public class StringUIntDic : SerializableDictionary<string, uint> { }
public class States : MonoBehaviour
{
    [Header("State Base Setting")]
    [Tooltip("상태에 따른 condition 사용 이펙트 오브젝트 지정")]
    [SerializeField] protected GameObject[] ConditionObjects;
    [Tooltip("공격 속성에 대한 내성 지정")]
    [SerializeField] protected int[] elementalTolerance = new int[15];

    [Header("Debug")]
    [SerializeField] protected List<ConditionCheck> conditionList = new List<ConditionCheck>();


    protected const int GuardConstant = 500;
    protected ParticularEvent particular;   
    protected bool isHitting = false;
    protected bool isDied = false;
    protected StringUIntDic state; 
    protected Dictionary<int, float> continueusTimeCheck = new Dictionary<int, float>();
    protected List<BarriorInfo> Barriors = new List<BarriorInfo>();

    protected virtual void Awake()
    {
        particular = GameObject.Find("GameManager").GetComponent<ParticularEvent>();
    }

    public virtual void BarriorAdd(string code, uint barrior, float time)
    {
     
    }
    public virtual void Heal(int heal) 
    {
    
    }

    public virtual bool GetIsHit()
    {
        return isHitting;
    }

    public virtual void Damage(DamageInfo damageInfo, bool isSpawnDamageText) 
    {

    }

    public virtual void Execute(bool isSpawnDamageText) { }

    public virtual void ConditionChange(Condition con, int value, float time)
    {
        ConditionCheck condition;
        if (!isinConditionList(con.SerialCode))
        {
            condition.con = con;
            condition.point = value;
            condition.time = time;
            conditionList.Add(condition);
            StartCoroutine(ConditionTime(con));
        }
        else
        {
            int index = conditionList.FindIndex(x => x.con.SerialCode.Equals(con.SerialCode));
            condition = conditionList.Find(x => x.con.SerialCode.Equals(con.SerialCode));
            condition.point = (condition.point + value > condition.con.MaxStack ? condition.con.MaxStack : condition.point + value);
            condition.time = (condition.time > time ? condition.time : time);          
            conditionList[index] = condition;
        }
    }

    public virtual void ConditionChange(string code, int value, float time)
    {
        ConditionCheck condition;
        Condition con = particular.FindCondition(code);
        if (!isinConditionList(code))
        {
            condition.con = con;
            condition.point = value;
            condition.time = time;
            conditionList.Add(condition);
            StartCoroutine(ConditionTime(con));
        }
        else
        {
            int index = conditionList.FindIndex(x => x.con.SerialCode.Equals(code));
            condition = conditionList.Find(x => x.con.SerialCode.Equals(code));
            condition.point = (condition.point + value > condition.con.MaxStack ? condition.con.MaxStack : condition.point + value);
            condition.time = (condition.time > time ? condition.time : time);
            conditionList[index] = condition;
        }
    }

    protected virtual IEnumerator ConditionTime(Condition con)
    {
        ConditionCheck condition;
        int index;
        do
        {
            yield return null;
            index = conditionList.FindIndex(x => x.con.SerialCode.Equals(con.SerialCode));
            condition = conditionList.Find(x => x.con.SerialCode.Equals(con.SerialCode));
            condition.time -= Time.deltaTime;
            conditionList[index] = condition;
        }
        while (condition.time > 0);
        conditionList.RemoveAt(index);
    }
  
    protected virtual Color ElementalColors(Elemental elemental) {
        Vector3 tmp = new Vector3(255, 255, 255);
        switch (elemental)
        {
            case Elemental.Fire:
                tmp = new Vector3(255, 102, 26);
                break;
            case Elemental.Water:
                tmp = new Vector3(20, 132, 243);
                break;
            case Elemental.Ground:
                tmp = new Vector3(225, 168, 36);
                break;
            case Elemental.Wind:
                tmp = new Vector3(112, 230, 243);
                break;
            case Elemental.Plant:
                tmp = new Vector3(144, 219, 79);
                break;
            case Elemental.Electric:
                tmp = new Vector3(238, 238, 0);
                break;
            case Elemental.Rock:
                tmp = new Vector3(161, 145, 131);
                break;
            case Elemental.Frozen:
                tmp = new Vector3(163, 196, 239);
                break;
            case Elemental.Esp:
                tmp = new Vector3(255, 63, 255);
                break;
            case Elemental.Spirit:
                tmp = new Vector3(87, 23, 151);
                break;
            case Elemental.Nature:
                tmp = new Vector3(58, 196, 150);
                break;
            case Elemental.Harmony:
                tmp = new Vector3(149, 149, 149);
                break;
            case Elemental.Light:
                tmp = new Vector3(252, 252, 252);
                break;
            case Elemental.Dark:
                tmp = new Vector3(21, 21, 21);
                break;
        }
        tmp /= 225;
        return new Color(tmp.x, tmp.y, tmp.z);
    }

    protected virtual int ElementalCodeGet(Elemental elemental)
    {
        switch (elemental)
        {
            case Elemental.Fire:
                return 1;
            case Elemental.Water:
                return 2;
            case Elemental.Ground:
                return 3;
            case Elemental.Wind:
                return 4;
            case Elemental.Plant:
                return 5;
            case Elemental.Electric:
                return 6;
            case Elemental.Rock:
                return 7;
            case Elemental.Frozen:
                return 8;
            case Elemental.Esp:
                return 9;
            case Elemental.Spirit:
                return 10;
            case Elemental.Nature:
                return 11;
            case Elemental.Harmony:
                return 12;
            case Elemental.Light:
                return 13;
            case Elemental.Dark:
                return 14;
        }
        return 0;
    }

    public virtual void RemoveCondition(Condition con)
    {
        conditionList.Remove(conditionList.Find(x => x.con.SerialCode.Equals(con.SerialCode)));
    }

    public virtual void RemoveCondition(string code)
    {
        conditionList.Remove(conditionList.Find(x => x.con.SerialCode.Equals(code)));
    }

    public virtual void RemoveCondition()
    {
        conditionList.Clear();
    }

    protected virtual float GetElementalInteractionValue(Elemental mine, Elemental get, int luck) 
    {
        if (get == Elemental.None)
            return 1;
        if (get == Elemental.Multi)
        {
            if (mine == Elemental.None || mine == Elemental.Harmony || mine == Elemental.Light || mine == Elemental.Dark) return 1;
            if (mine == Elemental.Nature) return UnityEngine.Random.Range(0, 100) + luck * 0.5f > 50 ? 1 : 0.75f;
            int interaction = Mathf.RoundToInt(UnityEngine.Random.Range(0, 100) + luck * 0.33f);
            return interaction < 33 ? 0.75f : interaction > 67 ? 1.25f : 1;
        }
        int index = 0;
        switch (get)
        {
            case Elemental.Fire:
                index = 0;
                break;
            case Elemental.Water:
                index = 1;
                break;
            case Elemental.Ground:
                index = 2;
                break;
            case Elemental.Wind:
                index = 3;
                break;
            case Elemental.Plant:
                index = 4;
                break;
            case Elemental.Electric:
                index = 5;
                break;
            case Elemental.Rock:
                index = 6;
                break;
            case Elemental.Frozen:
                index = 7;
                break;
            case Elemental.Esp:
                index = 8;
                break;
            case Elemental.Spirit:
                index = 9;
                break;
            case Elemental.Nature:
                index = 10;
                break;
            case Elemental.Harmony:
                index = 11;
                break;
            case Elemental.Light:
                index = 12;
                break;
            case Elemental.Dark:
                index = 13;
                break;
        }

        if (particular.GetElementalInteraction()[index, 0].Contains(mine)) return 1.25f;
        else if (particular.GetElementalInteraction()[index, 1].Contains(mine)) return 0.75f;
        else return 1;
    }

    public virtual bool GetIsDie() 
    {
        return isDied;
    }

    public virtual bool isinConditionList(string serial) {
        return conditionList.Exists(x => x.con.SerialCode.Equals(serial));
    }

    public virtual uint GetState(string name)
    {
        return state[name];
    }
}
public class PlayerManager : States
{
    //public int[] tmpSkilllcode = new int[3];
    //public int[] tmpMoveSkilllcode = new int[2];
    private Transform player;
    private PlayerControl playerControl;
    private int selectedCharacter = 0;
    private float ultimitCooltime = 0;
    private Dictionary<string, Dictionary<string, Func<States, int, DamageType, int>>> MyEvents = new Dictionary<string, Dictionary<string, Func<States, int, DamageType, int>>> {
        { "_normalAttackDoing", new Dictionary<string, Func<States, int, DamageType, int>>() },
        { "_skillDoing", new Dictionary<string, Func<States, int, DamageType, int>>() },
        { "_attackDoing", new Dictionary<string, Func<States, int, DamageType, int>>() },
        { "_normalAttackHitting", new Dictionary<string, Func<States, int, DamageType, int>>() },
        { "_skillHitting", new Dictionary<string, Func<States, int, DamageType, int>>() },
        { "_hitting", new Dictionary<string, Func<States, int, DamageType, int>>() },
        { "_GetDamaged", new Dictionary<string, Func<States, int, DamageType, int>>() },
    };

    [Tooltip("공용 이동 스킬의 작성입니다.")]
    [SerializeField] private Skills[] moveSkills;

    [Tooltip("무브 스킬에서 사용할 오브젝트들입니다.")]
    [SerializeField] private GameObject[] moveSkillObjects;

    private bool isMoveSkillOn = false;
    private bool[] MoveSkillsOn = new bool[] { false, false, false, false, false, false };
    private bool[] CharacterGet = new bool[8] { true, true, true, true, false, false, false, false };
    private float[ , ] MoveSkillsCooltime = new float[,] { {0, 0}, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };

    private readonly float[,] characterStateEditValue = new float[8, 7] { //힘, 지력, 방어력, 저항력, 불굴, 치명타 계수, 치명타 확률 순서대로 보정
        { 1, 1, 1, 1, 1, 1, 1 },
        { 0.4f, 1.4f, 0.8f, 1.2f, 0.8f, 0, 0 },
        { 1.2f, 0.7f, 0.7f, 0.65f, 0.8f, 1.3f, 0.8f },
        { 0.9f, 1.1f, 1, 0.8f, 0.9f, 1.1f, 1.2f },
        { 1.5f, 1.2f, 0.9f, 1, 1.3f, 1.3f, 0.5f },
        { 0.9f, 0.7f, 1.1f, 1.3f, 1, 0.5f, 1 },
        { 0.6f, 0.5f, 0.8f, 0.9f, 1.1f, 1, 1 },
        { 0.75f, 1.8f, 0.5f, 0.85f, 1, 0.25f, 0.8f }
    };

    private StringUIntDic stateLevel = new StringUIntDic() {
        //각 레벨은 1부터 시작하여 총 만렙 20렙을 가진다.
        //운, 불굴의 최대치 100, 공격력-지력-방어력-저항력의 레벨로 얻는 스탯 최대치 400
    {"HPLevel", 1},           //내구성 레벨 : 체력의 증가
    {"SkillLevel", 1},        //스킬 레벨 : MP 증가
    {"PowerLevel", 1},        //공격 레벨 : 공격력 증가
    {"MagicLevel", 1},        //마법 레벨 : 지력 증가
    {"GuardLevel",1},         //방어 레벨 : 방어력 증가
    {"ResisLevel", 1},        //저항 레벨 : 저항 증가
    {"MaxMP", 1},             //강인함 레벨 : 불굴 증가
    {"CriticalLevel", 1},     //치명 레벨 : 치명타 계수 증가
    {"LuckyLevel", 1},        //운 레벨 : 운 증가
    };

    private StringUIntDic editedState = new StringUIntDic() {
    {"HP", 350},             
    {"MaxMP", 200},          
    {"Power",0},
    {"Intellect",0},
    {"Defence",0},
    {"Resistance",0},
    {"Indomitable",0},
    {"CriticalPoint",0},
    {"Luck", 0},
    {"CriticalProb",0},         //치명타 확률 : 치명타가 일어날 가능성
    {"SkillPercent", 300},
    };

    private int[] moveSKillCodes = new int[2]{ -1, -1 };

    private int[,] settingSkillCodes = new int[8, 3] {
        {0, 0, 0 },
        {0, 0, 0 },
        {0, 0, 0 },
        {0, 0, 0 },
        {0, 0, 0 },
        {0, 0, 0 },
        {0, 0, 0 },
        {0, 0, 0 },
    };

    private bool isFlying;

    private Dictionary<string, float>[] conditionStatusChange = new Dictionary<string, float>[11]{
        new Dictionary<string, float>(){{ "normal" , 1} },
        new Dictionary<string, float>(){{ "normal" , 1} },
        new Dictionary<string, float>(){{ "normal" , 1} },
        new Dictionary<string, float>(){{ "normal" , 1} },
        new Dictionary<string, float>(){{ "normal" , 1} },
        new Dictionary<string, float>(){{ "normal" , 1} },
        new Dictionary<string, float>(){{ "normal" , 1} },
        new Dictionary<string, float>(){{ "normal" , 1} },
        // 이 아래는 특수한 디버프
        new Dictionary<string, float>(){{ "normal" , 1} },
        new Dictionary<string, float>(){{ "normal" , 1} },
        new Dictionary<string, float>(){{ "normal" , 1} }
    };

    private IEnumerator courutine;
    private IEnumerator[] MoveSkillCoroutine = new IEnumerator[6];

    [ContextMenu("Send String to LanguageManager")]
    public void SaveString()
    {
        List<string> list = new List<string>() { };
        foreach (Skills i in moveSkills)
        {
            list.Add(i.skillName);
            list.Add(i.skillInfo);
        }
        GameObject.Find("GameManager").GetComponent<LanguageManager>().SetTextList(("Playable Move Skills"), list);
    }

    [ContextMenu("Get String to LanguageManager")]
    public void LoadString()
    {
        List<string> list = GameObject.Find("GameManager").GetComponent<LanguageManager>().GetTextList(("Playable Move Skills"));
        int j = 0;
        for (int i = 0; i < list.Count; i += 2)
        {
            moveSkills[j].skillName = list[i];
            moveSkills[j].skillInfo = list[i + 1];
            j++;
        }
    }

    protected new void Awake()
    {
        state = new StringUIntDic() {
    {"Level", 1},            //총 레벨 : 스텟 포인트 조정 시 필요
    {"MaxHP", 350},          //최대 체력 : 가질 수 있는 체력의 최대치
    {"HP", 350},             //체력 : 공격을 받아낼 수 있는 정도. 0이 되면 게임 오버
    {"Barrior",0},           //보호막 : 체력 대신 소모되는 스테이터스
    {"MaxMP", 200},          //최대 마나 : 가질 수 있는 마나의 최대치
    {"MP", 200},             //마나 : 스킬을 사용할 수 있는 자원
    {"EXP", 0},              //경험치 : 일정량이 오르면 레벨이 증가한다
    {"UltimatePoint", 0},    //궁극기 포인트 : 모두 모으면 궁극기 가동 가능
    {"SoulPoint", 0},        //영기 : 죽이거나 쓰러트려서 모은 결정체. 힘의 결정체와 합쳐 그 힘을 실체화할 수 있다
    {"Power", 30},           //힘 : 주로 물리 피해를 결정하는 능력치
    {"Intellect", 25},       //지력 : 주로 마법 피해를 결정하는 능력치
    {"Defence", 40},         //방어력 : 물리 피해 감소력
    {"Resistance", 30},      //저항력 : 마법 피해 감소력
    {"Indomitable", 0},      //불굴 : cc기 시간과 위력을 감소시키는 정도. 단위는 %
    {"CriticalPoint", 125},  //치명타 계수 : 치명타가 터질 시 데미지에 곱해지는 정도. 단위는 %
    {"Luck", 0},             //운 : 랜덤한 요소의 보정치. 단위는 %
    {"SkillPercent", 300},   //스킬데미지증감수 : 스킬이 가하는 데미지 증가
    };
        particular = GetComponent<ParticularEvent>();
        DontDestroyOnLoad(gameObject);
        InvokeRepeating("AutoHealMP", 1.75f, 1.75f);
    }

    private void AutoHealMP() 
    {
        if (GameObject.Find("Player") != null)
        {
            uint healMP = (uint)Mathf.RoundToInt(state["MaxMP"] * 0.01f);
            state["MP"] = state["MP"] + healMP > state["MaxMP"] ? state["MaxMP"] : state["MP"] + healMP;
        }
    }

    private void Update()
    {     
        editedState["Power"] = (uint)Mathf.RoundToInt(characterStateEditValue[selectedCharacter, 0] * state["Power"] * (conditionStatusChange[0].Sum(x => x.Value) > 0 ? conditionStatusChange[0].Sum(x => x.Value) : 0));
        editedState["Intellect"] = (uint)Mathf.RoundToInt(characterStateEditValue[selectedCharacter, 1] * state["Intellect"] * (conditionStatusChange[1].Sum(x => x.Value) > 0 ? conditionStatusChange[1].Sum(x => x.Value) : 0));
        editedState["Defence"] = (uint)Mathf.RoundToInt(characterStateEditValue[selectedCharacter, 2] * state["Defence"] * (conditionStatusChange[2].Sum(x => x.Value) > 0 ? conditionStatusChange[2].Sum(x => x.Value) : 0));
        editedState["Resistance"] = (uint)Mathf.RoundToInt(characterStateEditValue[selectedCharacter, 3] * state["Resistance"] * (conditionStatusChange[3].Sum(x => x.Value) > 0 ? conditionStatusChange[3].Sum(x => x.Value) : 0));
        editedState["Indomitable"] = (uint)Mathf.RoundToInt(characterStateEditValue[selectedCharacter, 4] * state["Indomitable"] * (conditionStatusChange[4].Sum(x => x.Value) > 0 ? conditionStatusChange[4].Sum(x => x.Value) : 0));
        editedState["CriticalPoint"] = (uint)Mathf.RoundToInt(characterStateEditValue[selectedCharacter, 5] * state["CriticalPoint"] * (conditionStatusChange[5].Sum(x => x.Value) > 0 ? conditionStatusChange[5].Sum(x => x.Value) : 0));
        editedState["Luck"] = (uint)Mathf.RoundToInt(state["Luck"] * (conditionStatusChange[6].Sum(x => x.Value) > 0 ? conditionStatusChange[6].Sum(x => x.Value) : 0));
        editedState["CriticalProb"] = (uint)Mathf.RoundToInt(characterStateEditValue[selectedCharacter, 6] * state["Luck"] * (conditionStatusChange[7].Sum(x => x.Value) > 0 ? conditionStatusChange[7].Sum(x => x.Value) : 0));

        editedState["MaxHP"] = (uint)Mathf.RoundToInt(state["MaxHP"] * (conditionStatusChange[8].Sum(x => x.Value) > 0 ? conditionStatusChange[0].Sum(x => x.Value) : 0));
        editedState["MaxMP"] = (uint)Mathf.RoundToInt(state["MaxMP"] * (conditionStatusChange[9].Sum(x => x.Value) > 0 ? conditionStatusChange[1].Sum(x => x.Value) : 0));
        editedState["SkillPercent"] = (uint)Mathf.RoundToInt(state["SkillPercent"] * (conditionStatusChange[10].Sum(x => x.Value) > 0 ? conditionStatusChange[2].Sum(x => x.Value) : 0));
        isDied = state["HP"] <= 0;

        state["Barrior"] = (uint)Mathf.RoundToInt((Barriors.Count > 0 ? Barriors.Sum(x => x.barrior) : 0));
        for (int i = Barriors.Count-1; i >= 0 ; i--) {
            BarriorInfo tmp = Barriors[i];
            tmp.time -= Time.deltaTime;
            Barriors[i] = tmp;
            if (Barriors[i].time <= 0 || Barriors[i].barrior <= 0) {
                Barriors.RemoveAt(i);
            }
        }
    }
    public override void BarriorAdd(string code, uint barrior, float time)
    {
        int index = Barriors.FindIndex((BarriorInfo tmp) => (code == tmp.code));
        BarriorInfo tmp;
        if (index == -1)
        {
            tmp.code = code;
            tmp.barrior = barrior;
            tmp.time = time;
            Barriors.Add(tmp);
            return;
        }
        tmp = Barriors[index];
        tmp.barrior = (tmp.barrior > barrior ? tmp.barrior : barrior);
        tmp.time = (tmp.time > time ? tmp.time : time);
        Barriors[index] = tmp;
    }

    public override void Heal(int heal)
    {
        ConditionCheck incur = conditionList.Find(x => x.con.SerialCode.Equals("_Incurable"));
        ConditionCheck overh = conditionList.Find(x => x.con.SerialCode.Equals("_Overheal"));
        if (incur.point != 0 || overh.point != 0) heal *= (int)(1 - (0.25f * incur.point) + (0.25f * overh.point));
        if (state["HP"] + heal > state["MaxHP"]) state["HP"] = state["MaxHP"];
        else state["HP"] += (uint)heal;
        if (GameObject.Find("Player") != null) particular.SummonFlatText(player.position).Changetext(heal.ToString(), 6, FontStyle.Italic, Color.green);
    }

    public override void Damage(DamageInfo damageInfo ,bool isSpawnDamageText)
    {
        uint damag = (uint)Mathf.RoundToInt((damageInfo.damage * (damageInfo.criticalPercent ? (damageInfo.criticalPoint * 0.01f) : 1)) + (state["MaxHP"] * damageInfo.maxHPDamage * 0.01f) + (state["HP"] * damageInfo.nowHPDamage * 0.01f) + ((state["MaxHP"] - state["HP"]) * damageInfo.lostHPDamage * 0.01f));
        damag = (uint)Mathf.RoundToInt(damag * ((100f - elementalTolerance[ElementalCodeGet(damageInfo.elemental)]) / 100f));
        if (damageInfo.damageChangeFunc != null && damageInfo.damageChangeFunc.Count > 0 && damageInfo.damageType != DamageType.True)
        {
            foreach (var i in damageInfo.damageChangeFunc)
                damag = i(this, damag);
        }
        if (damageInfo.conditionCheck != null && damageInfo.conditionCheck.Length > 0) 
            foreach (ConditionCheckwithString i in damageInfo.conditionCheck)
            {
                Condition condition = particular.FindCondition(i.con);
                ConditionChange(condition, i.point, i.time);
            }
        if (GameObject.Find("Player") != null && !conditionList.Exists(x => x.con.SerialCode.Equals("_Unstoppable"))) player.GetComponent<PlayerControl>().setCCInfo(new CCinfo(damageInfo.cc, damageInfo.ccTime, damageInfo.ccStrength), damageInfo.order);
        bool isProtection = conditionList.Exists(x => x.con.SerialCode.Equals("_Protection"));
        if (isProtection) return;    
        long realguard;
        switch (damageInfo.damageType)
        {            
            case DamageType.Physics:
                realguard = (editedState["Defence"] - damageInfo.penetrate > 0) ? (editedState["Defence"] - damageInfo.penetrate) : 0;
                damag = (uint)Mathf.RoundToInt(damag - (damag * realguard / (float)(realguard + GuardConstant)));
                break;
            case DamageType.Magic:
                realguard = (editedState["Resistance"] - damageInfo.penetrate > 0) ? (editedState["Resistance"] - damageInfo.penetrate) : 0;
                damag = (uint)Mathf.RoundToInt(damag - (damag * realguard / (float)(realguard + GuardConstant)));
                break;
        }
        if (damageInfo.damageChangeEvent != null) damageInfo.damageChangeEvent.Invoke(this, damag);
        if (damag <= 0) isSpawnDamageText = false;
        if (state["HP"] + state["Barrior"] - damag < 0)
        {
            if (isSpawnDamageText) particular.SummonFlatText(player.position + 1.2f * new Vector3(-0.75f + UnityEngine.Random.Range(-0.1f, 0.1f), 0.3f + UnityEngine.Random.Range(-0.1f, 0.1f), -2), player).Changetext((damag - state["Barrior"]).ToString(), damageInfo.criticalPercent ? 10 : 6, FontStyle.Italic, ElementalColors(damageInfo.elemental));
            if (isSpawnDamageText && state["Barrior"] > 0) particular.SummonFlatText(player.position + 1.2f * new Vector3(-0.75f + UnityEngine.Random.Range(-0.1f, 0.1f), 0.3f + UnityEngine.Random.Range(-0.1f, 0.1f) + 0.5f, -2), player).Changetext((state["Barrior"]).ToString(), damageInfo.criticalPercent ? 10 : 6, FontStyle.Italic, new Color(0, 1, 1));
            state["HP"] = 0;         
            for (int i = Barriors.Count - 1; i >= 0; i--)
            {
                Barriors.RemoveAt(i);
            }
        }
        else if (state["Barrior"] > 0)
        {
            uint damageCount = damag;
            for (int i = 0; i <= Barriors.Count; i++)
            {
                BarriorInfo tmp;
                if (i >= Barriors.Count) break;
                if (Barriors[i].barrior > damageCount)
                {
                    tmp = Barriors[i];
                    tmp.barrior -= damageCount;
                    Barriors[i] = tmp;
                    damageCount = 0;
                    break;
                }
                tmp = Barriors[i];
                damageCount -= tmp.barrior;
                tmp.barrior = 0;
                Barriors[i] = tmp;
            }
            state["HP"] -= (uint)damageCount;
            if (isSpawnDamageText && damageCount > 0) particular.SummonFlatText(player.position + 1.2f * new Vector3(-0.75f + UnityEngine.Random.Range(-0.1f, 0.1f), 0.3f + UnityEngine.Random.Range(-0.1f, 0.1f), -2), player).Changetext((damageCount).ToString(), damageInfo.criticalPercent ? 10 : 6, FontStyle.Italic, ElementalColors(damageInfo.elemental));
            if (isSpawnDamageText) particular.SummonFlatText(player.position + 1.2f * new Vector3(-0.75f + UnityEngine.Random.Range(-0.1f, 0.1f), 0.3f + UnityEngine.Random.Range(-0.1f, 0.1f) + 0.5f, -2), player).Changetext((damag - damageCount).ToString(), damageInfo.criticalPercent ? 10 : 6, FontStyle.Italic, new Color(0, 1, 1));
        }
        else {
            if (isSpawnDamageText) particular.SummonFlatText(player.position + 1.2f * new Vector3(-0.75f + UnityEngine.Random.Range(-0.1f, 0.1f), 0.3f + UnityEngine.Random.Range(-0.1f, 0.1f), -2), player).Changetext(damag.ToString(), damageInfo.criticalPercent ? 10 : 6, FontStyle.Italic, ElementalColors(damageInfo.elemental));
            state["HP"] -= (uint)damag; 
        }
        isHitting = false;
    }

    public override void Execute(bool isSpawnDamageText)
    {
        if (isSpawnDamageText) particular.SummonFlatText(player.position + 1.2f * new Vector3(player.GetComponent<SpriteRenderer>().flipX ? 0.75f : -0.75f + UnityEngine.Random.Range(-0.1f, 0.1f), 0.3f + UnityEngine.Random.Range(-0.1f, 0.1f), -2)).Changetext(2, 10, FontStyle.Italic, Color.red);
        state["HP"] = 0;
        for (int i = Barriors.Count - 1; i >= 0; i--)
        {
            Barriors.RemoveAt(i);
        }
    }

    public void UltimatePointUp(int point, bool coolIgnore = false, float cooltime = 5f) {
        if (coolIgnore)
        {
            state["UltimatePoint"] = (uint)((state["UltimatePoint"] + point > 250) ? 250 : state["UltimatePoint"] + point);
            return;
        }
        else if (ultimitCooltime <= 0) {
            state["UltimatePoint"] = (uint)((state["UltimatePoint"] + point > 250) ? 250 : state["UltimatePoint"] + point);
            StartCoroutine("UltimateCool", cooltime);
        }
    }

    public void UsingMP(int cost)
    {
        state["MP"] = (uint)(state["MP"] - cost >= 0 ? state["MP"] - cost : 0);
    }

    public void UsingHP(int cost)
    {
        state["HP"] = (uint)(state["HP"] - cost >= 1 ? state["HP"] - cost : 1);
    }

    public void UsingUT()
    {
        state["UltimatePoint"] = 0;
    }

    public uint GetEditedState(string name)
    {
        return editedState[name];
    }

    public bool GetPlayerHasCondition(string value)
    {
        return conditionList.Exists(x => x.con.SerialCode.Equals(value));
    }

    public bool GetIsFlying() {
        return isFlying;
    }

    public void SetChatacter(int n)
    {
        selectedCharacter = n;
    }

    public int GetCharacter()
    {
        return selectedCharacter;
    }

    public void SetSettingSkillCode(int characterCode, int order, int value)
    {
        settingSkillCodes[characterCode, order] = value;
    }

    public int GetSettingSkillCode(int characterCode, int order)
    {
        return settingSkillCodes[characterCode, order];
    }

    public void SetConditionStatusChange(int statusOrder, string conditionCode, float value)
    {
        conditionStatusChange[statusOrder][conditionCode] = value;
    }

    public void RemoveConditionStatusChange(int statusOrder, string conditionCode)
    {
        conditionStatusChange[statusOrder].Remove(conditionCode);
    }

    public float GetConditionChange(int statusOrder, string conditionCode)
    {
        return conditionStatusChange[statusOrder][conditionCode];
    }

    public void SetPlayer(PlayerControl player) 
    {
        this.player = player.transform;
        playerControl = player;
    }

    public void SetIsHit(bool hit)
    {
        isHitting = hit;
    }

    public void ResetState()
    {
        state["HP"] = state["MaxHP"];
        state["MP"] = state["MaxMP"];
        state["UltimatePoint"] = 0;
        selectedCharacter = 0;
    }

    public bool GetIsUsingMoveSkill()
    {
        return isMoveSkillOn;
    }

    public bool GetMoveSkillsOn(int index)
    {
        return MoveSkillsOn[index];
    }

    public void StopCourutineWhenCCed()
    {
        if (courutine != null) StopCoroutine(courutine);
        for (int i = 0; i < 6; i++)
        {
            if (MoveSkillsOn[i]) StartCoroutine(SkillCooltime(i, moveSkills[i].skillCooltime));
            MoveSkillsOn[i] = false;
        }
        for (int i = 1; i < player.childCount; i++)
        {
            if(player.GetChild(i).tag != "NotChild") Destroy(player.GetChild(i).gameObject);
        }
        isFlying = false;
        isMoveSkillOn = false;
    }

    public Skills GetMoveSkillWithindex(int index) {
        return moveSkills[index];
    }

    public void SetMoveSkillCode(int order, int value)
    {
        moveSKillCodes[order] = value;
    }

    public int GetMoveSkillCode(int order)
    {
        return moveSKillCodes[order];
    }

    public float GetMoveSkillCooltime(int index, int firstorNow)
    {
        return MoveSkillsCooltime[index, firstorNow];
    }

    public void UsingMoveSkill()
    {
        int index = -1;

        MoveSkillCoroutine[0] = Slash();
        MoveSkillCoroutine[1] = Flash();
        MoveSkillCoroutine[2] = Cannon();
        MoveSkillCoroutine[3] = Fly();
        MoveSkillCoroutine[4] = Target();
        MoveSkillCoroutine[5] = Unstoppable();

        if (Input.GetButtonDown("MoveSkill1") && moveSKillCodes[0] != -1 && !isMoveSkillOn)
        {
            if (MoveSkillsCooltime[moveSKillCodes[0], 0] <= 0)  index = moveSKillCodes[0]; 
        }
        if (Input.GetButtonDown("MoveSkill2") && moveSKillCodes[1] != -1 && !isMoveSkillOn)
        {
            if (MoveSkillsCooltime[moveSKillCodes[1], 0] <= 0) index = moveSKillCodes[1]; 
        }
        if (index >= 0) StartCoroutine("MoveSkillUsing", index);
    }

    public void ResetCooltime()
    {
        for (int i = 0; i < 6; i++) MoveSkillsCooltime[i, 0] = 0;
    }

    public bool GetCharacterActivate(int index)
    {
        return CharacterGet[index];
    }


    private IEnumerator MoveSkillUsing(int i) {       
        float t = 0.15f;
        while (!transform.GetChild(selectedCharacter).GetComponent<CharacterBase>().GetIsAttaking() || (Input.GetButton("Attack") || !transform.GetChild(selectedCharacter).GetComponent<CharacterBase>().GetIsUsingSkill() || isMoveSkillOn) && t > 0)
        {
            yield return null;
            t -= Time.deltaTime;
        }
        if (t <= 0) goto Finish;
        yield return new WaitWhile(() => (!transform.GetChild(selectedCharacter).GetComponent<CharacterBase>().GetIsAttaking() || (Input.GetButton("Attack") || !transform.GetChild(selectedCharacter).GetComponent<CharacterBase>().GetIsUsingSkill() || isMoveSkillOn)));
        if (!transform.GetChild(selectedCharacter).GetComponent<CharacterBase>().GetIsAttaking() || (Input.GetButton("Attack") || !transform.GetChild(selectedCharacter).GetComponent<CharacterBase>().GetIsUsingSkill() || isMoveSkillOn)) goto Finish;
        
        MoveSkillsOn[i] = true;
        isMoveSkillOn = true;
        courutine = MoveSkillCoroutine[i];
        yield return StartCoroutine(courutine);
        isMoveSkillOn = false;    
        MoveSkillsOn[i] = false;
    Finish:
        yield return null;
    }

    private IEnumerator Slash()
    {

        playerControl.GetAnimator().SetInteger("Skills", 275);
        playerControl.GetAnimator().SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        player.GetComponent<Rigidbody2D>().drag = 0;

        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)) player.GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 65f;
        else player.GetComponent<Rigidbody2D>().velocity = (player.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right) * 65f;
        
        PlayerAttacks pa = Instantiate(moveSkillObjects[0], player.position + Vector3.back + Vector3.right * 0.75f * (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1), player.rotation, player).GetComponent<PlayerAttacks>();
        pa.transform.localScale = pa.transform.localScale * (playerControl.GetComponent<SpriteRenderer>().flipX ? -1 : 1);
        DamageInfo di = pa.GetDamage();
        di.damage = (uint)Mathf.RoundToInt(editedState["Power"] * 0.15f);
        pa.SetDamageInfo(di);

        yield return new WaitForSeconds(0.05f);
        Instantiate(moveSkillObjects[5], player.position + (Vector3)(player.GetComponent<Rigidbody2D>().velocity * 0.07f), Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(player.GetComponent<Rigidbody2D>().velocity.y, player.GetComponent<Rigidbody2D>().velocity.x)));
        yield return new WaitForSeconds(0.15f);

        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerControl.GetAnimator().SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", false));
        player.GetComponent<Rigidbody2D>().gravityScale = 6;


        StartCoroutine(SkillCooltime(0, moveSkills[0].skillCooltime));
    }
    private IEnumerator Flash()
    {        
        Instantiate(moveSkillObjects[1], player.position, player.rotation);
        
        Vector2 toFlash;

        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)) toFlash = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        else toFlash = (player.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right);

        Instantiate(moveSkillObjects[6], player.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(-toFlash.y, -toFlash.x)));

        RaycastHit2D raycastHit = Physics2D.BoxCast(player.transform.position + (Vector3)(toFlash * 8), new Vector3(0.5f, 1f, 0), 0f, Vector2.down, 0f, LayerMask.GetMask("Ground"));
        if (raycastHit.collider != null) {
            raycastHit = Physics2D.BoxCast(player.transform.position, new Vector3(0.5f, 1f, 0), 0f, toFlash, 8f, LayerMask.GetMask("Ground"));
            player.transform.position = new Vector3(raycastHit.point.x , raycastHit.point.y, -1.1f);
        }
        else player.transform.position = player.transform.position + (Vector3)(toFlash * 8);
        player.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity.x * Vector2.right + Vector2.up * (player.GetComponent<Rigidbody2D>().velocity.y > 0 ? player.GetComponent<Rigidbody2D>().velocity.y : 0);
        Instantiate(moveSkillObjects[6], player.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(toFlash.y, toFlash.x)), player);
        
        yield return null;

        StartCoroutine(SkillCooltime(1, moveSkills[1].skillCooltime));
    }
    private IEnumerator Cannon()
    {
        playerControl.GetAnimator().SetInteger("Skills", 275);
        playerControl.GetAnimator().SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", true));
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        player.GetComponent<Rigidbody2D>().drag = 0;

        Vector2 toShot;
        if ((Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)) toShot = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        else toShot = (player.GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right);

        player.GetComponent<Rigidbody2D>().velocity = toShot * 20;

        PlayerAttacks pa = Instantiate(moveSkillObjects[2], player.position + Vector3.back, player.rotation, player).GetComponent<PlayerAttacks>();
        DamageInfo di = pa.GetDamage();
        di.elemental = Elemental.Fire;
        di.damage = (uint)Mathf.RoundToInt(editedState["Power"] * 1.1f);
        pa.SetDamageInfo(di);

        yield return new WaitWhile(() => Physics2D.BoxCast(player.position, new Vector3(0.8f, 1.8f, 0), 0f, toShot, 0.2f, LayerMask.GetMask("Ground")).collider == null);

        playerControl.GetAnimator().SetBool("Skill On", playerControl.ChangeMotionCheck("Skill On", false));
        if(pa != null)Destroy(pa.gameObject);
        player.GetComponent<Rigidbody2D>().gravityScale = 6;

        StartCoroutine(SkillCooltime(2, moveSkills[2].skillCooltime));
    }
    private IEnumerator Fly()
    {
        isFlying = true;
        GameObject g = Instantiate(moveSkillObjects[3], player);
        player.GetComponent<Rigidbody2D>().gravityScale = 0.01f;
        player.GetComponent<Rigidbody2D>().drag = 0;
        player.GetComponent<Rigidbody2D>().velocity = player.GetComponent<Rigidbody2D>().velocity.x * Vector2.right + Vector2.up * 0.0001f;
        isMoveSkillOn = false;
        yield return new WaitForSeconds(3f);
        Destroy(g);
        player.GetComponent<Rigidbody2D>().gravityScale = 6;
        isFlying = false;
        MoveSkillsOn[3] = false;
        StartCoroutine(SkillCooltime(3, moveSkills[3].skillCooltime));
    }
    private IEnumerator Target()
    {
        var objects = GameObject.FindGameObjectsWithTag("Enemy").ToList();

        // LINQ 메소드를 이용해 가장 가까운 적을 찾습니다.
        var neareastObject = objects
            .OrderBy(obj =>
            {
                return Vector3.Distance(player.position, obj.transform.position);
            })
        .FirstOrDefault();
        if (neareastObject != null && Vector3.Distance(player.position, neareastObject.transform.position) < 17f)
        {
            Instantiate(moveSkillObjects[1], player.position, player.rotation);
            player.position = (neareastObject.transform.position + Vector3.forward * (player.position.z - neareastObject.transform.position.z)) - (player.GetComponent<SpriteRenderer>().flipX ? 1.75f : -1.75f) * Vector3.left;
            Instantiate(moveSkillObjects[7], neareastObject.transform.position, Quaternion.Euler(0,0,0), neareastObject.transform);
            StartCoroutine(SkillCooltime(4, moveSkills[4].skillCooltime));
        }
        else 
        {
            particular.SummonFlatText(player.position +  new Vector3(0, 0.36f, -2), player).Changetext(3, 5, FontStyle.Italic, Color.white);
            StartCoroutine(SkillCooltime(4, 0.2f));
        }
        yield return null;
    }
    private IEnumerator Unstoppable()
    {
        playerControl.ResetCCInfo();
        Instantiate(moveSkillObjects[4], player.position + Vector3.up * 0.5f + Vector3.back * 3, player.rotation, player);
        ConditionChange(particular.FindCondition("_Unstoppable"), 1, 1.5f);
        yield return null;
        StartCoroutine(SkillCooltime(5, moveSkills[5].skillCooltime));
    }
    
    private IEnumerator SkillCooltime(int skillcode, float time)
    {
        if(MoveSkillsCooltime[skillcode, 1] < time) MoveSkillsCooltime[skillcode,1] = time;
        MoveSkillsCooltime[skillcode,0] = time;
        do
        {
            yield return null;
            MoveSkillsCooltime[skillcode, 0] -= Time.deltaTime;
        }
        while (MoveSkillsCooltime[skillcode, 0] > 0);
        MoveSkillsCooltime[skillcode, 0] = 0;
        MoveSkillsCooltime[skillcode, 1] = 0;
    }

    IEnumerator UltimateCool(float cooltime) {
        ultimitCooltime = cooltime;
        do
        {
            yield return null;
            ultimitCooltime -= Time.deltaTime;
        }
        while (ultimitCooltime > 0);
        ultimitCooltime = 0;
    }


}
