using UnityEngine;

[CreateAssetMenu]
public class SackDataSO : ScriptableObject
{
    [Header("등급")]
    public SackLevel level;

    [Header("용량")]
    public int capacity;
}
