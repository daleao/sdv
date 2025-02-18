namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System;
using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationGetHarvestSpawnedObjectQualityPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationGetHarvestSpawnedObjectQualityPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationGetHarvestSpawnedObjectQualityPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.GetHarvestSpawnedObjectQuality));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to nerf Ecologist forage quality + add quality to foraged minerals for Gemologist + increment respective
    ///     mod data fields.
    /// </summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool GameLocationGetHarvestSpawnedObjectQualityPrefix(
        GameLocation __instance, ref int __result, Farmer who, bool isForage, Vector2 tile, Random? random)
    {
        try
        {
            if (!__instance.Objects.TryGetValue(tile, out var spawned))
            {
                __result = 0;
                return false; // don't run original logic
            }

            if (who.HasProfession(Profession.Ecologist) && isForage)
            {
                who.ApplyEcologistEdibility(spawned);
                Data.AppendToEcologistItemsForaged(spawned.ItemId);
                __result = who.GetEcologistForageQuality();
                return false; // don't run original logic
            }

            if (who.HasProfession(Profession.Gemologist) && !isForage)
            {
                Data.AppendToGemologistMineralsCollected(spawned.ItemId);
                __result = who.GetGemologistMineralQuality();
                return false; // don't run original logic
            }

            return true; // run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
