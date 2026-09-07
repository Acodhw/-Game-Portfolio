using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float scoreTimer = 0;
    private bool timer = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Screen.SetResolution(1920, 1080, true);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer) {
            scoreTimer += Time.deltaTime;
        }
    }

    public void ResetTimer() {
        scoreTimer = 0;
        timer = false;
    }

    public void PauseTimer()
    {
        timer = false;
    }

    public void StartTimer()
    {
        timer = true;
    }

    public float GetTimerTime()
    {
        return scoreTimer;
    }
}
