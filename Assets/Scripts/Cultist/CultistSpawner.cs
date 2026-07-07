using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CultistSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform CampFire;

    [SerializeField] private float minRadius = 20f;
    [SerializeField] private float maxRadius = 30f;

    private readonly List<CultistController> aliveCultistList = new();

    public void Spawn(PoolTypeEnums cultistType, CultistWeaponType weaponType, int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnCultist(cultistType, weaponType);
        }
    }

    private void SpawnCultist(PoolTypeEnums cultistType, CultistWeaponType weaponType)
    {
        Vector2 direction = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minRadius, maxRadius);
        Vector3 randomPosition = new(direction.x * distance, 0f, direction.y * distance);

        if (!NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 5f, NavMesh.AllAreas)) return;

        GameObject cultist = ObjectPool.Instance.GetFromPool(cultistType);
        cultist.transform.position = hit.position;

        CultistController controller = cultist.GetComponent<CultistController>();
        controller.SetWeapon(weaponType);
        controller.SetUp(player, CampFire);

        Register(controller);
    }

    public List<CultistSaveData> CreateSaveData()
    {
        List<CultistSaveData> dataList = new();

        foreach (CultistController cultist in aliveCultistList)
        {
            if (cultist == null || cultist.CurrentHp <= 0) continue;
            if (!cultist.gameObject.activeInHierarchy) continue;

            dataList.Add(cultist.CreateSaveData());
        }

        return dataList;
    }

    public void LoadSaveData(List<CultistSaveData> dataList)
    {
        ClearCultistList();

        if (dataList == null) return;

        foreach (CultistSaveData data in dataList)
        {
            LoadCultist(data);
        }
    }

    public void LoadCultist(CultistSaveData data)
    {
        if (data == null) return;

        GameObject cultist = ObjectPool.Instance.GetFromPool(data.cultistType);
        if (cultist == null) return;

        CultistController controller = cultist.GetComponent<CultistController>();
        if (controller == null)
        {
            Debug.LogError($"{data.cultistType} 풀 오브젝트에 CultistController가 없습니다.");
            ObjectPool.Instance.ReturnToPool(cultist, data.cultistType);
            return;
        }

        controller.LoadSaveData(data, player, CampFire);
        Register(controller);
    }

    private void Register(CultistController controller)
    {
        if (controller == null || aliveCultistList.Contains(controller)) return;

        aliveCultistList.Add(controller);

        CultistStats stats = controller.GetComponent<CultistStats>();
        if (stats == null) return;

        stats.OnDead -= Unregister;
        stats.OnDead += Unregister;
    }

    private void Unregister(CultistStats stats)
    {
        if (stats == null) return;

        CultistController controller = stats.GetComponent<CultistController>();
        if (controller == null) return;

        aliveCultistList.Remove(controller);
    }

    private void ClearCultistList()
    {
        List<CultistController> tempList = new(aliveCultistList);

        foreach (CultistController cultist in tempList)
        {
            if (cultist == null) continue;

            ObjectPool.Instance.ReturnToPool(cultist.gameObject, cultist.CultistType);
        }

        aliveCultistList.Clear();
    }
}
