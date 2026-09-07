using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BetaEnding : MonoBehaviour
{
    public string titleSceneName = "Title";
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    public float holdDuration = 5.0f;

    private bool isExecuting = false;

    private void Start()
    {
        StartEnding();
    }

    public void StartEnding()
    {
        if (isExecuting) return;
        isExecuting = true;

        StartCoroutine(EndingRoutine());
    }

    private IEnumerator EndingRoutine()
    {
        if (fadeImage == null)
        {
            LoadingScene.LoadScene(titleSceneName);
            yield break;
        }

        fadeImage.gameObject.SetActive(true);

        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1f, 0f, time / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;

        yield return new WaitForSecondsRealtime(holdDuration);

        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        LoadingScene.LoadScene(titleSceneName);
    }
}