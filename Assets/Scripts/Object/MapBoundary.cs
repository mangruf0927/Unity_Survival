using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [SerializeField] private CampFire campFire;
    [SerializeField] private GameObject boundaryPrefab;

    [SerializeField] private int wallCount;
    [SerializeField] private List<float> radiusList = new() { 50f, 140f, 230f, 320f, 410f };

    [SerializeField] private List<EnemySpawner> enemySpawnerList;

    private readonly List<GameObject> boundaryList = new();

    private void Awake()
    {
        UpdateSpawners(campFire.CurrentLevel);
    }

    private void Start()
    {
        for (int level = 1; level <= radiusList.Count; level++)
        {
            CreateBoundary(level);
        }
        UpdateBoundaries(campFire.CurrentLevel);

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
        int radiusIndex = level - 1;

        if (radiusIndex < 0 || radiusIndex >= radiusList.Count)
            return;

        GameObject boundary = new($"Level{level}");
        boundary.transform.SetParent(transform, false);

        boundaryList.Add(boundary);

        float radius = radiusList[radiusIndex];
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
            scale.y = 15f;
            wall.transform.localScale = scale;

            wall.transform.localPosition = new Vector3(x, scale.y * 0.5f, z);

            Vector3 direction = new(x, 0f, z);

            wall.transform.localRotation = Quaternion.LookRotation(direction);
        }
    }

    public void UpdateBoundaries(int level)
    {
        for (int i = 0; i < boundaryList.Count; i++)
        {
            if (boundaryList[i] == null) continue;

            boundaryList[i].SetActive(i > level - 2);
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
