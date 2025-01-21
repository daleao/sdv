namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;
using FarmerExtensions = DaLion.Professions.Framework.Extensions.FarmerExtensions;

#endregion using directives

[UsedImplicitly]
internal sealed class BobberBarUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BobberBarUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BobberBarUpdatePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<BobberBar>(nameof(BobberBar.update));
    }

    #region harmony patches

    /// <summary>Patch for Prestiged Aquarist instant catch.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BobberBarUpdatePostfix(BobberBar __instance)
    {
        if (!Game1.player.HasProfession(Profession.Aquarist, true))
        {
            return;
        }

        Utility.ForEachBuilding(b =>
        {
            if (b is not FishPond pond || pond.fishType.Value != __instance.whichFish ||
                !pond.HasUnlockedFinalPopulationGate() || pond.currentOccupants.Value < pond.maxOccupants.Value)
            {
                return true; // continue enumeration
            }

            __instance.distanceFromCatching = 1f;
            if (__instance.treasure)
            {
                __instance.treasureCaught = true;
            }

            return false; // break enumeration
        });
    }

    /// <summary>Patch to slow-down catching bar decrease for Aquarist.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? BobberBarUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (Game1.player.professions.Contains(<aquarist_id>)) distanceFromCatching += Game1.player.GetAquaristCatchingBarCompensation();
        // After: distanceFromCatching -= (beginnersRod ? 0.002f : 0.003f) * distanceFromCatchPenaltyModifier;
        try
        {
            var isNotAquarist = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stfld, typeof(BobberBar).RequireField(nameof(BobberBar.distanceFromCatching)))], nth: 2)
                .Move()
                .StripLabels(out var labels)
                .AddLabels(isNotAquarist)
                .InsertProfessionCheck(Farmer.pirate, labels: labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Brfalse_S, isNotAquarist),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(BobberBar).RequireField("distanceFromCatching")),
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions)
                                .RequireMethod(nameof(FarmerExtensions.GetAquaristCatchingHandicap))),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stfld, typeof(BobberBar).RequireField("distanceFromCatching")),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching Aquarist catching bar loss.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var isNotFisher = generator.DefineLabel();
            var isNotPrestigedFisher = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R8, 0.25)], ILHelper.SearchOption.First)
                .Move()
                .AddLabels(isNotFisher, isNotPrestigedFisher)
                .InsertProfessionCheck(Farmer.fisher)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Brfalse_S, isNotFisher),
                        new CodeInstruction(OpCodes.Ldc_R8, 0.25),
                        new CodeInstruction(OpCodes.Add),
                    ])
                .InsertProfessionCheck(Farmer.fisher + 100)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestigedFisher),
                        new CodeInstruction(OpCodes.Ldc_R8, 0.25),
                        new CodeInstruction(OpCodes.Add),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed doubling Wild Bait effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
