using UnityEngine;
using UnityEngine.Events;

public class InterectionScriptProto : MonoBehaviour
{
    [SerializeField][Tooltip("상호작용 시 발생할 이벤트를 설정합니다")]
    private UnityEvent interactionEvents;
    public void Interaction() {
        interactionEvents.Invoke();
    }
}
