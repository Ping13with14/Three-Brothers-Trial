using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ClearSelectionOnPlay
{
    static ClearSelectionOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            Selection.activeObject = null;
        }
    }
}
