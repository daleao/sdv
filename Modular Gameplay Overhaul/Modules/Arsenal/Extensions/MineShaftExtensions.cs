namespace DaLion.Overhaul.Modules.Arsenal.Extensions;

#region using directives

using DaLion.Shared.Extensions;
using StardewValley.Locations;

#endregion using directives

/// <summary>Extensions for the <see cref="MineShaft"/> class.</summary>
public static class MineShaftExtensions
{
    /// <summary>Selects an appropriate ore for the current mine level.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <returns>An ore index.</returns>
    public static int ChooseOre(this MineShaft shaft)
    {
        return shaft.mineLevel switch
        {
            < 40 => SObject.copper,
            < 80 => SObject.iron,
            < 120 => SObject.gold,
            77377 => Game1.random.Choose(SObject.copper, SObject.iron),
            _ => SObject.iridium,
        };
    }

    /// <summary>Selects an appropriate gemstone for the current mine level.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <returns>A gemstone index.</returns>
    public static int ChooseGem(this MineShaft shaft)
    {
        return shaft.mineLevel switch
        {
            < 40 => Game1.random.Choose(Constants.AmethystIndex, Constants.TopazIndex),
            < 80 => Game1.random.Choose(Constants.AquamarineIndex, Constants.JadeIndex),
            < 120 => Globals.GarnetIndex.HasValue
                ? Game1.random.Choose(Constants.RubyIndex, Constants.EmeraldIndex, Globals.GarnetIndex.Value)
                : Game1.random.Choose(Constants.RubyIndex, Constants.EmeraldIndex),
            _ => Globals.GarnetIndex.HasValue
                ? Game1.random.Choose(
                    Constants.AmethystIndex,
                    Constants.TopazIndex,
                    Constants.AquamarineIndex,
                    Constants.JadeIndex,
                    Constants.RubyIndex,
                    Constants.EmeraldIndex,
                    Globals.GarnetIndex.Value)
                : Game1.random.Choose(
                    Constants.AmethystIndex,
                    Constants.TopazIndex,
                    Constants.AquamarineIndex,
                    Constants.JadeIndex,
                    Constants.RubyIndex,
                    Constants.EmeraldIndex),
        };
    }

    /// <summary>Selects an appropriate geode for the current mine level.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <returns>A geode index.</returns>
    public static int ChooseGeode(this MineShaft shaft)
    {
        return shaft.mineLevel switch
        {
            < 40 => Constants.GeodeIndex,
            < 80 => Constants.FrozenGeodeIndex,
            < 120 => Constants.MagmaGeodeIndex,
            _ => Game1.random.NextDouble() < 0.1
                ? Constants.OmniGeodeIndex
                : Game1.random.Choose(Constants.GeodeIndex, Constants.FrozenGeodeIndex, Constants.MagmaGeodeIndex),
        };
    }

    /// <summary>Selects an appropriate ore for the current mine level.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <returns>An ore index.</returns>
    public static int ChooseForageMineral(this MineShaft shaft)
    {
        return shaft.mineLevel switch
        {
            < 40 => Game1.random.Choose(Constants.QuartzIndex, Constants.EarthCrystalIndex),
            < 80 => Game1.random.Choose(Constants.QuartzIndex, Constants.FrozenTearIndex),
            < 120 => Game1.random.Choose(Constants.QuartzIndex, Constants.FireQuartzIndex),
            _ => Constants.QuartzIndex,
        };
    }
}
