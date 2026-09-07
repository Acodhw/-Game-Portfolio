using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoticutiApear : MonoBehaviour
{
    public int eventNum;
    private PlayerControl pc;

    [System.Serializable]
    public class TalkingEventUI
    {
        public Text name;
        public Text talking;
        public Image illust;
        public Image illust_full;
    }

    public TalkingEventUI teu;
    public GameObject teu_obj;
    public GameObject pl_inter;
    public GameObject poti;
    public Sprite tamimg;
    public Sprite Maple;
    public GameObject BOSS_inter;
    int talkingnum;
    bool movechung;

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
            pl_inter.SetActive(false);
            teu_obj.SetActive(true);
            string n = "마인애플";
            string a = "4개의 사슬을 모두 풀었군...";

            teu.illust.sprite = Maple;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "마인애플";
            a = "자, 그러면";

            teu.illust.sprite = Maple;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "마인애플";
            a = "나의 신수의 봉인이 풀렸겠군!";

            teu.illust.sprite = Maple;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "마인애플";
            a = "나와라, 포티큐티, 새로운 도전자를 시험하라!";

            teu.illust.sprite = Maple;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));
            teu.illust.sprite = tamimg;
            teu_obj.SetActive(false);
            poti.GetComponent<Animator>().SetTrigger("attack");
            yield return new WaitForSeconds(2f);
            BOSS_inter.SetActive(true);
            pl_inter.SetActive(true);
            gmm.setSEvent(eventNum, true);
        }
        pc.ispause = false;
    }
}
