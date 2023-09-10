namespace DaLion.Chargeable.Framework;

/// <summary>The upgrade level of a <see cref="Tool"/>.</summary>
public enum UpgradeLevel
{
    /// <summary>No upgrade.</summary>
    None,

    /// <summary>Copper upgrade.</summary>
    Copper,

    /// <summary>Steel upgrade.</summary>
    Steel,

    /// <summary>Gold upgrade.</summary>
    Gold,

    /// <summary>Iridium upgrade.</summary>
    Iridium,

    /// <summary>Radioactive upgrade. Requires Moon Misadventures.</summary>
    Radioactive,

    /// <summary>Extra upgrade for tools with the Reaching Enchantment.</summary>
    Enchanted,
}
