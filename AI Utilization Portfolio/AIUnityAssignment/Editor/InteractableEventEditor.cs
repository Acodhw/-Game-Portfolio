using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(InteractableEvent))]
public class InteractableEventEditor : Editor
{
    SerializedProperty eventID;
    SerializedProperty currentStartIndex;
    SerializedProperty sequence;

    private void OnEnable()
    {
        eventID = serializedObject.FindProperty("eventID");
        currentStartIndex = serializedObject.FindProperty("currentStartIndex");
        sequence = serializedObject.FindProperty("sequence");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🎯 Event Core Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(eventID);
        EditorGUILayout.PropertyField(currentStartIndex);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("📜 Interaction Sequence", EditorStyles.boldLabel);

        for (int i = 0; i < sequence.arraySize; i++)
        {
            SerializedProperty step = sequence.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginVertical("helpbox");

            SerializedProperty stepNote = step.FindPropertyRelative("stepNote");
            SerializedProperty stepType = step.FindPropertyRelative("stepType");
            int typeIndex = stepType.enumValueIndex;

            // 캐스팅 (InteractableEvent에 CheckItem, CheckEvent가 추가되었다고 가정)
            InteractableEvent.StepType currentType = (InteractableEvent.StepType)typeIndex;

            EditorGUILayout.BeginHorizontal();
            string title = $"[{i}] {stepType.enumDisplayNames[typeIndex]}";
            if (!string.IsNullOrEmpty(stepNote.stringValue)) title += $" - {stepNote.stringValue}";

            step.isExpanded = EditorGUILayout.Foldout(step.isExpanded, title, true, EditorStyles.foldoutHeader);

            if (GUILayout.Button("▲", EditorStyles.miniButtonLeft, GUILayout.Width(25)))
            {
                if (i > 0) sequence.MoveArrayElement(i, i - 1);
                break;
            }
            if (GUILayout.Button("▼", EditorStyles.miniButtonMid, GUILayout.Width(25)))
            {
                if (i < sequence.arraySize - 1) sequence.MoveArrayElement(i, i + 1);
                break;
            }
            if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(25)))
            {
                sequence.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            if (step.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space(2);

                EditorGUILayout.PropertyField(stepNote);
                EditorGUILayout.PropertyField(stepType);
                EditorGUILayout.Space(5);

                switch (currentType)
                {
                    case InteractableEvent.StepType.Dialogue:
                        DrawPropertyWithVariables(step, "speakerName");
                        EditorGUILayout.Space(3);
                        DrawPropertyWithVariables(step, "textContent");
                        EditorGUILayout.Space(3);
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("nextStepIndex"));
                        break;

                    case InteractableEvent.StepType.Choice:
                        DrawPropertyWithVariables(step, "speakerName");
                        EditorGUILayout.Space(3);
                        DrawPropertyWithVariables(step, "textContent");
                        EditorGUILayout.Space(3);
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("choices"), true);
                        break;

                    case InteractableEvent.StepType.UnityAction:
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("onActionTrigger"));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("nextStepIndex"));
                        break;

                    case InteractableEvent.StepType.GiveItem:
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("itemCode"));
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("itemAmount"));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("nextStepIndex"));
                        break;

                    case InteractableEvent.StepType.ChangeStartIndex:
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("newStartIndex"));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("nextStepIndex"));
                        break;

                    case InteractableEvent.StepType.CheckItem:
                        EditorGUILayout.HelpBox("인벤토리에 해당 아이템이 있는지 검사합니다.", MessageType.Info);
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("itemCode"));
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("itemAmount"), new GUIContent("필요 수량"));
                        EditorGUILayout.Space(3);
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("trueNextIndex"), new GUIContent("있을 경우 이동할 Step"));
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("falseNextIndex"), new GUIContent("없을 경우 이동할 Step"));
                        break;

                    case InteractableEvent.StepType.CheckEvent:
                        EditorGUILayout.HelpBox("특정 이벤트가 지정된 값인지 검사합니다.", MessageType.Info);
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("eventID"), new GUIContent("검사할 이벤트 ID"));
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("itemAmount"), new GUIContent("요구 상태값 (0=False, 1=True 등)")); // itemAmount를 재활용하거나 상태값 변수 사용
                        EditorGUILayout.Space(3);
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("trueNextIndex"), new GUIContent("조건 만족시 이동할 Step"));
                        EditorGUILayout.PropertyField(step.FindPropertyRelative("falseNextIndex"), new GUIContent("조건 불만족시 이동할 Step"));
                        break;

                    case InteractableEvent.StepType.End:
                        EditorGUILayout.HelpBox("이 스텝에서 상호작용이 완전히 종료됩니다.", MessageType.Info);
                        break;
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space(2);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("➕ Add Step", GUILayout.Height(25), GUILayout.Width(120)))
        {
            sequence.arraySize++;
            SerializedProperty newStep = sequence.GetArrayElementAtIndex(sequence.arraySize - 1);

            newStep.FindPropertyRelative("stepNote").stringValue = "새로운 스텝";
            newStep.FindPropertyRelative("stepType").enumValueIndex = (int)InteractableEvent.StepType.Dialogue;
            newStep.FindPropertyRelative("speakerName").stringValue = "";
            newStep.FindPropertyRelative("textContent").stringValue = "";
            newStep.FindPropertyRelative("itemCode").intValue = 0;
            newStep.FindPropertyRelative("itemAmount").intValue = 1;
            newStep.FindPropertyRelative("newStartIndex").intValue = 0;
            newStep.FindPropertyRelative("nextStepIndex").intValue = sequence.arraySize;

            if (newStep.FindPropertyRelative("trueNextIndex") != null) newStep.FindPropertyRelative("trueNextIndex").intValue = sequence.arraySize;
            if (newStep.FindPropertyRelative("falseNextIndex") != null) newStep.FindPropertyRelative("falseNextIndex").intValue = sequence.arraySize;
            if (newStep.FindPropertyRelative("eventID") != null) newStep.FindPropertyRelative("eventID").intValue = 0;

            newStep.FindPropertyRelative("choices").arraySize = 0;
            newStep.isExpanded = true;
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPropertyWithVariables(SerializedProperty step, string propertyName)
    {
        SerializedProperty prop = step.FindPropertyRelative(propertyName);
        EditorGUILayout.PropertyField(prop);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+ {PlayerName}", EditorStyles.miniButton, GUILayout.Width(100)))
        {
            prop.stringValue += "{PlayerName}";
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif