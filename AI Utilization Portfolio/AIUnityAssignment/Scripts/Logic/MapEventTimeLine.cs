using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class MapEventTimeline : MonoBehaviour
{
    [Header("Event Settings")]
    [Tooltip("GameManager에 저장/조회할 고유 이벤트 ID")]
    public int eventID;

    private PlayableDirector director;
    private bool isEventTriggered = false;

    void Start()
    {
        director = GetComponent<PlayableDirector>();

        if (GameManager.Instance != null && GameManager.Instance.GetEventState(eventID) > 0)
        {
            director.time = director.duration;
            director.Evaluate();
            director.Stop();

            isEventTriggered = true;
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }
    public void PlayEvent()
    {
        if (isEventTriggered) return;
        isEventTriggered = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetEventState(eventID, 1);
        }

        if (director != null)
        {
            director.Play();
        }
    }
}