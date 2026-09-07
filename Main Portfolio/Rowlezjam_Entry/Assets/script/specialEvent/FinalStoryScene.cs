using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalStoryScene : MonoBehaviour
{
    bool gotoNext = false;
    public Transform god;
    public Text tx;
    public Image fade;
    public float fadeTime;
    public string nextScene;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        yield return StartCoroutine(PrintTx(0.05f, "Thou hast done well, Elementalist."));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "The time has come to fulfill our final aspiration."));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "Why do you seek to destroy the four elements"));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "The four elements, borrowing from the realm and power of the divine"));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "have guided humanity's growth to this point."));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "However, humanity can progress further."));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "They shall discover not just four,"));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "but 118 elements, or perhaps even more."));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "Thus, mankind must no longer be bound by these four elements."));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "This is the final task that I,"));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "the Elemental Deity, must accomplish."));
        yield return new WaitForSeconds(1.5f);
        tx.text = "";
        while (god.position.y > 1.25f) {
            god.position = Vector3.Lerp(god.position, new Vector3(1.97f, 1, -3), 0.5f);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "Through my demise,"));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "humanity shall be liberated from the gods"));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "and forge a world of their own."));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "Therefore, Elementalist"));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "Use the power of the four elements to engage me in battle"));
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(PrintTx(0.05f, "and STRIKE ME DOWN!"));
        yield return new WaitForSeconds(1.5f);
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
        gameManager.goleftmoved = false;
        gameManager.PotalMovePosition = Vector3.zero;
        gameManager.MovingToSavePosition = true;
        SceneManager.LoadScene(nextScene);
    }
}
