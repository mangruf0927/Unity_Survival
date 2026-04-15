public enum ToolType { SACK, AXE, SPEAR, REVOLVER, RIFLE }
public enum MeleeLevel { OLD, GOOD, STRONG }
public enum SackLevel { OLD, GOOD, GIANT }
public enum AmmoType { REVOLVER, RIFLE }
public enum ItemType { FOOD, FUEL, MATERIAL, AMMO }

[System.Flags]
public enum ItemProperty
{
    NONE = 0,
    MATERIAL = 1 << 0,
    COOKABLE = 1 << 1,
    TRADE = 1 << 2
}