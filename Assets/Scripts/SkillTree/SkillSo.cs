using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

/// <summary>
/// 技能数据定义：ScriptableObject，在编辑器中创建和配置技能属性
/// </summary>
[CreateAssetMenu(fileName ="NewSkill",menuName ="SkillTree/Skill")]
public class SkillSo : ScriptableObject
{
    public string skillName;       // 技能名称（与 SkillManger 中 switch-case 匹配）
    public int maxlevel;           // 技能最大等级
    public Sprite skillIcon;       // 技能图标
}
