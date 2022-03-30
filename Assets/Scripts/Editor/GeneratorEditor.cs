using UnityEngine;
using UnityEditor;

public abstract class GeneratorEditor : Editor
{
    protected bool AutoUpdate = false;

    public override void OnInspectorGUI()
    {
        bool valuesChanged = DrawDefaultInspector();

        ITest generatable = target as ITest;
        if(generatable == null)
            return;

        AutoUpdate = EditorGUILayout.Toggle("Auto Update", AutoUpdate);

        if(valuesChanged && AutoUpdate || GUILayout.Button("Test"))
        {
            generatable.Reset();
            generatable.Test();
        }

        if(GUILayout.Button("Reset"))
        {
            generatable.Reset();
        }
    }
}
