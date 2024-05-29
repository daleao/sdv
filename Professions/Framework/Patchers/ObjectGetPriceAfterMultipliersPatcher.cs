namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using static StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectGetPriceAfterMultipliersPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectGetPriceAfterMultipliersPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectGetPriceAfterMultipliersPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>("getPriceAfterMultipliers");
    }

    #region harmony patches

    /// <summary>Patch to modify price multipliers for various modded professions.</summary>
    [HarmonyPrefix]
    private static bool ObjectGetPriceAfterMultipliersPrefix(
        SObject __instance, ref float __result, float startPrice, long specificPlayerID)
    {
        var saleMultiplier = 1f;
        try
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                if (Game1.player.useSeparateWallets)
                {
                    if (specificPlayerID == -1)
                    {
                        if (farmer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID || !farmer.isActive())
                        {
                            continue;
                        }
                    }
                    else if (farmer.UniqueMultiplayerID != specificPlayerID)
                    {
                        continue;
                    }
                }
                else if (!farmer.isActive())
                {
                    continue;
                }

                var multiplier = 1f;

                // professions
                if (farmer.HasProfession(Profession.Producer) &&
                    (__instance.IsAnimalOrDerivedGood() || (__instance.QualifiedItemId == QualifiedObjectIds.Honey &&
                                                            Config.BeesAreAnimals)))
                {
                    multiplier += farmer.GetProducerSaleBonus();
                }

                if (farmer.HasProfession(Profession.Angler) && (__instance.IsFish() ||
                                                                __instance.preserve?.Value == PreserveType.SmokedFish))
                {
                    multiplier += farmer.GetAnglerSaleBonus();
                }

                if (!ModHelper.ModRegistry.IsLoaded("DaLion.Taxes") &&
                    farmer.HasProfession(Profession.Conservationist))
                {
                    multiplier += Data.ReadAs<float>(farmer, DataKeys.ConservationistActiveTaxDeduction);
                }

                // events
                if (farmer.eventsSeen.Contains("2120303") && __instance.IsWildBerry())
                {
                    multiplier *= 3f;
                }
                else if (farmer.eventsSeen.Contains("3910979") &&
                         __instance.QualifiedItemId is QualifiedObjectIds.SpringOnion)
                {
                    multiplier *= 5f;
                }

                // books
                if (farmer.stats.Get("Book_Artifact") != 0 && __instance.Type is "Arch")
                {
                    multiplier *= 3f;
                }

                saleMultiplier = Math.Max(saleMultiplier, multiplier);
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }

        __result = startPrice * saleMultiplier;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
