using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting_Diff : MonoBehaviour
{
    bool settingDif = false;
    private GameManager gmm;

    public GameObject btns;
    public Text tx;
    public Image fade;
    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine("events");
    }

    IEnumerator events()
    {
        yield return new WaitForSeconds(1f);

        string a = "...";
        tx.text = "";
        for (int i = 0; i < a.Length; i++) {
            tx.text += a[i];
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1.5f);

        a = "난이도를 설정해 주세요. 변경이 불가하오니 신중히 선택해 주세요.";
        tx.text = "";
        for (int i = 0; i < a.Length; i++)
        {
            tx.text += a[i];
            yield return new WaitForSeconds(0.02f);
        }
        btns.SetActive(true);
        yield return new WaitUntil(() => settingDif);

        a = "난이도가 설정되었습니다.";
        tx.text = "";
        for (int i = 0; i < a.Length; i++)
        {
            tx.text += a[i];
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1.5f);

        a = "다음으로 키 테스트입니다.";
        tx.text = "";
        for (int i = 0; i < a.Length; i++)
        {
            tx.text += a[i];
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1.5f);

        a = "확인키 Z를 눌러주세요.";
        tx.text = "";
        for (int i = 0; i < a.Length; i++)
        {
            tx.text += a[i];
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitUntil(() => Input.GetButtonDown("Check"));

        a = "확인되었습니다.";
        tx.text = "";
        for (int i = 0; i < a.Length; i++)
        {
            tx.text += a[i];
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1.5f);
        StartCoroutine("fadeIn");
    }

    public void diffBtn(int diff)
    {
        gmm.setDifficulty(diff);
        settingDif = true;
        btns.SetActive(false);
    }

    IEnumerator fadeIn()
    {
        fade.gameObject.SetActive(true);
        for (float i = 0; i <= 1; i += 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        LoadingScene.LoadScene("Tutorial");
    }
}
