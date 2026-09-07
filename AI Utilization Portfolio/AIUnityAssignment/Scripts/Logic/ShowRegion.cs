using System.Collections;
using UnityEngine;
using TMPro;

public class ShowRegion : MonoBehaviour
{
    public static ShowRegion Instance { get; private set; }

    [Header("UI References")]
    public CanvasGroup regionPanelGroup;
    public TextMeshProUGUI regionText;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float displayDuration = 2.0f;

    private Coroutine currentCoroutine;
    private string lastRegionName = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (regionPanelGroup != null)
        {
            regionPanelGroup.alpha = 0f;
            regionPanelGroup.gameObject.SetActive(false);
        }
    }

    public void ShowRegionName(string newRegionName)
    {
        if (lastRegionName == newRegionName && regionPanelGroup.alpha > 0) return;

        lastRegionName = newRegionName;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(RegionDisplayRoutine(newRegionName));
    }

    private IEnumerator RegionDisplayRoutine(string regionName)
    {
        regionText.text = regionName;
        regionPanelGroup.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            regionPanelGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        regionPanelGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            regionPanelGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        regionPanelGroup.alpha = 0f;
        regionPanelGroup.gameObject.SetActive(false);
    }
}