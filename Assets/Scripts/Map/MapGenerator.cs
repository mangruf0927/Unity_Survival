using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int seed;
    [SerializeField] private bool useSeed;

    [SerializeField] private float noiseScale;      // 값이 작으면 완만, 크면 울퉁불퉁
    [SerializeField] private float heightStep;
    [SerializeField] private int maxHeightStep;

    [SerializeField] private float cellSize;
    [SerializeField] private float cellThickness;
    [SerializeField] private int radius;
    [SerializeField] private GameObject groundObj;

    private float noiseOffsetX;
    private float noiseOffsetZ;

    private readonly Dictionary<Vector2Int, CellData> cellDictionary = new();

    private void Start()
    {
        InitializeSeed();
        GenerateGround();
    }

    private void InitializeSeed()
    {
        if (!useSeed) seed = Random.Range(1, 1000000);

        Random.InitState(seed);

        noiseOffsetX = Random.Range(-100000f, 100000f);
        noiseOffsetZ = Random.Range(-100000f, 100000f);
    }

    private void GenerateGround()
    {
        cellDictionary.Clear();

        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector2Int coordinate = new(x, z);
                if (!IsInsideMap(coordinate)) continue;

                float height = GetCellHeight(coordinate);
                CreateCell(coordinate, height);
            }
        }
    }

    private bool IsInsideMap(Vector2Int coordinate)
    {
        float roundedRadius = radius + 0.5f;

        return coordinate.sqrMagnitude <= roundedRadius * roundedRadius;
    }

    private void CreateCell(Vector2Int coordinate, float height)
    {
        GameObject cell = Instantiate(groundObj, transform);
        cell.transform.localScale = new Vector3(cellSize, cellThickness, cellSize);
        cell.transform.position = new Vector3(coordinate.x * cellSize, height - cellThickness * 0.5f, coordinate.y * cellSize);

        CellData cellData = new(coordinate, height, cell);
        cellDictionary.Add(coordinate, cellData);
    }

    private float GetCellHeight(Vector2Int coordinate)
    {
        bool isCampFire = Mathf.Abs(coordinate.x) <= 1 && Mathf.Abs(coordinate.y) <= 1;
        if (isCampFire) return 0f;

        float sampleX = coordinate.x * noiseScale + noiseOffsetX;
        float sampleZ = coordinate.y * noiseScale + noiseOffsetZ;

        float noise = Mathf.PerlinNoise(sampleX, sampleZ);

        float centeredNoise = noise * 2f - 1f;

        int step = Mathf.RoundToInt(centeredNoise * maxHeightStep);

        return step * heightStep;
    }
}
