using UnityEngine;

public abstract class EquippableItem : MonoBehaviour
{
    [SerializeField] private string itemName;

    public virtual bool CanDrop => true;

    public abstract void OnEquip(PlayerController player);
    public abstract void OnUnequip(PlayerController player);
}
