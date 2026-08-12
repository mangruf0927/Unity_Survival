using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerInteraction interaction;
    [SerializeField] private PlayerEquipment equipment;
    [SerializeField] private ObjectPlacement objectPlacement;

    [SerializeField] private Animator animator;

    public Rigidbody Rigid => movement.Rigid;
    public Animator Animator => animator;

    public Weapon CurrentWeapon => equipment.CurrentWeapon;
    public RecoveryItem CurrentRecovery => equipment.CurrentRecovery;

    public event Action<EquippableItem> OnEquipped
    {
        add => equipment.OnEquipped += value;
        remove => equipment.OnEquipped -= value;
    }

    public event Action OnSackChanged
    {
        add => equipment.OnSackChanged += value;
        remove => equipment.OnSackChanged -= value;
    }

    public void SetDirection(Vector2 direction) => movement.SetDirection(direction);
    public Vector2 GetDirection() => movement.Direction;

    public void SetRun(bool state) => movement.SetRun(state);
    public bool IsRun() => movement.IsRun;
    public bool IsGround() => movement.IsGround();

    public void Move() => movement.Move();
    public void Look() => movement.Look();
    public void Stop() => movement.Stop();
    public void Jump() => movement.Jump();
    public void Fall() => movement.Fall();

    public void Eat(int hunger, int hp) => playerStats.EatFood(hunger, hp);
    public void RecoverHp(int hp) => playerStats.RecoverHp(hp);

    public void SetItemHovering(bool state) => interaction.SetItemHovering(state);
    public void SetHolding(bool state) => interaction.SetHolding(state);
    public bool HasInteractable() => interaction.HasInteractable;

    public void SetWeapon(Weapon weapon) => equipment.SetWeapon(weapon);
    public void SetSack(Sack sack) => equipment.SetSack(sack);
    public void SetRecoveryItem(RecoveryItem recovery) => equipment.SetRecoveryItem(recovery);
    public void SetAimPoint(Vector3 point) => equipment.SetAimPoint(point);

    public bool GetEquippableItem(EquippableItem item) => equipment.GetEquippableItem(item);
    public void EquipItem(int index) => equipment.EquipItem(index);
    public void UnequipItem() => equipment.UnequipItem();
    public void DropEquippableItem() => equipment.DropEquippableItem();
    public void ConsumeEquippedItem(EquippableItem item) => equipment.ConsumeEquippedItem(item);
    public bool GetCollectibleItem(Item item) => equipment.GetCollectibleItem(item);
    public void DropCollectibleItem() => equipment.DropCollectibleItem();
    public void AddAmmo(AmmoType ammoType, int amount) => equipment.AddAmmo(ammoType, amount);
    public void Reload() => equipment.Reload();
    public void StartRecoveryUse() => equipment.StartRecoveryUse();
    public void CancelRecoveryUse() => equipment.CancelRecoveryUse();

    public void StartPlacement(PlaceableItem item) => objectPlacement.StartPlacement(item, this);
    public void CancelPlacement() => objectPlacement.CancelPlacement();

    public void UpdateAnimation()
    {
        if (GetDirection() == Vector2.zero) animator.SetFloat("speed", 0f);
        else if (IsRun()) animator.SetFloat("speed", 2f);
        else animator.SetFloat("speed", 1f);
    }

    // Save/Load
    public InventorySaveData CreateInventorySaveData()
    {
        return equipment.CreateInventorySaveData();
    }

    public void LoadInventorySaveData(InventorySaveData data, EquippableDatabase equippableDatabase, ItemDataBase itemDatabase)
    {
        equipment.LoadInventorySaveData(data, equippableDatabase, itemDatabase);
    }
}
