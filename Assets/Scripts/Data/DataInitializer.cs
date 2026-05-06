using System.Collections;
using UnityEngine;

public class DataInitializer : MonoBehaviour
{
    private IEnumerator Start()
    {
        if (!DataManager.Instance.IsLoaded)
        {
            yield return DataManager.Instance.LoadAll();
        }
    }
}
