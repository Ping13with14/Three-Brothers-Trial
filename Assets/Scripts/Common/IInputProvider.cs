using UnityEngine;

/// <summary>
/// 输入抽象接口：解耦输入设备，方便未来切换键盘/手柄或自定义按键
/// </summary>
public interface IInputProvider
{
    float Horizontal { get; }                    // 水平轴（平滑，用于移动）
    float Vertical { get; }                      // 垂直轴（平滑，用于移动）
    float HorizontalRaw { get; }                 // 水平轴原始值（无平滑，用于瞄准方向）
    float VerticalRaw { get; }                   // 垂直轴原始值（无平滑，用于瞄准方向）
    bool IsAttackPressed { get; }                // 近战攻击键（Slash 轴）
    bool IsShootPressed { get; }                 // 远程射击键（Shoot 轴）
    bool IsInteractionPressed { get; }           // 交互键（Interact 轴，与NPC/物品交互）
    bool IsCancelPressed { get; }                // 取消键（Cancel 轴，关闭面板/返回）
    bool IsToggleSkillTreePressed { get; }       // 切换技能树面板（ToggleSkillTree 轴）
    bool IsToggleStatsPressed { get; }           // 切换属性面板（ToggleStats 轴）
    bool IsChangeEquipmentPressed { get; }       // 切换装备/武器（ChangeEquipment 轴）
    bool IsDebugKeyPressed { get; }              // 调试键（回车 Return，用于快速测试）
}
