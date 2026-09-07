using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameOver : MonoBehaviour
{
    public Text tx;
    public GameObject otherBTN;
    public Image fade;
    private GameManager gmm;
    private PlayerState ps;
    // Start is called before the first frame update
    void Start()
    {
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        ps = GameObject.Find("GameManager").GetComponent<PlayerState>();
        StartCoroutine("GameOverScene");
    }

    IEnumerator GameOverScene() {
        yield return new WaitForSeconds(2.7f);
        string a = "GAME OVER";
        for (int i = 0; i < a.Length; i++)
        {
            tx.text += a[i];
            yield return new WaitForSeconds(0.1f);
        }
        otherBTN.SetActive(true);
    }

    public void totitle() 
    {
        StartCoroutine("fadein", "Title");    
    }

    public void loadTo()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/GuwaGuwa" + gmm.getDatanum() + ".dat"))
        {
            gmm.loadAll();
            StartCoroutine("fadein", ps.savedscene);
            ps.savedscene = "";
        }
        else
        {
            gmm.Reset();
            StartCoroutine("fadein", "Tutorial");
        }
    }

    IEnumerator fadein(string scene)
    {
        fade.gameObject.SetActive(true);
        for (float i = 0; i < 1; i += 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        LoadingScene.LoadScene(scene);
    }
}
