using UnityEngine;

/// <summary>
/// 输入抽象接口：解耦输入设备，方便未来切换键盘/手柄或自定义按键
/// </summary>
public interface IInputProvider
{
    float Horizontal { get; }
    float Vertical { get; }
    float HorizontalRaw { get; }
    float VerticalRaw { get; }
    bool IsAttackPressed { get; }
    bool IsShootPressed { get; }
    bool IsInteractionPressed { get; }
    bool IsCancelPressed { get; }
    bool IsToggleSkillTreePressed { get; }
    bool IsToggleStatsPressed { get; }
    bool IsChangeEquipmentPressed { get; }
    bool IsDebugKeyPressed { get; }
}
