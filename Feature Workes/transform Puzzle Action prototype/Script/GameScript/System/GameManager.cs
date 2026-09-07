using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct stateInfo {
    public Transform owner;
    public int hp, maxhp, atk, def, damRedRate;
    public bool isStun;
    public StatusEffect[] havEff;
    public stateInfo(Transform o, int h, int mh, int a, int d, int drr, bool st, params StatusEffect[] e) {
        owner = o;
        hp = h;
        maxhp = mh;
        atk = a;
        def = drr;
        damRedRate = drr;
        isStun = st;
        havEff = e;
    }
}

public enum StatusEffect
{
    none,
    poison,
    regeneration,
}

public struct EffectInfo
{
    StatusEffect effect;
    float effectTime;
    int effectPower;
}

public enum CCInfo {
    none,
    stun, // 기절
    push, // 밀림
    airborne, // 공중에 뜸
}

[Serializable]
public struct DamageInfo
{
    public int damage;
    public bool isFixed;

    public CCInfo ccinfo;
    public float stunPower;

    public bool isCrit;
    public float critRate;

    public bool isContinousdamage;
    public float nexthitTime;

    public StatusEffect effect;
    public float effTime;

    public Vector2 attackDir;
    public Transform attackOwner;

    public Func<float, stateInfo, int> damageChangeFunc;
    public UnityEvent<float, stateInfo> damageAfterEvent;
}

public class GameManager : MonoBehaviour
{
    private const uint eventCount = 65535;
    private const int defenceRate = 100;
    private bool onCutScene = false;
    private bool[] eventsActed = new bool[eventCount];

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public int GetDefenceRate() { return defenceRate; }

    public void SetOnCutScene(bool ocs) { onCutScene = ocs; }
    public bool GetOnCutScene() { return onCutScene; }

    public void SetEventActed(int index, bool did) { eventsActed[index] = did;}
    public bool GetEventActed(int index) { return eventsActed[index]; }
}

static class YieldCache
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());
    private static readonly Dictionary<float, WaitForSecondsRealtime> _timeIntervalReal = new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;
        if (!_timeInterval.TryGetValue(seconds, out wfs))
            _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
        return wfs;
    }

    public static WaitForSecondsRealtime WaitForSecondsRealTime(float seconds)
    {
        WaitForSecondsRealtime wfsReal;
        if (!_timeIntervalReal.TryGetValue(seconds, out wfsReal))
            _timeIntervalReal.Add(seconds, wfsReal = new WaitForSecondsRealtime(seconds));
        return wfsReal;
    }
}
