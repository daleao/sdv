namespace DaLion.Overhaul.Modules.Combat.Configs;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Integrations.GMCM;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for CMBT.</summary>
public sealed class ControlsUiConfig
{
    private bool _enableAutoSelection = true;
    private uint _meleeAutoSelectionRange = 2;
    private uint _rangedAutoSelectionRange = 4;
    private SocketStyle _forgeSocketStyle = SocketStyle.Diamond;

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

    /// <summary>The style used to display stat bonuses in weapon tooltips.</summary>
    public enum TooltipStyle
    {
        /// <summary>Display the absolute value of the stat, minus it's default value for the weapon type.</summary>
        Absolute,

        /// <summary>Display the relative value of the stat, with respect to the default value for the weapon type.</summary>
        Relative,

        /// <summary>The vanilla confusing nonsense.</summary>
        Vanilla,
    }

    #endregion dropdown enums

    /// <summary>Gets a value indicating whether to allow drifting in the movement direction when swinging weapons.</summary>
    [JsonProperty]
    [GMCMPriority(500)]
    public bool SlickMoves { get; internal set; } = true;

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your weapon.</summary>
    [JsonProperty]
    [GMCMPriority(501)]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow auto-selecting a weapon or slingshot.</summary>
    [JsonProperty]
    [GMCMPriority(502)]
    public bool EnableAutoSelection
    {
        get => this._enableAutoSelection;
        internal set
        {
            this._enableAutoSelection = value;
            if (value || !Context.IsWorldReady)
            {
                return;
            }

            CombatModule.State.AutoSelectableMelee = null;
            CombatModule.State.AutoSelectableRanged = null;
        }
    }

    /// <summary>Gets a value indicating how close an enemy must be to auto-select a weapon, in tiles.</summary>
    [JsonProperty]
    [GMCMPriority(503)]
    [GMCMRange(1, 5)]
    public uint MeleeAutoSelectionRange
    {
        get => this._meleeAutoSelectionRange;
        internal set
        {
            this._meleeAutoSelectionRange = Math.Max(value, 1);
        }
    }

    /// <summary>Gets a value indicating how close an enemy must be to auto-select a slingshot, in tiles.</summary>
    [JsonProperty]
    [GMCMPriority(504)]
    [GMCMRange(1, 10)]
    public uint RangedAutoSelectionRange
    {
        get => this._rangedAutoSelectionRange;
        internal set
        {
            this._rangedAutoSelectionRange = Math.Max(value, 1);
        }
    }

    /// <summary>Gets the chosen key(s) for toggling auto-selection.</summary>
    [JsonProperty]
    [GMCMPriority(505)]
    public KeybindList SelectionKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the <see cref="Color"/> used to indicate tools enabled or auto-selection.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(506)]
    [GMCMColorPicker(false, (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)]
    [GMCMDefaultColor(0, 255, 255)]
    public Color SelectionBorderColor { get; internal set; } = Color.Aqua;

    /// <summary>Gets a value indicating whether to color-code tool names, <see href="https://tvtropes.org/pmwiki/pmwiki.php/Main/ColourCodedForYourConvenience"> for your convenience</see>.</summary>
    [JsonProperty]
    [GMCMPriority(507)]
    public bool ColorCodedForYourConvenience { get; internal set; } = true;

    /// <summary>Gets the <see cref="Color"/> used by common-tier weapons.</summary>
    [JsonProperty]
    [GMCMPriority(508)]
    [GMCMOverride(typeof(GenericModConfigMenu), "CombatConfigColorByTierOverride")]
    public Color[] ColorByTier { get; internal set; } =
    {
        new(34, 17, 34),
        Color.Green,
        Color.Blue,
        Color.Purple,
        Color.Red,
        Color.MonoGameOrange,
        Color.MonoGameOrange,
    };

    /// <summary>Gets the style of the sprite used to represent gemstone forges in tooltips.</summary>
    [JsonProperty]
    [GMCMPriority(509)]
    public SocketStyle ForgeSocketStyle
    {
        get => this._forgeSocketStyle;
        internal set
        {
            if (value == this._forgeSocketStyle)
            {
                return;
            }

            this._forgeSocketStyle = value;
            ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/GemstoneSockets");
        }
    }

    /// <summary>Gets the relative position where forge gemstones should be drawn.</summary>
    [JsonProperty]
    [GMCMPriority(510)]
    public SocketPosition ForgeSocketPosition { get; internal set; } = SocketPosition.AboveSeparator;

    /// <summary>Gets the style of the tooltips for displaying stat bonuses for weapons.</summary>
    [JsonProperty]
    [GMCMPriority(511)]
    public TooltipStyle WeaponTooltipStyle { get; internal set; }

    /// <summary>Gets a value indicating whether to override the draw method to include the currently-equipped ammo.</summary>
    [JsonProperty]
    [GMCMPriority(512)]
    public bool DrawCurrentAmmo { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace the mouse cursor with a bulls-eye while firing.</summary>
    [JsonProperty]
    [GMCMPriority(513)]
    public bool BullseyeReplacesCursor { get; internal set; } = true;
}
