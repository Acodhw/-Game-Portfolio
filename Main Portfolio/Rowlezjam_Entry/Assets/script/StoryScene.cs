using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class StoryScene : MonoBehaviour
{
    bool gotoNext = false;
    public GameObject[] storyPictures;
    public Text tx;
    public Image fade;
    public float fadeTime;
    public string nextScene;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Story");
    }

    void Update()
    {
        if (Input.GetButtonUp("Check") && !gotoNext)
        {
            gotoNext = true;
            StartCoroutine("LoadNextScene");
        }
    }

    IEnumerator Story()
    {
        float i = 1;
        while (i > 0)
        {
            i -= Time.deltaTime / fadeTime;
            fade.color = new Color(0, 0, 0, i);
            yield return null;
        }
        storyPictures[0].SetActive(true);
        yield return StartCoroutine(PrintTx(0.1f, "In a world where four elements"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "comprised all of existence,"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "there stood a tower that"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "governed these elements."));
        yield return new WaitForSeconds(0.5f);
        storyPictures[0].SetActive(false);
        storyPictures[1].SetActive(true);
        yield return StartCoroutine(PrintTx(0.1f, "One fateful day,"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "an intruder breached the tower"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "and absconded with the four elements."));
        yield return new WaitForSeconds(0.5f);
        storyPictures[1].SetActive(false);
        storyPictures[2].SetActive(true);
        yield return StartCoroutine(PrintTx(0.1f, "The deity overseeing the elements"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "summoned an elementalist."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "\"Elementalist, the very foundations"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "of our world have been stolen."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "Embark on a quest"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "to recover the elements")); 
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "and restore them to this tower.\""));
        yield return new WaitForSeconds(0.5f);
        storyPictures[2].SetActive(false);
        storyPictures[3].SetActive(true);
        yield return StartCoroutine(PrintTx(0.1f, "Thus began the elementalist's adventure"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PrintTx(0.1f, "to reclaim the stolen elements."));
        yield return new WaitForSeconds(1f);
        if (!gotoNext)
        {
            gotoNext = true;
            StartCoroutine("LoadNextScene");
        }
    }

    IEnumerator PrintTx(float delay, string storyLine)
    {
        tx.text = "";
        int count = 0;
        while (count != storyLine.Length)
        {
            if (count < storyLine.Length)
            {
                tx.text += storyLine[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator LoadNextScene()
    {
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime / fadeTime;
            fade.color = new Color(0, 0, 0, i);
            yield return null;
        }
        SceneManager.LoadScene(nextScene);
    }
}
