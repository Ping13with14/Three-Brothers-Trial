using UnityEngine;

/// <summary>
/// 游戏全局配置：集中管理各系统的默认数值，替代硬编码
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
    public int defaultDamage = 10;
    public float defaultWeaponRange = 1.5f;
    public float defaultKnockbackForce = 10f;
    public float defaultKnockbackTime = 0.2f;
    public float defaultStunTime = 0.3f;

    [Header("移动属性")]
    public int defaultSpeed = 5;

    [Header("生命属性")]
    public int defaultMaxHealth = 100;
}

[System.Serializable]
public class EnemyConfig
{
    public int defaultMaxHealth = 30;
    public int defaultExpReward = 3;
}
