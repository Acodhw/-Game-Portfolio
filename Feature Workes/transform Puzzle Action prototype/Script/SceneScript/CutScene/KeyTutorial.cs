using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyTutorial : MonoBehaviour
{

    [SerializeField]
    private Transform player;

    [SerializeField]
    private CanvasGroup ShowTutorialUI;

    [SerializeField]
    private float startX;

    [SerializeField]
    private float endX;

    private bool isShown;
    private bool end;

    private void Update()
    {
        if (startX < endX) {
            if (player.position.x >= startX && !isShown) {
                isShown = true;
                StartCoroutine(ShowTutorial());
            }
            if (player.position.x >= endX && isShown && !end) {
                end = true;
                StartCoroutine(RemoveTutorial());
            }
        }
        else {
            if (player.position.x <= startX && !isShown)
            {
                isShown = true;
                StartCoroutine(ShowTutorial());
            }
            if (player.position.x <= endX && isShown && !end)
            {
                end = true;
                StartCoroutine(RemoveTutorial());
            }
        }
    }

    IEnumerator ShowTutorial()
    {
        ShowTutorialUI.gameObject.SetActive(true);
        for(float i = 0; i < 0.5f; i += Time.deltaTime)
        {
            ShowTutorialUI.alpha = i * 2;
            yield return null;
        }

    }

    IEnumerator RemoveTutorial()
    {
        for (float i = 0.5f; i > 0; i -= Time.deltaTime)
        {
            ShowTutorialUI.alpha = i * 2;
            yield return null;
        }
        ShowTutorialUI.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
