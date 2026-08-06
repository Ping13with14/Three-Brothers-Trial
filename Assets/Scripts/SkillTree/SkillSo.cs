using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(fileName ="NewSkill",menuName ="SkillTree/Skill")]
public class SkillSo : ScriptableObject
{

    public string skillName;
    public int maxlevel;
    public Sprite skillIcon;
}
