using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUi : MonoBehaviour
{
    GameManager gameManager;
    MainCharacter character;
    AudioSource SoundSource;
    TalkManager talkManager;

    public GameObject mainUI;
    public GameObject pauseUi;
    public GameObject talkingUI;

    public Transform OutSwipe;
    public Transform LeftSwipe;
    public Transform RightSwipe;

    public Sprite[] elementalIcon;
    public Sprite[] CatchedObjSprite;
    public Transform[] sceneTransitionObj;

    public Image CatchedImg;
    public Image elementalIconPoint;
    public Image HpBar;
    public Image MpBar;

    public RectTransform Pan;
    public Text nameTx;
    public Text TalkTx;
    public GameObject TalkFinishIcon;

    public GameObject cursor1;
    public GameObject cursor2;

    public GameObject Avoidicon;

    public AudioClip buttonSound;
    public AudioClip cursorChange;
    public AudioClip talkingSound;

    [HideInInspector]
    public bool paused = false;
    [HideInInspector]
    public bool talking = false;
    [HideInInspector]
    public bool UIStart = false;
    [HideInInspector]
    public bool GameOverd = false;

    bool cursoredContinue = true;
    bool gotoOtherScene = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        talkManager = gameManager.GetComponent<TalkManager>();
        SoundSource = GetComponent<AudioSource>();
        character = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        StartCoroutine("SwipingStart");
    }

    IEnumerator SwipingStart()
    {
        gameManager.SaveFile();
        Time.timeScale = 0;
        float speed = 0;
        if (!gameManager.goleftmoved)
        {         
            LeftSwipe.transform.localPosition = Vector3.zero + Vector3.forward;
            LeftSwipe.gameObject.SetActive(true);
            RightSwipe.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.1f);
            while (LeftSwipe.localPosition.x < 15)
            {
                speed += Time.fixedDeltaTime * 0.8f;
                LeftSwipe.localPosition += Vector3.right * speed;
                yield return new WaitForSecondsRealtime(0.02f);
            }
            LeftSwipe.gameObject.SetActive(false);
        }
        else {
            RightSwipe.transform.localPosition = Vector3.zero + Vector3.forward;
            RightSwipe.gameObject.SetActive(true);
            LeftSwipe.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.1f);
            while (RightSwipe.localPosition.x > -15)
            {
                speed += Time.fixedDeltaTime * 0.8f;
                RightSwipe.localPosition -= Vector3.right * speed;
                yield return new WaitForSecondsRealtime(0.02f);
            }
            RightSwipe.gameObject.SetActive(false);
        }
        RightSwipe.localPosition = new Vector3(-15, 0, 1);
        LeftSwipe.localPosition = new Vector3(15, 0, 1);
        RightSwipe.gameObject.SetActive(false);
        LeftSwipe.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1;
        UIStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        HpBar.fillAmount = gameManager.Hp / 30f;
        MpBar.fillAmount = gameManager.Mp / 15f;
        elementalIconPoint.sprite = elementalIcon[gameManager.nowElement];
        CatchedImg.sprite = CatchedObjSprite[gameManager.nowCatched];

        pauseUi.SetActive(paused && UIStart);
        talkingUI.SetActive(!paused && talking && UIStart);
        mainUI.SetActive(!paused && !talking && UIStart);

        cursor1.SetActive(cursoredContinue);
        cursor2.SetActive(!cursoredContinue);

        Avoidicon.SetActive(character.canAvoid);
        if (!GameOverd)
        {
            if (paused)
            {
                Time.timeScale = 0;
                if (Input.GetButtonUp("Vertical"))
                {
                    cursoredContinue = !cursoredContinue;
                    SoundSource.PlayOneShot(cursorChange);
                }

                if (Input.GetButtonUp("Check"))
                {
                    if (cursoredContinue)
                    {
                        paused = false;
                        SoundSource.PlayOneShot(buttonSound);
                    }
                    else
                    {
                        if (!gotoOtherScene)
                        {
                            gotoOtherScene = true;
                            StartCoroutine("GoToTitle");
                        }
                    }
                }
            }

            if (Input.GetButtonDown("Cancel") )
            {
                if (!paused && !talking && Time.timeScale > 0)
                {
                    paused = true;
                    cursoredContinue = true;
                    SoundSource.PlayOneShot(buttonSound);
                }
                else
                {
                    paused = false;
                    SoundSource.PlayOneShot(buttonSound);
                    Time.timeScale = 1;
                }
            }
        }
    }

    IEnumerator GoToTitle()
    {
        SoundSource.PlayOneShot(buttonSound);
        OutSwipe.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);
        while (OutSwipe.localPosition.y > 0)
        {
            OutSwipe.localPosition = (OutSwipe.localPosition + (new Vector3(0,-1f,1) - OutSwipe.localPosition) * Time.fixedDeltaTime);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1;
        gameManager.SaveFile();
        gameManager.LoadFile();
        SceneManager.LoadScene("Title");
    }

    public IEnumerator GameOver() {
        SoundSource.PlayOneShot(buttonSound);
        OutSwipe.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);
        while (OutSwipe.localPosition.y > 0)
        {
            OutSwipe.localPosition = (OutSwipe.localPosition + (new Vector3(0, -1f, 1) - OutSwipe.localPosition) * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSecondsRealtime(0.05f);
        SceneManager.LoadScene("GameOver");
    }

    public void TalkingEvent(int EventIndex) {
        if (!talking && !paused)
        {
            talking = true;
            StartCoroutine("Talking", EventIndex);
        }
    }

    IEnumerator Talking(int eventNum) {
        Pan.position = new Vector3(0, -70, 0);
        while (Pan.localPosition.y < -16)
        {
            Pan.localPosition = Vector3.Lerp(Pan.localPosition, new Vector3(0, -10, 0), 0.2f);
            yield return null;
        }
        Pan.localPosition = new Vector3(0, -16, 0);
        yield return new WaitForSeconds(0.02f);
        string[] talkdata = talkManager.ReturnTalkData(eventNum);
        for(int i = 0; i < talkdata.Length; i++) {
            TalkFinishIcon.SetActive(false);
            string[] data = talkdata[i].Split('$');

            nameTx.text = data[0] + ":";
            yield return StartCoroutine(PrintTx(0.05f, data[1]));          
            yield return new WaitForSeconds(0.02f);
            TalkFinishIcon.SetActive(true);
            yield return new WaitUntil(() => Input.GetButtonDown("Check"));
            SoundSource.PlayOneShot(cursorChange);
            yield return new WaitForSeconds(0.02f);
        }
        nameTx.text = "";
        TalkTx.text = "";
        float speed = 0;
        while (Pan.localPosition.y > -70)
        {
            speed += Time.fixedDeltaTime * 1.2f;
            Pan.localPosition += Vector3.down * speed;
            yield return new WaitForSecondsRealtime(0.02f);
        }
        Pan.localPosition = new Vector3(0, -70, 0);
        yield return new WaitForSeconds(0.02f);
        talking = false;
    }

    IEnumerator PrintTx(float delay, string storyLine)
    {
        TalkTx.text = "";
        int count = 0;
        while (count != storyLine.Length)
        {
            if (count < storyLine.Length)
            {
                TalkTx.text += storyLine[count].ToString();
                count++;
                SoundSource.PlayOneShot(talkingSound);
            }
            float time = 0;
            while (time < delay)
            {
                time += Time.deltaTime;
                if (Input.GetButtonDown("Check"))
                    goto finish;
                yield return null;
            }
        }
    finish:
        TalkTx.text = storyLine;
    }
}
