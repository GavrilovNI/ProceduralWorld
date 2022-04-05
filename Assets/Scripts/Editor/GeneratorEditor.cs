#nullable enable
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ITest), editorForChildClasses: true)]
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
            generatable.ResetTest();
            generatable.Test();
        }
    }

    protected void ResetTest()
    {
        if(target is ITest generatable)
            generatable.ResetTest();
    }

    protected void MarkDirty()
    {
        if(AutoUpdate)
            Test();
    }
}
#nullable restore
