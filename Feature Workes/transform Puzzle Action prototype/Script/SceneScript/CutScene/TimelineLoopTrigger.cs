using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class TimelineLoopTrigger : MonoBehaviour
{
    public PlayableDirector director;
    public string keycode;
    public double loopStartTime = 0f; // 반복을 시작할 시간 (초)
    public double loopEndTime = 0f; // 반복을 시작할 시간 (초)
    private bool isInputReceived = false;

    void Update()
    {
        // 예: 스페이스바를 누르면 다음으로 넘어감
        if (InputSystem.actions.FindAction(keycode).WasPressedThisFrame() && !isInputReceived && loopStartTime <= director.time)
        {
            isInputReceived = true;
            if(loopEndTime > 0)director.time = loopEndTime;
        }
    }

    // 타임라인의 Signal Emitter가 호출할 함수
    public void CheckLoopCondition()
    {
        if (!isInputReceived)
        {
            // 입력이 아직 없으면 시간을 되감음
            director.time = loopStartTime;
            // director.Play(); // 가끔 멈추는 경우를 대비해 확실하게 재생
        }
        // 입력이 있었으면 아무것도 안 하고 자연스럽게 다음 프레임으로 넘어감
    }
}