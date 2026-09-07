using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class alter : MonoBehaviour
{
    bool moving = false;
    GameManager gameManager;
    GameObject UIOBJ;
    MainUi mainUi;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mainUi = GameObject.Find("Canvas").GetComponent<MainUi>();
        UIOBJ = transform.GetChild(0).gameObject;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UIOBJ.SetActive(true);
            if (Input.GetButtonDown("Check") && !moving && !mainUi.paused && !mainUi.talking)
            {
                moving = true;
                SceneManager.LoadScene("FinalAppear");
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
