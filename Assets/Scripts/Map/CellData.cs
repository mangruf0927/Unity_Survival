using System;
using UnityEngine;

public enum CenterType { NONE, CAMPFIRE, ENEMYSPAWN }

[Serializable]
public class CellData
{
    public Vector2Int Coordinate { get; private set; }
    public float Height { get; private set; }
    public GameObject GroundObject { get; private set; }
    public CenterType Type { get; private set; }

    public CellData(Vector2Int coordinate, float height, GameObject obj)
    {
        Coordinate = coordinate;
        Height = height;
        GroundObject = obj;
        Type = CenterType.NONE;
    }

    public void SetCenterType(CenterType centerType)
    {
        Type = centerType;
    }
}
