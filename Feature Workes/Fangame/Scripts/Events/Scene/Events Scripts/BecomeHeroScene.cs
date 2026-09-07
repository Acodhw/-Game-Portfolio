using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BecomeHeroScene : MonoBehaviour
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
    public GameObject chungGO;
    public Sprite StrongChicken;
    public Sprite sakgun;
    public Sprite sakgun_Smile;
    public Sprite Chung;
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
        if (chungGO.transform.position.x < -0.07f && movesak)
        {
            chungGO.transform.Translate(Vector3.right * 5f * Time.deltaTime);
        }
    }

    IEnumerator EventStart()
    {
        
        pl_inter.SetActive(false);
        yield return new WaitForSeconds(1f);
        teu_obj.SetActive(true);       
        string n = "강하닭";
        string a = "오, 왔군요~. 시험 결과가 성공적이였나 보군요~.";

        teu.illust.sprite = StrongChicken;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "강하닭";
        a = "여기 모인 분들은 삭군이를 축하하러 온 것이죠~? 삭군은 새로운 용사가 되었습니다~!";

        teu.illust.sprite = StrongChicken;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "삭로";
        a = "축하한다";

        teu.illust.sprite = tranimg;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.5f);

        n = "노란사과";
        a = "삭블이브를 볼 수 있겠지, 부럽다.";

        teu.illust.sprite = tranimg;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.5f);

        n = "방벽그림";
        a = "오늘부로우리마을에강한용사가둘이나생기겠군매우좋아좋아";

        teu.illust.sprite = tranimg;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.5f);

        n = "감자님";
        a = ".... 그런가... 축하... 아.. 좋아질 수도.....";

        teu.illust.sprite = tranimg;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(0.5f);

        n = "???";
        a = "삭군아!";

        teu.illust.sprite = tranimg;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));
        teu_obj.SetActive(false);
        chungGO.GetComponent<Animator>().SetTrigger("walk");
        movesak = true;
        yield return new WaitWhile(() => chungGO.transform.position.x < -0.756f);
        chungGO.GetComponent<Animator>().SetTrigger("idle");
        teu_obj.SetActive(true);

        n = "삭군";
        a = "청삭아!";

        teu.illust.sprite = sakgun_Smile;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "청삭이";
        a = "드디어 용사가 되었구나! 축하해!";

        teu.illust.sprite = Chung;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "삭군";
        a = "고마워.";

        teu.illust.sprite = sakgun_Smile;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "삭군";
        a = "내 꿈은 이제부터야! 반드시 삭블이브같은 용사가 되겠어!";

        teu.illust.sprite = sakgun;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "청삭이";
        a = "그래. 힘내!";

        teu.illust.sprite = Chung;
        teu.name.text = n;
        teu.talking.text = "";

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
        yield return new WaitForSeconds(1f);

        fade.gameObject.SetActive(true);
        for (float i = 0; i <= 1; i += 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }

        LoadingScene.LoadScene("Appear_Ink");
        pc.ispause = false;
    }
}
