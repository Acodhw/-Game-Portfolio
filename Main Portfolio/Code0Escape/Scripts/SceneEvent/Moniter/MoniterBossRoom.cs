using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoniterBossRoom : MonoBehaviour
{
    public string NextRoom;
    public GameObject Boss;
    public float NextEventTime;

    TalkSystem _talkSystem;
    public List<TalkData> talking;
    bool eventOn = false;
    // Start is called before the first frame update
    void Start()
    {
        _talkSystem = GameObject.Find("PlayerPrefeb").GetComponent<TalkSystem>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Boss == null && !eventOn) {
            eventOn = true;
            _talkSystem.SandTalk(talking);
            StartCoroutine("nextEvent");
        }
    }

    IEnumerator nextEvent() {
        yield return new WaitForSeconds(NextEventTime);
        LoadingSceneManager.LoadScene(NextRoom);
    }
}
