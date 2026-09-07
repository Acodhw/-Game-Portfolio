using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [Header("Title Buttons")]
    public Button newStartBtn;
    public Button loadBtn;
    public Button quitBtn;

    [Header("Fade Effect")]
    [Tooltip("검은색 페이드용 이미지")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    [Header("Load UI Objects")]
    public GameObject loadPanel;
    public Transform saveLoadContent;
    public Button loadCancelBtn;
    public GameObject saveLoadButtonPrefab;

    [Header("Scene Settings")]
    [Tooltip("게임 시작 씬")]
    public string firstSceneName = "TutorialScene";

    private AudioAdd uiAudio;

    void Start()
    {
        uiAudio = GetComponent<AudioAdd>();

        newStartBtn.onClick.AddListener(OnNewStartClicked);
        newStartBtn.onClick.AddListener(PlayUISound);

        loadBtn.onClick.AddListener(OnLoadUIOpened);
        loadBtn.onClick.AddListener(PlayUISound);

        quitBtn.onClick.AddListener(OnQuitClicked);
        quitBtn.onClick.AddListener(PlayUISound);

        loadCancelBtn.onClick.AddListener(OnLoadUICancelled);
        loadCancelBtn.onClick.AddListener(PlayUISound);

        loadPanel.SetActive(false);

        StartCoroutine(FadeRoutine(1f, 0f, fadeDuration, () => {
            fadeImage.gameObject.SetActive(false);
        }));
    }
    private void PlayUISound()
    {
        if (uiAudio != null) uiAudio.PlaySound();
    }

    private void OnNewStartClicked()
    {
        StartCoroutine(FadeRoutine(0f, 1f, fadeDuration, () => {
            LoadingScene.LoadScene(firstSceneName);
        }));
    }

    private void OnQuitClicked()
    {
        StartCoroutine(FadeRoutine(0f, 1f, fadeDuration, () => {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }));
    }
    private void OnLoadUIOpened()
    {
        loadPanel.SetActive(true);
        RefreshLoadSlots();
    }

    private void OnLoadUICancelled()
    {
        loadPanel.SetActive(false);
    }

    private void RefreshLoadSlots()
    {
        foreach (Transform child in saveLoadContent)
            Destroy(child.gameObject);

        if (saveLoadButtonPrefab == null) return;

        for (int i = 0; i <= 20; i++)
        {
            int slotIndex = i;
            GameObject slotObj = Instantiate(saveLoadButtonPrefab, saveLoadContent);

            TextMeshProUGUI numText = slotObj.transform.Find("FileNO")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI sceneText = slotObj.transform.Find("SceneName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerText = slotObj.transform.Find("PlayerName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeText = slotObj.transform.Find("PlayTime")?.GetComponent<TextMeshProUGUI>();

            if (numText != null) numText.text = (i == 0) ? "Auto Save" : $"File {i}";

            SaveData dataMeta = GameManager.Instance.LoadSaveDataMeta(i);

            Button btn = slotObj.GetComponent<Button>();

            if (dataMeta != null)
            {
                if (sceneText != null) sceneText.text = dataMeta.sceneDisplayName;
                if (playerText != null) playerText.text = dataMeta.playerName;
                if (timeText != null)
                {
                    System.TimeSpan t = System.TimeSpan.FromSeconds(dataMeta.playTime);
                    timeText.text = $"플레이 시간 : {string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds)}";
                }
                if (btn != null)
                {
                    btn.onClick.AddListener(() => OnLoadSlotClicked(slotIndex));
                    btn.onClick.AddListener(PlayUISound);
                }
            }
            else
            {
                if (sceneText != null) sceneText.text = "";
                if (playerText != null) playerText.text = "비어있음";
                if (timeText != null) timeText.text = "";
                if (btn != null) btn.interactable = false;
            }
        }
    }

    private void OnLoadSlotClicked(int slot)
    {
        loadPanel.SetActive(false);
        SaveData dataMeta = GameManager.Instance.LoadSaveDataMeta(slot);
        if (dataMeta == null) return;
        string targetSceneName = dataMeta.currentSceneName;

        StartCoroutine(FadeRoutine(0f, 1f, fadeDuration, () => {
            GameManager.Instance.LoadGame(slot);
            LoadingScene.LoadScene(targetSceneName);
        }));
    }
    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration, System.Action onComplete = null)
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = startAlpha;
        fadeImage.color = color;

        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;

        onComplete?.Invoke();
    }
}