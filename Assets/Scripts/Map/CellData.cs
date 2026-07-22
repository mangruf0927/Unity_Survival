using System;
using UnityEngine;

[Serializable]
public class CellData : MonoBehaviour
{
    public Vector2Int Coordinate { get; private set; }
    public float Height { get; private set; }
    public GameObject GroundPrefab { get; private set; }

    public CellData(Vector2Int coordinate, float height, GameObject prefab)
    {
        Coordinate = coordinate;
        Height = height;
        GroundPrefab = prefab;
    }
}
