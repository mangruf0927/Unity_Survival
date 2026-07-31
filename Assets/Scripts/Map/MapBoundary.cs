using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    [SerializeField] private CampFire campFire;
    [SerializeField] private GameObject boundaryPrefab;

    [SerializeField] private int cellSize;
    [SerializeField] private int wallCount;
    [SerializeField] private float wallHeight;
    [SerializeField] private float offsetY;
    [SerializeField] private List<int> radiusList;

    private readonly List<GameObject> boundaryList = new();

    private void Start()
    {
        for (int level = 1; level <= radiusList.Count; level++)
        {
            CreateBoundary(level);
        }
        UpdateBoundaries(campFire.CurrentLevel);
        campFire.OnLevelUp += UpdateBoundaries;
    }

    private void OnDestroy()
    {
        campFire.OnLevelUp -= UpdateBoundaries;
    }

    private void CreateBoundary(int level)
    {
        GameObject boundary = new($"Level{level}");
        boundary.transform.SetParent(transform, false);
        boundaryList.Add(boundary);

        float radius = GetRadius(level);
        float angleGap = 360f / wallCount;
        float wallLength = 2f * radius * Mathf.Tan(Mathf.PI / wallCount) * 1.001f;

        for (int i = 0; i < wallCount; i++)
        {
            float angle = angleGap * i;
            float radian = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(radian) * radius;
            float z = Mathf.Cos(radian) * radius;

            GameObject wall = Instantiate(boundaryPrefab, boundary.transform);

            Vector3 scale = wall.transform.localScale;
            scale.x = wallLength;
            scale.y = wallHeight;

            wall.transform.localScale = scale;
            Vector3 direction = new(x, 0f, z);

            wall.transform.SetLocalPositionAndRotation(new Vector3(x, scale.y * 0.5f + offsetY, z), Quaternion.LookRotation(direction));
        }
    }

    private float GetRadius(int level)
    {
        return (radiusList[level - 1] - 0.5f) * cellSize;
    }

    private void UpdateBoundaries(int level)
    {
        for (int i = 0; i < boundaryList.Count; i++)
        {
            if (boundaryList[i] == null) continue;

            boundaryList[i].SetActive(i > level - 2);
        }
    }
}
