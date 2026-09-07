using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchingObject : MonoBehaviour
{
    public int CatchedCode;
    public AudioClip catchSound;
    AudioSource audioSource;
    GameManager gameManager;
    GameObject UIOBJ;
    GameObject CheckOBJ;
    GameObject TimeObj;
    MainUi mainUi;
    UnityEngine.UI.Image timeImg;

    float time = 0;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mainUi = GameObject.Find("Canvas").GetComponent<MainUi>();
        UIOBJ = transform.GetChild(0).gameObject;
        CheckOBJ = UIOBJ.transform.GetChild(1).gameObject;
        TimeObj = UIOBJ.transform.GetChild(0).gameObject;
        timeImg = TimeObj.GetComponent<UnityEngine.UI.Image>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            UIOBJ.SetActive(true);
            if (time <= 0)
            {
                CheckOBJ.SetActive(true);
                TimeObj.SetActive(false);
                if (Input.GetButtonDown("Check") && !mainUi.paused && !mainUi.talking)
                {
                    gameManager.nowCatched = CatchedCode;
                    audioSource.PlayOneShot(catchSound);
                    time = 10;
                }
            }
            else
            {
                CheckOBJ.SetActive(false);
                TimeObj.SetActive(true);
            }          
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UIOBJ.SetActive(false);
        }
    }

    private void Update()
    {
        timeImg.fillAmount = time / 10f;
        if (time > 0)
            time -= Time.deltaTime;
        else if (time < 0)
            time = 0;  
    }

}
