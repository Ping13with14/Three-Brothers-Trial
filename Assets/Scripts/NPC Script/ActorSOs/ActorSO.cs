using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话角色的可脚本化脚本
/// </summary>
[CreateAssetMenu(fileName = "ActorSO",menuName ="Dialogue/NPC")]
public class ActorSO : ScriptableObject
{
    public string actorName;
    public Sprite portratait;
}
