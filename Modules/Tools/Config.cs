namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using DaLion.Overhaul.Modules.Tools.Configs;
using DaLion.Overhaul.Modules.Tools.Integrations;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for TOLS.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <inheritdoc cref="AxeConfig"/>
    [JsonProperty]
    public AxeConfig Axe { get; internal set; } = new();

    /// <inheritdoc cref="PickaxeConfig"/>
    [JsonProperty]
    public PickaxeConfig Pick { get; internal set; } = new();

    /// <inheritdoc cref="HoeConfig"/>
    [JsonProperty]
    public HoeConfig Hoe { get; internal set; } = new();

    /// <inheritdoc cref="WateringCanConfig"/>
    [JsonProperty]
    public WateringCanConfig Can { get; internal set; } = new();

    /// <inheritdoc cref="WateringCanConfig"/>
    [JsonProperty]
    public ScytheConfig Scythe { get; internal set; } = new();

    /// <summary>Gets a value indicating whether determines whether charging requires holding a mod key.</summary>
    [JsonProperty]
    public bool HoldToCharge { get; internal set; } = true;

    /// <summary>Gets the chosen mod key(s) for charging resource tools.</summary>
    [JsonProperty]
    public KeybindList ChargeKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether to play the shockwave animation when the charged Axe is released.</summary>
    [JsonProperty]
    public bool PlayShockwaveAnimation { get; internal set; } = true;

    /// <summary>Gets affects the shockwave travel speed. Lower is faster. Set to 0 for instant.</summary>
    [JsonProperty]
    public uint TicksBetweenCrests { get; internal set; } = 4;

    /// <summary>Gets a value indicating whether determines whether to show affected tiles overlay while charging.</summary>
    [JsonProperty]
    public bool HideAffectedTiles { get; internal set; } = false;

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your tools.</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow auto-selecting tools.</summary>
    [JsonProperty]
    public bool EnableAutoSelection { get; internal set; } = true;

    /// <summary>Gets the chosen key(s) for toggling auto-selection.</summary>
    [JsonProperty]
    public KeybindList SelectionKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the <see cref="Color"/> used to indicate tools enabled or auto-selection.</summary>
    [JsonProperty]
    public Color SelectionBorderColor { get; internal set; } = Color.Magenta;

    /// <summary>Gets a value indicating whether to color the title text of upgraded tools.</summary>
    [JsonProperty]
    public bool ColorCodedForYourConvenience { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to allow upgrading tools at the Volcano Forge.</summary>
    [JsonProperty]
    public bool EnableForgeUpgrading { get; internal set; } = true;

    /// <inheritdoc />
    public override bool Validate()
    {
        var isValid = true;
        Log.T("[TOLS]: Verifying tool configs...");

        var maxToolPower = MoonMisadventuresIntegration.Instance?.IsLoaded == true ? 7 : this.EnableForgeUpgrading ? 6 : 5;

        if (this.Axe.RadiusAtEachPowerLevel.Length != maxToolPower)
        {
            var preface = this.Axe.RadiusAtEachPowerLevel.Length < maxToolPower ? "Missing" : "Too many";
            Log.W($"[TOLS]: {preface} values in Axe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Axe.RadiusAtEachPowerLevel = new uint[maxToolPower];
            uint i = 0;
            while (i < maxToolPower)
            {
                this.Axe.RadiusAtEachPowerLevel[i] = ++i;
            }

            isValid = false;
        }

        if (this.Pick.RadiusAtEachPowerLevel.Length != maxToolPower)
        {
            var preface = this.Pick.RadiusAtEachPowerLevel.Length < maxToolPower ? "Missing" : "Too many";
            Log.W($"[TOLS]: {preface} values Pickaxe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Pick.RadiusAtEachPowerLevel = new uint[maxToolPower];
            uint i = 0;
            while (i < maxToolPower)
            {
                this.Pick.RadiusAtEachPowerLevel[i] = ++i;
            }

            isValid = false;
        }

        if (this.Hoe.AffectedTilesAtEachPowerLevel.Length != maxToolPower)
        {
            var preface = this.Hoe.AffectedTilesAtEachPowerLevel.Length < maxToolPower ? "Missing" : "Too many";
            Log.W($"[TOLS]: {preface} values in Hoe.AffectedTilesAtEachPowerLevel. The default values will be restored.");
            this.Hoe.AffectedTilesAtEachPowerLevel = maxToolPower switch
            {
                > 6 => new (uint, uint)[] { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), (8, 3), (9, 4), },
                > 5 => new (uint, uint)[] { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), (8, 3), },
                _ => new (uint, uint)[] { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), },
            };

            isValid = false;
        }

        if (this.Can.AffectedTilesAtEachPowerLevel.Length != maxToolPower)
        {
            var preface = this.Can.AffectedTilesAtEachPowerLevel.Length < maxToolPower ? "Missing" : "Too many";
            Log.W($"[TOLS]: {preface} values in Can.AffectedTilesAtEachPowerLevel. The default values will be restored.");
            this.Can.AffectedTilesAtEachPowerLevel = maxToolPower switch
            {
                > 6 => new (uint, uint)[] { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), (8, 3), (9, 4), },
                > 5 => new (uint, uint)[] { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), (8, 3), },
                _ => new (uint, uint)[] { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), },
            };

            isValid = false;
        }

        if (this.HoldToCharge && !this.ChargeKey.IsBound)
        {
            Log.W(
                "[TOLS]: 'ChargingRequiresModKey' setting is set to true, but no ModKey is bound. Default keybind will be restored. To disable the ModKey, set this value to false.");
            this.ChargeKey = KeybindList.ForSingle(SButton.LeftShift);
            isValid = false;
        }

        if (this.Axe.ChargedStaminaCostMultiplier < 0)
        {
            Log.W("[TOLS]: Axe 'ChargedStaminaMultiplier' is set to an illegal negative value. The value will default to 1");
            this.Axe.ChargedStaminaCostMultiplier = 1f;
            isValid = false;
        }

        if (this.Pick.ChargedStaminaMultiplier < 0)
        {
            Log.W("[TOLS]: Pickaxe 'ChargedStaminaMultiplier' is set to an illegal negative value. The value will default to 1");
            this.Pick.ChargedStaminaMultiplier = 1f;
            isValid = false;
        }

        if (this.TicksBetweenCrests > 100)
        {
            Log.W(
                "[TOLS]: The value of 'TicksBetweenWaves' is excessively large. This is probably a mistake. The default value will be restored.");
            this.TicksBetweenCrests = 4;
            isValid = false;
        }

        return isValid;
    }
}
