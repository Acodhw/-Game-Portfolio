using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkFirst : MonoBehaviour
{
    GameManager gameManager;
    MainUi mainUi;
    public int eventNum;
    bool sended = false;
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
                    sended = true;
                    mainUi.TalkingEvent(eventNum);
                    gameManager.didEvents.Add(eventNum);
                }
            }
        }
    }
}
