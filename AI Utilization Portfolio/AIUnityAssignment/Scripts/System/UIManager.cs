using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI State (Read Only)")]
    public bool isPaused = false;
    public bool isTalking = false;
    public bool onInventory = false;
    public bool isTyping = false;

    [Header("UI Lock")]
    [Tooltip("true이면 일시정지 창이 열리지 않습니다.")]
    public bool pauseLock = false;
    [Tooltip("true이면 인벤토리 창이 열리지 않습니다.")]
    public bool inventoryLock = false;

    [Header("Item Cooldown")]
    private float lastItemUseTime = -1f;
    private const float ITEM_USE_COOLDOWN = 0.1f; 

    [Header("Fade Effect")]
    public Image fadeImage; 

    [Header("Main HUD Objects")]
    [Tooltip("돌아다니면서 표시되는 텍스트")]
    public TextMeshProUGUI floatingNameText;
    public TextMeshProUGUI floatingContentText;

    [Tooltip("상호작용 가능 알림 팝업")]
    public GameObject interactAlertObj;

    [Tooltip("퀵슬롯 아이콘 표시용 이미지")]
    public Image mainQuickSlotIcon;

    [Tooltip("퀵슬롯 남은 개수 표시")]
    public TextMeshProUGUI mainQuickSlotAmountText;

    [Tooltip("메인 화면에 항상 떠있는 스킬 아이콘 패널")]
    public GameObject mainHudSkillPanel;

    private bool? lastAttackActiveState = null;

    [Header("Skill UI Objects")]
    public GameObject mainPanel;
    public GameObject setValuePanel;
    public Button finishBtn;
    public Transform blockInvenContent;
    public Transform playerInvenContent;
    public TextMeshProUGUI setValueTitleText;
    public Button option1Btn;
    public Button option2Btn;
    public Button option3Btn;
    public Button cancelBtn;

    [Header("Skill UI Prefab")]
    public GameObject skillButtonPrefab;

    private ComponentChangingObj targetObj;
    private string pendingComponent = "";
    private static List<string> playerGlobalInventory = new List<string>();

    [Header("Talk UI Objects")]
    public GameObject talkPanel;
    public GameObject talkNamePanel; 
    public TextMeshProUGUI talkNameText;
    public TextMeshProUGUI talkContentText;
    public GameObject talkSelectionContainer;

    [Header("Talk Setting")]
    public float typingSpeed = 0.05f;
    private Coroutine typingCoroutine;
    private string currentTalkFullSentence = "";

    [Header("Talk UI Prefab")]
    public GameObject talkButtonPrefab;

    [Header("Inventory UI Objects")]
    public GameObject inventoryPanel;
    public Transform inventoryContent;
    public TextMeshProUGUI invenItemNameText;
    public TextMeshProUGUI invenItemDescText;
    public Button invenUseBtn;
    public Button invenQuickBtn;
    public Button invenDropBtn;
    public Button invenCancelBtn;
    public TextMeshProUGUI invenWarningText;

    [Header("Inventory UI Prefab")]
    public GameObject inventoryButtonPrefab;
    private int currentSelectedInvenItemCode = -1;
    private class InvenBtnCache
    {
        public GameObject btnObj;
        public TextMeshProUGUI amountText;
        public int lastAmount;
    }

    private Dictionary<int, InvenBtnCache> spawnedInvenBtns = new Dictionary<int, InvenBtnCache>();

    [Header("Pause UI Objects")]
    public GameObject pausePanel;
    public Slider masterVolSlider;
    public Slider bgmVolSlider;
    public Slider bgsVolSlider;
    public Slider sfVolSlider;
    public Button pauseSaveBtn, pauseLoadBtn, pauseTitleBtn, pauseContinueBtn;
    public enum SaveLoadMode { Save, Load }
    private SaveLoadMode currentSaveLoadMode;

    [Header("Save & Load UI Objects")]
    public GameObject saveLoadPanel;
    public TextMeshProUGUI saveLoadTitleText;
    public Transform saveLoadContent;
    public Button saveLoadCancelBtn;

    [Header("Save/Load UI Prefab")]
    public GameObject saveLoadButtonPrefab;
    private int pendingSaveLoadSlot = -1;

    [Header("Caution UI Objects")]
    public GameObject cautionPanel;
    public TextMeshProUGUI cautionDescText;
    public Button cautionYesBtn;
    public Button cautionNoBtn;

    private System.Action onCautionYes;
    private System.Action onCautionNo;

    private AudioAdd uiAudio;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }
    }

    void Start()
    {
        uiAudio = GetComponent<AudioAdd>();

        if (finishBtn != null)
        {
            finishBtn.onClick.AddListener(OnFinishClicked);
            finishBtn.onClick.AddListener(PlayUISound);
        }
        if (cancelBtn != null)
        {
            cancelBtn.onClick.AddListener(OnSetValueCancelClicked);
            cancelBtn.onClick.AddListener(PlayUISound);
        }

        InitInventoryUI();
        InitPauseUI();
        InitSaveLoadUI();
        InitCautionUI();

        CloseAllUI();

        SetInteractAlert(false);
        HideFloatingTalk();

        if (fadeImage != null)
        {
            StartCoroutine(FadeRoutine(1f, 0f, 0.75f, () => {
                fadeImage.gameObject.SetActive(false);

                if (!talkPanel.activeSelf)
                {
                    isTalking = false;
                }
            }));
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (cautionPanel.activeSelf) { OnCautionNoClicked(); PlayUISound(); }
            else if (saveLoadPanel.activeSelf) { CloseSaveLoadUI(); PlayUISound(); }
            else if (setValuePanel.activeSelf) { OnSetValueCancelClicked(); PlayUISound(); }
            else if (mainPanel.activeSelf) { OnFinishClicked(); PlayUISound(); }
            else if (onInventory) { ToggleInventory(); PlayUISound(); }
            else if (!isTalking)
            {
                if (!isPaused && pauseLock) return;
                TogglePause();
            }
        }

        if (Input.GetButtonDown("Inventory"))
        {
            if (!isPaused && !isTalking && !cautionPanel.activeSelf)
            {
                if (!onInventory && inventoryLock) return;

                ToggleInventory();
                PlayUISound();
            }
        }

        UpdateMainHUD();

        if (onInventory)
        {
            SyncInventoryUI();
        }
    }

    public void PlayUISound()
    {
        if (uiAudio != null) uiAudio.PlaySound();
    }

    private void CloseAllUI()
    {
        mainPanel.SetActive(false);
        setValuePanel.SetActive(false);
        talkPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        pausePanel.SetActive(false);
        saveLoadPanel.SetActive(false);
        cautionPanel.SetActive(false);

        isPaused = false;
        isTalking = false;
        onInventory = false;
        isTyping = false;
        Time.timeScale = 1f;
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration, System.Action onComplete = null)
    {
        if (fadeImage == null) yield break;

        isTalking = true; 

        yield return null;

        fadeImage.gameObject.SetActive(true);

        Color color = fadeImage.color;
        color.a = startAlpha;
        fadeImage.color = color;

        float time = 0f;
        while (time < duration)
        {
            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.1f);
            time += dt;

            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;

        onComplete?.Invoke();
    }

    public void SetInteractAlert(bool isVisible)
    {
        if (interactAlertObj != null) interactAlertObj.SetActive(isVisible);
    }

    public void ShowFloatingTalk(string name, string content)
    {
        if (floatingNameText != null)
        {
            floatingNameText.gameObject.SetActive(true);
            floatingNameText.text = name;
        }
        if (floatingContentText != null)
        {
            floatingContentText.gameObject.SetActive(true);
            floatingContentText.text = content;
        }
    }

    public void HideFloatingTalk()
    {
        if (floatingNameText != null) floatingNameText.gameObject.SetActive(false);
        if (floatingContentText != null) floatingContentText.gameObject.SetActive(false);
    }

    private void UpdateMainHUD()
    {
        PlayerState player = FindObjectOfType<PlayerState>();
        if (player == null) return;

        if (mainHudSkillPanel != null)
        {
            if (lastAttackActiveState == null || player.attackActive != lastAttackActiveState.Value)
            {
                lastAttackActiveState = player.attackActive;
                mainHudSkillPanel.SetActive(player.attackActive);
            }
        }

        if (mainQuickSlotIcon == null) return;

        player.CheckQuickSlotItem();

        if (player.quickSlotItemCode == -1)
        {
            mainQuickSlotIcon.gameObject.SetActive(false);
            if (mainQuickSlotAmountText != null) mainQuickSlotAmountText.gameObject.SetActive(false);
        }
        else
        {
            ItemData data = ItemList.Instance.GetItemData(player.quickSlotItemCode);
            if (data != null)
            {
                mainQuickSlotIcon.gameObject.SetActive(true);
                mainQuickSlotIcon.sprite = data.itemIcon;

                if (mainQuickSlotAmountText != null)
                {
                    InventoryItem invenItem = player.inventory.Find(x => x.itemCode == player.quickSlotItemCode);
                    if (invenItem != null)
                    {
                        mainQuickSlotAmountText.gameObject.SetActive(true);
                        mainQuickSlotAmountText.text = invenItem.amount.ToString();
                    }
                }
            }
        }
    }

    public void OpenTalk(string name, string content, AudioClip typingSound = null)
    {
        isTalking = true;
        talkPanel.SetActive(true);

        if (string.IsNullOrEmpty(name))
        {
            if (talkNamePanel != null) talkNamePanel.SetActive(false);
            talkNameText.text = "";
        }
        else
        {
            if (talkNamePanel != null) talkNamePanel.SetActive(true);
            talkNameText.text = name;
        }

        talkSelectionContainer.SetActive(false);

        currentTalkFullSentence = content;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentenceCoroutine(content, typingSound));
    }

    private IEnumerator TypeSentenceCoroutine(string content, AudioClip typingSound)
    {
        isTyping = true;
        talkContentText.text = "";

        bool isRichTextTag = false;

        foreach (char letter in content.ToCharArray())
        {
            if (letter == '<')
            {
                isRichTextTag = true;
            }

            talkContentText.text += letter;

            if (isRichTextTag)
            {
                if (letter == '>')
                {
                    isRichTextTag = false;
                }
                continue; 
            }

            if (letter != ' ' && typingSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(typingSound, true);
            }

            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        talkContentText.text = currentTalkFullSentence;
        isTyping = false;
    }

    public void CloseTalk()
    {
        isTalking = false;
        isTyping = false;
        talkPanel.SetActive(false);
    }

    public void CreateTalkSelection(string choiceText, UnityEngine.Events.UnityAction action)
    {
        talkSelectionContainer.SetActive(true);
        if (talkButtonPrefab == null) return;
        GameObject btnObj = Instantiate(talkButtonPrefab, talkSelectionContainer.transform);
        TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null) btnText.text = choiceText;
        Button btn = btnObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(action);
            btn.onClick.AddListener(ClearTalkSelections);
            btn.onClick.AddListener(PlayUISound);
        }
    }

    public void ClearTalkSelections()
    {
        foreach (Transform child in talkSelectionContainer.transform) Destroy(child.gameObject);
        talkSelectionContainer.SetActive(false);
    }
    public void OpenUI(ComponentChangingObj target)
    {
        targetObj = target;
        targetObj.PausePhysics();
        mainPanel.SetActive(true);
        setValuePanel.SetActive(false);
        RefreshAllUI();
    }

    public void OnFinishClicked()
    {
        mainPanel.SetActive(false);
        if (targetObj != null) { targetObj.ResumePhysics(); targetObj = null; }
    }

    private void RefreshAllUI()
    {
        foreach (Transform child in blockInvenContent) Destroy(child.gameObject);
        foreach (Transform child in playerInvenContent) Destroy(child.gameObject);
        foreach (var comp in targetObj.AddedComp)
            if (comp.Value == true) CreateSkillButton(blockInvenContent, comp.Key, () => OnBlockInvenItemClicked(comp.Key));
        foreach (var compName in playerGlobalInventory)
            CreateSkillButton(playerInvenContent, compName, () => OnPlayerInvenItemClicked(compName));
    }

    private void OnBlockInvenItemClicked(string compName)
    {
        targetObj.AddedComp[compName] = false;
        if (targetObj.changedTransform.ContainsKey(compName))
            targetObj.changedTransform[compName] = compName.StartsWith("Scale") || compName.StartsWith("Mass") ? 1f : 0f;
        playerGlobalInventory.Add(compName);
        RefreshAllUI();
    }

    private void OnPlayerInvenItemClicked(string compName)
    {
        if (targetObj.AddedComp[compName] == true) return;
        if (targetObj.changedTransform.ContainsKey(compName))
        {
            pendingComponent = compName;
            OpenSetValuePanel(compName);
        }
        else
        {
            playerGlobalInventory.Remove(compName);
            targetObj.AddedComp[compName] = true;
            RefreshAllUI();
        }
    }

    private void OpenSetValuePanel(string compName)
    {
        setValuePanel.SetActive(true);
        setValueTitleText.text = "Set: " + compName;

        option1Btn.onClick.RemoveAllListeners();
        option2Btn.onClick.RemoveAllListeners();
        option3Btn.onClick.RemoveAllListeners();

        option1Btn.onClick.AddListener(PlayUISound);
        option2Btn.onClick.AddListener(PlayUISound);
        option3Btn.onClick.AddListener(PlayUISound);

        if (compName.StartsWith("Scale") || compName.StartsWith("Mass"))
        {
            option1Btn.gameObject.SetActive(true); option2Btn.gameObject.SetActive(false); option3Btn.gameObject.SetActive(true);
            option1Btn.GetComponentInChildren<TextMeshProUGUI>().text = "x0.5"; option3Btn.GetComponentInChildren<TextMeshProUGUI>().text = "x2.0";
            option1Btn.onClick.AddListener(() => ApplyValueToComponent(0.5f)); option3Btn.onClick.AddListener(() => ApplyValueToComponent(2.0f));
        }
        else if (compName.StartsWith("Rot"))
        {
            option1Btn.gameObject.SetActive(true); option2Btn.gameObject.SetActive(true); option3Btn.gameObject.SetActive(true);
            option1Btn.GetComponentInChildren<TextMeshProUGUI>().text = "30"; option2Btn.GetComponentInChildren<TextMeshProUGUI>().text = "45"; option3Btn.GetComponentInChildren<TextMeshProUGUI>().text = "60";
            option1Btn.onClick.AddListener(() => ApplyValueToComponent(30f)); option2Btn.onClick.AddListener(() => ApplyValueToComponent(45f)); option3Btn.onClick.AddListener(() => ApplyValueToComponent(60f));
        }
    }

    public void OnSetValueCancelClicked() { pendingComponent = ""; setValuePanel.SetActive(false); }

    private void ApplyValueToComponent(float value)
    {
        if (string.IsNullOrEmpty(pendingComponent)) return;
        playerGlobalInventory.Remove(pendingComponent);
        targetObj.AddedComp[pendingComponent] = true;
        targetObj.changedTransform[pendingComponent] = value;
        pendingComponent = "";
        setValuePanel.SetActive(false);
        RefreshAllUI();
    }

    private void CreateSkillButton(Transform parent, string textStr, UnityEngine.Events.UnityAction action)
    {
        if (skillButtonPrefab == null) return;
        GameObject btnObj = Instantiate(skillButtonPrefab, parent);
        TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null) btnText.text = textStr;
        Button btn = btnObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(action);
            btn.onClick.AddListener(PlayUISound);
        }
    }
    private void InitInventoryUI()
    {
        // 💡 인벤토리 버튼들에 사운드 추가
        if (invenUseBtn != null) { invenUseBtn.onClick.AddListener(OnInvenUseClicked); invenUseBtn.onClick.AddListener(PlayUISound); }
        if (invenQuickBtn != null) { invenQuickBtn.onClick.AddListener(OnInvenQuickClicked); invenQuickBtn.onClick.AddListener(PlayUISound); }
        if (invenDropBtn != null) { invenDropBtn.onClick.AddListener(OnInvenDropClicked); invenDropBtn.onClick.AddListener(PlayUISound); }
        if (invenCancelBtn != null) { invenCancelBtn.onClick.AddListener(ToggleInventory); invenCancelBtn.onClick.AddListener(PlayUISound); }

        if (invenWarningText != null) invenWarningText.gameObject.SetActive(false);
    }

    public void ToggleInventory()
    {
        if (isPaused) return;
        if (!onInventory && inventoryLock) return;

        onInventory = !onInventory;
        inventoryPanel.SetActive(onInventory);
        if (onInventory)
        {
            SyncInventoryUI();
            invenItemNameText.text = ""; invenItemDescText.text = "";
            currentSelectedInvenItemCode = -1;
        }
    }
    private void SyncInventoryUI()
    {
        PlayerState player = FindObjectOfType<PlayerState>();
        if (player == null || ItemList.Instance == null) return;

        HashSet<int> currentItems = new HashSet<int>();

        foreach (var item in player.inventory)
        {
            currentItems.Add(item.itemCode);

            if (spawnedInvenBtns.TryGetValue(item.itemCode, out InvenBtnCache cache))
            {
                if (cache.lastAmount != item.amount)
                {
                    if (cache.amountText != null) cache.amountText.text = "X" + item.amount;
                    cache.lastAmount = item.amount;
                }
            }
            else
            {
                if (inventoryButtonPrefab == null) continue;
                GameObject newBtnObj = Instantiate(inventoryButtonPrefab, inventoryContent);

                ItemData data = ItemList.Instance.GetItemData(item.itemCode);
                string tempName = data != null ? data.itemName : "아이템_" + item.itemCode;
                string tempDesc = data != null ? data.itemDescription : "설명 없음";

                TextMeshProUGUI amountText = newBtnObj.transform.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
                if (amountText == null) amountText = newBtnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (amountText != null) amountText.text = "X" + item.amount;

                Image itemIcon = newBtnObj.transform.Find("Image")?.GetComponent<Image>();
                if (itemIcon == null) itemIcon = newBtnObj.GetComponent<Image>();
                if (itemIcon != null && data != null && data.itemIcon != null)
                    itemIcon.sprite = data.itemIcon;

                Button btn = newBtnObj.GetComponent<Button>();
                int code = item.itemCode;
                if (btn != null)
                {
                    btn.onClick.AddListener(() => OnInventoryItemClicked(code, tempName, tempDesc));
                    btn.onClick.AddListener(PlayUISound);
                }
                spawnedInvenBtns[code] = new InvenBtnCache
                {
                    btnObj = newBtnObj,
                    amountText = amountText,
                    lastAmount = item.amount
                };
            }
        }
        List<int> toRemove = new List<int>();
        foreach (var kvp in spawnedInvenBtns)
        {
            if (!currentItems.Contains(kvp.Key))
            {
                Destroy(kvp.Value.btnObj);
                toRemove.Add(kvp.Key);

                if (currentSelectedInvenItemCode == kvp.Key)
                {
                    currentSelectedInvenItemCode = -1;
                    invenItemNameText.text = "";
                    invenItemDescText.text = "";
                }
            }
        }
        foreach (int code in toRemove)
        {
            spawnedInvenBtns.Remove(code);
        }
    }

    private void OnInventoryItemClicked(int itemCode, string itemName, string itemDesc)
    {
        currentSelectedInvenItemCode = itemCode;
        invenItemNameText.text = itemName;
        invenItemDescText.text = itemDesc;
    }

    private void OnInvenUseClicked()
    {
        if (currentSelectedInvenItemCode == -1) return;
        UseItemLogic(currentSelectedInvenItemCode);
    }

    public void UseQuickSlotItem()
    {
        PlayerState player = FindObjectOfType<PlayerState>();
        if (player != null && player.quickSlotItemCode != -1)
        {
            UseItemLogic(player.quickSlotItemCode);
        }
    }

    private void UseItemLogic(int itemCode)
    {
        if (Time.unscaledTime - lastItemUseTime < ITEM_USE_COOLDOWN)
        {
            return;
        }

        ItemData data = ItemList.Instance.GetItemData(itemCode);
        if (data != null && !data.canUse)
        {
            ShowInvenWarning("사용 불가능한 아이템입니다.");
            return;
        }

        PlayerState player = FindObjectOfType<PlayerState>();
        if (player != null)
        {
            InventoryItem invenItem = player.inventory.Find(x => x.itemCode == itemCode);
            if (invenItem == null || invenItem.amount <= 0) return;

            lastItemUseTime = Time.unscaledTime;
            data?.onUseEffect?.Invoke();
            player.RemoveItem(itemCode, 1);

            if (onInventory) SyncInventoryUI();
            UpdateMainHUD();
        }
    }

    private void OnInvenQuickClicked()
    {
        if (currentSelectedInvenItemCode == -1) return;

        ItemData data = ItemList.Instance.GetItemData(currentSelectedInvenItemCode);
        if (data != null && !data.canUse)
        {
            ShowInvenWarning("사용할 수 없습니다.");
            return;
        }

        PlayerState player = FindObjectOfType<PlayerState>();
        if (player != null)
        {
            player.quickSlotItemCode = currentSelectedInvenItemCode;
            UpdateMainHUD();
            ToggleInventory();
        }
    }

    private void OnInvenDropClicked()
    {
        if (currentSelectedInvenItemCode == -1) return;

        ItemData data = ItemList.Instance.GetItemData(currentSelectedInvenItemCode);
        if (data != null && !data.canDrop)
        {
            ShowInvenWarning("버리면 큰일날 것 같다.");
            return;
        }

        PlayerState player = FindObjectOfType<PlayerState>();
        if (player != null)
        {
            player.RemoveItem(currentSelectedInvenItemCode, 1);
            SyncInventoryUI();
        }
    }

    private void ShowInvenWarning(string msg)
    {
        StopCoroutine("InvenWarningRoutine");
        StartCoroutine(InvenWarningRoutine(msg));
    }

    private IEnumerator InvenWarningRoutine(string msg)
    {
        invenWarningText.text = msg;
        invenWarningText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        invenWarningText.gameObject.SetActive(false);
    }
    private void InitPauseUI()
    {
        pauseSaveBtn.onClick.AddListener(() => OpenSaveLoadUI(SaveLoadMode.Save));
        pauseSaveBtn.onClick.AddListener(PlayUISound);

        pauseLoadBtn.onClick.AddListener(() => OpenSaveLoadUI(SaveLoadMode.Load));
        pauseLoadBtn.onClick.AddListener(PlayUISound);

        pauseContinueBtn.onClick.AddListener(TogglePause);
        pauseContinueBtn.onClick.AddListener(PlayUISound);

        pauseTitleBtn.onClick.AddListener(() => {
            ShowCaution("저장되지 않은 내용은 사라집니다.\n타이틀로 돌아가시겠습니까?", () => {
                Time.timeScale = 1f; 
                CloseAllUI();        
                StartCoroutine(FadeRoutine(0f, 1f, 0.75f, () => {
                    LoadingScene.LoadScene("TitleScene");
                }));
            });
        });
        pauseTitleBtn.onClick.AddListener(PlayUISound);

        masterVolSlider.onValueChanged.AddListener((v) => GameManager.Instance.SetMasterVolume(v));
        bgmVolSlider.onValueChanged.AddListener((v) => GameManager.Instance.SetBGMVolume(v));
        bgsVolSlider.onValueChanged.AddListener((v) => GameManager.Instance.SetBGSVolume(v));
        sfVolSlider.onValueChanged.AddListener((v) => GameManager.Instance.SetSFVolume(v));
    }

    public void TogglePause()
    {
        if (isTalking || onInventory) return;
        if (!isPaused && pauseLock) return;

        PlayerControl pc = FindObjectOfType<PlayerControl>();
        if (!isPaused && pc != null && pc.is2D)
        {
            pc.ForceCancelSkill();
        }
        PlayUISound();

        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            masterVolSlider.value = GameManager.Instance.currentConfig.masterVolume;
            bgmVolSlider.value = GameManager.Instance.currentConfig.bgmVolume;
            bgsVolSlider.value = GameManager.Instance.currentConfig.bgsVolume;
            sfVolSlider.value = GameManager.Instance.currentConfig.sfVolume;
        }
        else Time.timeScale = 1f;
    }

    private void InitSaveLoadUI()
    {
        saveLoadCancelBtn.onClick.AddListener(CloseSaveLoadUI);
        saveLoadCancelBtn.onClick.AddListener(PlayUISound);
    }

    public void OpenSaveLoadUI(SaveLoadMode mode)
    {
        currentSaveLoadMode = mode;
        saveLoadPanel.SetActive(true);
        saveLoadTitleText.text = (mode == SaveLoadMode.Save) ? "일지 기록 위치" : "어디부터 다시 읽을까요";
        RefreshSaveLoadSlots();
    }

    public void CloseSaveLoadUI() { saveLoadPanel.SetActive(false); }

    private void RefreshSaveLoadSlots()
    {
        foreach (Transform child in saveLoadContent) Destroy(child.gameObject);
        if (saveLoadButtonPrefab == null) return;

        int startIndex = (currentSaveLoadMode == SaveLoadMode.Save) ? 1 : 0;

        for (int i = startIndex; i <= 20; i++)
        {
            int slotIndex = i;
            GameObject slotObj = Instantiate(saveLoadButtonPrefab, saveLoadContent);

            TextMeshProUGUI numText = slotObj.transform.Find("FileNO")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI sceneText = slotObj.transform.Find("SceneName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerText = slotObj.transform.Find("PlayerName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeText = slotObj.transform.Find("PlayTime")?.GetComponent<TextMeshProUGUI>();

            if (numText != null) numText.text = (i == 0) ? "Auto Save" : $"File {i}";

            SaveData dataMeta = GameManager.Instance.LoadSaveDataMeta(i);

            if (dataMeta != null)
            {
                if (sceneText != null) sceneText.text = dataMeta.sceneDisplayName;
                if (playerText != null) playerText.text = dataMeta.playerName;
                if (timeText != null)
                {
                    System.TimeSpan t = System.TimeSpan.FromSeconds(dataMeta.playTime);
                    timeText.text = $"플레이 시간 : {string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds)}";
                }
            }
            else
            {
                if (sceneText != null) sceneText.text = "";
                if (playerText != null) playerText.text = "비어있음";
                if (timeText != null) timeText.text = "";
            }

            Button btn = slotObj.GetComponent<Button>();
            if (btn != null)
            {
                bool hasData = (dataMeta != null);
                btn.onClick.AddListener(() => OnSaveLoadSlotClicked(slotIndex, hasData));
                btn.onClick.AddListener(PlayUISound);
            }
        }
    }

    private void OnSaveLoadSlotClicked(int slot, bool hasData)
    {
        pendingSaveLoadSlot = slot;
        if (currentSaveLoadMode == SaveLoadMode.Save)
        {
            if (hasData) ShowCaution("내용을 덮어씌웁니다. 괜찮습니까?", ExecuteSave);
            else ExecuteSave();
        }
        else
        {
            if (hasData) ShowCaution("저장되지 않은 내역은 사라집니다. 괜찮습니까?", ExecuteLoad);
        }
    }

    private void ExecuteSave()
    {
        if (pendingSaveLoadSlot == -1) return;
        GameManager.Instance.SaveGame(pendingSaveLoadSlot);
        RefreshSaveLoadSlots();
    }

    private void ExecuteLoad()
    {
        if (pendingSaveLoadSlot == -1) return;
        Time.timeScale = 1f;
        SaveData dataMeta = GameManager.Instance.LoadSaveDataMeta(pendingSaveLoadSlot);
        if (dataMeta == null) return;
        string targetSceneName = dataMeta.currentSceneName;

        CloseAllUI();
        StartCoroutine(FadeRoutine(0f, 1f, 0.75f, () => {
            GameManager.Instance.LoadGame(pendingSaveLoadSlot);
            LoadingScene.LoadScene(targetSceneName);
        }));
    }

    private void InitCautionUI()
    {
        cautionYesBtn.onClick.AddListener(OnCautionYesClicked);
        cautionYesBtn.onClick.AddListener(PlayUISound);

        cautionNoBtn.onClick.AddListener(OnCautionNoClicked);
        cautionNoBtn.onClick.AddListener(PlayUISound);
    }

    public void ShowCaution(string message, System.Action onYes, System.Action onNo = null)
    {
        cautionDescText.text = message;
        onCautionYes = onYes;
        onCautionNo = onNo;
        cautionPanel.SetActive(true);
    }

    private void OnCautionYesClicked() { cautionPanel.SetActive(false); onCautionYes?.Invoke(); }
    private void OnCautionNoClicked() { cautionPanel.SetActive(false); onCautionNo?.Invoke(); }
}