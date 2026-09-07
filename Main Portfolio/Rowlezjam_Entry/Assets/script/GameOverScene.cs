using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameOverScene : MonoBehaviour
{
    GameManager gameManager;
    AudioSource SoundSource;
    public GameObject Menu;
    public GameObject Cursor1;
    public GameObject Cursor2;
    public UnityEngine.UI.Image fade;
    public UnityEngine.UI.Text gameover;
    public AudioClip start;
    public AudioClip select;
    public AudioClip txMessage;
    bool fadeFinished = false;
    bool ContinueCursored = true;
    bool loadingSceme = false;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        SoundSource = GetComponent<AudioSource>();
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut()
    {
        float i = 1;
        while (i > 0)
        {
            i -= Time.deltaTime / 1;
            fade.color = new Color(0, 0, 0, i);
            yield return null;
        }
        gameover.text = "";
        string tx = "GAME\nOVER";
        int count = 0;
        SoundSource.pitch = 0.5f;
        while (count != tx.Length)
        {
            if (count < tx.Length)
            {
                gameover.text += tx[count].ToString();
                count++;

                SoundSource.PlayOneShot(txMessage);
            }
            yield return new WaitForSecondsRealtime(0.75f);
        }
        SoundSource.pitch = 1f;
        yield return new WaitForSecondsRealtime(0.2f);
        Menu.SetActive(true);
        fadeFinished = true;
    }

    IEnumerator Fadein(bool isLoading)
    {
        loadingSceme = true;
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime / 1;
            fade.color = new Color(0, 0, 0, i);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(0.1f);
        if (isLoading)
        {
            gameManager.LoadFile();
            gameManager.MovingToSavePosition = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameManager.MovedScene);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Cursor1.gameObject.SetActive(ContinueCursored);
        Cursor2.gameObject.SetActive(!ContinueCursored);
        if (fadeFinished) {
            if (Input.GetButtonUp("Vertical"))
            {
                ContinueCursored = !ContinueCursored;
                SoundSource.PlayOneShot(select);
            }
            if (Input.GetButtonUp("Check") && !loadingSceme)
            {
                loadingSceme = true;
                StartCoroutine("Fadein", ContinueCursored);
            }
        }
    }
}
