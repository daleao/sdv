namespace DaLion.Stardew.Tools.Configs;

#region using directives

using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>General tool settings.</summary>
public class ToolConfig
{
    /// <inheritdoc cref="Configs.AxeConfig"/>
    public AxeConfig AxeConfig { get; set; } = new();

    /// <inheritdoc cref="Configs.PickaxeConfig"/>
    public PickaxeConfig PickaxeConfig { get; set; } = new();

    /// <inheritdoc cref="Configs.HoeConfig"/>
    public HoeConfig HoeConfig { get; set; } = new();

    /// <inheritdoc cref="Configs.WateringCanConfig"/>
    public WateringCanConfig WateringCanConfig { get; set; } = new();

    /// <summary>Whether charging requires a mod key to activate.</summary>
    public bool RequireModkey { get; set; } = true;

    /// <summary>The chosen mod key(s).</summary>
    public KeybindList Modkey { get; set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Whether to show affected tiles overlay while charging.</summary>
    public bool HideAffectedTiles { get; set; } = false;

    /// <summary>How much stamina the shockwave should consume.</summary>
    public float StaminaCostMultiplier { get; set; } = 3.0f;

    /// <summary>Affects the shockwave travel speed. Lower is faster. Set to 0 for instant.</summary>
    public uint TicksBetweenWaves { get; set; } = 4;

    /// <summary>Face the current cursor position before swinging your tools.</summary>
    public bool FaceMouseCursor { get; set; } = true;
}