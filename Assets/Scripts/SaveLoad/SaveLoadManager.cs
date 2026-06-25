using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EquippableDatabase equippableDatabase;
    [SerializeField] private ItemDataBase itemDataBase;

    public void SaveData()
    {
        if (timeSystem == null || playerStats == null || playerController == null)
        {
            Debug.LogError("Save failed. SaveLoadManager references are missing.");
            return;
        }

        SaveData saveData = new()
        {
            timeData = timeSystem.CreateSaveData(),
            playerData = playerStats.CreateSaveData(),
            inventoryData = playerController.CreateInventorySaveData()
        };

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, "Save.json");
        File.WriteAllText(path, json);

        Debug.Log($"Save Complete: {path}");
    }

    public void LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, "Save.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);

        if (saveData == null)
        {
            Debug.LogError("Load failed. Save data or TimeSystem is invalid.");
            return;
        }

        if (timeSystem == null || playerStats == null || playerController == null || equippableDatabase == null || itemDataBase == null)
        {
            Debug.LogError("Load failed. SaveLoadManager references are missing.");
            return;
        }

        timeSystem.LoadSaveData(saveData.timeData);
        playerStats.LoadSaveData(saveData.playerData);
        playerController.LoadInventorySaveData(saveData.inventoryData, equippableDatabase, itemDataBase);

        Debug.Log("Load Complete");
    }
}
