namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
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

/// <summary>Extensions for the <see cref="UpgradeLevel"/> enum.</summary>
public static partial class UpgradeLevelExtensions
{
    /// <summary>Returns the tool power-up <see cref="Color"/> for this <paramref name="upgrade"/>.</summary>
    /// <param name="upgrade">The <see cref="UpgradeLevel"/>.</param>
    /// <returns>A <see cref="Color"/>.</returns>
    public static Color GetPowerUpColor(this UpgradeLevel upgrade)
    {
        return upgrade switch
        {
            UpgradeLevel.Copper => Color.Orange,
            UpgradeLevel.Steel => Color.LightSteelBlue,
            UpgradeLevel.Gold => Color.Gold,
            UpgradeLevel.Iridium => Color.Violet,
            UpgradeLevel.Radioactive => Color.Chartreuse,
            >= UpgradeLevel.Mythicite => Color.Cyan,
            _ => Color.White,
        };
    }

    /// <summary>Returns a corresponding readable <see cref="Color"/> for this <paramref name="upgrade"/>.</summary>
    /// <param name="upgrade">The <see cref="UpgradeLevel"/>.</param>
    /// <returns>A <see cref="Color"/>.</returns>
    public static Color GetTextColor(this UpgradeLevel upgrade)
    {
        return upgrade.GetPowerUpColor().ChangeValue(0.5f);
    }
}
