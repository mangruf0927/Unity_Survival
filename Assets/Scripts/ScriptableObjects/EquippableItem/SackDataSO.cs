using UnityEngine;

[CreateAssetMenu]
public class SackDataSO : ScriptableObject
{
    [Header("등급")]
    public int level;

    [Header("용량")]
    public int capacity;
}
