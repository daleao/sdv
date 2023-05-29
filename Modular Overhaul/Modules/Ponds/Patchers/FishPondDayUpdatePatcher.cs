namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Ponds.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondDayUpdatePatcher"/> class.</summary>
    internal FishPondDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.dayUpdate));
    }

    #region harmony patches

    /// <summary>Reset held items each morning.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool FishPondDayUpdatePrefix(FishPond __instance, int dayOfMonth)
    {
        if (__instance.IsRadioactive())
        {
            var heldMetals =
                __instance.Read(DataKeys.MetalsHeld)
                    .ParseList<string>(";")
                    .Select(li => li?.ParseTuple<int, int>())
                    .WhereNotNull()
                    .ToList();
            for (var i = 0; i < heldMetals.Count; i++)
            {
                var (metal, daysLeft) = heldMetals[i];
                heldMetals[i] = (metal, --daysLeft);
            }

            __instance.Write(
                DataKeys.MetalsHeld,
                string.Join(';', heldMetals.Select(m => string.Join(',', m.Item1, m.Item2))));
        }

#if RELEASE
        return true; // run original logic
#elif DEBUG

        // **
        // * Replacement to help debugging.
        // **
        if (__instance.isUnderConstruction())
        {
            return true;
        }

        __instance.hasSpawnedFish.Value = false;
        ModHelper.Reflection.GetField<bool>(__instance, "_hasAnimatedSpawnedFish").SetValue(false);
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
                $"[PNDS]: Invalid Fish Pond at {__instance.GetCenterTile()}.\nThe object {__instance.GetFishObject().Name} does not have an associated entry in the FishPondData dictionary. Please clear this pond and replace the object with a valid fish.");
            return false;
        }

        if (__instance.currentOccupants.Value > 0)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());

            // if (r.NextDouble() < Utility.Lerp(0.15f, 0.95f, __instance.currentOccupants.Value / 10f)) -> removed by transpiler
            __instance.output.Value = __instance.GetFishProduce(r);

            __instance.daysSinceSpawn.Value += 1;
            if (__instance.daysSinceSpawn.Value > fishPondData.SpawnTime)
            {
                __instance.daysSinceSpawn.Value = fishPondData.SpawnTime;
            }

            if (__instance.daysSinceSpawn.Value >= fishPondData.SpawnTime)
            {
                var (key, value) = ModHelper.Reflection.GetMethod(__instance, "_GetNeededItemData")
                    .Invoke<KeyValuePair<int, int>>();
                if (key != -1)
                {
                    if (__instance.currentOccupants.Value >= __instance.maxOccupants.Value &&
                        __instance.neededItem.Value == null)
                    {
                        __instance.neededItem.Value = new SObject(key, 1);
                        __instance.neededItemCount.Set(value);
                    }
                }
                else
                {
                    __instance.SpawnFish();
                }
            }

            if (__instance.currentOccupants.Value == 10 && __instance.fishType.Value == 717)
            {
                foreach (var farmer in Game1.getAllFarmers())
                {
                    if (!farmer.mailReceived.Contains("FullCrabPond"))
                    {
                        farmer.mailReceived.Add("FullCrabPond");
                        farmer.activeDialogueEvents.Add("FullCrabPond", 14);
                    }
                }
            }

            ModHelper.Reflection.GetMethod(__instance, "doFishSpecificWaterColoring").Invoke();
        }

        BuildingDayUpdatePatcher.BuildingDayUpdateReverse(__instance, dayOfMonth);
        return false; // replaces original logic
#endif
    }

    /// <summary>Spontaneously grow algae.</summary>
    [HarmonyPostfix]
    private static void FishPondDayUpdatePostfix(FishPond __instance, ref FishPondData? ____fishPondData)
    {
        if (__instance.currentOccupants.Value != 0)
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());

        // if pond is empty, spontaneously grow algae/seaweed
        __instance.Increment(DataKeys.DaysEmpty);
        if (__instance.Read<uint>(DataKeys.DaysEmpty) < PondsModule.Config.DaysUntilAlgaeSpawn + 1)
        {
            return;
        }

        var spawned = r.NextAlgae();
        __instance.fishType.Value = spawned;
        ____fishPondData = null;
        __instance.UpdateMaximumOccupancy();
        __instance.currentOccupants.Value++;
        switch (spawned)
        {
            case ItemIDs.Seaweed:
                __instance.Increment(DataKeys.SeaweedLivingHere);
                break;
            case ItemIDs.GreenAlgae:
                __instance.Increment(DataKeys.GreenAlgaeLivingHere);
                break;
            case ItemIDs.WhiteAlgae:
                __instance.Increment(DataKeys.WhiteAlgaeLivingHere);
                break;
        }

        __instance.Write(DataKeys.DaysEmpty, null);
    }

    /// <summary>
    ///     Removes population-based roll from <see cref="FishPond.dayUpdate"/> (moved to
    ///     <see cref="FishPond.GetFishProduce"/>).
    /// </summary>
    private static IEnumerable<CodeInstruction>? FishPondDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Random).RequireMethod(nameof(Random.NextDouble))),
                    })
                .Count(new[] { new CodeInstruction(OpCodes.Bge_Un_S) }, out var count)
                .Remove(count)
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(FishPond).RequireField(nameof(FishPond.daysSinceSpawn))),
                    })
                .StripLabels();
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
