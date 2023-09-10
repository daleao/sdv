namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDayUpdatePatcher"/> class.</summary>
    internal GameLocationDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.DayUpdate));
    }

    #region harmony patches

    /// <summary>Spawn crows on Island Farm.</summary>
    [HarmonyPostfix]
    private static void GameLocationDayUpdatePostfix(GameLocation __instance)
    {
        if (TweexModule.Config.SpawnCrowsOnTheseMaps.Contains(__instance.NameOrUniqueName))
        {
            GenericAddCrows(__instance);
        }
    }

    #endregion harmony patches

    private static void GenericAddCrows(GameLocation location)
    {
        var numCrops = 0;
        foreach (var pair in location.terrainFeatures.Pairs)
        {
            if (pair.Value is HoeDirt { crop: { } })
            {
                numCrops++;
            }
        }

        var scarecrowPositions = new List<Vector2>();
        foreach (var pair in location.objects.Pairs)
        {
            if (pair.Value.bigCraftable.Value && pair.Value.IsScarecrow())
            {
                scarecrowPositions.Add(pair.Key);
            }
        }

        var potentialCrows = Math.Min(4, numCrops / 16);
        for (var i = 0; i < potentialCrows; i++)
        {
            if (Game1.random.NextDouble() > 0.3)
            {
                continue;
            }

            for (var attempts = 0; attempts < 10; attempts++)
            {
                var tile = location.terrainFeatures.Pairs.ElementAt(Game1.random.Next(location.terrainFeatures.Count())).Key;
                if (location.terrainFeatures[tile] is not HoeDirt { crop: { } crop } dirt || crop.currentPhase.Value <= 1)
                {
                    continue;
                }

                var isNearScarecrow = false;
                for (var j = 0; j < scarecrowPositions.Count; j++)
                {
                    var position = scarecrowPositions[j];
                    var radius = location.objects[position].GetRadiusForScarecrow();
                    if (!(Vector2.Distance(position, tile) < radius))
                    {
                        continue;
                    }

                    isNearScarecrow = true;
                    location.objects[position].SpecialVariable++;
                    break;
                }

                if (!isNearScarecrow)
                {
                    dirt.destroyCrop(tile, showAnimation: false, location);
                    GenericDoSpawnCrow(tile, location);
                }

                break;
            }
        }
    }

    private static void GenericDoSpawnCrow(Vector2 tile, GameLocation location)
    {
        if (location.critters is null && location.IsOutdoors)
        {
            location.critters = new List<Critter>();
        }

        location.critters?.Add(new Crow((int)tile.X, (int)tile.Y));
    }
}
