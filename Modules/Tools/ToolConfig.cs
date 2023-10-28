namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using DaLion.Overhaul.Modules.Tools.Configs;
using DaLion.Shared.Integrations.GMCM;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for TOLS.</summary>
public sealed class ToolConfig
{
    #region general

    /// <summary>Gets a value indicating whether to play the shockwave animation when the charged Axe is released.</summary>
    [JsonProperty]
    [GMCMSection("general")]
    [GMCMPriority(0)]
    public bool PlayShockwaveAnimation { get; internal set; } = true;

    /// <summary>Gets affects the shockwave travel speed. Lower is faster. Set to 0 for instant.</summary>
    [JsonProperty]
    [GMCMSection("general")]
    [GMCMPriority(1)]
    [GMCMRange(0, 10)]
    public uint TicksBetweenCrests { get; internal set; } = 4;

    /// <summary>Gets a value indicating whether to allow upgrading tools at the Volcano Forge.</summary>
    [JsonProperty]
    [GMCMSection("general")]
    [GMCMPriority(2)]
    public bool EnableForgeUpgrading { get; internal set; } = true;

    #endregion general

    #region controls & ui

    /// <summary>Gets a value indicating whether determines whether charging requires holding a mod key.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(10)]
    public bool HoldToCharge { get; internal set; } = true;

    /// <summary>Gets the chosen mod key(s) for charging resource tools.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(11)]
    public KeybindList ChargeKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your tools.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(12)]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow auto-selecting tools.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(13)]
    public bool EnableAutoSelection { get; internal set; } = true;

    /// <summary>Gets the chosen key(s) for toggling auto-selection.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(14)]
    public KeybindList SelectionKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the <see cref="Color"/> used to indicate tools enabled or auto-selection.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(15)]
    [GMCMColorPicker(false, (uint)IGenericModConfigMenuOptionsApi.ColorPickerStyle.RGBSliders)]
    [GMCMDefaultColor(0, 255, 255)]
    public Color SelectionBorderColor { get; internal set; } = Color.Aqua;

    /// <summary>Gets a value indicating whether to color the title text of upgraded tools.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(16)]
    public bool ColorCodedForYourConvenience { get; internal set; } = false;

    /// <summary>Gets a value indicating whether determines whether to show affected tiles overlay while charging.</summary>
    [JsonProperty]
    [GMCMSection("controls_ui")]
    [GMCMPriority(17)]
    public bool HideAffectedTiles { get; internal set; } = false;

    #endregion controls & ui

    /// <inheritdoc cref="AxeConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Tools/Axe", "tols.axe", true)]
    public AxeConfig Axe { get; internal set; } = new();

    /// <inheritdoc cref="PickaxeConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Tools/Pickaxe", "tols.pick", true)]
    public PickaxeConfig Pick { get; internal set; } = new();

    /// <inheritdoc cref="HoeConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Tools/Hoe", "tols.hoe", true)]
    public HoeConfig Hoe { get; internal set; } = new();

    /// <inheritdoc cref="WateringCanConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Tools/WateringCan", "tols.can", true)]
    public WateringCanConfig Can { get; internal set; } = new();

    /// <inheritdoc cref="ScytheConfig"/>
    [JsonProperty]
    [GMCMInnerConfig("DaLion.Overhaul.Modules.Tools/Scythe", "tols.scythe", true)]
    public ScytheConfig Scythe { get; internal set; } = new();
}
