using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserItem : MonoBehaviour
{
    // 记录每个ItemSo对应的活跃时效协程，重复使用时刷新持续时间
    private Dictionary<ItemSo, Coroutine> activeEffects = new Dictionary<ItemSo, Coroutine>();

    /// <summary>
    /// 使用物品：先提升上限再回复当前值，保证血量正确；时效物品重复使用时刷新计时
    /// </summary>
    public void ApplyItemEffects(ItemSo itemSo)
    {
        if (StatsManager.Instance == null) return;

        // 先更新最大生命值，再更新当前生命值，确保血量不受旧上限限制
        if(itemSo.maxHealth > 0)
            StatsManager.Instance.UpdateMaxHealth(itemSo.maxHealth);
        if(itemSo.currentHealth > 0)
            StatsManager.Instance.UpdateHealth(itemSo.currentHealth);
        if(itemSo.speed > 0)
            StatsManager.Instance.UpdateSpeed(itemSo.speed);
        if(itemSo.damage > 0)
            StatsManager.Instance.UpdateDamage(itemSo.damage);

        // 时效物品：重复使用时停止旧计时器，刷新持续时间
        if(itemSo.duration > 0)
        {
            if (activeEffects.TryGetValue(itemSo, out var existing) && existing != null)
                StopCoroutine(existing);
            activeEffects[itemSo] = StartCoroutine(EffectTimer(itemSo, itemSo.duration));
        }
    }

    /// <summary>
    /// 时效结束后还原属性：先还原当前值再还原上限，避免当前值超出上限
    /// </summary>
    private IEnumerator EffectTimer(ItemSo itemSo, float duration)
    {
        yield return new WaitForSeconds(duration);
        // 先还原当前生命值，再还原最大生命值
        if (itemSo.currentHealth > 0)
            StatsManager.Instance.UpdateHealth(-itemSo.currentHealth);
        if (itemSo.maxHealth > 0)
            StatsManager.Instance.UpdateMaxHealth(-itemSo.maxHealth);
        if (itemSo.speed > 0)
            StatsManager.Instance.UpdateSpeed(-itemSo.speed);
        if (itemSo.damage > 0)
            StatsManager.Instance.UpdateDamage(-itemSo.damage);
        activeEffects.Remove(itemSo);
    }
}
