using UnityEngine;

/// <summary>
/// 可击退接口：解耦击退逻辑，让 PlayerCombat 不依赖具体 Enemy 类
/// </summary>
public interface IKnockbackable
{
    /// <summary>
    /// 击退处理
    /// </summary>
    /// <param name="source">击退来源 Transform（用于计算击退方向）</param>
    /// <param name="force">击退力度</param>
    /// <param name="knockbackTime">击退持续时间（秒）</param>
    /// <param name="stunTime">眩晕/硬直时间（秒）</param>
    void Knockback(Transform source, float force, float knockbackTime, float stunTime);
}
