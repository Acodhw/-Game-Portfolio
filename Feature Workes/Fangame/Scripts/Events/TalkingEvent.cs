using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkingEvent : MonoBehaviour
{

    [System.Serializable]
    public class TalkingDatas
    {
        public string[] name;
        public string[] talking;
        public Sprite[] illust;
        public Sprite[] Fullillust;
    }

    [System.Serializable]
    public class TalkingEventUI
    {
        public Text name;
        public Text talking;
        public Image illust;
        public Image illust_full;
    }

    public int npcNum;
    private GameManager gmm;
    public TalkingDatas[] td;
    public TalkingEventUI teu;
    public GameObject teu_obj;
    public GameObject pl_inter;
    public GameObject eventcheck;
    private PlayerControl pc;
    private bool istalking;
    private bool talking_start = false;
    private bool pushedZ;
    private bool cantalk = true;
    private int eventnum = 0;
    private int talkingnum = 0;

    void pushz() {
        if (Input.GetButton("Check") && !pushedZ)
            StartCoroutine("checkingpushz");
        
    }

    IEnumerator checkingpushz() {
        pushedZ = true;
        yield return new WaitForSeconds(0.005f);
        pushedZ = false;
    }

    IEnumerator cantalkcheck()
    {
        cantalk = false;
        yield return new WaitForSeconds(0.1f);
        cantalk = true;
    }

    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (PlayerPrefs.HasKey("tmpsaver" + npcNum))
            eventnum = PlayerPrefs.GetInt("tmpsaver" + npcNum);
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();        
    }

    // Update is called once per frame
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == pc.tag)
        {
            if (!talking_start)
            {
                eventcheck.SetActive(true);
                if (pushedZ && cantalk && !pc.ispause)
                {
                    talkingnum = 0;
                    eventnum = gmm.getEventCode(npcNum);
                    StartCoroutine("talking");                 
                    talking_start = true;
                    eventcheck.SetActive(false);
                    pl_inter.SetActive(false);
                    teu_obj.SetActive(true);
                    pc.ispause = true;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == pc.tag)
        {
            if (eventcheck.activeSelf)
            {
                eventcheck.SetActive(false);                
            }
        }
    }

    void Update() 
    {
        pushz();
        if (talking_start && pushedZ && !istalking && cantalk) {
            if (td[eventnum].talking[talkingnum] == ".NEVENT") {
                teu_obj.SetActive(false);
                pl_inter.SetActive(true);
                eventcheck.SetActive(true);
                eventnum += 1;
                gmm.setEventCode(npcNum, eventnum);
                talking_start = false;
                pc.ispause = false;
                StartCoroutine("cantalkcheck");
            }
            else if (td[eventnum].talking[talkingnum] == ".FINTALK")
            {
                teu_obj.SetActive(false);
                pl_inter.SetActive(true);
                eventcheck.SetActive(true);
                talking_start = false;
                pc.ispause = false;
                StartCoroutine("cantalkcheck");
            }
            else
            {          
                StartCoroutine("talking");
            }
        }
    }

    IEnumerator talking()
    {     
        istalking = true;
        teu.illust.sprite = td[eventnum].illust[talkingnum];
        teu.illust_full.sprite = td[eventnum].Fullillust[talkingnum];
        teu.name.text = td[eventnum].name[talkingnum];
        teu.talking.text = "";
        for (int i = 0; i < td[eventnum].talking[talkingnum].Length; i++) {
            teu.talking.text += td[eventnum].talking[talkingnum][i];
            yield return new WaitForSeconds(0.005f);
        }
        talkingnum += 1;
        istalking = false;
        StartCoroutine("cantalkcheck");
    }

    public void SaveEvent() {
        
    }

    public void LoadEvent()
    {

    }
}
