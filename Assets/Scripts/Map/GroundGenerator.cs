using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GroundGenerator
{
    private readonly MapGrid mapGrid;
    private readonly Transform mapParent;
    private readonly GameObject groundPrefab;

    private readonly int mapRadius;
    private readonly float cellSize;
    private readonly float cellThickness;

    private readonly float noiseScale;
    private readonly float heightStep;
    private readonly int maxHeightStep;

    public GroundGenerator(MapGrid mapGrid, Transform mapParent, GameObject groundPrefab,
        int mapRadius, float cellSize, float cellThickness, float noiseScale, float heightStep, int maxHeightStep)
    {
        this.mapGrid = mapGrid;
        this.mapParent = mapParent;
        this.groundPrefab = groundPrefab;
        this.mapRadius = mapRadius;
        this.cellSize = cellSize;
        this.cellThickness = cellThickness;
        this.noiseScale = noiseScale;
        this.heightStep = heightStep;
        this.maxHeightStep = maxHeightStep;
    }

    public async UniTask GenerateAsync(float noiseOffsetX, float noiseOffsetZ, CancellationToken ct)
    {
        if (groundPrefab == null)
        {
            Debug.LogWarning("Ground Prefab is null");
            return;
        }

        Clear();

        GameObject parent = new("Grounds");
        parent.transform.SetParent(mapParent);

        const int cellsPerFrame = 100;
        int counter = 0;

        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int z = -mapRadius; z <= mapRadius; z++)
            {
                Vector2Int coordinate = new(x, z);
                if (!mapGrid.IsInsideRadius(coordinate, mapRadius)) continue;

                float height = GetCellHeight(coordinate, noiseOffsetX, noiseOffsetZ);
                CreateCell(coordinate, height, parent.transform);

                if (++counter % cellsPerFrame == 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
        }
    }

    private void Clear()
    {
        foreach (CellData cell in mapGrid.Cells)
        {
            if (cell.GroundObject != null) Object.Destroy(cell.GroundObject);
        }
        mapGrid.Clear();
    }

    private void CreateCell(Vector2Int coordinate, float height, Transform parent)
    {
        GameObject ground = Object.Instantiate(groundPrefab, parent);
        ground.transform.localScale = new Vector3(cellSize, cellThickness, cellSize);
        ground.transform.localPosition = new Vector3(coordinate.x * cellSize, height - cellThickness * 0.5f, coordinate.y * cellSize);

        CellData cellData = new(coordinate, height, ground);
        mapGrid.Add(cellData);
    }

    private float GetCellHeight(Vector2Int coordinate, float noiseOffsetX, float noiseOffsetZ)
    {
        if (mapGrid.IsCampFireArea(coordinate)) return 0f;

        float sampleX = coordinate.x * noiseScale + noiseOffsetX;
        float sampleZ = coordinate.y * noiseScale + noiseOffsetZ;

        float noise = Mathf.PerlinNoise(sampleX, sampleZ);
        float centeredNoise = noise * 2f - 1f;

        int step = Mathf.RoundToInt(centeredNoise * maxHeightStep);
        return step * heightStep;
    }
}
