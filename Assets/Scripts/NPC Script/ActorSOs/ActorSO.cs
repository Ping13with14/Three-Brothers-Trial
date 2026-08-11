using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话角色数据定义：ScriptableObject，存储 NPC 的名称和头像
/// </summary>
[CreateAssetMenu(fileName = "ActorSO",menuName ="Dialogue/NPC")]
public class ActorSO : ScriptableObject
{
    public string actorName;       // 角色名称（对话中显示的名称）
    public Sprite portratait;      // 角色头像
}
