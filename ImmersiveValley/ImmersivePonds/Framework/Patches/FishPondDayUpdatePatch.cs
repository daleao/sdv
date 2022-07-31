namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Common.Extensions.Collections;
using Common.Extensions.Reflection;
using Common.Extensions.Stardew;
using Common.Harmony;
using Extensions;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDayUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondDayUpdatePatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.dayUpdate));
    }

    #region harmony patches

    /// <summary>Rest held items each morning.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool FishPondDayUpdatePrefix(FishPond __instance, int dayOfMonth)
    {
        if (__instance.HasRadioactiveFish())
        {
            var heldMetals =
                __instance.Read("MetalsHeld")
                    .ParseList<string>(";")?
                    .Select(li => li?.ParseTuple<int, int>())
                    .WhereNotNull()
                    .ToList() ?? new List<(int, int)>();
            for (var i = 0; i < heldMetals.Count; ++i)
            {
                var (metal, daysLeft) = heldMetals[i];
                heldMetals[i] = (metal, --daysLeft);
            }

            __instance.Write("MetalsHeld",
                string.Join(';', heldMetals.Select(m => string.Join(',', m.Item1, m.Item2))));
        }

#if RELEASE
        return true; // run original logic
#elif DEBUG
        // Replacement to help debugging.

        if (__instance.isUnderConstruction()) return true;

        __instance.hasSpawnedFish.Value = false;
        ModEntry.ModHelper.Reflection.GetField<bool>(__instance, "_hasAnimatedSpawnedFish").SetValue(false);
        if (__instance.hasCompletedRequest.Value)
        {
            __instance.neededItem.Value = null;
            __instance.neededItemCount.Set(-1);
            __instance.hasCompletedRequest.Value = false;
        }

        var fishPondData = __instance.GetFishPondData();
        if (fishPondData is null)
        {
            Log.W(
                $"Invalid Fish Pond at {__instance.GetCenterTile()}.\nThe object {__instance.GetFishObject().Name} does not have an associated entry in the FishPondData dictionary. Please clear this pond and replace the object with a valid fish.");
            return false;
        }

        if (__instance.currentOccupants.Value > 0)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            //if (r.NextDouble() < Utility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f)) -> removed by transpiler 
            __instance.output.Value = __instance.GetFishProduce(r);

            __instance.daysSinceSpawn.Value += 1;
            if (__instance.daysSinceSpawn.Value > fishPondData.SpawnTime)
                __instance.daysSinceSpawn.Value = fishPondData.SpawnTime;

            if (__instance.daysSinceSpawn.Value >= fishPondData.SpawnTime)
            {
                var (key, value) = ModEntry.ModHelper.Reflection.GetMethod(__instance, "_GetNeededItemData")
                    .Invoke<KeyValuePair<int, int>>();
                if (key != -1)
                {
                    if (__instance.currentOccupants.Value >= __instance.maxOccupants.Value && __instance.neededItem.Value == null)
                    {
                        __instance.neededItem.Value = new(key, 1);
                        __instance.neededItemCount.Set(value);
                    }
                }
                else
                {
                    __instance.SpawnFish();
                }
            }

            if (__instance.currentOccupants.Value == 10 && __instance.fishType.Value == 717)
                foreach (var farmer in Game1.getAllFarmers())
                    if (!farmer.mailReceived.Contains("FullCrabPond"))
                    {
                        farmer.mailReceived.Add("FullCrabPond");
                        farmer.activeDialogueEvents.Add("FullCrabPond", 14);
                    }

            ModEntry.ModHelper.Reflection.GetMethod(__instance, "doFishSpecificWaterColoring").Invoke();
        }

        BuildingDayUpdatePatch.BuildingDayUpdateReverse(__instance, dayOfMonth);
        return false; // replaces original logic
#endif
    }

    /// <summary>Spontaneously grow algae + calculate roe production.</summary>
    [HarmonyPostfix]
    private static void FishPondDayUpdatePostfix(FishPond __instance, ref FishPondData? ____fishPondData)
    {
        if (__instance.currentOccupants.Value != 0) return;

        var r = new Random(Guid.NewGuid().GetHashCode());

        // if pond is empty, spontaneously grow algae/seaweed
        __instance.Increment("DaysEmpty");
        if (__instance.Read<int>("DaysEmpty") < ModEntry.Config.DaysUntilAlgaeSpawn + 1) return;

        var spawned = Utils.ChooseAlgae(r: r);
        __instance.fishType.Value = spawned;
        ____fishPondData = null;
        __instance.UpdateMaximumOccupancy();
        ++__instance.currentOccupants.Value;

        switch (spawned)
        {
            case Constants.SEAWEED_INDEX_I:
                __instance.Increment("SeaweedLivingHere");
                break;
            case Constants.GREEN_ALGAE_INDEX_I:
                __instance.Increment("GreenAlgaeLivingHere");
                break;
            case Constants.WHITE_ALGAE_INDEX_I:
                __instance.Increment("WhiteAlgaeLivingHere");
                break;
        }

        __instance.Write("DaysEmpty", null);
    }

    /// <summary>Removes population-based roll from <see cref="FishPond.dayUpdate"/> (moved to <see cref="FishPond.GetFishProduce"/>).</summary>
    private static IEnumerable<CodeInstruction>? FishPondDayUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Random).RequireMethod(nameof(Random.NextDouble)))
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Bge_Un_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).RequireField(nameof(FishPond.daysSinceSpawn)))
                )
                .RemoveLabels();
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing day update production roll.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}