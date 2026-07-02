using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [SerializeField] private CampFire campFire;

    [SerializeField] private GameObject boundaryPrefab;
    [SerializeField] private int boundaryCount;
    [SerializeField] private int wallCount;
    [SerializeField] private float radiusGap;
    [SerializeField] private List<EnemySpawner> enemySpawnerList;

    private readonly List<GameObject> boundaryList = new();

    private void Awake()
    {
        UpdateSpawners(campFire.CurrentLevel);
    }

    private void Start()
    {
        for (int level = 1; level <= boundaryCount; level++)
        {
            CreateBoundary(level);
        }
        campFire.OnLevelUp += UpdateBoundaries;
        campFire.OnLevelUp += UpdateSpawners;
    }

    private void OnDestroy()
    {
        campFire.OnLevelUp -= UpdateBoundaries;
        campFire.OnLevelUp -= UpdateSpawners;
    }

    private void CreateBoundary(int level)
    {
        GameObject boundary = new GameObject($"Level{level}");
        boundary.transform.SetParent(transform, false);
        boundaryList.Add(boundary);

        float radius = radiusGap * level;
        float angleGap = 360f / wallCount;
        float wallLength = 2f * Mathf.PI * radius / wallCount;

        for (int i = 0; i < wallCount; i++)
        {
            float angle = angleGap * i;
            float radian = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(radian) * radius;
            float z = Mathf.Cos(radian) * radius;

            GameObject wall = Instantiate(boundaryPrefab, boundary.transform);

            Vector3 scale = wall.transform.localScale;
            scale.x = wallLength;
            scale.y = 4f;
            wall.transform.localScale = scale;

            wall.transform.position = new Vector3(x, scale.y * 0.5f, z);

            Vector3 direction = new Vector3(x, 0f, z);
            wall.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void UpdateBoundaries(int level)
    {
        for (int i = 0; i < boundaryList.Count; i++)
        {
            if (boundaryList[i] == null) continue;

            bool shouldBeActive = i > level - 2;
            boundaryList[i].SetActive(shouldBeActive);
        }
    }

    private void UpdateSpawners(int level)
    {
        foreach (EnemySpawner spawner in enemySpawnerList)
        {
            if (spawner == null) continue;

            spawner.gameObject.SetActive(level >= spawner.RequiredLevel);
        }
    }
}
