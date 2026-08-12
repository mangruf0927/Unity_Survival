using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    private readonly Dictionary<Vector2Int, CellData> cellDictionary = new();
    private readonly List<int> levelRadiusList;

    public IEnumerable<CellData> Cells => cellDictionary.Values;

    public MapGrid(List<int> levelRadiusList)
    {
        this.levelRadiusList = levelRadiusList;
    }

    public void Add(CellData cell)
    {
        cellDictionary.Add(cell.Coordinate, cell);
    }

    public bool TryGetCell(Vector2Int coordinate, out CellData cell)
    {
        return cellDictionary.TryGetValue(coordinate, out cell);
    }

    public void Clear()
    {
        cellDictionary.Clear();
    }

    public List<CellData> GetAvailableCells(int level)
    {
        List<CellData> availableCellList = new();

        foreach (CellData cell in cellDictionary.Values)
        {
            if (cell.Type != CenterType.NONE) continue;
            if (IsCampFireArea(cell.Coordinate)) continue;
            if (!IsCellInLevel(cell.Coordinate, level)) continue;

            availableCellList.Add(cell);
        }
        SortByCoordinate(availableCellList);
        return availableCellList;
    }

    public List<CellData> GetStructureCells(Vector2Int start, Vector2Int size, int level)
    {
        List<CellData> structureCellList = new();

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                Vector2Int coordinate = new(start.x + x, start.y + z);

                if (!cellDictionary.TryGetValue(coordinate, out CellData cell)) return null;
                if (cell.Type != CenterType.NONE) return null;
                if (!IsCellInLevel(coordinate, level)) return null;

                structureCellList.Add(cell);
            }
        }
        return structureCellList;
    }

    public List<CellData> GetEnvironmentCells()
    {
        List<CellData> environmentCellList = new();

        foreach (CellData cell in cellDictionary.Values)
        {
            if (cell.Type == CenterType.STRUCTURE) continue;
            if (IsCampFireArea(cell.Coordinate)) continue;

            environmentCellList.Add(cell);
        }

        SortByCoordinate(environmentCellList);
        return environmentCellList;
    }

    public bool IsCampFireArea(Vector2Int coordinate)
    {
        return Mathf.Abs(coordinate.x) <= 1 && Mathf.Abs(coordinate.y) <= 1;
    }

    public bool IsInsideRadius(Vector2Int coordinate, int radius)
    {
        return coordinate.sqrMagnitude < radius * radius;
    }

    private bool IsCellInLevel(Vector2Int coordinate, int level)
    {
        int outerRadius = GetLevelRadius(level);

        if (outerRadius < 0) return false;
        if (!IsInsideRadius(coordinate, outerRadius)) return false;
        if (level == 1) return true;

        int innerRadius = GetLevelRadius(level - 1);

        return !IsInsideRadius(coordinate, innerRadius);
    }

    private int GetLevelRadius(int level)
    {
        if (levelRadiusList == null) return -1;
        if (level < 1 || level > levelRadiusList.Count) return -1;

        return levelRadiusList[level - 1];
    }


    private void SortByCoordinate(List<CellData> cells)
    {
        cells.Sort((a, b) =>
        {
            int xCompare = a.Coordinate.x.CompareTo(b.Coordinate.x);
            return xCompare != 0 ? xCompare : a.Coordinate.y.CompareTo(b.Coordinate.y);
        });
    }

}
