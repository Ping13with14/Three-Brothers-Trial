using UnityEngine;

/// <summary>
/// 游戏全局配置：ScriptableObject，集中管理玩家和敌人的默认数值，替代硬编码
/// 创建路径：Assets → Create → Game → GameConfig
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("玩家配置")]
    public PlayerConfig player;

    [Header("敌人配置")]
    public EnemyConfig enemy;
}

[System.Serializable]
public class PlayerConfig
{
    [Header("战斗属性")]
    public int defaultDamage = 10;                // 默认攻击力
    public float defaultWeaponRange = 1.5f;       // 默认武器攻击范围半径
    public float defaultKnockbackForce = 10f;     // 默认击退力度
    public float defaultKnockbackTime = 0.2f;     // 默认击退持续时间（秒）
    public float defaultStunTime = 0.3f;          // 默认眩晕/硬直时间（秒）

    [Header("移动属性")]
    public int defaultSpeed = 5;                  // 默认移动速度

    [Header("生命属性")]
    public int defaultMaxHealth = 100;            // 默认最大生命值
}

[System.Serializable]
public class EnemyConfig
{
    [Header("战斗属性")]
    public int defaultMaxHealth = 30;             // 默认最大生命值

    [Header("奖励属性")]
    public int defaultExpReward = 3;              // 默认击败经验奖励
}
