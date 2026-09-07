using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TalkEvent : MonoBehaviour
{
    public int EventCode;
    public AudioClip TalkSound;
    AudioSource audioSource;
    GameManager gameManager;
    GameObject UIOBJ;
    MainUi mainUi;
    UnityEngine.UI.Image timeImg;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mainUi = GameObject.Find("Canvas").GetComponent<MainUi>();
        UIOBJ = transform.GetChild(0).gameObject;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UIOBJ.SetActive(true);
            if (Input.GetButtonDown("Check") && !mainUi.paused && !mainUi.talking)
            {
                mainUi.TalkingEvent(EventCode);
                audioSource.PlayOneShot(TalkSound);
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
}
