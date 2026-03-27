using UnityEngine;

public class AmmoItem : Item
{
    [SerializeField] private AmmoType ammoType;

    public AmmoType AmmoType => ammoType;
}
