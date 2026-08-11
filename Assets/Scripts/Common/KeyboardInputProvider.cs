using UnityEngine;

/// <summary>
/// 键盘输入实现：封装 Unity Input Manager 中定义的轴和按钮
/// 按键映射参考：Edit → Project Settings → Input Manager
/// </summary>
public class KeyboardInputProvider : IInputProvider
{
    public float Horizontal => Input.GetAxis("Horizontal");              // A/D 或 ←→ 键
    public float Vertical => Input.GetAxis("Vertical");                  // W/S 或 ↑↓ 键
    public float HorizontalRaw => Input.GetAxisRaw("Horizontal");        // A/D 原始值（无平滑）
    public float VerticalRaw => Input.GetAxisRaw("Vertical");            // W/S 原始值（无平滑）
    public bool IsAttackPressed => Input.GetButtonDown("Slash");         // 近战攻击（默认鼠标左键）
    public bool IsShootPressed => Input.GetButtonDown("Shoot");          // 远程射击（默认鼠标右键或 Ctrl）
    public bool IsInteractionPressed => Input.GetButtonDown("Interact"); // 交互（默认 E 键）
    public bool IsCancelPressed => Input.GetButtonDown("Cancel");        // 取消/返回（默认 Esc）
    public bool IsToggleSkillTreePressed => Input.GetButtonDown("ToggleSkillTree"); // 技能树（默认 K 键）
    public bool IsToggleStatsPressed => Input.GetButtonDown("ToggleStats");           // 属性面板（默认 Tab）
    public bool IsChangeEquipmentPressed => Input.GetButtonDown("ChangeEquipment");   // 切换武器（默认 Q 键）
    public bool IsDebugKeyPressed => Input.GetKeyDown(KeyCode.Return);  // 调试快捷升级（回车键）
}
