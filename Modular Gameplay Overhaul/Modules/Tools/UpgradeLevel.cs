namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>The upgrade level of a <see cref="Tool"/>.</summary>
[EnumExtensions]
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

    /// <summary>Mythicite upgrade. Requires Moon Misadventures.</summary>
    Mythicite,

    /// <summary>Extra upgrade for tools with the Reaching Enchantment.</summary>
    Enchanted,
}
