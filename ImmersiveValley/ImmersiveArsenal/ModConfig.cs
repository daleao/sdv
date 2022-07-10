namespace DaLion.Stardew.Arsenal;

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Make weapons more unique and useful.</summary>
    public bool RebalancedWeapons { get; set; } = true;

    /// <summary>Improves certain underwhelming enchantments.</summary>
    public bool RebalancedEnchants { get; set; } = true;

    /// <summary>Introduces new enchantments.</summary>
    public bool NewEnchants { get; set; } = true;

    /// <summary>Allows Slingshot to deal critical damage and be affected by critical modifiers.</summary>
    public bool AllowSlingshotCrit { get; set; } = true;

    /// <summary>Allow Slingshot to be enchanted with weapon enchantments (Prismatic Shard) at the Forge.</summary>
    public bool AllowSlingshotEnchants { get; set; } = true;

    /// <summary>Allow Slingshot to be enchanted with weapon forges (gemstones) at the Forge.</summary>
    public bool AllowSlingshotForges { get; set; } = true;

    /// <summary>Projectiles should not be useless for the first 100ms.</summary>
    public bool RemoveSlingshotGracePeriod { get; set; } = true;

    /// <summary>Damage mitigation should not be soft-capped at 50%.</summary>
    public bool RemoveDefenseSoftCap { get; set; } = true;

    /// <summary>Replace the starting Rusty Sword with a Wooden Blade.</summary>
    public bool WoodyReplacesRusty { get; set; } = true;

    /// <summary>Replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    public bool InfinityPlusOneWeapons { get; set; } = true;

    public int RequiredKillCountToPurifyDarkSword { get; set; } = 500;
}