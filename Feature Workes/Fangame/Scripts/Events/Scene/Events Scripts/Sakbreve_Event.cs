using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sakbreve_Event : MonoBehaviour
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
    public GameObject SakBreve;
    public GameObject talking_Icon;
    public Sprite sakstory;
    public Sprite tamimg;
    public Sprite sakbreve;
    public Sprite sakgun;
    public Sprite sakgun_Smile;
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
        if (SakBreve.transform.position.x > -7.183f && movechung)
        {
            SakBreve.transform.Translate(Vector3.left * 3f * Time.deltaTime);
        }
    }

    // Update is called once per frame
    IEnumerator EventStart()
    {
        pc.ispause = true;
        if (!gmm.getSEvent(eventNum))
        {
            talking_Icon.SetActive(false); 
            pl_inter.SetActive(false);            
            teu_obj.SetActive(true);
            string n = "???";
            string a = "우리마을에 새롭게 합격된 용사가 있다니.";

            teu.illust.sprite = tamimg;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "???";
            a = "예정대로, 내가 축하하러 가야겠군.";

            teu.illust.sprite = tamimg;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            teu_obj.SetActive(false);
            SakBreve.GetComponent<SpriteRenderer>().flipX = true;
            SakBreve.GetComponent<Animator>().SetTrigger("walk");
            movechung = true;
            yield return new WaitWhile(() => SakBreve.transform.position.x > -7.183f);
            SakBreve.GetComponent<Animator>().SetTrigger("idle");
            teu_obj.SetActive(true);

            n = "삭군";
            a = "...!!!";

            teu.illust.sprite = sakgun;
            teu.name.text = n;
            teu.talking.text = "";
           
            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            teu.illust_full.sprite = sakstory;
            for (float i = 0; i <= 1; i += 0.01f)
            {
                teu.illust_full.color = new Color(1, 1, 1, i);
                yield return new WaitForSeconds(0.01f);
            }

            n = "???";
            a = "넌 나를 알고 있겠지?";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭군";
            a = "당신은.....";

            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));
            teu.illust_full.sprite = tamimg;

            n = "삭블이브";
            a = "나는 삭블이브. 너도 알다시피 용사다.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";
            

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));
            n = "삭군";
            a = "그.. 현재 남아있는 최강의 용사이자, 온 국민의 지지를 받는 그분 맞으시죠?";

            teu.illust.sprite = sakgun;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "그렇게까지 거창하게 부를 필요는 없고, 그냥 용사 정도면 충분해.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "아무튼 새로운 용사가 된 것을 축하하마.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭군";
            a = "네!";

            teu.illust.sprite = sakgun_Smile;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "그럼 일단, 해야하는 걸 알려줄께.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "저 끝에는 4개의 공간으로 갈 수 있는 포탈이 있어.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "최근, 몬스터가 많이 발생한다는건 알고 있지?";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "고대인들이 봉인한, 팬텀의 부활 때문이야.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "우리는 그것에 대해, 새로운 재 봉인 체계를 만들려 해.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "우리는 강해진 몬스터와 싸우며, 동시에 그 몬스터가 강해진 원인인";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "팬텀의 '오브'를 모아야 해.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "그 몬스터들을 쓰러트리면, 오브가 자동적으로 모여.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "봉인은 봉인된 팬텀의 힘을 사용해 계속해서 유지하는거라,";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "팬텀의 적당한 오브를 사용해 봉인을 강화할 예정이야.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));
            

            n = "삭블이브";
            a = "너가 모아야 할 양은 총 10개, 10개의 오브를 모은 후 팬텀의 봉인 장소로 가는거야.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "뭘 해야할지 알겠지?";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭군";
            a = "네!";

            teu.illust.sprite = sakgun_Smile;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "그리고 왠만하면 탐색도 여러번 해봐.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "너에게 유용한 스킬이 떨어진 곳이 있을 수 있어.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = "일단 나도 너에게 스킬 하나를 줄께. 확인해봐.";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭블이브";
            a = ".. 말이 길어졌네. 자, 모험을 하러 떠나봐!";

            teu.illust.sprite = sakbreve;
            teu.name.text = n;
            teu.talking.text = "";


            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));

            n = "삭군";
            a = "네!";

            teu.illust.sprite = sakgun_Smile;
            teu.name.text = n;
            teu.talking.text = "";

            for (int i = 0; i < a.Length; i++)
            {
                teu.talking.text += a[i];
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));
            gmm.GetComponent<PlayerState>().setCanSelectSkills(1, true);

            teu.illust.sprite = tamimg;
            teu_obj.SetActive(false);
            talking_Icon.SetActive(true);
            pl_inter.SetActive(true);
            gmm.setSEvent(eventNum, true);
        }
        pc.ispause = false;
    }
}
