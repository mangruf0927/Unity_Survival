using System;
using UnityEngine;

public enum CenterType { NONE, CAMPFIRE, STRUCTURE, ITEMSPOT, ENEMYSPAWN }

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

    public void SetHeight(float height, float cellThickness)
    {
        Height = height;

        Vector3 position = GroundObject.transform.localPosition;
        position.y = height - cellThickness * 0.5f;
        GroundObject.transform.localPosition = position;
    }
}
