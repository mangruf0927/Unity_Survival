using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [SerializeField] private CampFire campFire;

    [SerializeField] private GameObject boundaryPrefab;
    [SerializeField] private int boundaryCount;
    [SerializeField] private int wallCount;
    [SerializeField] private float radiusGap;

    private readonly List<GameObject> boundaryList = new();

    private void Start()
    {
        for (int level = 1; level <= boundaryCount; level++)
        {
            CreateBoundary(level);
        }
        campFire.OnLevelUp += RemoveBoundary;
    }

    private void OnDestroy()
    {
        campFire.OnLevelUp -= RemoveBoundary;
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

    public void RemoveBoundary(int level)
    {
        if (level <= 1) return;

        int index = level - 2;
        if (index < 0 || index >= boundaryList.Count) return;

        boundaryList[index].SetActive(false);
    }
}
