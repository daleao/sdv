namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationGetHarvestSpawnedObjectQualityPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationGetHarvestSpawnedObjectQualityPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationGetHarvestSpawnedObjectQualityPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.GetHarvestSpawnedObjectQuality));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to nerf Ecologist forage quality + add quality to foraged minerals for Gemologist + increment respective
    ///     mod data fields.
    /// </summary>
    [HarmonyPrefix]
    private static bool GameLocationGetHarvestSpawnedObjectQualityPrefix(GameLocation __instance, ref int __result, Farmer who, bool isForage, Vector2 tile, Random? random)
    {
        var spawned = __instance.Objects[tile];
        var isForagedMineral = spawned.IsForagedMineral();
        if (who.HasProfession(Profession.Ecologist) && isForage && !isForagedMineral)
        {
            who.ApplyEcologistEdibility(spawned);
            Data.AppendToEcologistItemsForaged(spawned.ItemId);
            __result = who.GetEcologistForageQuality();
            return false; // don't run original logic
        }

        if (who.HasProfession(Profession.Gemologist) && isForagedMineral)
        {
            Data.AppendToGemologistMineralsCollected(spawned.ItemId);
            __result = who.GetGemologistMineralQuality();
            return false; // don't run original logic
        }

        if (!isForage)
        {
            __result = SObject.lowQuality;
            return false; // don't run original logic
        }

        random ??= Utility.CreateDaySaveRandom(tile.X, tile.Y * 777f);

        if (random.NextBool(who.ForagingLevel / 30f))
        {
            __result = SObject.highQuality;
        }

        if (random.NextBool(who.ForagingLevel / 15f))
        {
            __result = SObject.medQuality;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
