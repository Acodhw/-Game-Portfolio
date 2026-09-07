using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableEvent : MonoBehaviour
{
    public enum StepType
    {
        Dialogue,         
        Choice,           
        UnityAction,      
        GiveItem,         
        ChangeStartIndex, 
        End,              
        CheckItem,        
        CheckEvent        
    }

    [System.Serializable]
    public class ChoiceOption
    {
        public string choiceText;      
        public int targetStepIndex;    
    }

    [System.Serializable]
    public class InteractionStep
    {
        public string stepNote = "메모장"; 

        public StepType stepType;

        [Header("Dialogue / Choice Settings")]
        public string speakerName;
        [TextArea(2, 4)]
        public string textContent;     

        [Tooltip("대화 시 한 글자마다 재생될 사운드")]
        public AudioClip typingSound;

        [Header("Choice Settings")]
        [Tooltip("StepType이 Choice일 때만 사용됩니다.")]
        public List<ChoiceOption> choices = new List<ChoiceOption>();

        [Header("Event Settings")]
        [Tooltip("StepType이 UnityAction일 때 실행될 이벤트들 (사운드 재생, 애니메이션 실행 등)")]
        public UnityEvent onActionTrigger;

        [Header("Item & Event Check Settings")]
        [Tooltip("지급/검사할 아이템 코드, 또는 검사할 이벤트의 요구 상태값")]
        public int itemCode;
        public int itemAmount;

        [Tooltip("검사할 특정 이벤트의 ID (CheckEvent 전용)")]
        public int eventID;

        [Header("Change Start Index Settings")]
        [Tooltip("StepType이 ChangeStartIndex일 때, 다음에 상호작용할 때 시작될 인덱스 번호")]
        public int newStartIndex;

        [Header("Next Step (Branching)")]
        [Tooltip("이 스텝이 끝난 후 이동할 다음 배열의 인덱스 번호")]
        public int nextStepIndex;

        [Tooltip("조건을 만족했을 때 (True) 이동할 스텝 인덱스")]
        public int trueNextIndex;

        [Tooltip("조건을 만족하지 못했을 때 (False) 이동할 스텝 인덱스")]
        public int falseNextIndex;
    }

    [Header("Event Identification")]
    [Tooltip("이 이벤트의 고유 ID입니다. 이 값이 같으면 진행도를 공유합니다. (예: 1001 = 촌장님 대화)")]
    public int eventID = 0;

    [Header("Interaction Sequence")]
    [Tooltip("인스펙터에서 대화/이벤트 노드를 순서대로 생성합니다.")]
    public List<InteractionStep> sequence = new List<InteractionStep>();

    [Header("Current State (Read Only)")]
    [Tooltip("현재 이 오브젝트와 상호작용 시 시작될 인덱스입니다. (GameManager에서 불러옴)")]
    public int currentStartIndex = 0;

    private bool isInteracting = false;
    private int currentStepIndex = 0;

    [Tooltip("Input Manager에 설정된 'Interaction' 키 이름입니다.")]
    private string interactInput = "Interaction";

    void Start()
    {
        if (GameManager.Instance != null)
        {
            int savedIndex = GameManager.Instance.GetEventState(eventID);
            if (savedIndex > 0 || GameManager.Instance.eventCountDict.ContainsKey(eventID))
            {
                currentStartIndex = savedIndex;
            }
        }
    }

    void Update()
    {
        if (isInteracting && (Input.GetButtonDown(interactInput) || Input.GetMouseButtonDown(0)))
        {
            if (currentStepIndex >= 0 && currentStepIndex < sequence.Count)
            {
                InteractionStep step = sequence[currentStepIndex];

                if (step.stepType == StepType.Dialogue || step.stepType == StepType.Choice)
                {
                    if (UIManager.Instance != null)
                    {
                        if (UIManager.Instance.isTyping)
                        {
                            UIManager.Instance.PlayUISound();
                            UIManager.Instance.SkipTyping();
                        }
                        else if (step.stepType == StepType.Dialogue)
                        {
                            UIManager.Instance.PlayUISound();
                            PlayStep(step.nextStepIndex);
                        }
                    }
                    else if (step.stepType == StepType.Dialogue)
                    {
                        PlayStep(step.nextStepIndex);
                    }
                }
            }
        }
    }

    public void StartInteraction()
    {
        if (sequence.Count == 0 || isInteracting) return;

        isInteracting = true;
        PlayStep(currentStartIndex);
    }

    private void PlayStep(int index)
    {
        if (index < 0 || index >= sequence.Count || sequence[index].stepType == StepType.End)
        {
            EndInteraction();
            return;
        }

        currentStepIndex = index;
        InteractionStep step = sequence[index];

        string processedSpeaker = ProcessVariables(step.speakerName);
        string processedText = ProcessVariables(step.textContent);

        switch (step.stepType)
        {
            case StepType.Dialogue:
                UIManager.Instance.OpenTalk(processedSpeaker, processedText, step.typingSound);
                break;

            case StepType.Choice:
                UIManager.Instance.OpenTalk(processedSpeaker, processedText, step.typingSound);
                UIManager.Instance.ClearTalkSelections();

                foreach (var choice in step.choices)
                {
                    int targetIndex = choice.targetStepIndex;
                    string processedChoiceText = ProcessVariables(choice.choiceText);
                    UIManager.Instance.CreateTalkSelection(processedChoiceText, () => PlayStep(targetIndex));
                }
                break;

            case StepType.UnityAction:
                step.onActionTrigger?.Invoke();
                PlayStep(step.nextStepIndex);
                break;

            case StepType.GiveItem:
                PlayerState playerState = FindObjectOfType<PlayerState>();
                if (playerState != null)
                {
                    playerState.AddItem(step.itemCode, step.itemAmount);
                }
                PlayStep(step.nextStepIndex);
                break;

            case StepType.ChangeStartIndex:
                currentStartIndex = step.newStartIndex;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SetEventState(eventID, currentStartIndex);
                }
                PlayStep(step.nextStepIndex);
                break;

            case StepType.CheckItem:
                bool hasItem = false;
                PlayerState pState = FindObjectOfType<PlayerState>();
                if (pState != null)
                {
                    InventoryItem invItem = pState.inventory.Find(x => x.itemCode == step.itemCode);
                    if (invItem != null && invItem.amount >= step.itemAmount)
                    {
                        hasItem = true;
                    }
                }
                PlayStep(hasItem ? step.trueNextIndex : step.falseNextIndex);
                break;

            case StepType.CheckEvent:
                bool conditionMet = false;
                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.GetEventState(step.eventID) == step.itemAmount)
                    {
                        conditionMet = true;
                    }
                }
                PlayStep(conditionMet ? step.trueNextIndex : step.falseNextIndex);
                break;
        }
    }

    private string ProcessVariables(string rawText)
    {
        if (string.IsNullOrEmpty(rawText)) return rawText;

        string result = rawText;
        if (GameManager.Instance != null)
        {
            result = result.Replace("{PlayerName}", GameManager.Instance.currentPlayerName);
        }

        return result;
    }

    public void EndInteraction()
    {
        isInteracting = false;
        currentStepIndex = -1;
        if (UIManager.Instance != null) UIManager.Instance.CloseTalk();
    }
}