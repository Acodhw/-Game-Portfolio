using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTalkEvent : MonoBehaviour
{
    TalkSystem _talkSystem;
    public List<TalkData> talking;
    // Start is called before the first frame update
    void Start()
    {
        _talkSystem = GameObject.Find("PlayerPrefeb").GetComponent<TalkSystem>();
        _talkSystem.SandTalk(talking);
    }

}
