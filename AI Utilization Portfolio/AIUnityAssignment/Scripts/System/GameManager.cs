using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ObjectStateData
{
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
    public bool isActive;
}

[System.Serializable]
public class StringFloatPair { public string key; public float value; }
[System.Serializable]
public class StringBoolPair { public string key; public bool value; }

[System.Serializable]
public class ComponentChangingSaveData
{
    public bool isActive;
    public List<StringFloatPair> floatData = new List<StringFloatPair>();
    public List<StringBoolPair> boolData = new List<StringBoolPair>();
}

[System.Serializable]
public class MapData
{
    public Quaternion mapRotation;
    public List<ObjectStateData> movableObjects = new List<ObjectStateData>();
    public List<ComponentChangingSaveData> componentObjects = new List<ComponentChangingSaveData>();
}

[System.Serializable]
public class SaveData
{
    public string playerName = "이름없음";
    public float playTime = 0f;
    public string sceneDisplayName = "어딘가";
    public int currentHP;
    public float currentStamina;
    public Vector3 playerPosition;
    public string currentSceneName;
    public List<InventoryItem> inventory = new List<InventoryItem>();

    public bool attackActive;
    public List<bool> skillActive = new List<bool>();
    public int selectedSkill;
    public int quickSlotItemCode;

    public List<string> mapKeys = new List<string>();
    public List<MapData> mapValues = new List<MapData>();

    public List<int> eventKeys = new List<int>();
    public List<int> eventValues = new List<int>();
}

[System.Serializable]
public class ConfigData
{
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float bgsVolume = 1f;
    public float sfVolume = 1f;
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private string saveFolderPath;

    public Dictionary<string, MapData> mapDataDict = new Dictionary<string, MapData>();
    public Dictionary<int, int> eventCountDict = new Dictionary<int, int>(); // 이벤트 발동 여부 딕셔너리
    public ConfigData currentConfig = new ConfigData();

    public bool isFileLoaded = false;

    public float currentPlayTime = 0f;
    public string currentPlayerName = "플레이어";

    private float _lastAspect = -1f;

    [Header("Portal Transition Data")]
    public bool hasPendingPortal = false;
    public Vector3 pendingPlayerPosition;
    public Quaternion pendingMapRotation;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFolderPath = Application.persistentDataPath;
            LoadConfig();
            CheckSaveFiles();

#if !UNITY_EDITOR
            InitScreenResolution();
#endif
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _lastAspect = -1f;
        ApplyCameraRect();
    }
    public void SetPortalData(string nextScene, Vector3 nextPlayerPos, Vector3 nextMapRotEuler)
    {
        hasPendingPortal = true;
        pendingPlayerPosition = nextPlayerPos;
        pendingMapRotation = Quaternion.Euler(nextMapRotEuler);

        if (!mapDataDict.ContainsKey(nextScene))
        {
            mapDataDict[nextScene] = new MapData();
        }
        mapDataDict[nextScene].mapRotation = pendingMapRotation;
    }

    private void Update()
    {
        currentPlayTime += Time.unscaledDeltaTime;

        float aspect = (float)Screen.width / Screen.height;
        if (!Mathf.Approximately(aspect, _lastAspect))
        {
            _lastAspect = aspect;
            ApplyCameraRect();
        }
    }

    public void InitScreenResolution()
    {
        const int targetWidth = 1920;
        const int targetHeight = 1080;
        float deviceAspect = (float)Screen.width / Screen.height;
        float targetAspect = (float)targetWidth / targetHeight;

        int finalWidth, finalHeight;
        if (deviceAspect >= targetAspect)
        {
            finalHeight = targetHeight;
            finalWidth = Mathf.RoundToInt(targetHeight * deviceAspect);
        }
        else
        {
            finalWidth = targetWidth;
            finalHeight = Mathf.RoundToInt(targetWidth / deviceAspect);
        }
        Screen.SetResolution(finalWidth, finalHeight, FullScreenMode.FullScreenWindow);
    }

    public void ApplyCameraRect()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        const float targetAspect = 16f / 9f;
        float currentAspect = (float)Screen.width / Screen.height;

        Rect rect;
        if (currentAspect > targetAspect)
        {
            float normalizedWidth = targetAspect / currentAspect;
            rect = new Rect((1f - normalizedWidth) / 2f, 0f, normalizedWidth, 1f);
        }
        else if (currentAspect < targetAspect)
        {
            float normalizedHeight = currentAspect / targetAspect;
            rect = new Rect(0f, (1f - normalizedHeight) / 2f, 1f, normalizedHeight);
        }
        else
        {
            rect = new Rect(0f, 0f, 1f, 1f);
        }
        mainCam.rect = rect;
    }

    public void CheckSaveFiles()
    {
        isFileLoaded = false;
        for (int i = 0; i <= 20; i++)
        {
            if (File.Exists(GetSaveFilePath(i)))
            {
                isFileLoaded = true;
                break;
            }
        }
    }

    private string GetSaveFilePath(int slot) { return Path.Combine(saveFolderPath, $"save_{slot}.dat"); }
    private string GetConfigFilePath() { return Path.Combine(saveFolderPath, "config.json"); }

    public void SaveConfig()
    {
        string json = JsonUtility.ToJson(currentConfig, true);
        File.WriteAllText(GetConfigFilePath(), json);
    }

    public void LoadConfig()
    {
        string path = GetConfigFilePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentConfig = JsonUtility.FromJson<ConfigData>(json);
        }
        else SaveConfig();
    }

    public void SetMasterVolume(float vol) { currentConfig.masterVolume = vol; SaveConfig(); }
    public void SetBGMVolume(float vol) { currentConfig.bgmVolume = vol; SaveConfig(); }
    public void SetBGSVolume(float vol) { currentConfig.bgsVolume = vol; SaveConfig(); }
    public void SetSFVolume(float vol) { currentConfig.sfVolume = vol; SaveConfig(); }

    public SaveData LoadSaveDataMeta(int slot)
    {
        string path = GetSaveFilePath(slot);
        if (File.Exists(path)) return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        return null;
    }

    public void SaveGame(int slot)
    {
        SaveData data = new SaveData();
        data.playerName = currentPlayerName;
        data.playTime = currentPlayTime;
        data.currentSceneName = SceneManager.GetActiveScene().name;

        MapManager mapManager = FindObjectOfType<MapManager>();
        if (mapManager != null)
        {
            data.sceneDisplayName = mapManager.sceneDisplayName;
            UpdateMapData(mapManager.sceneID, mapManager.ExtractCurrentMapData());
        }
        else data.sceneDisplayName = "미지의 구역";

        foreach (var kvp in mapDataDict)
        {
            data.mapKeys.Add(kvp.Key);
            data.mapValues.Add(kvp.Value);
        }

        foreach (var kvp in eventCountDict)
        {
            data.eventKeys.Add(kvp.Key);
            data.eventValues.Add(kvp.Value);
        }

        PlayerState player = FindObjectOfType<PlayerState>();
        if (player != null)
        {
            data.currentHP = player.currentHP;
            data.currentStamina = player.currentStamina;
            data.inventory = new List<InventoryItem>(player.inventory);

            PlayerControl pc = FindObjectOfType<PlayerControl>();
            data.playerPosition = pc != null ? pc.transform.position : player.transform.position;

            data.attackActive = player.attackActive;
            data.skillActive = new List<bool>(player.skillActive);
            data.selectedSkill = player.selectedSkill;
            data.quickSlotItemCode = player.quickSlotItemCode;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSaveFilePath(slot), json);
        CheckSaveFiles();
        Debug.Log($"[GameManager] 슬롯 {slot}번에 게임 저장 완료.");
    }

    public void LoadGame(int slot)
    {
        string path = GetSaveFilePath(slot);
        if (!File.Exists(path)) return;

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));

        currentPlayerName = data.playerName;
        currentPlayTime = data.playTime;

        mapDataDict.Clear();
        for (int i = 0; i < data.mapKeys.Count; i++) mapDataDict[data.mapKeys[i]] = data.mapValues[i];

        eventCountDict.Clear();
        for (int i = 0; i < data.eventKeys.Count; i++) eventCountDict[data.eventKeys[i]] = data.eventValues[i];

        PlayerState playerState = FindObjectOfType<PlayerState>();
        if (playerState != null)
        {
            playerState.currentHP = data.currentHP;
            playerState.currentStamina = data.currentStamina;
            playerState.inventory = new List<InventoryItem>(data.inventory);

            playerState.attackActive = data.attackActive;
            playerState.skillActive = data.skillActive != null ? data.skillActive.ToArray() : new bool[3];
            playerState.selectedSkill = data.selectedSkill;
            playerState.quickSlotItemCode = data.quickSlotItemCode;
        }

        hasPendingPortal = true;
        pendingPlayerPosition = data.playerPosition;

        Debug.Log($"[GameManager] 슬롯 {slot}번 데이터 로드 완료. (저장된 씬: {data.currentSceneName})");
    }

    public void UpdateMapData(string sceneID, MapData data) { mapDataDict[sceneID] = data; }
    public MapData GetMapData(string sceneID) { return mapDataDict.ContainsKey(sceneID) ? mapDataDict[sceneID] : null; }
    public int GetEventState(int eventID) { return eventCountDict.ContainsKey(eventID) ? eventCountDict[eventID] : 0; }
    public void SetEventState(int eventID, int state) { eventCountDict[eventID] = state; }
}