#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(PersistentID))]
public class PersistentIDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PersistentID pid = (PersistentID)target;

        if (GUILayout.Button("Generate New ID"))
        {
            Undo.RecordObject(pid, "Generate ID");
            typeof(PersistentID).GetField("uniqueID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(pid, Guid.NewGuid().ToString());

            EditorUtility.SetDirty(pid);
        }
    }
}
#endif