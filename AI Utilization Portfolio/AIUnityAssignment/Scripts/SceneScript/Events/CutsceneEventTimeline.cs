using UnityEngine;
using UnityEngine.Playables;

public class CutsceneEventTimeline : MonoBehaviour
{
    public enum TriggerType
    {
        OnStart,         
        OnPlayerEnterArea 
    }

    [Header("이벤트 식별")]
    [Tooltip("이 컷신의 고유 이벤트 ID")]
    public int eventID;

    [Header("실행 조건")]
    [Tooltip("이 컷신이 실행되기 위한 트리거 방식")]
    public TriggerType triggerType;

    [Tooltip("선행 이벤트가 필요한지 여부")]
    public bool requirePrerequisite;
    [Tooltip("선행되어야 하는 이벤트 ID ")]
    public int prerequisiteEventID;

    [Header("컴포넌트 참조")]
    [Tooltip("씬 내에 존재하는 PlayableDirector")]
    public PlayableDirector timelineDirector;
    [Tooltip("실행할 타임라인 에셋")]
    public PlayableAsset timelineAsset;
    [Tooltip("트리거 방식을 OnPlayerEnterArea로 했을 때 사용할 박스 콜라이더")]
    public BoxCollider triggerArea;

    private bool hasPlayed = false;
    private Component playerControl;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning($"[CutsceneEventTimeline] GameManager가 없어 이벤트 {eventID}를 초기화할 수 없습니다.");
            return;
        }
        if (GameManager.Instance.eventCountDict.TryGetValue(eventID, out int count))
        {
            if (count > 0)
            {
                hasPlayed = true;
                return;
            }
        }

        if (triggerType == TriggerType.OnPlayerEnterArea)
        {
            GameObject playerObj = GameObject.FindObjectOfType(System.Type.GetType("PlayerControl")) as GameObject;
            Component[] allComponents = FindObjectsOfType<Component>();
            foreach (var comp in allComponents)
            {
                if (comp.GetType().Name == "PlayerControl")
                {
                    playerControl = comp;
                    break;
                }
            }
        }

        if (triggerType == TriggerType.OnStart)
        {
            TryPlayCutscene();
        }
    }

    private void Update()
    {
        if (hasPlayed || triggerType != TriggerType.OnPlayerEnterArea) return;

        if (playerControl != null && triggerArea != null)
        {
            if (IsPlayerInArea(playerControl.transform.position))
            {
                TryPlayCutscene();
            }
        }
    }
    private bool IsPlayerInArea(Vector3 playerPosition)
    {
        Vector3 localPos = triggerArea.transform.InverseTransformPoint(playerPosition);

        Vector3 center = triggerArea.center;
        Vector3 size = triggerArea.size;

        bool isInsideX = Mathf.Abs(localPos.x - center.x) <= (size.x / 2f);
        bool isInsideY = Mathf.Abs(localPos.y - center.y) <= (size.y / 2f);
        bool isInsideZ = Mathf.Abs(localPos.z - center.z) <= (size.z / 2f);

        return isInsideX && isInsideY && isInsideZ;
    }
    private void TryPlayCutscene()
    {
        if (hasPlayed) return;
        if (requirePrerequisite)
        {
            int preReqCount = 0;
            GameManager.Instance.eventCountDict.TryGetValue(prerequisiteEventID, out preReqCount);

            if (preReqCount == 0)
            {
                return;
            }
        }

        if (timelineDirector != null && timelineAsset != null)
        {
            timelineDirector.playableAsset = timelineAsset;
            timelineDirector.Play();
        }

        hasPlayed = true;
        GameManager.Instance.eventCountDict[eventID] = 1;
    }
}