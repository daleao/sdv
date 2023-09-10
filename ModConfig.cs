namespace DaLion.Chargeable;

#region using directives

using System.Linq;
using DaLion.Chargeable.Framework.Configs;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for Tools.</summary>
public sealed class ModConfig
{
    /// <inheritdoc cref="AxeConfig"/>
    public AxeConfig Axe { get; internal set; } = new();

    /// <inheritdoc cref="PickaxeConfig"/>
    public PickaxeConfig Pick { get; internal set; } = new();

    /// <summary>Gets a value indicating whether determines whether charging requires a mod key to activate.</summary>
    public bool RequireModkey { get; internal set; } = true;

    /// <summary>Gets the chosen mod key(s).</summary>
    public KeybindList Modkey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets affects the shockwave travel speed. Lower is faster. Set to 0 for instant.</summary>
    public uint TicksBetweenWaves { get; internal set; } = 4;

    /// <summary>Validate the config settings, replacing invalid values if necessary.</summary>
    internal void Validate(IModHelper helper)
    {
        var isValid = true;

        Log.T("[Tools]: Verifying tool configs...");

        if (this.Axe.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("Missing values in Axe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Axe.RadiusAtEachPowerLevel = new uint[] { 1, 2, 3, 4, 5 };
            isValid = false;
        }

        if (this.Pick.RadiusAtEachPowerLevel.Length < 5)
        {
            Log.W("Missing values Pickaxe.RadiusAtEachPowerLevel. The default values will be restored.");
            this.Pick.RadiusAtEachPowerLevel = new uint[] { 1, 2, 3, 4, 5 };
            isValid = false;
        }

        if (this.RequireModkey && !this.Modkey.IsBound)
        {
            Log.W(
                "'RequireModkey' setting is set to true, but no Modkey is bound. Default keybind will be restored. To disable the Modkey, set this value to false.");
            this.Modkey = KeybindList.ForSingle(SButton.LeftShift);
            isValid = false;
        }

        if (this.Axe.StaminaCostMultiplier < 0)
        {
            Log.W("Axe 'StaminaCostMultiplier' is set to an illegal negative value. The value will default to 0");
            this.Axe.StaminaCostMultiplier = 0;
            isValid = false;
        }

        if (this.Pick.StaminaCostMultiplier < 0)
        {
            Log.W("Pick 'StaminaCostMultiplier' is set to an illegal negative value. The value will default to 0");
            this.Pick.StaminaCostMultiplier = 0;
            isValid = false;
        }

        if (this.TicksBetweenWaves > 100)
        {
            Log.W(
                "The value of 'TicksBetweenWaves' is excessively large. This is probably a mistake. The default value will be restored.");
            this.TicksBetweenWaves = 4;
            isValid = false;
        }

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

        if (!isValid)
        {
            helper.WriteConfig(this);
        }
    }
}
