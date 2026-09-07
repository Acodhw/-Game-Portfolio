using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    private static string nextSceneName;

    public static void LoadScene(string sceneName)
    {
        nextSceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    [Header("UI References")]
    public Image progressBar;

    [Header("Fade Settings")]
    [Tooltip("페이드 이미지")]
    public Image fadeImage;
    public float fadeDuration = 0.5f; 

    private void Awake()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;
        }
    }

    private void Start()
    {
        StartCoroutine(LoadTargetScene());
    }

    private IEnumerator LoadTargetScene()
    {
        if (progressBar != null) progressBar.fillAmount = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        yield return StartCoroutine(FadeRoutine(1f, 0f));
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount >= 1.0f)
                {
                    yield return StartCoroutine(FadeRoutine(0f, 1f));
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha)
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);

        Color color = fadeImage.color;
        color.a = startAlpha;
        fadeImage.color = color;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }
}