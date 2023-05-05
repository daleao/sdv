namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Tools.Configs;
using DaLion.Overhaul.Modules.Tools.Integrations;
using HarmonyLib;
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

    /// <summary>Gets the chosen mod key(s).</summary>
    [JsonProperty]
    public KeybindList ModKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether determines whether charging requires holding a mod key.</summary>
    [JsonProperty]
    public bool HoldToCharge { get; internal set; } = true;

    /// <summary>Gets a value indicating whether determines whether to show affected tiles overlay while charging.</summary>
    [JsonProperty]
    public bool HideAffectedTiles { get; internal set; } = false;

    /// <summary>Gets affects the shockwave travel speed. Lower is faster. Set to 0 for instant.</summary>
    [JsonProperty]
    public uint TicksBetweenWaves { get; internal set; } = 4;

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your tools.</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow auto-selecting tools.</summary>
    [JsonProperty]
    public bool EnableAutoSelection { get; internal set; } = true;

    /// <summary>Gets the <see cref="Color"/> used to indicate tools enabled or auto-selection.</summary>
    [JsonProperty]
    public Color SelectionBorderColor { get; internal set; } = Color.Magenta;

    /// <summary>Gets a value indicating whether to color the title text of upgraded tools.</summary>
    [JsonProperty]
    public bool ColorCodedForYourConvenience { get; internal set; }

    /// <inheritdoc />
    public override bool Validate()
    {
        var isValid = true;

        Log.T("[TOLS]: Verifying tool configs...");

        var isMoonMisadventuresLoaded = MoonMisadventuresIntegration.Instance?.IsLoaded == true;
        if (this.Axe.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("[TOLS]: Missing values in Axe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Axe.RadiusAtEachPowerLevel = new uint[] { 1, 2, 3, 4, 5 };
            if (isMoonMisadventuresLoaded)
            {
                this.Axe.RadiusAtEachPowerLevel.AddRangeToArray(new uint[] { 6, 7 });
            }

            isValid = false;
        }

        if (this.Pick.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("[TOLS]: Missing values Pickaxe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Pick.RadiusAtEachPowerLevel = new uint[] { 1, 2, 3, 4, 5 };
            if (isMoonMisadventuresLoaded)
            {
                this.Pick.RadiusAtEachPowerLevel.AddRangeToArray(new uint[] { 6, 7 });
            }

            isValid = false;
        }

        if (this.Hoe.AffectedTilesAtEachPowerLevel.Length < 5)
        {
            Log.W("[TOLS]: Missing values in Hoe.AffectedTilesAtEachPowerLevel. The default values will be restored.");
            this.Hoe.AffectedTilesAtEachPowerLevel = new (uint, uint)[]
            {
                (3, 0), (5, 0), (3, 1), (6, 1), (5, 2),
            };

            if (isMoonMisadventuresLoaded)
            {
                this.Hoe.AffectedTilesAtEachPowerLevel.AddRangeToArray(new (uint, uint)[] { (7, 3), (9, 4) });
            }

            isValid = false;
        }

        if (this.Can.AffectedTilesAtEachPowerLevel.Length < 5)
        {
            Log.W("[TOLS]: Missing values in Can.AffectedTilesAtEachPowerLevel. The default values will be restored.");
            this.Can.AffectedTilesAtEachPowerLevel = new (uint, uint)[]
            {
                (3, 0), (5, 0), (3, 1), (6, 1), (5, 2),
            };

            if (isMoonMisadventuresLoaded)
            {
                this.Can.AffectedTilesAtEachPowerLevel.AddRangeToArray(new (uint, uint)[] { (7, 3), (9, 4) });
            }

            isValid = false;
        }

        if (this.HoldToCharge && !this.ModKey.IsBound)
        {
            Log.W(
                "[TOLS]: 'ChargingRequiresModKey' setting is set to true, but no ModKey is bound. Default keybind will be restored. To disable the ModKey, set this value to false.");
            this.ModKey = KeybindList.ForSingle(SButton.LeftShift);
            isValid = false;
        }

        if (this.Axe.ChargedStaminaMultiplier < 0)
        {
            Log.W("[TOLS]: Axe 'ChargedStaminaMultiplier' is set to an illegal negative value. The value will default to 1");
            this.Axe.ChargedStaminaMultiplier = 1f;
            isValid = false;
        }

        if (this.Pick.ChargedStaminaMultiplier < 0)
        {
            Log.W("[TOLS]: Pickaxe 'ChargedStaminaMultiplier' is set to an illegal negative value. The value will default to 1");
            this.Pick.ChargedStaminaMultiplier = 1f;
            isValid = false;
        }

        if (this.TicksBetweenWaves > 100)
        {
            Log.W(
                "[TOLS]: The value of 'TicksBetweenWaves' is excessively large. This is probably a mistake. The default value will be restored.");
            this.TicksBetweenWaves = 4;
            isValid = false;
        }

        if (isMoonMisadventuresLoaded)
        {
            Log.I("[TOLS]: Moon Misadventures detected.");

            switch (this.Axe.RadiusAtEachPowerLevel.Length)
            {
                case < 7:
                    Log.I("[TOLS]: Adding default radius values for higher Axe upgrades.");
                    this.Axe.RadiusAtEachPowerLevel =
                        this.Axe.RadiusAtEachPowerLevel.AddRangeToArray(new uint[] { 6, 7 });
                    isValid = false;
                    break;

                case > 7:
                    Log.W("[TOLS]: Too many values in Axe.RadiusAtEachPowerLevel. Additional values will be removed.");
                    this.Axe.RadiusAtEachPowerLevel = this.Axe.RadiusAtEachPowerLevel.Take(7).ToArray();
                    isValid = false;
                    break;
            }

            switch (this.Pick.RadiusAtEachPowerLevel.Length)
            {
                case < 7:
                    Log.I("[TOLS]: Adding default radius values for higher Pickaxe upgrades.");
                    this.Pick.RadiusAtEachPowerLevel =
                        this.Pick.RadiusAtEachPowerLevel.AddRangeToArray(new uint[] { 6, 7 });
                    isValid = false;
                    break;

                case > 7:
                    Log.W("[TOLS]: Too many values in Pickaxe.RadiusAtEachPowerLevel. Additional values will be removed.");
                    this.Pick.RadiusAtEachPowerLevel =
                        this.Pick.RadiusAtEachPowerLevel.Take(7).ToArray();
                    isValid = false;
                    break;
            }

            switch (this.Hoe.AffectedTilesAtEachPowerLevel.Length)
            {
                case < 7:
                    Log.I("[TOLS]: Adding default length and radius values for higher Hoe upgrades.");
                    this.Hoe.AffectedTilesAtEachPowerLevel =
                        this.Hoe.AffectedTilesAtEachPowerLevel.AddRangeToArray(new (uint, uint)[] { (7, 3), (9, 4), });
                    isValid = false;
                    break;

                case > 7:
                    Log.W("[TOLS]: Too many values in Hoe.AffectedTilesAtEachPowerLevel. Additional values will be removed.");
                    this.Hoe.AffectedTilesAtEachPowerLevel =
                        this.Hoe.AffectedTilesAtEachPowerLevel.Take(7).ToArray();
                    isValid = false;
                    break;
            }

            switch (this.Can.AffectedTilesAtEachPowerLevel.Length)
            {
                case < 7:
                    Log.I("[TOLS]: Adding default length and radius values for higher Watering Can upgrades.");
                    this.Can.AffectedTilesAtEachPowerLevel =
                        this.Can.AffectedTilesAtEachPowerLevel.AddRangeToArray(new (uint, uint)[] { (7, 3), (9, 4), });
                    isValid = false;
                    break;

                case > 7:
                    Log.W("[TOLS]: Too many values in Can.AffectedTilesAtEachPowerLevel. Additional values will be removed.");
                    this.Can.AffectedTilesAtEachPowerLevel =
                        this.Can.AffectedTilesAtEachPowerLevel.Take(7).ToArray();
                    isValid = false;
                    break;
            }
        }
        else
        {
            if (this.Axe.RadiusAtEachPowerLevel.Length > 5)
            {
                Log.W("[TOLS]: Too many values in Axe.RadiusAtEachPowerLevel. Additional values will be removed.");
                this.Axe.RadiusAtEachPowerLevel = this.Axe.RadiusAtEachPowerLevel.Take(5).ToArray();
                isValid = false;
            }

            if (this.Pick.RadiusAtEachPowerLevel.Length > 5)
            {
                Log.W("[TOLS]: Too many values in Pickaxe.RadiusAtEachPowerLevel. Additional values will be removed.");
                this.Pick.RadiusAtEachPowerLevel =
                    this.Pick.RadiusAtEachPowerLevel.Take(5).ToArray();
                isValid = false;
            }

            if (this.Hoe.AffectedTilesAtEachPowerLevel.Length > 5)
            {
                Log.W("[TOLS]: Too many values in Hoe.AffectedTilesAtEachPowerLevel. Additional values will be removed.");
                this.Hoe.AffectedTilesAtEachPowerLevel =
                    this.Hoe.AffectedTilesAtEachPowerLevel.Take(7).ToArray();
                isValid = false;
            }

            if (this.Can.AffectedTilesAtEachPowerLevel.Length > 5)
            {
                Log.W("[TOLS]: Too many values in Can.AffectedTilesAtEachPowerLevel. Additional values will be removed.");
                this.Can.AffectedTilesAtEachPowerLevel =
                    this.Can.AffectedTilesAtEachPowerLevel.Take(7).ToArray();
                isValid = false;
            }
        }

        return isValid;
    }
}
