namespace DaLion.Enchantments;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Config schema for the Ponds mod.</summary>
public sealed class EnchantmentsConfig
{
    private SocketStyle _gemstoneSocketStyle = SocketStyle.Diamond;

    #region dropdown enums

    /// <summary>The style used to draw forged gemstones.</summary>
    public enum SocketStyle
    {
        /// <summary>None. Keep vanilla style.</summary>
        None,

        /// <summary>A diamond-shaped icon.</summary>
        Diamond,

        /// <summary>A more rounded icon.</summary>
        Round,

        /// <summary>Shaped like an iridium ore.</summary>
        Iridium,
    }

    /// <summary>The position of the forged gemstones.</summary>
    public enum SocketPosition
    {
        /// <summary>The normal position, immediately above the item's description.</summary>
        Standard,

        /// <summary>Above the horizontal separator, immediately below the item's name and level.</summary>
        AboveSeparator,
    }

    #endregion dropdown enums

    /// <summary>Gets the style of the sprite used to represent gemstone sockets in tooltips.</summary>
    [JsonProperty]
    [GMCMPriority(0)]
    public SocketStyle GemstoneSocketStyle
    {
        get => this._gemstoneSocketStyle;
        internal set
        {
            if (value == this._gemstoneSocketStyle)
            {
                return;
            }

            this._gemstoneSocketStyle = value;
            ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/GemstoneSockets");
        }
    }

    /// <summary>Gets the relative position where forge gemstone sockets should be drawn.</summary>
    [JsonProperty]
    [GMCMPriority(1)]
    public SocketPosition GemstoneSocketPosition { get; internal set; } = SocketPosition.AboveSeparator;
}
