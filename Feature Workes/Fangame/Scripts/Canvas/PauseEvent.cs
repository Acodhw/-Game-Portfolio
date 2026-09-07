using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseEvent : MonoBehaviour
{
    public GameObject pause;
    public GameObject setting;
    public GameObject keys;
    public GameObject isReal;

    public void settingOn()
    {
        pause.SetActive(false);
        setting.SetActive(true);
    }

    public void settingOff()
    {
        pause.SetActive(true);
        setting.SetActive(false);
    }

    public void keysOn()
    {
        pause.SetActive(false);
        keys.SetActive(true);
    }

    public void keysOff()
    {
        pause.SetActive(true);
        keys.SetActive(false);
    }

    public void isRealOn()
    {
        pause.SetActive(false);
        isReal.SetActive(true);
    }

    public void isRealOff()
    {
        pause.SetActive(true);
        isReal.SetActive(false);
    }

    public void goTitle()
    {
        LoadingScene.LoadScene("Title");
    }
}
