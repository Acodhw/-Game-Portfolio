using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        if (Screen.width < 1920) Screen.SetResolution(960, 540, true);
        else Screen.SetResolution(1920, 1080, true);
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
