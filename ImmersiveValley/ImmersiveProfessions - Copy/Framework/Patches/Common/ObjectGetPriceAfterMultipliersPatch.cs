namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using DaLion.Common;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Reflection;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectGetPriceAfterMultipliersPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectGetPriceAfterMultipliersPatch()
    {
        Target = RequireMethod<SObject>("getPriceAfterMultipliers");
    }

    #region harmony patches

    /// <summary>Patch to modify price multipliers for various modded professions.</summary>
    // ReSharper disable once RedundantAssignment
    [HarmonyPrefix]
    private static bool ObjectGetPriceAfterMultipliersPrefix(SObject __instance, ref float __result,
        float startPrice, long specificPlayerID)
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
                            continue;
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
                if (farmer.HasProfession(Profession.Producer) && __instance.IsAnimalProduct())
                    multiplier += farmer.GetProducerPriceBonus();
                if (farmer.HasProfession(Profession.Angler) && __instance.IsFish())
                    multiplier += farmer.GetAnglerPriceBonus();

                // events
                else if (farmer.eventsSeen.Contains(2120303) && __instance.IsWildBerry())
                    multiplier *= 3f;
                else if (farmer.eventsSeen.Contains(3910979) && __instance.IsSpringOnion())
                    multiplier *= 5f;

                // tax bonus
                if (farmer.IsLocalPlayer && farmer.HasProfession(Profession.Conservationist) &&
                    ModEntry.TaxesConfig is null)
                    multiplier *= farmer.GetConservationistPriceMultiplier();

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