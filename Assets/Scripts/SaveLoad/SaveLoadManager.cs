using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private TimeSystem timeSystem;

    public void SaveData()
    {
        if (timeSystem == null)
        {
            Debug.LogError("TimeSystem is not assigned.");
            return;
        }

        SaveData saveData = new SaveData
        {
            timeData = timeSystem.CreateSaveData()
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

        if (timeSystem == null || saveData == null || saveData.timeData == null)
        {
            Debug.LogError("Load failed. Save data or TimeSystem is invalid.");
            return;
        }

        timeSystem.LoadSaveData(saveData.timeData);

        Debug.Log("Load Complete");
    }
}
