namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class BreakableContainerReleaseContentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BreakableContainerReleaseContentsPatcher"/> class.</summary>
    internal BreakableContainerReleaseContentsPatcher()
    {
        this.Target = this.RequireMethod<BreakableContainer>(nameof(BreakableContainer.releaseContents));
    }

    #region harmony patches

    /// <summary>Better weapon odds from mine containers.</summary>
    [HarmonyPrefix]
    private static bool BreakableContainerReleaseContentsPrefix(BreakableContainer __instance, GameLocation location, Farmer who)
    {
        if (!ArsenalModule.Config.Weapons.EnableRebalance || location is not MineShaft shaft)
        {
            return true; // run original logic
        }

        try
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            if (r.NextDouble() < 0.2)
            {
                ArsenalModule.State.ContainerDropAccumulator += 0.005d;
                return false; // don't run original logic
            }

            var x = (int)__instance.TileLocation.X;
            var y = (int)__instance.TileLocation.Y;
            var mineLevel = shaft.mineLevel;
            if (shaft.isContainerPlatform(x, y))
            {
                shaft.updateMineLevelData(0, -1);
            }

            if (r.NextDouble() < ArsenalModule.State.ContainerDropAccumulator)
            {
                ArsenalModule.State.ContainerDropAccumulator = 0.05;
                Game1.createItemDebris(
                    MineShaft.getSpecialItemForThisMineLevel(mineLevel, x, y),
                    (new Vector2(x, y) * Game1.tileSize) + new Vector2(32f, 32f),
                    r.Next(4),
                    location);
                return false; // don't run original logic
            }

            if (r.NextDouble() < 0.05 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            {
                Game1.createMultipleObjectDebris(890, x, y, r.Next(1, 3), who.UniqueMultiplayerID, location);
            }

            if (shaft.GetAdditionalDifficulty() > 0)
            {
                // qi gem
                if (r.NextDouble() < 0.0064)
                {
                    Game1.createMultipleObjectDebris(858, x, y, 1, location);
                }

                // trimmed lucky purple shorts ?
                if (r.NextDouble() < 0.008)
                {
                    Game1.createItemDebris(
                        new SObject(Vector2.Zero, 71),
                        (new Vector2(x, y) * 64f) + new Vector2(32f),
                        0);
                }

                if (r.NextDouble() > 0.85)
                {
                    switch (r.Next(11))
                    {
                        case 1:
                            Game1.createMultipleObjectDebris(SObject.coal, x, y, r.Next(1, 3), location);
                            break;
                        case 2:
                            Game1.createMultipleObjectDebris(SObject.wood, x, y, r.Next(2, 6), location);
                            break;
                        case 3:
                            Game1.createMultipleObjectDebris(SObject.stone, x, y, r.Next(2, 6), location);
                            break;
                        case 4:
                            Game1.createMultipleObjectDebris(Constants.HardwoodIndex, x, y, 1, location); // hardwood
                            break;
                        case 5:
                            Game1.createMultipleObjectDebris(62 + (r.Next(7) * 2), x, y, 1, location); // gemstone
                            break;
                        case 6:
                            Game1.createMultipleObjectDebris(r.Next(535, 538), x, y, 1, location); // geode
                            break;
                        case 7:
                            Game1.createMultipleObjectDebris(80 + (r.Next(3) * 2), x, y, 1, location); // forage mineral
                            break;
                        case 8:
                            Game1.createMultipleObjectDebris(r.Next(218, 245), x, y, 1, location); // food
                            break;
                        case 9:
                            Game1.createMultipleObjectDebris(r.Next(286, 288), x, y, 1, location); // bomb
                            break;
                        case 10:
                            Game1.createMultipleObjectDebris(r.Next(918, 921), x, y, 1, location); // level-3 fertilizer
                            break;
                        default:
                            Game1.createMultipleObjectDebris(
                                r.Choose(SObject.copper, SObject.iron, SObject.gold, SObject.iridium),
                                x,
                                y,
                                r.Next(1, 4),
                                location);
                            break;
                    }

                    ArsenalModule.State.ContainerDropAccumulator += 0.005d;
                    return false; // don't run original logic
                }
            }

            if (r.NextDouble() < 0.65)
            {
                if (r.NextDouble() < 0.8)
                {
                    switch (r.Next(7))
                    {
                        case 0:
                            Game1.createMultipleObjectDebris(SObject.coal, x, y, r.Next(1, 3), location);
                            break;
                        case 1:
                            Game1.createMultipleObjectDebris(SObject.wood, x, y, r.Next(2, 6), location);
                            break;
                        case 2:
                            Game1.createMultipleObjectDebris(SObject.stone, x, y, r.Next(2, 6), location);
                            break;
                        case 3: // ore
                            Game1.createMultipleObjectDebris(
                                mineLevel < 40
                                    ? SObject.copper
                                    : mineLevel < 80
                                        ? SObject.iron
                                        : mineLevel < 120
                                            ? SObject.gold
                                            : SObject.iridium,
                                x,
                                y,
                                r.Next(1, 4),
                                location);
                            break;
                        case 4: // ore - 1
                            Game1.createMultipleObjectDebris(
                                mineLevel < 40
                                    ? SObject.stone
                                    : mineLevel < 80
                                        ? SObject.copper
                                        : mineLevel < 120
                                            ? SObject.iron
                                            : SObject.gold,
                                x,
                                y,
                                r.Next(2, 6),
                                location);
                            break;
                        case 5: // ore - 2
                            Game1.createMultipleObjectDebris(
                                mineLevel < 40
                                    ? SObject.wood
                                    : mineLevel < 80
                                        ? SObject.stone
                                        : mineLevel < 120
                                            ? SObject.copper
                                            : SObject.iron,
                                x,
                                y,
                                r.Next(2, 6),
                                location);
                            break;
                        case 6: // etc.
                            Game1.createMultipleObjectDebris(
                                mineLevel > 120 ? Constants.BoneFragment : Constants.MixedSeeds, x, y, 1, location);
                            break;
                    }
                }
                else
                {
                    Game1.createMultipleObjectDebris(Constants.CaveCarrotIndex, x, y, r.Next(1, 3), location);
                }
            }
            else if (r.NextDouble() < 0.4)
            {
                switch (r.Next(5))
                {
                    case 0:
                        Game1.createMultipleObjectDebris(shaft.ChooseGem(), x, y, 1, location);
                        break;
                    case 1:
                        Game1.createMultipleObjectDebris(shaft.ChooseGeode(), x, y, 1, location);
                        break;
                    case 2:
                        Game1.createMultipleObjectDebris(shaft.ChooseForageMineral(), x, y, 1, location);
                        break;
                    case 3:
                        Game1.createMultipleObjectDebris(Constants.HardwoodIndex, x, y, 1, location);
                        break;
                    case 4: // fertilizer or sap
                        var what = who.timesReachedMineBottom > 0 && r.NextDouble() < 0.5
                            ? mineLevel < 80
                                ? r.Choose(
                                    Constants.BasicFertilizerIndex,
                                    Constants.BasicRetainingSoilIndex,
                                    Constants.SpeedGroIndex)
                                : r.Choose(
                                    Constants.QualityFertilizerIndex,
                                    Constants.BasicRetainingSoilIndex,
                                    Constants.SpeedGroIndex)
                            : Constants.SapIndex;
                        Game1.createMultipleObjectDebris(
                            what,
                            x,
                            y,
                            what == Constants.SapIndex ? r.Next(2, 4) : 1,
                            location);
                        break;
                }
            }

            ArsenalModule.State.ContainerDropAccumulator += 0.01d;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
