using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearInkMO : MonoBehaviour
{

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
    public Sprite Inkmo;
    public Sprite Inkmo_Eyed;
    public Sprite Inkmo_Eyed_dot;

    public SpriteRenderer inkmo;

    public Image fade;
    private GameManager gmm;
    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine("EventStart");
    }
    IEnumerator EventStart()
    {
        fade.gameObject.SetActive(true);
        for (float i = 1; i >= 0; i -= 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        fade.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        teu_obj.SetActive(true);
        string n = "???";
        string a = "...";

        teu.illust.sprite = Inkmo;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "???";
        a = "결국 이렇게 되는 것인가?";

        teu.illust.sprite = Inkmo;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        n = "???";
        a = "과거 삭블이브때처럼, 신기한 인연이야.";

        teu.illust.sprite = Inkmo;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        fade.gameObject.SetActive(true);
        fade.color = new Color(0, 0, 0, 1);
        inkmo.sprite = Inkmo_Eyed_dot;
        for (float i = 1; i >= 0; i -= 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        fade.gameObject.SetActive(false);

        n = "???";
        a = "오늘부로, 새로운 역사가 시작된다.";

        teu.illust.sprite = Inkmo_Eyed;
        teu.name.text = n;
        teu.talking.text = "";

        for (int i = 0; i < a.Length; i++)
        {
            teu.talking.text += a[i];
            yield return new WaitForSeconds(0.005f);
        }
        yield return new WaitForSeconds(5f);

        fade.gameObject.SetActive(true);
        for (float i = 0; i <= 1; i += 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        gmm.GetComponent<PlayerState>().heal(9999);
        gmm.setMovingPoint(new Vector3(-9.036f, -0.464f, -1));
        LoadingScene.LoadScene("Sak_Village_HeroSak");
    }
}
