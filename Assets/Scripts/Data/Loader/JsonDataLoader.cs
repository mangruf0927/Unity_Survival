using System;
using System.Collections;
using UnityEngine;

public static class JsonDataLoader
{
    public static TextAsset[] LoadAll(string filePath)
    {
        TextAsset[] textAssets = Resources.LoadAll<TextAsset>(filePath);

        if (textAssets == null || textAssets.Length == 0)
        {
            Debug.LogError($"File not found in Resources: {filePath}");
        }

        return textAssets;
    }

    public static IEnumerator LoadText(string fileName, System.Action<string> onLoaded)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{fileName}");

        if (textAsset == null)
        {
            Debug.LogError($"File not found in Resources: Data/{fileName}");
            onLoaded?.Invoke(null);
            yield break;
        }
        onLoaded?.Invoke(textAsset.text);
        yield return null;
    }
}
