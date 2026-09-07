using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCheck : MonoBehaviour
{
    public List<TalkData> ChangeingTalk;
    public AudioSource source;
    public AudioClip doorOpen;
    public TalkEvent hdmiTalk;
    public GameObject door;
    public PuzzleCheck p1;
    public PuzzleCheck p2;
    bool eventon = false;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (p1.switchCheck && p2.switchCheck && !eventon)
        {
            eventon = true;
            source.clip = doorOpen;
            source.Play();
            door.SetActive(false);
            hdmiTalk.ChangeTalk(ChangeingTalk);
        }
    }
    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag.Equals("Player"))
        {
            LoadingSceneManager.LoadScene("MoniterBossRoom");
        }
    }
}
