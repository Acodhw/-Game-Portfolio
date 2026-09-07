using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CutsceneEventPlayer : MonoBehaviour
{

    [System.Serializable]
    public class CutsceneStep
    {
        [Header("Dialogue")]
        public string speakerName;

        [TextArea(2, 4)]
        public string text;

        public float duration = 2f;

        [Header("Event")]
        public UnityEvent onStepEvent; 
    }


    [System.Serializable]
    public class CutsceneBlock
    {
        public string blockName;
        public List<CutsceneStep> steps = new List<CutsceneStep>();
    }

    [Header("UI")]
    public TextMeshProUGUI nameUI;
    public TextMeshProUGUI textUI;

    [Header("Cutscene Blocks")]
    public List<CutsceneBlock> blocks = new List<CutsceneBlock>();

    private Coroutine playRoutine;
    public void PlayBlock(int blockIndex)
    {
        if (blockIndex < 0 || blockIndex >= blocks.Count) return;

        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = StartCoroutine(PlayBlockRoutine(blocks[blockIndex]));
    }

    public void PlayStep(int blockIndex, int stepIndex)
    {
        if (blockIndex < 0 || blockIndex >= blocks.Count) return;

        var block = blocks[blockIndex];

        if (stepIndex < 0 || stepIndex >= block.steps.Count) return;

        ExecuteStep(block.steps[stepIndex]);
    }
    IEnumerator PlayBlockRoutine(CutsceneBlock block)
    {
        foreach (var step in block.steps)
        {
            ExecuteStep(step);
            yield return new WaitForSeconds(step.duration);
        }

        EndCutscene();
    }

    void ExecuteStep(CutsceneStep step)
    {
        string processedName = ProcessVariables(step.speakerName);
        string processedText = ProcessVariables(step.text);

        if (nameUI != null)
            nameUI.text = string.IsNullOrEmpty(processedName) ? "" : processedName;

        if (textUI != null)
            textUI.text = processedText;

        step.onStepEvent?.Invoke();
    }

    string ProcessVariables(string rawText)
    {
        if (string.IsNullOrEmpty(rawText)) return rawText;

        string result = rawText;

        if (GameManager.Instance != null)
        {
            var gm = GameManager.Instance;
            result = result.Replace("{PlayerName}", gm.currentPlayerName);
        }

        return result;
    }
    void EndCutscene()
    {
        if (nameUI != null)
            nameUI.text = "";

        if (textUI != null)
            textUI.text = "";

        playRoutine = null;
    }
}