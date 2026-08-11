using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地点数据定义：ScriptableObject，存储地点的唯一ID和显示名称
/// </summary>
[CreateAssetMenu(menuName ="LocationSO")]
public class LocationSO : ScriptableObject
{
    public string locationID;      // 地点唯一标识ID（用于任务追踪）
    public string displayName;     // 地点显示名称


}
