using System.Collections;
using UnityEngine;

public class TutorialShow : MonoBehaviour
{
    public CanvasGroup tutorial;
    public Transform player;

    [Header("Enter Zone")]
    public Vector3 enterCenter;
    public Vector3 enterSize = new Vector3(3, 3, 3);

    [Header("Exit Zone")]
    public Vector3 exitCenter;
    public Vector3 exitSize = new Vector3(5, 3, 5);

    [Header("Event Settings")]
    [Tooltip("한 번만 실행되도록 할 GameManager의 이벤트 ID. (-1이면 세이브 연동 안함)")]
    public int tutorialEventID = -1;

    public float fadeDuration = 0.5f;

    private bool isShowing = false;
    private bool hasShown = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        tutorial.alpha = 0f;
        tutorial.gameObject.SetActive(false);

        if (tutorialEventID >= 0 && GameManager.Instance != null)
        {
            if (GameManager.Instance.GetEventState(tutorialEventID) != 0)
            {
                hasShown = true;
            }
        }
    }

    void Update()
    {
        Vector3 pos = player.position;

        Bounds enter = new Bounds(transform.position + enterCenter, enterSize);
        Bounds exit = new Bounds(transform.position + exitCenter, exitSize);

        bool inEnter = enter.Contains(pos);
        bool inExit = exit.Contains(pos);

        if (!isShowing && !hasShown && inEnter)
        {
            isShowing = true;
            ShowTutorial();
        }

        if (isShowing && inExit)
        {
            isShowing = false;
            hasShown = true;
            if (tutorialEventID >= 0 && GameManager.Instance != null)
            {
                GameManager.Instance.SetEventState(tutorialEventID, 1);
            }

            HideTutorial();
        }
    }

    void ShowTutorial()
    {
        tutorial.gameObject.SetActive(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(0f, 1f));
    }

    void HideTutorial()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(1f, 0f, () =>
        {
            tutorial.gameObject.SetActive(false);
        }));
    }

    IEnumerator Fade(float from, float to, System.Action onComplete = null)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            tutorial.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        tutorial.alpha = to;
        onComplete?.Invoke();
    }

    void OnDrawGizmos()
    {
        DrawBox(enterCenter, enterSize, Color.green);
        DrawBox(exitCenter, exitSize, Color.red);
    }

    void DrawBox(Vector3 center, Vector3 size, Color color)
    {
        Gizmos.color = color;

        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + center, transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, size);

        Gizmos.matrix = old;
    }
}