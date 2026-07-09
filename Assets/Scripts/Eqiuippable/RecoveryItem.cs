using System;
using UnityEngine;

public class RecoveryItem : EquippableItem
{
    [SerializeField] private int recoveryHp;
    [SerializeField] private float holdTime;

    // public int RecoveryHp => recoveryHp;
    public float HoldTime => holdTime;
    public override bool CanDrop => canDrop;

    public override void OnEquip(PlayerController player)
    {
        player.SetRecoveryItem(this);
        gameObject.SetActive(true);
    }

    public override void OnUnequip(PlayerController player)
    {
        player.CancelRecoveryUse();
        player.SetRecoveryItem(null);
        gameObject.SetActive(false);
    }

    public void Apply(PlayerController player)
    {
        player.RecoverHp(recoveryHp);
        player.ConsumeEquippedItem(this);
    }
}
