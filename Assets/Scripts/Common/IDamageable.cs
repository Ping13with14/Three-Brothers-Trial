using UnityEngine;

/// <summary>
/// 可伤害接口：解耦生命值变更逻辑，让 PlayerCombat 不依赖具体 Enemy 类
/// </summary>
public interface IDamageable
{
    void ChangeHealth(int amount);
    int CurrentHealth { get; }
    int MaxHealth { get; }
}
