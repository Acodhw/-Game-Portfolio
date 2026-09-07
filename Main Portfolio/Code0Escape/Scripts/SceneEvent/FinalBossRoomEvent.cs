using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossRoomEvent : MonoBehaviour
{
    public float firstEventTime;

    public AudioSource source;
    public AudioClip phase1;
    public AudioClip phase2;

    public Animator firstboss;
    public Animator secondboss;

    public string NextRoom;
    public GameObject Boss;
    public GameObject NextBoss;
    public float NextEventTime;
    public float LastEventTime;

    TalkSystem _talkSystem;
    public List<TalkData> talking;
    public List<TalkData> talking2;
    bool eventOn = false;
    bool secondeventOn = false;
    // Start is called before the first frame update
    void Start()
    {
        _talkSystem = GameObject.Find("PlayerPrefeb").GetComponent<TalkSystem>();
        StartCoroutine("FirstBossEvent");

    }

    // Update is called once per frame
    void Update()
    {
        if (Boss == null && !eventOn)
        {
            eventOn = true;
            _talkSystem.SandTalk(talking);
            StartCoroutine("nextEvent");
        }

        if (NextBoss == null && !secondeventOn)
        {
            secondeventOn = true;
            _talkSystem.SandTalk(talking2);
            StartCoroutine("LastEvent");
        }
    }

    IEnumerator FirstBossEvent()
    {
        yield return new WaitForSeconds(firstEventTime);
        firstboss.SetTrigger("NextEvent");
        yield return new WaitForSeconds(5.3f);
        source.clip = phase1;
        source.Play();
    }

    IEnumerator nextEvent()
    {
        source.Stop();
        yield return new WaitForSeconds(NextEventTime);
        secondboss.SetTrigger("NextEvent");
        yield return new WaitForSeconds(3);
        source.clip = phase2;
        source.Play();
    }

    IEnumerator LastEvent()
    {
        yield return new WaitForSeconds(LastEventTime);
        LoadingSceneManager.LoadScene(NextRoom);
    }
}
