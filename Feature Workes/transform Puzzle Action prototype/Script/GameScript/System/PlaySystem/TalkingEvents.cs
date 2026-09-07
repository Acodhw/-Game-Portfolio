using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct ChoiceEvent
{
    [Tooltip("대화 선택지 텍스트입니다")]
    public string choice;
    [Tooltip("대화 선택지를 지정합니다. 선택지 - 선택 시 이벤트 코드. 지정되지 않으면 선택지 활성화시키지 않습니다")]
    public int nextEvent;
}

[Serializable]
public struct TalkSource
{
    [Tooltip("보여줄 이름을 지정합니다")]
    public string name;
    [TextArea]
    [Tooltip("실제 대화창에 입력될 입니다")]
    public string talk;
    [Tooltip("이 대화에서 보여줄 이미지를 지정합니다")]
    public Sprite talkImage;
    [Tooltip("다음 텍스트가 보여지기까지의 속도를 지정합니다")]
    public float delay;
    [Tooltip("대화가 자동으로 넘어가는 시간을 지정합니다. 0 이하일 경우 자동으로 넘어가지 않습니다")]
    public float autoNextTime;
    [Tooltip("이 대화가 끝난 후 다음 대화 위치를 지정합니다. -1일 경우 대화를 끝냅니다")]
    public int nextTalk;
    [Tooltip("대화 선택지를 지정합니다. 지정되지 않으면 선택지 활성화시키지 않습니다.")]
    public List<ChoiceEvent> choices;
}

[Serializable]
public struct TalkEvent
{
    [Tooltip("대화 소스입니다")]
    public List<TalkSource> sources;
}

public class TalkingEvents : MonoBehaviour
{
    [SerializeField]
    private List<TalkEvent> events;

    public TalkEvent GetEvent(int eventNum) {  return events[eventNum]; }
}
