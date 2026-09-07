using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class potal : MonoBehaviour
{
    public bool isGoLeft;
    public string NextScene;
    public Vector2 nextPosition;
    public AudioClip PotalSound;

    bool moving = false;

    Transform LeftGoSwipe;
    Transform RightGoSwipe;
    AudioSource audioSource;
    GameManager gameManager;
    GameObject UIOBJ;
    MainUi mainUi;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mainUi = GameObject.Find("Canvas").GetComponent<MainUi>();
        LeftGoSwipe = mainUi.LeftSwipe;
        RightGoSwipe = mainUi.RightSwipe;
        UIOBJ = transform.GetChild(0).gameObject;
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UIOBJ.SetActive(true);
            if (Input.GetButtonDown("Check") && !moving && !mainUi.paused && !mainUi.talking)
            {
                moving = true;
                StartCoroutine("GotoNext");
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

    IEnumerator GotoNext()
    {
        audioSource.PlayOneShot(PotalSound);
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 0;
        if (isGoLeft)
        {
            LeftGoSwipe.localPosition = new Vector3(15, 0, 1);
            LeftGoSwipe.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f);
            while (LeftGoSwipe.localPosition.x > 0)
            {
                LeftGoSwipe.localPosition = (LeftGoSwipe.localPosition + (new Vector3(-1f, 0, 1) - LeftGoSwipe.localPosition) * Time.fixedDeltaTime * 3f);
                yield return new WaitForSecondsRealtime(0.02f);
            }
        }
        else
        {
            RightGoSwipe.localPosition = new Vector3(-15, 0, 1);
            RightGoSwipe.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f);
            while (RightGoSwipe.localPosition.x < 0)
            {
                RightGoSwipe.localPosition = (RightGoSwipe.localPosition + (new Vector3(1f, 0, 1) - RightGoSwipe.localPosition) * Time.fixedDeltaTime * 3f);
                yield return new WaitForSecondsRealtime(0.02f);
            }          
        }
        yield return new WaitForSecondsRealtime(0.05f);
        gameManager.MovedScene = NextScene;
        gameManager.goleftmoved = isGoLeft;
        gameManager.PotalMovePosition = nextPosition;
        gameManager.MovingToSavePosition = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(NextScene);      
    }
}
