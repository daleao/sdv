namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Tools.Configs;
using DaLion.Overhaul.Modules.Tools.Integrations;
using HarmonyLib;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for Tools.</summary>
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

    /// <summary>Gets a value indicating whether determines whether charging requires a mod key to activate.</summary>
    [JsonProperty]
    public bool RequireModkey { get; internal set; } = true;

    /// <summary>Gets the chosen mod key(s).</summary>
    [JsonProperty]
    public KeybindList Modkey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets a value indicating whether determines whether to show affected tiles overlay while charging.</summary>
    [JsonProperty]
    public bool HideAffectedTiles { get; internal set; } = false;

    /// <summary>Gets how much stamina the shockwave should consume.</summary>
    [JsonProperty]
    public float StaminaCostMultiplier { get; internal set; } = 1f;

    /// <summary>Gets affects the shockwave travel speed. Lower is faster. Set to 0 for instant.</summary>
    [JsonProperty]
    public uint TicksBetweenWaves { get; internal set; } = 4;

    /// <summary>Gets a value indicating whether face the current cursor position before swinging your tools.</summary>
    [JsonProperty]
    public bool FaceMouseCursor { get; internal set; } = true;

    /// <inheritdoc />
    internal override bool Validate()
    {
        var isValid = true;

        Log.T("Verifying tool configs...");
        if (this.Axe.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("Missing values in Axe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Axe.RadiusAtEachPowerLevel = new uint[] { 1, 2, 3, 4, 5 };
            if (MoonMisadventuresIntegration.IsLoaded)
            {
                this.Axe.RadiusAtEachPowerLevel.AddRangeToArray(new uint[] { 6, 7 });
            }

            isValid = false;
        }

        if (this.Pick.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("Missing values Pickaxe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Pick.RadiusAtEachPowerLevel = new uint[] { 1, 2, 3, 4, 5 };
            if (MoonMisadventuresIntegration.IsLoaded)
            {
                this.Pick.RadiusAtEachPowerLevel.AddRangeToArray(new uint[] { 6, 7 });
            }

            isValid = false;
        }

        if (this.Hoe.AffectedTiles.Length < 5 || this.Hoe.AffectedTiles.Any(row => row.Length != 2))
        {
            Log.W("Incorrect or missing values in Hoe.AffectedTiles. The default values will be restored.");
            this.Hoe.AffectedTiles = new[]
            {
                new uint[] { 3, 0 }, new uint[] { 5, 0 }, new uint[] { 3, 1 }, new uint[] { 6, 1 }, new uint[] { 5, 2 },
            };

            if (MoonMisadventuresIntegration.IsLoaded)
            {
                this.Hoe.AffectedTiles.AddRangeToArray(new[] { new uint[] { 7, 3 }, new uint[] { 9, 4 } });
            }

            isValid = false;
        }

        if (this.Can.AffectedTiles.Length < 5 || this.Can.AffectedTiles.Any(row => row.Length != 2))
        {
            Log.W("Incorrect or missing values in Can.AffectedTiles. The default values will be restored.");
            this.Can.AffectedTiles = new[]
            {
                new uint[] { 3, 0 }, new uint[] { 5, 0 }, new uint[] { 3, 1 }, new uint[] { 6, 1 }, new uint[] { 5, 2 },
            };

            if (MoonMisadventuresIntegration.IsLoaded)
            {
                this.Can.AffectedTiles.AddRangeToArray(new[] { new uint[] { 7, 3 }, new uint[] { 9, 4 } });
            }

            isValid = false;
        }

        if (this.RequireModkey && !this.Modkey.IsBound)
        {
            Log.W(
                "'RequireModkey' setting is set to true, but no Modkey is bound. Default keybind will be restored. To disable the Modkey, set this value to false.");
            this.Modkey = KeybindList.ForSingle(SButton.LeftShift);
            isValid = false;
        }

        if (this.StaminaCostMultiplier < 0)
        {
            Log.W("'StaminaCostMultiplier' is set to an illegal negative value. The value will default to 0");
            this.StaminaCostMultiplier = 0;
            isValid = false;
        }

        if (this.TicksBetweenWaves > 100)
        {
            Log.W(
                "The value of 'TicksBetweenWaves' is excessively large. This is probably a mistake. The default value will be restored.");
            this.TicksBetweenWaves = 4;
            isValid = false;
        }

        if (MoonMisadventuresIntegration.IsLoaded)
        {
            Log.I("Moon Misadventures detected.");

            switch (this.Axe.RadiusAtEachPowerLevel.Length)
            {
                case < 7:
                    Log.I("Adding default radius values for higher Axe upgrades.");
                    this.Axe.RadiusAtEachPowerLevel =
                        this.Axe.RadiusAtEachPowerLevel.AddRangeToArray(new uint[] { 6, 7 });
                    isValid = false;
                    break;

                case > 7:
                    Log.W("Too many values in Axe.RadiusAtEachPowerLevel. Additional values will be removed.");
                    this.Axe.RadiusAtEachPowerLevel = this.Axe.RadiusAtEachPowerLevel.Take(7).ToArray();
                    isValid = false;
                    break;
            }

            switch (this.Pick.RadiusAtEachPowerLevel.Length)
            {
                case < 7:
                    Log.I("Adding default radius values for higher Pickaxe upgrades.");
                    this.Pick.RadiusAtEachPowerLevel =
                        this.Pick.RadiusAtEachPowerLevel.AddRangeToArray(new uint[] { 6, 7 });
                    isValid = false;
                    break;

                case > 7:
                    Log.W("Too many values in Pickaxe.RadiusAtEachPowerLevel. Additional values will be removed.");
                    this.Pick.RadiusAtEachPowerLevel =
                        this.Pick.RadiusAtEachPowerLevel.Take(7).ToArray();
                    isValid = false;
                    break;
            }

            switch (this.Hoe.AffectedTiles.Length)
            {
                case < 7:
                    Log.I("Adding default length and radius values for higher Hoe upgrades.");
                    this.Hoe.AffectedTiles = this.Hoe.AffectedTiles.AddRangeToArray(new[]
                    {
                        new uint[] { 7, 3 }, new uint[] { 9, 4 },
                    });
                    isValid = false;
                    break;

                case > 7:
                    Log.W("Too many values in Hoe.AffectedTiles. Additional values will be removed.");
                    this.Hoe.AffectedTiles =
                        this.Hoe.AffectedTiles.Take(7).ToArray();
                    isValid = false;
                    break;
            }

            switch (this.Can.AffectedTiles.Length)
            {
                case < 7:
                    Log.I("Adding default length and radius values for higher Watering Can upgrades.");
                    this.Can.AffectedTiles = this.Can.AffectedTiles.AddRangeToArray(new[]
                    {
                        new uint[] { 7, 3 }, new uint[] { 9, 4 },
                    });
                    isValid = false;
                    break;

                case > 7:
                    Log.W("Too many values in Can.AffectedTiles. Additional values will be removed.");
                    this.Can.AffectedTiles =
                        this.Can.AffectedTiles.Take(7).ToArray();
                    isValid = false;
                    break;
            }
        }
        else
        {
            if (this.Axe.RadiusAtEachPowerLevel.Length > 5)
            {
                Log.W("Too many values in Axe.RadiusAtEachPowerLevel. Additional values will be removed.");
                this.Axe.RadiusAtEachPowerLevel = this.Axe.RadiusAtEachPowerLevel.Take(5).ToArray();
                isValid = false;
            }

            if (this.Pick.RadiusAtEachPowerLevel.Length > 5)
            {
                Log.W("Too many values in Pickaxe.RadiusAtEachPowerLevel. Additional values will be removed.");
                this.Pick.RadiusAtEachPowerLevel =
                    this.Pick.RadiusAtEachPowerLevel.Take(5).ToArray();
                isValid = false;
            }

            if (this.Hoe.AffectedTiles.Length > 5)
            {
                Log.W("Too many values in Hoe.AffectedTiles. Additional values will be removed.");
                this.Hoe.AffectedTiles =
                    this.Hoe.AffectedTiles.Take(7).ToArray();
                isValid = false;
            }

            if (this.Can.AffectedTiles.Length > 5)
            {
                Log.W("Too many values in Can.AffectedTiles. Additional values will be removed.");
                this.Can.AffectedTiles =
                    this.Can.AffectedTiles.Take(7).ToArray();
                isValid = false;
            }
        }

        return isValid;
    }
}
