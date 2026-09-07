using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_Before_Event : MonoBehaviour
{

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
    public GameObject sak;
    public Sprite maple;
    public Sprite sakgun;
    public Sprite sakgun_notgood;
    public Sprite tranimg;
    bool movesak = false;

    private GameManager gmm;
    public Image fade;
    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (sak.transform.position.x < -0.756f && movesak)
        {
            sak.transform.Translate(Vector3.right * 3f * Time.deltaTime);
        }
    }

    IEnumerator EventStart()
    {

        pl_inter.SetActive(false);
        teu_obj.SetActive(true);
        string n = "마인애플";
        string a = "이번 도전자인가?";

        teu.illust.sprite = tranimg;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "마인애플";
        a = "오거라";

        teu.illust.sprite = tranimg;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "삭군";
        a = "긴장된다...";

        teu.illust.sprite = sakgun_notgood;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));
        teu_obj.SetActive(false);
        sak.GetComponent<Animator>().SetTrigger("walk");
        movesak = true;
        yield return new WaitWhile(() => sak.transform.position.x < -0.756f);
        sak.GetComponent<Animator>().SetTrigger("idle");
        teu_obj.SetActive(true);

        n = "삭군";
        a = "저.. 전 삭군이라고 해요....";

        teu.illust.sprite = sakgun_notgood;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }

        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "마인애플";
        a = "...";

        teu.illust.sprite = tranimg;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "마인애플";
        a = "긴장 풀어.";

        teu.illust.sprite = tranimg;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "삭군";
        a = "?";

        teu.illust.sprite = sakgun_notgood;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "마인애플";
        a = "우리는 그렇게 엄격하지 않거든.";

        teu.illust.sprite = maple;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "삭군";
        a = "하.. 하지만";

        teu.illust.sprite = sakgun_notgood;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "마인애플";
        a = "나도 도전자가 편해야 대하기 쉽거든.";

        teu.illust.sprite = maple;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "삭군";
        a = "네.";

        teu.illust.sprite = sakgun;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "마인애플";
        a = "뭐 이번 도전은 내가 널 보호하기 때문에, 죽지는 않아. 걱정마.";

        teu.illust.sprite = maple;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "마인애플";
        a = "일단 이동하고 말해줄께.";

        teu.illust.sprite = maple;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(1f);

        fade.gameObject.SetActive(true);       
        gmm.GetComponent<PlayerState>().SetCan_Shot(true);
        gmm.GetComponent<PlayerState>().setCanSelectSkills(0, true);
        for (float i = 0; i <= 1; i += 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        
        gmm.setMovingPoint(new Vector3(-10.35f, -0.464f, -1));
        LoadingScene.LoadScene("Testing_Roby");      
        pc.ispause = false;
    }
}

