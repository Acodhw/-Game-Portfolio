using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CutSceneEventTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("이벤트 코드를 지정합니다")]
    public int eventCode;
    [SerializeField]
    [Tooltip("트리거를 발동시킬 레이어를 지정합니다")]
    private LayerMask triggerLayer;
    [SerializeField]
    [Tooltip("실행 디렉터를 지정합니다")]
    private PlayableDirector director;
    [SerializeField]
    [Tooltip("실행할 타임라인을 지정합니다.")]
    private TimelineAsset timeline;

    private GameManager manager;

    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((triggerLayer & (1 << collision.gameObject.layer)) != 0 && !manager.GetEventActed(eventCode))
        {
            director.playableAsset = timeline;
            director.Play();
            manager.SetEventActed(eventCode, true);
        }
    }
}
