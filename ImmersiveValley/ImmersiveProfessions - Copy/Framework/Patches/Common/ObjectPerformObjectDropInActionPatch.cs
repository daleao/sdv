namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformObjectDropInActionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectPerformObjectDropInActionPatch()
    {
        Target = RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
        Postfix!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Patch to remember initial machine state.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    // ReSharper disable once RedundantAssignment
    private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.heldObject.Value !=
                  null; // remember whether this machine was already holding an object
        return true; // run original logic
    }

    /// <summary>Patch to increase Artisan production + integrate Quality Artisan Products.</summary>
    [HarmonyPostfix]
    private static void ObjectPerformObjectDropInActionPostfix(SObject __instance, bool __state, Item dropInItem,
        bool probe, Farmer who)
    {
        // if there was an object inside before running the original method, or if the machine is still empty after running the original method, then do nothing
        if (__state || __instance.heldObject.Value is null || probe) return;

        var user = who;
        var owner = ModEntry.Config.LaxOwnershipRequirements ? Game1.player : __instance.GetOwner();
        var held = __instance.heldObject.Value;
        if (__instance.IsArtisanMachine() && dropInItem is SObject dropIn)
        {
            // artisan users can preserve the input quality
            if (user.HasProfession(Profession.Artisan)) held.Quality = dropIn.Quality;

            // artisan-owned machines work faster and may upgrade quality
            if (owner.HasProfession(Profession.Artisan))
            {
                if (held.Quality < SObject.bestQuality && Game1.random.NextDouble() < 0.05)
                    held.Quality += held.Quality == SObject.highQuality ? 2 : 1;

                if (owner.HasProfession(Profession.Artisan, true))
                    __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 4;
                else
                    __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 10;
            }

            // golden mayonnaise is always iridium quality
            if (__instance.name == "Mayonnaise Machine" && dropIn.ParentSheetIndex == 928 &&
                !ModEntry.ModHelper.ModRegistry.IsLoaded("ughitsmegan.goldenmayoForProducerFrameworkMod"))
                held.Quality = SObject.bestQuality;
        }
    }

    /// <summary>Patch to reduce prestiged Breeder incubation time.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectPerformObjectDropInActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: minutesUntilReady.Value /= 2
        /// To: minutesUntilReady.Value /= who.professions.Contains(100 + <breeder_id>) ? 3 : 2

        var i = 0;
    repeat:
        try
        {
            var notPrestigedBreeder = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindProfessionCheck(Profession.Breeder.Value, true)
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_0)
                )
                .GetInstructionsUntil(out var got, true, true,
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_2)
                )
                .AddLabels(notPrestigedBreeder)
                .Insert(got)
                .Retreat()
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_2)
                )
                .ReplaceWith(
                    new(OpCodes.Ldc_I4_S, Profession.Breeder.Value + 100)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .SetOperand(notPrestigedBreeder)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Breeder incubation bonus.\nHelper returned {ex}");
            return null;
        }

        // repeat injection three times
        if (++i < 3) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches
}