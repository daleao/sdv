namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using LinqFasterer;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationGetFishPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private const int MAGNET_INDEX_I = 703;

    /// <summary>Construct an instance.</summary>
    internal GameLocationGetFishPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.getFish));
    }

    #region harmony patches

    /// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (ShouldRerollFish(who, whichFish, hasRerolled)) goto <choose_fish>
        ///	Before: caught = new Object(whichFish, 1);

        var startOfFishRoll = generator.DefineLabel();
        var shouldntReroll = generator.DefineLabel();
        var hasRerolled = generator.DeclareLocal(typeof(bool));
        var shuffleMethod = typeof(StardewValley.Utility).GetMethods().WhereF(mi => mi.Name == "Shuffle").ElementAtOrDefaultF(1);
        if (shuffleMethod is null)
        {
            Log.E($"Failed to acquire {typeof(StardewValley.Utility)}::Shuffle method.");
            return null;
        }

        try
        {
            helper
                .InsertInstructions( // set hasRerolled to false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_S, hasRerolled)
                )
                .FindLast( // find index of caught = new Object(whichFish, 1)
                    new CodeInstruction(OpCodes.Newobj,
                        typeof(SObject).GetConstructor(new[]
                            {typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int)}))
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_1)
                )
                .AddLabels(shouldntReroll) // branch here if shouldn't reroll
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
                    new CodeInstruction(OpCodes.Ldloc_1), // local 1 = whichFish
                    new CodeInstruction(OpCodes.Ldloc_S, hasRerolled),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationGetFishPatch).RequireMethod(nameof(ShouldRerollFish))),
                    new CodeInstruction(OpCodes.Brfalse_S, shouldntReroll),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stloc_S, hasRerolled), // set hasRerolled to true
                    new CodeInstruction(OpCodes.Br, startOfFishRoll)
                )
                .RetreatUntil( // start of choose fish
                    new CodeInstruction(OpCodes.Call, shuffleMethod.MakeGenericMethod(typeof(string)))
                )
                .Retreat(2)
                .AddLabels(startOfFishRoll); // branch here to reroll
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding modded Fisher fish reroll.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static bool ShouldRerollFish(Farmer who, int currentFish, bool hasRerolled) =>
        (currentFish is > 166 and < 173 || ModEntry.Config.SeaweedIsTrash && currentFish.IsAlgaeIndex())
               && who.CurrentTool is FishingRod rod
               && rod.getBaitAttachmentIndex() != MAGNET_INDEX_I
               && who.HasProfession(Profession.Fisher) && !hasRerolled;

    #endregion private methods
}