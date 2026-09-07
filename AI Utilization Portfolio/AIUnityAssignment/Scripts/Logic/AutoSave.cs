using UnityEngine;

public class AutoSave : MonoBehaviour
{
    [Header("자동 저장 설정")]
    [Tooltip("씬이 시작될 때(Start) 자동으로 저장을 실행할지 여부")]
    public bool saveOnStart = true;

    [Tooltip("오브젝트가 켜질 때(OnEnable) 자동으로 저장을 실행할지 여부")]
    public bool saveOnEnable = false;

    private void Start()
    {
        if (saveOnStart)
        {
            TriggerAutoSave();
        }
    }

    private void OnEnable()
    {
        if (saveOnEnable)
        {
            TriggerAutoSave();
        }
    }

    public void TriggerAutoSave()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame(0);
        }
    }
}