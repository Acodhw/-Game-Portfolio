using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLoadScene : MonoBehaviour
{
    public string nextScene;
    public float fadeDuration = 1.0f;

    private bool isExecuting = false;

    public void SceneLoadEvent()
    {
        if (isExecuting) return;
        isExecuting = true;

        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        if (UIManager.Instance != null && UIManager.Instance.fadeImage != null)
        {
            UIManager.Instance.isTalking = true;
            UIManager.Instance.fadeImage.gameObject.SetActive(true);

            Color color = UIManager.Instance.fadeImage.color;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.unscaledDeltaTime;
                color.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
                UIManager.Instance.fadeImage.color = color;
                yield return null;
            }

            color.a = 1f;
            UIManager.Instance.fadeImage.color = color;
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        LoadingScene.LoadScene(nextScene);
    }
}