namespace DaLion.Overhaul.Modules.Enchantments;

#region using directives

using Newtonsoft.Json;
using StardewValley.Tools;

#endregion using directives

/// <summary>The user-configurable settings for ENCH.</summary>
public sealed class Config : Shared.Configs.Config
{
    #region dropdown enums

    /// <summary>The style used to draw forged gemstones.</summary>
    public enum ForgeSocketStyle
    {
        /// <summary>A diamond-shaped icon.</summary>
        Diamond,

        /// <summary>A more rounded icon.</summary>
        Round,

        /// <summary>Shaped like an iridium ore.</summary>
        Iridium,
    }

    /// <summary>The position of the forged gemstones.</summary>
    public enum ForgeSocketPosition
    {
        /// <summary>The normal position, immediately above the item's description.</summary>
        Standard,

        /// <summary>Above the horizontal separator, immediately below the item's name and level.</summary>
        AboveSeparator,
    }

    #endregion dropdown enums

    /// <summary>Gets a value indicating whether to use the overhauled enchantments for <see cref="MeleeWeapon"/>s.</summary>
    [JsonProperty]
    public bool MeleeEnchantments { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to use the overhauled enchantments for <see cref="Slingshot"/>s.</summary>
    [JsonProperty]
    public bool RangedEnchantments { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to improve certain underwhelming gemstone enchantments.</summary>
    [JsonProperty]
    public bool RebalancedForges { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace generic Forge text with specific gemstone icons and empty sockets.</summary>
    [JsonProperty]
    public bool DrawForgeSockets { get; internal set; } = true;

    /// <summary>Gets the style of the sprite used to represent gemstone forges in tooltips.</summary>
    [JsonProperty]
    public ForgeSocketStyle SocketStyle { get; internal set; } = ForgeSocketStyle.Diamond;

    /// <summary>Gets the relative position where forge gemstones should be drawn.</summary>
    [JsonProperty]
    public ForgeSocketPosition SocketPosition { get; internal set; } = ForgeSocketPosition.AboveSeparator;
}
