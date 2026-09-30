using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableMapTest : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject houseReference;

    private AsyncOperationHandle<GameObject> houseHandle;
    private GameObject housePrefab;

    private List<GameObject> spawnedObjects = new();

    private IEnumerator Start()
    {
        houseHandle = houseReference.LoadAssetAsync<GameObject>();

        yield return houseHandle;

        if (houseHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("House 로드 실패");
            yield break;
        }

        housePrefab = houseHandle.Result;

        SpawnHouse(new Vector3(0, 0, 0));
        SpawnHouse(new Vector3(3, 0, 0));
        SpawnHouse(new Vector3(6, 0, 0));
    }

    private void SpawnHouse(Vector3 position)
    {
        GameObject house = Instantiate(housePrefab, position, Quaternion.identity);

        spawnedObjects.Add(house);
    }

    public void ReleaseMap()
    {
        // 4. 인스턴스부터 제거
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }

        spawnedObjects.Clear();

        // 5. 그 다음 Addressable 원본 해제
        if (houseHandle.IsValid())
        {
            Addressables.Release(houseHandle);
        }

        housePrefab = null;
    }
}
