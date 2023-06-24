namespace DaLion.Overhaul.Modules.Slingshots;

#region using directives

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for SLNGS.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <summary>Gets a value indicating whether to re-balance the damage and knockback modifiers of Slingshots.</summary>
    [JsonProperty]
    public bool EnableRebalance { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow slingshots to deal critical damage and be affected by critical modifiers.</summary>
    [JsonProperty]
    public bool EnableCriticalHits { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow slingshots to be enchanted with weapon forges (gemstones) at the Forge.</summary>
    [JsonProperty]
    public bool EnableEnchantments { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to enable the custom slingshot stun smack special move.</summary>
    [JsonProperty]
    public bool EnableSpecialMove { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow forging the Infinity Slingshot.</summary>
    [JsonProperty]
    public bool EnableInfinitySlingshot { get; internal set; } = true;

    /// <summary>Gets a value indicating whether projectiles should not be useless for the first 100ms.</summary>
    [JsonProperty]
    public bool DisableGracePeriod { get; internal set; } = true;

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your slingshot (for special moves).</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when charging slingshots.</summary>
    [JsonProperty]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow auto-selecting a slingshot.</summary>
    [JsonProperty]
    public bool EnableAutoSelection { get; internal set; } = true;

    /// <summary>Gets the chosen mod key(s).</summary>
    [JsonProperty]
    public KeybindList SelectionKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the <see cref="Color"/> used to indicate tools enabled for auto-selection.</summary>
    [JsonProperty]
    public Color SelectionBorderColor { get; internal set; } = Color.Magenta;

    /// <summary>Gets a value indicating how close an enemy must be to auto-select a slingshot, in tiles.</summary>
    [JsonProperty]
    public uint AutoSelectionRange { get; internal set; } = 4;

    /// <summary>Gets a value indicating whether to color-code slingshot names, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience"> for your convenience</see>.</summary>
    [JsonProperty]
    public bool ColorCodedForYourConvenience { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to override the draw method to include the currently-equipped ammo.</summary>
    [JsonProperty]
    public bool DrawCurrentAmmo { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the mouse cursor with a bulls-eye while firing.</summary>
    [JsonProperty]
    public bool BullseyeReplacesCursor { get; internal set; } = true;
}
