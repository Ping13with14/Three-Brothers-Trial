using UnityEngine;

/// <summary>
/// 可伤害接口：解耦生命值变更逻辑，让攻击方不依赖具体被攻击者类
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 变更生命值（正数为加血，负数为扣血）
    /// </summary>
    /// <param name="amount">变更量</param>
    void ChangeHealth(int amount);

    /// <summary>
    /// 当前生命值
    /// </summary>
    int CurrentHealth { get; }

    /// <summary>
    /// 最大生命值
    /// </summary>
    int MaxHealth { get; }
}
