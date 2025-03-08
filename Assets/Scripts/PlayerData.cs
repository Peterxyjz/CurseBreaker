using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillData
{
    public string skillName;
    public bool isUnlocked;

    public SkillData(string name, bool unlocked)
    {
        skillName = name;
        isUnlocked = unlocked;
    }
}

[Serializable]
public class PlayerData
{
    public float maxHp;
    public float currentHp;
    public float damage;
    public List<SkillData> unlockedSkills; // ✅ Dùng List<SkillData> thay vì KeyValuePair

    public PlayerData()
    {
        
        unlockedSkills = new List<SkillData>
        {
            new SkillData("Skill0", false),
            new SkillData("Skill1", false),
            new SkillData("Skill2", false),
            new SkillData("Skill3", false)
        };
    }
}
