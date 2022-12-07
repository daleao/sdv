namespace DaLion.Ligo.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationGetFishPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationGetFishPatcher"/> class.</summary>
    internal GameLocationGetFishPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.getFish));
    }

    #region harmony patches

    /// <summary>Patch for Fisher to re-roll reeled fish if first roll resulted in trash.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (ShouldRerollFish(who, whichFish, hasRerolled)) goto <choose_fish>
        // Before: caught = new Ammo(whichFish, 1);
        try
        {
            var startOfFishRoll = generator.DefineLabel();
            var shouldntReroll = generator.DefineLabel();
            var hasRerolled = generator.DeclareLocal(typeof(bool));
            var shuffleMethod = typeof(Utility)
                                    .GetMethods()
                                    .Where(mi => mi.Name == "Shuffle")
                                    .ElementAtOrDefault(1) ?? ThrowHelper.ThrowMissingMethodException<MethodInfo>("Failed to acquire {typeof(Utility)}::Shuffle method.");
            helper
                .InsertInstructions(
                    // set hasRerolled to false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_S, hasRerolled))
                .FindLast(
                    // find index of caught = new Ammo(whichFish, 1)
                    new CodeInstruction(
                        OpCodes.Newobj,
                        typeof(SObject).GetConstructor(new[]
                        {
                            typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int),
                        })))
                .RetreatUntil(new CodeInstruction(OpCodes.Ldloc_1))
                .AddLabels(shouldntReroll) // branch here if shouldn't reroll
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = Farmer who
                    new CodeInstruction(OpCodes.Ldloc_1), // local 1 = whichFish
                    new CodeInstruction(OpCodes.Ldloc_S, hasRerolled),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationGetFishPatcher).RequireMethod(nameof(ShouldRerollFish))),
                    new CodeInstruction(OpCodes.Brfalse_S, shouldntReroll),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stloc_S, hasRerolled), // set hasRerolled to true
                    new CodeInstruction(OpCodes.Br, startOfFishRoll))
                .RetreatUntil(
                    // start of choose fish
                    new CodeInstruction(OpCodes.Call, shuffleMethod.MakeGenericMethod(typeof(string))))
                .Retreat(2)
                .AddLabels(startOfFishRoll); // branch here to reroll
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding modded Fisher fish reroll.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static bool ShouldRerollFish(Farmer who, int currentFish, bool hasRerolled)
    {
        return (currentFish.IsTrashIndex() || currentFish.IsAlgaeIndex())
               && who.CurrentTool is FishingRod rod
               && rod.getBaitAttachmentIndex() != Constants.MagnetBaitIndex
               && who.HasProfession(Profession.Fisher) && !hasRerolled;
    }

    #endregion private methods
}
