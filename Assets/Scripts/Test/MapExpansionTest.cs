using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapExpansionTest : MonoBehaviour
{
    [SerializeField] private int boundaryCount = 5;
    [SerializeField] private int wallCount = 24;
    [SerializeField] private float radiusGap = 5f;
    [SerializeField] private float safeZoneRadius = 3f;
    [SerializeField] private float safeZoneRadiusGap = 2f;
    [SerializeField] private int lineCount = 24;

    private readonly List<GameObject> boundaryList = new();
    private GameObject safeZoneLine;

    private void Start()
    {
        for (int level = 1; level <= boundaryCount; level++)
        {
            CreateRing(level);
        }
    }

    private void CreateSafeZoneLine(int level)
    {
        if (safeZoneLine != null)
        {
            Destroy(safeZoneLine);
        }

        safeZoneLine = new GameObject("SafeZoneLine");
        safeZoneLine.transform.SetParent(transform);

        float radius = safeZoneRadius + safeZoneRadiusGap * level;
        float angleGap = 360f / lineCount;
        float lineLength = 2f * Mathf.PI * radius / lineCount * 0.5f;

        for (int i = 0; i < lineCount; i++)
        {
            float angle = angleGap * i;
            float radian = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(radian) * radius;
            float z = Mathf.Cos(radian) * radius;

            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = $"Line_{i}";
            line.transform.SetParent(safeZoneLine.transform);
            line.transform.position = new Vector3(x, 0.03f, z);
            line.transform.localScale = new Vector3(lineLength, 0.02f, 0.1f);
            line.transform.rotation = Quaternion.LookRotation(new Vector3(x, 0f, z));

            line.GetComponent<Renderer>().material.color = Color.yellow;
            Destroy(line.GetComponent<Collider>());
        }
    }

    private void Update()
    {
        if (Keyboard.current.digit0Key.wasPressedThisFrame) CreateSafeZoneLine(0);
        if (Keyboard.current.digit1Key.wasPressedThisFrame) ExpandMap(1);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ExpandMap(2);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ExpandMap(3);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) ExpandMap(4);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) ExpandMap(5);

        if (Keyboard.current.rKey.wasPressedThisFrame) ResetMap();
    }

    private void ExpandMap(int level)
    {
        RemoveRing(level - 1);
        CreateSafeZoneLine(level);
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

    private void ResetMap()
    {
        foreach (GameObject ring in boundaryList)
        {
            ring.SetActive(true);
        }

        if (safeZoneLine != null)
        {
            Destroy(safeZoneLine);
        }
    }
}
