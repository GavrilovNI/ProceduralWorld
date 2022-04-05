#nullable enable
using UnityEngine;
using UnityEditor;

public abstract class GeneratorEditor : Editor
{
    protected bool AutoUpdate = false;

    public override void OnInspectorGUI()
    {
        if(DrawDefaultInspector())
            MarkDirty();

        if(target is not ITest)
            return;

        AutoUpdate = EditorGUILayout.Toggle("Auto Update", AutoUpdate);

        if(GUILayout.Button("Test"))
            Test();

        if(GUILayout.Button("Reset"))
            ResetTest();
    }

    protected void Test()
    {
        if(target is ITest generatable)
        {
            generatable.Reset();
            generatable.Test();
        }
    }

    protected void ResetTest()
    {
        if(target is ITest generatable)
            generatable.Reset();
    }

    protected void MarkDirty()
    {
        if(AutoUpdate)
            Test();
    }
}
#nullable restore
