using UnityEngine;

/// <summary>
/// 全局输入管理器：提供当前活动的 IInputProvider
/// </summary>
public static class InputManager
{
    private static IInputProvider currentProvider;

    /// <summary>
    /// 获取当前输入提供者，首次调用时自动初始化为键盘输入
    /// </summary>
    public static IInputProvider Provider
    {
        get
        {
            if (currentProvider == null)
                currentProvider = new KeyboardInputProvider();
            return currentProvider;
        }
    }

    /// <summary>
    /// 切换输入设备（如从键盘切换到手柄）
    /// </summary>
    public static void SetProvider(IInputProvider provider)
    {
        currentProvider = provider;
    }
}
