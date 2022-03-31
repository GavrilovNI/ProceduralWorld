#nullable enable
using UnityEditor;

[CustomEditor(typeof(TestWorld))]
public class TestWorldEditor : GeneratorEditor
{
    private bool _showSettings = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedProperty settings = serializedObject.FindProperty("_generationSettings");

        bool found = settings.objectReferenceValue != null;
        if(found)
        {
            _showSettings = EditorGUILayout.Foldout(_showSettings, "Settings");

            if(_showSettings)
            {
                serializedObject.Update();

                Editor? editor = null;
                Editor.CreateCachedEditor(settings?.objectReferenceValue, null, ref editor);
                EditorGUI.BeginChangeCheck();
                editor.OnInspectorGUI();
                if(EditorGUI.EndChangeCheck())
                    MarkDirty();

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#nullable disable

