using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstScene : MonoBehaviour
{
    GameManager gm;
    public string titleSceneName;
    public string startSceneName;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(gm.isFileLoaded) UnityEngine.SceneManagement.SceneManager.LoadScene(titleSceneName);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(startSceneName);
    }

}
