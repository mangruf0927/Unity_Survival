using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableLoader
{
    private readonly List<AsyncOperationHandle> handles = new();

    public async UniTask<GameObject> LoadPrefabAsync(AssetReferenceGameObject reference, CancellationToken ct)
    {
        if (reference == null || !reference.RuntimeKeyIsValid())
        {
            Debug.LogError("Addressable 참조가 올바르지 않습니다.");
            return null;
        }

        var handle = Addressables.LoadAssetAsync<GameObject>(reference.RuntimeKey);

        try
        {
            await handle.ToUniTask(cancellationToken: ct);

            handles.Add(handle);
            return handle.Result;
        }
        catch (OperationCanceledException)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            throw;
        }
        catch (Exception exception)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            Debug.LogError($"Addressable 로드 실패: {reference.RuntimeKey}\n{exception}");
            return null;
        }
    }

    public void ReleaseAll()
    {
        foreach (var handle in handles)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        handles.Clear();
    }
}