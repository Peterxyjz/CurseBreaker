using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/playerData.json";

    // ✅ Lưu dữ liệu
    public static void SavePlayerData(PlayerController player)
    {
        Debug.Log($"{savePath}");
        PlayerData data = new PlayerData
        {
            maxHp = player.GetMaxHp(),
            currentHp = player.GetCurrentHp(),
            damage = player.GetDamagePlayer(),
            unlockedSkills = ConvertDictionaryToList(player.GetUnlockedSkills()) // ✅ Chuyển đổi Dictionary -> List
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"✅ Dữ liệu đã lưu: {json}");
    }

    // ✅ Tải dữ liệu
    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(savePath))
        {

            Debug.Log($"{savePath}");
            string json = File.ReadAllText(savePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            if (data.unlockedSkills == null)
            {
                data.unlockedSkills = new List<SkillData>
                {
                    new SkillData("Skill0", false),
                    new SkillData("Skill1", false),
                    new SkillData("Skill2", false),
                    new SkillData("Skill3", false)
                };
            }

            Debug.Log($"✅ Dữ liệu đã tải: {json}");
            return data;
        }

        return null; // 🔄 Trả về dữ liệu mặc định nếu không có file save
    }

    // 🔄 Chuyển đổi Dictionary<string, bool> -> List<SkillData>
    private static List<SkillData> ConvertDictionaryToList(Dictionary<string, bool> dictionary)
    {
        List<SkillData> list = new List<SkillData>();
        foreach (var kvp in dictionary)
        {
            list.Add(new SkillData(kvp.Key, kvp.Value));
        }
        return list;
    }

    // 🔄 Chuyển đổi List<SkillData> -> Dictionary<string, bool>
    public static Dictionary<string, bool> ConvertListToDictionary(List<SkillData> list)
    {
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        foreach (var skill in list)
        {
            dictionary[skill.skillName] = skill.isUnlocked;
        }
        return dictionary;
    }
    public static void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            //File.Delete(savePath);
            Debug.Log("Save file deleted.");
        }
    }
   
}
