using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CanvasGroup))]
public class BasicGlobalUIShow : MonoBehaviour
{
    [Tooltip("보이기 시작 거리")]
    public float visibleRange = 15f;
    [Tooltip("페이드 구간 거리")]
    public float fadeRange = 5f;

    public Camera targetCamera;

    [Tooltip("최소 거리")]
    public float referenceDistance = 10f;

    private CanvasGroup canvasGroup;
    private Vector3 originalScale;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;

        if (targetCamera == null)
        {
            CinemachineBrain brain = FindObjectOfType<CinemachineBrain>();
            if (brain != null)
                targetCamera = brain.GetComponent<Camera>();
            else
                targetCamera = Camera.main;
        }
    }

    void Update()
    {
        if (targetCamera == null) return;
        bool is2DMode = targetCamera.orthographic;
        transform.rotation = targetCamera.transform.rotation;

        if (is2DMode)
        {
            transform.localScale = originalScale;
            canvasGroup.alpha = 1f;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, targetCamera.transform.position);
            float scaleFactor = distance / referenceDistance;
            transform.localScale = originalScale * scaleFactor;
            Vector3 viewportPoint = targetCamera.WorldToViewportPoint(transform.position);
            bool inFOV = viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;

            if (!inFOV || distance > visibleRange)
            {
                canvasGroup.alpha = 0f;
            }
            else
            {
                if (distance < visibleRange - fadeRange)
                {
                    canvasGroup.alpha = 1f;
                }
                else
                {
                    float t = (visibleRange - distance) / fadeRange;
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                }
            }
        }
    }
}