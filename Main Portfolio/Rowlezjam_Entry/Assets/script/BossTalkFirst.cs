using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossTalkFirst : MonoBehaviour
{
    GameManager gameManager;
    MainUi mainUi;
    public int eventNum;
    public string nextScene;
    public Transform RightGoSwipe;
    bool sended = false;
    public bool finished = false;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mainUi = GameObject.Find("Canvas").GetComponent<MainUi>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!sended)
        {
            if (mainUi.UIStart)
            {
                if (!gameManager.didEvents.Contains(eventNum))
                {                    
                    mainUi.TalkingEvent(eventNum);
                    gameManager.didEvents.Add(eventNum);
                    sended = true;
                }
                else {
                    finished = true;
                }
            }
        }
        else {
            if (!mainUi.talking)
                finished = true;
        }
    }

    public void nextScenego() {
        StartCoroutine(GoNextScene());
    }
    IEnumerator GoNextScene() {
        yield return new WaitForSeconds(10f);
        Time.timeScale = 0;
        RightGoSwipe.localPosition = new Vector3(-15, 0, 1);
        RightGoSwipe.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        while (RightGoSwipe.localPosition.x < 0)
        {
            RightGoSwipe.localPosition = (RightGoSwipe.localPosition + (new Vector3(1f, 0, 1) - RightGoSwipe.localPosition) * Time.fixedDeltaTime * 3f);
            yield return new WaitForSecondsRealtime(0.02f);
        }

        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1;
        SceneManager.LoadScene(nextScene);
    }
}
