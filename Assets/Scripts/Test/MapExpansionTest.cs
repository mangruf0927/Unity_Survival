using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapExpansionTest : MonoBehaviour
{
    [SerializeField] private int boundaryCount = 5;
    [SerializeField] private int wallCount = 24;
    [SerializeField] private float radiusGap = 5f;

    private readonly List<GameObject> boundaryList = new();

    private void Start()
    {
        for (int level = 1; level <= boundaryCount; level++)
        {
            CreateRing(level);
        }
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) RemoveRing(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) RemoveRing(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) RemoveRing(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) RemoveRing(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) RemoveRing(4);

        if (Keyboard.current.rKey.wasPressedThisFrame) ResetRings();
    }

    private void CreateRing(int level)
    {
        GameObject ring = new GameObject($"Barrier_Lv{level}");
        ring.transform.SetParent(transform);
        boundaryList.Add(ring);

        float radius = radiusGap * level;
        float angleGap = 360f / wallCount;
        float wallLength = 2f * Mathf.PI * radius / wallCount;

        for (int i = 0; i < wallCount; i++)
        {
            float angle = angleGap * i;
            float radian = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(radian) * radius;
            float z = Mathf.Cos(radian) * radius;

            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = $"Wall_{i}";
            wall.transform.SetParent(ring.transform);
            wall.transform.position = new Vector3(x, 1f, z);
            wall.transform.localScale = new Vector3(wallLength, 2f, 1f);

            Vector3 outsideDirection = new Vector3(x, 0f, z);
            wall.transform.rotation = Quaternion.LookRotation(outsideDirection);
        }
    }

    private void RemoveRing(int index)
    {
        if (index < 0 || index >= boundaryList.Count) return;

        boundaryList[index].SetActive(false);
    }

    private void ResetRings()
    {
        foreach (GameObject ring in boundaryList)
        {
            ring.SetActive(true);
        }
    }
}
