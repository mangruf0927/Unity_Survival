using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private TimeSystem timeSystem;
    [SerializeField] private DayNightEventSystem dayNightEventSystem;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CampFire campFire;
    [SerializeField] private WorkTableInventory workTableInventory;
    [SerializeField] private CultistSpawner cultistSpawner;
    [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private EquippableSpawner equippableSpawner;

    [SerializeField] private EquippableDatabase equippableDatabase;
    [SerializeField] private ItemDataBase itemDataBase;

    private const string SaveFileName = "Save.json";

    public void SaveData()
    {
        if (timeSystem == null || dayNightEventSystem == null || playerStats == null || playerController == null ||
            campFire == null || workTableInventory == null || cultistSpawner == null || itemSpawner == null)
        {
            Debug.LogError("Save failed. SaveLoadManager references are missing.");
            return;
        }

        TimeSaveData timeData = timeSystem.CreateSaveData();
        dayNightEventSystem.CreateSaveData(timeData);

        SaveData saveData = new()
        {
            timeData = timeData,
            playerData = playerStats.CreateSaveData(),
            inventoryData = playerController.CreateInventorySaveData(),
            campFireData = campFire.CreateSaveData(),
            workTableSaveData = workTableInventory.CreateSaveData(),
            cultistSaveDataList = cultistSpawner.CreateSaveData(),
            itemSaveDataList = itemSpawner.CreateSaveData(),
            equippableSaveDataList = equippableSpawner.CreateSaveData()
        };

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);

        File.WriteAllText(path, json);
        Debug.Log($"Save Complete: {path}");
    }

    public void LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);

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

        if (timeSystem == null || dayNightEventSystem == null || playerStats == null || playerController == null ||
            campFire == null || workTableInventory == null || cultistSpawner == null || itemSpawner == null)
        {
            Debug.LogError("Load failed. SaveLoadManager references are missing.");
            return;
        }

        dayNightEventSystem.LoadSaveData(saveData.timeData);
        timeSystem.LoadSaveData(saveData.timeData);

        playerStats.LoadSaveData(saveData.playerData);
        playerController.LoadInventorySaveData(saveData.inventoryData, equippableDatabase, itemDataBase);
        campFire.LoadSaveData(saveData.campFireData);
        workTableInventory.LoadSaveData(saveData.workTableSaveData);
        cultistSpawner.LoadSaveData(saveData.cultistSaveDataList);
        itemSpawner.LoadSaveData(saveData.itemSaveDataList);
        equippableSpawner.LoadSaveData(saveData.equippableSaveDataList);

        Debug.Log("Load Complete");
    }

    public void DeleteData()
    {
        string path = Path.Combine(Application.persistentDataPath, "Save.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found: {path}");
            return;
        }

        File.Delete(path);
        Debug.Log($"Delete Complete: {path}");
    }
}
