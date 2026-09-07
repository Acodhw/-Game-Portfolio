#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(TutorialShow))]
public class TutorialShowEditor : Editor
{
    BoxBoundsHandle enterHandle = new BoxBoundsHandle();
    BoxBoundsHandle exitHandle = new BoxBoundsHandle();

    void OnSceneGUI()
    {
        TutorialShow t = (TutorialShow)target;
        Transform tr = t.transform;

        enterHandle.center = t.enterCenter;
        enterHandle.size = t.enterSize;

        Handles.color = Color.green;

        EditorGUI.BeginChangeCheck();
        using (new Handles.DrawingScope(tr.localToWorldMatrix))
        {
            enterHandle.DrawHandle();
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(t, "Modify Enter Zone");
            t.enterCenter = enterHandle.center;
            t.enterSize = enterHandle.size;
        }

        exitHandle.center = t.exitCenter;
        exitHandle.size = t.exitSize;

        Handles.color = Color.red;

        EditorGUI.BeginChangeCheck();
        using (new Handles.DrawingScope(tr.localToWorldMatrix))
        {
            exitHandle.DrawHandle();
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(t, "Modify Exit Zone");
            t.exitCenter = exitHandle.center;
            t.exitSize = exitHandle.size;
        }
    }
}
#endif