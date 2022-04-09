#nullable enable
using UnityEditor;

[CustomEditor(typeof(TestWorld))]
public class TestWorldEditor : TestEditor
{
    private bool _showMeshSettings = false;
    private bool _showNoiseSettings = false;

    private void DrawScriptableObject(SerializedProperty serializedProperty, ref bool opened)
    {
        bool found = serializedProperty.objectReferenceValue != null;
        if(found)
        {
            opened = EditorGUILayout.Foldout(opened, serializedProperty.displayName);

            if(opened)
            {
                serializedObject.Update();

                Editor? editor = null;
                Editor.CreateCachedEditor(serializedProperty.objectReferenceValue, null, ref editor);
                EditorGUI.BeginChangeCheck();
                editor.OnInspectorGUI();
                if(EditorGUI.EndChangeCheck())
                    MarkDirty();

                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SerializedProperty meshSettings = serializedObject.FindProperty("_meshSettings");
        SerializedProperty noiseSettings = serializedObject.FindProperty("_noiseSettings");

        DrawScriptableObject(meshSettings, ref _showMeshSettings);
        DrawScriptableObject(noiseSettings, ref _showNoiseSettings);
    }
}
#nullable restore
