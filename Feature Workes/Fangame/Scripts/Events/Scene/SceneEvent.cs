using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneEvent : MonoBehaviour
{
    public MonoBehaviour Events_Scripts;
    private PlayerControl pc;
    bool isEventGone;

    public Image gameTitle;
    public Text gameTitleName;
    bool titleset_finish = false;
    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();       
    }   

    // Update is called once per frame
    void Update()
    {
        if(!pc.ispause && !titleset_finish)
            StartCoroutine("sceneTitleSet");
    }

    IEnumerator sceneTitleSet()
    {
        titleset_finish = true;
        if (Events_Scripts != null) {
            Events_Scripts.StartCoroutine("EventStart");
        }
        gameTitle.gameObject.SetActive(true);
        gameTitleName.gameObject.SetActive(true);
        for (float i = 0; i < 1; i += 0.01f)
        {
            gameTitle.color = new Color(1, 1, 1, i);
            gameTitleName.color = new Color(1, 1, 1, i);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        for (float i = 1; i >= 0; i -= 0.01f)
        {
            gameTitle.color = new Color(1, 1, 1, i);
            gameTitleName.color = new Color(1, 1, 1, i);
            yield return new WaitForSeconds(0.01f);
        }
        gameTitle.gameObject.SetActive(false);
        gameTitleName.gameObject.SetActive(false);

    }
}
