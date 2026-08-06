using UnityEngine;

/// <summary>
/// 可击退接口：解耦击退逻辑，让 PlayerCombat 不依赖具体 Enemy 类
/// </summary>
public interface IKnockbackable
{
    void Knockback(Transform source, float force, float knockbackTime, float stunTime);
}
