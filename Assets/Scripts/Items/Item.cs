using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private ItemType itemType;
    [SerializeField] private int value;

    public string ItemName => itemName;
    public ItemType ItemType => itemType;
    public int Value => value;
}
