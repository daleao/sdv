namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Constants;
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
            < 40 => ObjectIds.CopperOre,
            < 80 => ObjectIds.IronOre,
            < 120 => ObjectIds.GoldOre,
            77377 => Game1.random.Choose(ObjectIds.CopperOre, ObjectIds.IronOre),
            _ => ObjectIds.IridiumOre,
        };
    }

    /// <summary>Selects an appropriate gemstone for the current mine level.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <returns>A gemstone index.</returns>
    public static int ChooseGem(this MineShaft shaft)
    {
        return shaft.mineLevel switch
        {
            < 40 => Game1.random.Choose(ObjectIds.Amethyst, ObjectIds.Topaz),
            < 80 => Game1.random.Choose(ObjectIds.Aquamarine, ObjectIds.Jade),
            < 120 => JsonAssetsIntegration.GarnetIndex.HasValue
                ? Game1.random.Choose(ObjectIds.Ruby, ObjectIds.Emerald, JsonAssetsIntegration.GarnetIndex.Value)
                : Game1.random.Choose(ObjectIds.Ruby, ObjectIds.Emerald),
            _ => JsonAssetsIntegration.GarnetIndex.HasValue
                ? Game1.random.Choose(
                    ObjectIds.Amethyst,
                    ObjectIds.Topaz,
                    ObjectIds.Aquamarine,
                    ObjectIds.Jade,
                    ObjectIds.Ruby,
                    ObjectIds.Emerald,
                    JsonAssetsIntegration.GarnetIndex.Value)
                : Game1.random.Choose(
                    ObjectIds.Amethyst,
                    ObjectIds.Topaz,
                    ObjectIds.Aquamarine,
                    ObjectIds.Jade,
                    ObjectIds.Ruby,
                    ObjectIds.Emerald),
        };
    }

    /// <summary>Selects an appropriate geode for the current mine level.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <returns>A geode index.</returns>
    public static int ChooseGeode(this MineShaft shaft)
    {
        return shaft.mineLevel switch
        {
            < 40 => ObjectIds.Geode,
            < 80 => ObjectIds.FrozenGeode,
            < 120 => ObjectIds.MagmaGeode,
            _ => Game1.random.NextDouble() < 0.1
                ? ObjectIds.OmniGeode
                : Game1.random.Choose(ObjectIds.Geode, ObjectIds.FrozenGeode, ObjectIds.MagmaGeode),
        };
    }

    /// <summary>Selects an appropriate ore for the current mine level.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <returns>An ore index.</returns>
    public static int ChooseForageMineral(this MineShaft shaft)
    {
        return shaft.mineLevel switch
        {
            < 40 => Game1.random.Choose(ObjectIds.Quartz, ObjectIds.EarthCrystal),
            < 80 => Game1.random.Choose(ObjectIds.Quartz, ObjectIds.FrozenTear),
            < 120 => Game1.random.Choose(ObjectIds.Quartz, ObjectIds.FireQuartz),
            _ => ObjectIds.Quartz,
        };
    }
}
