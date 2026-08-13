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

        if (navMeshSurface.navMeshData == null)
        {
            navMeshSurface.BuildNavMesh();
            return !ct.IsCancellationRequested;
        }

        await navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData).ToUniTask(cancellationToken: ct);

        return !ct.IsCancellationRequested;
    }
}