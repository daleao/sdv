namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

using Extensions;

using Professions = Utility.Professions;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectGetPriceAfterMultipliersPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectGetPriceAfterMultipliersPatch()
    {
        Original = RequireMethod<SObject>("getPriceAfterMultipliers");
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
            foreach (var player in Game1.getAllFarmers())
            {
                if (Game1.player.useSeparateWallets)
                {
                    if (specificPlayerID == -1)
                    {
                        if (player.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID || !player.isActive())
                            continue;
                    }
                    else if (player.UniqueMultiplayerID != specificPlayerID)
                    {
                        continue;
                    }
                }
                else if (!player.isActive())
                {
                    continue;
                }

                var multiplier = 1f;

                // professions
                if (player.HasProfession("Producer") && __instance.IsAnimalProduct())
                    multiplier += Professions.GetProducerPriceBonus(player);
                if (player.HasProfession("Angler") && __instance.IsFish())
                    multiplier += Professions.GetAnglerPriceBonus(player);

                // events
                else if (player.eventsSeen.Contains(2120303) && __instance.IsWildBerry())
                    multiplier *= 3f;
                else if (player.eventsSeen.Contains(3910979) && __instance.IsSpringOnion())
                    multiplier *= 5f;

                // tax bonus
                if (player.IsLocalPlayer && player.HasProfession("Conservationist"))
                    multiplier *= Professions.GetConservationistPriceMultiplier();

                saleMultiplier = Math.Max(saleMultiplier, multiplier);
            }
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }

        __result = startPrice * saleMultiplier;
        return false; // don't run original logic
    }

    #endregion harmony patches
}