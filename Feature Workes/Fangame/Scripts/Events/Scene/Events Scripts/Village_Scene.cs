using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Village_Scene : MonoBehaviour
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
    public GameObject chungsak;
    public GameObject talking_Icon;
    public Sprite sakstory;
    public Sprite tamimg;
    public Sprite chung;
    public Sprite sakgun;
    int talkingnum;
    bool movechung;

    private GameManager gmm;

    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
    }

    private void Update()
    {
        if (chungsak.transform.position.x > -7.766f && movechung) 
        {
            chungsak.transform.Translate(Vector3.left * 3f * Time.deltaTime);
        }
    }

    // Update is called once per frame
    IEnumerator EventStart()
    {
        pc.ispause = true;
        if (!gmm.getSEvent(eventNum))
        {           
            pl_inter.SetActive(false);
            chungsak.GetComponent<SpriteRenderer>().flipX = true;
            chungsak.GetComponent<Animator>().SetTrigger("walk");
            movechung = true;
            yield return new WaitWhile(() => chungsak.transform.position.x > -7.766f);          
            chungsak.GetComponent<Animator>().SetTrigger("idle");
            teu_obj.SetActive(true);
            string n = "청삭이";
            string a = "훈련 마치고 오는 길이지?";

            teu.illust.sprite = chung;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭군";
            a = "그래. 오늘이 바로 시험날이니까.";

            teu.illust.sprite = sakgun;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭군";
            a = "과거 삭블이브에게 구해졋을 때, 그때부터 꾼 꿈이야.";

            teu.illust.sprite = sakgun;
            teu.name.text = n;
            teu.talking.text = "";
            teu.illust_full.sprite = sakstory;
            for (float i = 0; i <= 1; i += 0.01f)
            {
                teu.illust_full.color = new Color(1, 1, 1, i);
                yield return new WaitForSeconds(0.01f);
            }
            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "청삭이";
            a = "그분이 없었다면 이 마을도 살아있진 않았겠지.";

            teu.illust.sprite = chung;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "청삭이";
            a = "...";

            teu.name.text = n;
            teu.talking.text = "";            

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "청삭이";
            a = "분위기가 잠깐 무거워졌네. 힘내, 오늘 시험.";

            teu.illust.sprite = chung;
            teu.name.text = n;
            teu.talking.text = "";
            teu.illust_full.sprite = tamimg;

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));
            n = "삭군";
            a = "알았어!";

            teu.illust.sprite = sakgun;
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
            talking_Icon.SetActive(true);
            pl_inter.SetActive(true);
            gmm.setSEvent(eventNum, true);
        }
        pc.ispause = false;
    }
}
