using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using System.Text.RegularExpressions;

public class FirstStoryCutscene : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public TMP_InputField inputField;
    public PlayableDirector director;

    public List<string> beforeInputTexts;
    public List<string> rejectTexts;
    public List<string> successTexts;

    public List<string> rejectNameList;

    private HashSet<string> rejectNameSet;

    const float fadeInTime = 1f;
    const float displayTime = 1.5f;
    const float fadeOutTime = 0.5f;

    void Awake()
    {
        rejectNameSet = new HashSet<string>(rejectNameList);
    }

    public void StartCutscene()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        if (director != null)
            director.Pause();

        textUI.alpha = 0;
        textUI.text = "";
        inputField.gameObject.SetActive(false);

        yield return ShowTextList(beforeInputTexts);

        while (true)
        {
            inputField.text = "";
            inputField.gameObject.SetActive(true);
            inputField.ActivateInputField();

            bool done = false;

            inputField.onSubmit.RemoveAllListeners();
            inputField.onSubmit.AddListener((value) => done = true);

            yield return new WaitUntil(() => done);

            string input = inputField.text.Trim();
            inputField.gameObject.SetActive(false);

            if (!IsValidName(input))
            {
                yield return ShowTextList(rejectTexts);
            }
            else
            {
                if (GameManager.Instance != null)
                    GameManager.Instance.currentPlayerName = input;

                yield return ShowTextList(successTexts);
                break;
            }
        }

        if (director != null)
            director.Play();
    }
    IEnumerator ShowTextList(List<string> list)
    {
        foreach (var text in list)
        {
            yield return ShowText(text);
        }
    }

    IEnumerator ShowText(string text)
    {
        if (GameManager.Instance != null)
            text = text.Replace("{PlayerName}", GameManager.Instance.currentPlayerName);

        textUI.text = text;

        yield return FadeIn();
        yield return new WaitForSecondsRealtime(displayTime);
        yield return FadeOut();
    }
    IEnumerator FadeIn()
    {
        float t = 0;
        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime;
            textUI.alpha = t / fadeInTime;
            yield return null;
        }
        textUI.alpha = 1;
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeOutTime)
        {
            t += Time.unscaledDeltaTime;
            textUI.alpha = 1 - (t / fadeOutTime);
            yield return null;
        }
        textUI.alpha = 0;
    }
    bool IsValidName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        if (Regex.IsMatch(name, @"^[ㄱ-ㅎ]+$"))
            return false;

        if (rejectNameSet.Contains(name))
            return false;

        return true;
    }
}