namespace DaLion.Stardew.Arsenal;

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Face the current cursor position before swinging your arsenal.</summary>
    public bool FaceMouseCursor { get; set; } = true;

    /// <summary>Enable new enchantments for melee weapons, and rebalance some old ones.</summary>
    public bool NewWeaponEnchants { get; set; } = true;

    /// <summary>Guaranteed smash crit on Duggies. Guaranteed smash miss on flying enemies.</summary>
    public bool ImmersiveClubSmash { get; set; } = true;

    /// <summary>Make parry great again by increasing it's damage by 10% per defense point.</summary>
    public bool DefenseImprovesParryDamage { get; set; } = true;

    /// <summary>Replace the defensive special move of some swords with an offensive lunge move.</summary>
    public bool BringBackStabbySwords { get; set; } = true;

    /// <summary>Replace the starting Rusty Sword with a Wooden Blade.</summary>
    public bool WoodyReplacesRusty { get; set; } = true;

    /// <summary>Replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    public bool InfinityPlusOneWeapons { get; set; } = true;

    /// <summary>Your Dark Sword must slay this many enemies before it can be purified.</summary>
    public int RequiredKillCountToPurifyDarkSword { get; set; } = 500;


    /// <summary>Improves certain underwhelming enchantments.</summary>
    public bool RebalancedForges { get; set; } = true;


    /// <summary>Removes the 50% soft-cap on player defense.</summary>
    public bool RemoveFarmerDefenseSoftCap { get; set; } = true;

    /// <summary>Monster defense is effectively squared.</summary>
    public bool ImprovedEnemyDefense { get; set; } = true;

    /// <summary>Damage mitigation is skipped for critical hits.</summary>
    public bool CritsIgnoreDefense { get; set; } = true;


    /// <summary>Increases the health of all monsters.</summary>
    public float MonsterHealthMultiplier { get; set; } = 1.5f;

    /// <summary>Increases the damage dealt by all monsters.</summary>
    public float MonsterDamageMultiplier { get; set; } = 1f;

    /// <summary>Increases the resistance of all monsters.</summary>
    public float MonsterDefenseMultiplier { get; set; } = 1f;

    /// <summary>Randomizes monster stats to add variability to monster encounters.</summary>
    public bool VariedMonsterStats { get; set; } = true;
}