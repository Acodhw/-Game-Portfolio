using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerPauseStart : MonoBehaviour
{
    public bool isTimerPause = false;
    GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(isTimerPause) manager.PauseTimer();
        else manager.StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
