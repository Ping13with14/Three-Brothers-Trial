using UnityEngine;

/// <summary>
/// 键盘输入实现：封装 Unity 默认输入轴和按钮
/// </summary>
public class KeyboardInputProvider : IInputProvider
{
    public float Horizontal => Input.GetAxis("Horizontal");
    public float Vertical => Input.GetAxis("Vertical");
    public float HorizontalRaw => Input.GetAxisRaw("Horizontal");
    public float VerticalRaw => Input.GetAxisRaw("Vertical");
    public bool IsAttackPressed => Input.GetButtonDown("Slash");
    public bool IsShootPressed => Input.GetButtonDown("Shoot");
    public bool IsInteractionPressed => Input.GetButtonDown("Interact");
    public bool IsCancelPressed => Input.GetButtonDown("Cancel");
    public bool IsToggleSkillTreePressed => Input.GetButtonDown("ToggleSkillTree");
    public bool IsToggleStatsPressed => Input.GetButtonDown("ToggleStats");
    public bool IsChangeEquipmentPressed => Input.GetButtonDown("ChangeEquipment");
    public bool IsDebugKeyPressed => Input.GetKeyDown(KeyCode.Return);
}
