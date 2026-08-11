using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器工具：进入播放模式时自动清空 Hierarchy 选中项，防止误操作
/// </summary>
[InitializeOnLoad]
public class ClearSelectionOnPlay
{
    static ClearSelectionOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    /// <summary>
    /// 退出编辑模式进入播放模式前清空选中对象
    /// </summary>
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            Selection.activeObject = null;
        }
    }
}
