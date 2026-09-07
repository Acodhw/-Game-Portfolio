using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Only_Talking_Event : MonoBehaviour
{
    public int eventNum;
    private PlayerControl pc;
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

    public TalkingDatas td;
    public TalkingEventUI teu;
    public GameObject teu_obj;
    public GameObject pl_inter;
    int talkingnum;

    private GameManager gmm;

    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    IEnumerator EventStart()
    {
        pc.ispause = true;
        if (!gmm.getSEvent(eventNum))
        {
            teu_obj.SetActive(true);
            pl_inter.SetActive(false);


            while (talkingnum < td.talking.Length)
            {
                teu.illust.sprite = td.illust[talkingnum];
                teu.illust_full.sprite = td.Fullillust[talkingnum];
                teu.name.text = td.name[talkingnum];
                teu.talking.text = "";
                for (int i = 0; i < td.talking[talkingnum].Length; i++)
                {
                    teu.talking.text += td.talking[talkingnum][i];
                    yield return new WaitForSeconds(0.005f);
                }
                talkingnum += 1;
                yield return new WaitUntil(() => Input.GetButtonDown("Check"));
            }
            teu_obj.SetActive(false);
            pl_inter.SetActive(true);
            gmm.setSEvent(eventNum, true);
        }
        pc.ispause = false;
    }
}
