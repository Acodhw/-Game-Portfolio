using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    [SerializeField]
    private string startScene;

    public void StartBtn() {
        LoadingScene.LoadScene(startScene);
    }
}
