using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToCastle : MonoBehaviour
{
    public Image fade;
    private PlayerControl pc;
    private GameManager gmm;
    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        gmm = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine("fadeIn");
    }

    IEnumerator fadeIn()
    {
        fade.gameObject.SetActive(true);
        pc.ispause = true;
        for (float i = 0; i <= 1; i += 0.01f)
        {
            fade.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        gmm.setMovingPoint(new Vector3(-14.96f, 7.13f, -1));
        LoadingScene.LoadScene("BecomeHero");
    }
}
