using UnityEngine;
using UnityEngine.AI;

public class CultistSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private CampFire campFire;

    [SerializeField] private float minRadius = 20f;
    [SerializeField] private float maxRadius = 30f;

    public void Spawn(PoolTypeEnums cultistType, int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnCultist(cultistType);
        }
    }

    private void SpawnCultist(PoolTypeEnums cultistType)
    {
        Vector2 direction = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minRadius, maxRadius);
        Vector3 randomPosition = new(direction.x * distance, 0f, direction.y * distance);

        if (!NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas)) return;

        GameObject cultist = ObjectPool.Instance.GetFromPool(cultistType);
        cultist.transform.position = hit.position;
        CultistController controller = cultist.GetComponent<CultistController>();
        controller.SetUp(player, campFire);
    }
}