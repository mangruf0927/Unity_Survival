using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator
{
    private readonly NavMeshSurface navMeshSurface;

    public NavMeshGenerator(NavMeshSurface navMeshSurface)
    {
        this.navMeshSurface = navMeshSurface;
    }

    public async UniTask<bool> GenerateAsync(CancellationToken ct)
    {
        if (navMeshSurface == null)
        {
            Debug.LogWarning("NavMeshSurface is null");
            return false;
        }

        bool isNewData = navMeshSurface.navMeshData == null;

        if (isNewData)
        {
            NavMeshData navMeshData = new(navMeshSurface.agentTypeID)
            {
                name = navMeshSurface.gameObject.name
            };
            navMeshSurface.navMeshData = navMeshData;
        }

        bool isCanceled = await navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData).ToUniTask(cancellationToken: ct).SuppressCancellationThrow();
        if (isCanceled) return false;
        if (isNewData) navMeshSurface.AddData();

        return !ct.IsCancellationRequested;
    }
}