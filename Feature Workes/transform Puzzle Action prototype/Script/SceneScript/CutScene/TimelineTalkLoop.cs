using Mono.Cecil.Cil;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTalkLoop : MonoBehaviour
{
    [SerializeField]
    private float loopAnimLength;
    [SerializeField]
    private float jumpTime;
    [SerializeField]
    private TalkingUI tu;

    public PlayableDirector director;

    private bool goNext;
    private bool onTalking;
    private bool finished;
    private TalkingEvents te;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        te = GameObject.Find("GameManager").GetComponent<TalkingEvents>();
    }

    // Update is called once per frame
    void Update()
    {
        if (goNext && !finished)
        {
            director.time = jumpTime;
            finished = true;
        }
    }

    public void TalkLoop(int code)
    {
        if (!onTalking)
        {
            StartCoroutine(WaitTalk(code));
            onTalking = true;
        }
        if(!goNext) director.time -= loopAnimLength;
    }

    IEnumerator WaitTalk(int code)
    {
        yield return StartCoroutine(tu.TalkEventOn(te.GetEvent(code), false));
        goNext = true;
    }
}
