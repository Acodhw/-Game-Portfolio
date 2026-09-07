using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TalkEvent : MonoBehaviour
{
    TalkSystem _talkSystem;
    public List<TalkData> talking;
    bool canTalk = false;
    bool talkCool = true;
    // Start is called before the first frame update
    void Start()
    {
        _talkSystem = GameObject.Find("PlayerPrefeb").GetComponent<TalkSystem>();
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag.Equals("Player"))
        {
            canTalk = true;
        }
    }

    void OnTriggerExit(Collider hit)
    {
        if (hit.tag.Equals("Player"))
        {
            canTalk = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _talkSystem.InteractionMessage(canTalk);
        if (canTalk && Input.GetKey(KeyCode.Z)) {
            _talkSystem.SandTalk(talking);            
            canTalk = false;
        }
    }

    public void ChangeTalk(List<TalkData> t) {
        talking = t.ToList();
    }
}
