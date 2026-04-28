using System.Collections;
using UnityEngine;

public static class JsonDataLoader
{
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
