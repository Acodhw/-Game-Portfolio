using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    GameManager gameManager;
    AudioSource SoundSource;

    public GameObject StartTx;
    public GameObject LoadMenu;
    public GameObject Cursor1;
    public GameObject Cursor2;
    public UnityEngine.UI.Image fade;

    bool IsOnLoadMenu = false;
    bool newStartSelected = false;
    bool fadeFinished = false;
    bool loadingSceme = false;

    public string NewStartSceneName;
    public AudioClip start;
    public AudioClip select;

    public Transform slider;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        SoundSource = GetComponent<AudioSource>();
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut() {
        float i = 1;
        while (i > 0)
        {
            i -= Time.deltaTime / 1;
            fade.color = new Color(0, 0, 0, i);
            yield return null;
        }
        fadeFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        StartTx.SetActive(!IsOnLoadMenu);
        LoadMenu.SetActive(IsOnLoadMenu);
        Cursor1.SetActive(newStartSelected);
        Cursor2.SetActive(!newStartSelected);
        if (fadeFinished)
        {
            if (Input.GetButtonUp("Check"))
            {
                if (!IsOnLoadMenu)
                {
                    if (gameManager.FileLoaded)
                    {
                        IsOnLoadMenu = true;
                        newStartSelected = true;
                        SoundSource.PlayOneShot(select);
                    }
                    else if (!loadingSceme)
                        StartCoroutine("StartNextScene", false);
                    
                }
                else
                {
                    if (!loadingSceme)
                    {
                        if (newStartSelected)
                            StartCoroutine("StartNextScene", false);
                        else
                            StartCoroutine("StartNextScene", true);
                    }
                }
            }
            else if (Input.GetButtonUp("Cancel"))
            {
                if (IsOnLoadMenu)
                {
                    IsOnLoadMenu = false;
                    SoundSource.PlayOneShot(select);
                }
                else {
                    Application.Quit();
                }
            }
            else if (Input.GetButtonUp("Vertical"))
            {
                if (IsOnLoadMenu)
                {
                    newStartSelected = !newStartSelected;
                    SoundSource.PlayOneShot(select);
                }
            }
        }
    }

    IEnumerator StartNextScene(bool FileLoad) {
        loadingSceme = true;
        SoundSource.PlayOneShot(start);
        yield return new WaitForSeconds(0.2f);
        while (slider.position.x < 0) {
            slider.position = Vector3.Lerp(slider.position, new Vector3(1f, 0, 1), 0.03f);
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
        if (FileLoad)
        {
            gameManager.MovingToSavePosition = true;
            SceneManager.LoadScene(gameManager.MovedScene);
        }
        else
        {
            gameManager.InitStates();
            SceneManager.LoadScene(NewStartSceneName);
        }
    }
}
