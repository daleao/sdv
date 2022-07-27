namespace DaLion.Stardew.Arsenal;

/// <summary>The mod user-defined settings.</summary>
public class ModConfig
{
    /// <summary>Replace the defensive special move of some swords with a lunge move.</summary>
    public bool BringBackStabbySwords { get; set; } = true;

    /// <summary>Make weapons more unique and useful.</summary>
    public bool RebalancedWeapons { get; set; } = true;

    /// <summary>Improves certain underwhelming enchantments.</summary>
    public bool RebalancedEnchants { get; set; } = true;

    /// <summary>The stat improved by Topaz forge.</summary>
    public Perk TopazPerk { get; set; } = Perk.Cooldown;

    /// <summary>Add new stunning smack special move for slingshots.</summary>
    public bool AllowSlingshotSpecialMove { get; set; } = true;

    /// <summary>Allows slingshots to deal critical damage and be affected by critical modifiers.</summary>
    public bool AllowSlingshotCrit { get; set; } = true;

    /// <summary>Allow slingshots to be enchanted with weapon enchantments (Prismatic Shard) at the Forge.</summary>
    public bool AllowSlingshotEnchants { get; set; } = true;

    /// <summary>Allow slingshots to be enchanted with weapon forges (gemstones) at the Forge.</summary>
    public bool AllowSlingshotForges { get; set; } = true;

    /// <summary>Projectiles should not be useless for the first 100ms.</summary>
    public bool RemoveSlingshotGracePeriod { get; set; } = true;

    /// <summary>Damage mitigation should not be soft-capped at 50%.</summary>
    public bool RemoveDefenseSoftCap { get; set; } = true;

    /// <summary>Guaranteed smash crit on Duggies. Guaranteed smash miss on flying enemies..</summary>
    public bool ImmersiveClubSmash { get; set; } = true;

    /// <summary>Make parry great again by increasing it's damage by 10% per defense point.</summary>
    public bool DefenseImprovesParryDamage { get; set; } = true;

    /// <summary>Replace the starting Rusty Sword with a Wooden Blade.</summary>
    public bool WoodyReplacesRusty { get; set; } = true;

    /// <summary>Replace lame Galaxy and Infinity weapons with something truly legendary.</summary>
    public bool InfinityPlusOneWeapons { get; set; } = true;

    /// <summary>Your Dark Sword must slay this many enemies before it can be purified.</summary>
    public int RequiredKillCountToPurifyDarkSword { get; set; } = 500;

    public enum Perk
    {
        Defense,
        Cooldown,
        Precision
    }
}