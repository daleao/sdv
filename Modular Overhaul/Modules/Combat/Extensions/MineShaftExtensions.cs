namespace DaLion.Overhaul.Modules.Combat.Extensions;

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
            < 40 => Game1.random.Choose(ItemIDs.Amethyst, ItemIDs.Topaz),
            < 80 => Game1.random.Choose(ItemIDs.Aquamarine, ItemIDs.Jade),
            < 120 => Globals.GarnetIndex.HasValue
                ? Game1.random.Choose(ItemIDs.Ruby, ItemIDs.Emerald, Globals.GarnetIndex.Value)
                : Game1.random.Choose(ItemIDs.Ruby, ItemIDs.Emerald),
            _ => Globals.GarnetIndex.HasValue
                ? Game1.random.Choose(
                    ItemIDs.Amethyst,
                    ItemIDs.Topaz,
                    ItemIDs.Aquamarine,
                    ItemIDs.Jade,
                    ItemIDs.Ruby,
                    ItemIDs.Emerald,
                    Globals.GarnetIndex.Value)
                : Game1.random.Choose(
                    ItemIDs.Amethyst,
                    ItemIDs.Topaz,
                    ItemIDs.Aquamarine,
                    ItemIDs.Jade,
                    ItemIDs.Ruby,
                    ItemIDs.Emerald),
        };
    }

    /// <summary>Selects an appropriate geode for the current mine level.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <returns>A geode index.</returns>
    public static int ChooseGeode(this MineShaft shaft)
    {
        return shaft.mineLevel switch
        {
            < 40 => ItemIDs.Geode,
            < 80 => ItemIDs.FrozenGeode,
            < 120 => ItemIDs.MagmaGeode,
            _ => Game1.random.NextDouble() < 0.1
                ? ItemIDs.OmniGeode
                : Game1.random.Choose(ItemIDs.Geode, ItemIDs.FrozenGeode, ItemIDs.MagmaGeode),
        };
    }

    /// <summary>Selects an appropriate ore for the current mine level.</summary>
    /// <param name="shaft">The <see cref="MineShaft"/>.</param>
    /// <returns>An ore index.</returns>
    public static int ChooseForageMineral(this MineShaft shaft)
    {
        return shaft.mineLevel switch
        {
            < 40 => Game1.random.Choose(ItemIDs.Quartz, ItemIDs.EarthCrystal),
            < 80 => Game1.random.Choose(ItemIDs.Quartz, ItemIDs.FrozenTear),
            < 120 => Game1.random.Choose(ItemIDs.Quartz, ItemIDs.FireQuartz),
            _ => ItemIDs.Quartz,
        };
    }
}
