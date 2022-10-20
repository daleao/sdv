namespace DaLion.Stardew.Arsenal;

/// <summary>The mod user-defined settings.</summary>
public sealed class ModConfig
{
    /// <summary>Gets or sets a value indicating whether face the current cursor position before swinging your arsenal.</summary>
    public bool FaceMouseCursor { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether enable new enchantments for melee weapons, and rebalance some old ones.</summary>
    public bool NewWeaponEnchants { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether guaranteed smash crit on Duggies. Guaranteed smash miss on flying enemies.</summary>
    public bool ImmersiveClubSmash { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether make parry great again by increasing it's damage by 10% per defense point.</summary>
    public bool DefenseImprovesParryDamage { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether replace the defensive special move of some swords with an offensive lunge move.</summary>
    public bool BringBackStabbySwords { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether replace the starting Rusty Sword with a Wooden Blade.</summary>
    public bool WoodyReplacesRusty { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    public bool InfinityPlusOneWeapons { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether improves certain underwhelming enchantments.</summary>
    public bool RebalancedForges { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether removes the 50% soft-cap on player defense.</summary>
    public bool RemoveFarmerDefenseSoftCap { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether monster defense is effectively squared.</summary>
    public bool ImprovedEnemyDefense { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether damage mitigation is skipped for critical hits.</summary>
    public bool CritsIgnoreDefense { get; set; } = true;

    /// <summary>Gets or sets increases the health of all monsters.</summary>
    public float MonsterHealthMultiplier { get; set; } = 1.5f;

    /// <summary>Gets or sets increases the damage dealt by all monsters.</summary>
    public float MonsterDamageMultiplier { get; set; } = 1f;

    /// <summary>Gets or sets increases the resistance of all monsters.</summary>
    public float MonsterDefenseMultiplier { get; set; } = 1f;

    /// <summary>Gets or sets a value indicating whether randomizes monster stats to add variability to monster encounters.</summary>
    public bool VariedMonsterStats { get; set; } = true;
}
