namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.MushroomPropagator;

#region using directives

using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly, RequiresMod("blueberry.MushroomPropagator")]
internal sealed class PropagatorPopExtraHeldMushroomsPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static Func<SObject, int>? _GetSourceMushroomQuality;

    /// <summary>Construct an instance.</summary>
    internal PropagatorPopExtraHeldMushroomsPatch()
    {
        Target = "BlueberryMushroomMachine.Propagator".ToType().RequireMethod("PopExtraHeldMushrooms");
    }

    #region harmony patches

    /// <summary>Patch for Propagator output quantity (Harvester) and quality (Ecologist).</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? PropagatorPopExtraHeldMushroomsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: ceq 0
        /// To: Game1.player.professions.Contains(<forager_id>) ? !cgt 0 : clt 0

        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck(Profession.Forager.Value) // find index of forager check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_0)
                )
                .SetOpCode(OpCodes.Ldc_I4_1)
                .Advance()
                .InsertProfessionCheck(Profession.Forager.Value + 100)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Cgt_Un),
                    new CodeInstruction(OpCodes.Not),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .InsertWithLabels(
                    new[] { isNotPrestiged },
                    new CodeInstruction(OpCodes.Clt_Un)
                )
                .Remove()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Blueberry's Mushroom Propagator output quantity.\nHelper returned {ex}");
            return null;
        }

        /// From: int popQuality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : SourceMushroomQuality);
        /// To: int popQuality = PopExtraHeldMushroomsSubroutine(this);

        var owner = generator.DeclareLocal(typeof(Farmer));
        try
        {
            helper
                .FindProfessionCheck(Profession.Ecologist.Value) // find index of ecologist check
                .Retreat()
                .GetLabels(out var labels)
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4)
                )
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(PropagatorPopExtraHeldMushroomsPatch).RequireMethod(
                            nameof(PopExtraHeldMushroomsSubroutine)))
                )
                .RemoveLabels();

        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Blueberry's Mushroom Propagator output quality.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int PopExtraHeldMushroomsSubroutine(SObject propagator)
    {
        var who = ModEntry.Config.LaxOwnershipRequirements ? Game1.player : propagator.GetOwner();
        _GetSourceMushroomQuality ??= "BlueberryMushroomMachine.Propagator".ToType()
            .RequireField("SourceMushroomQuality")
            .CompileUnboundFieldGetterDelegate<SObject, int>();
        return who.HasProfession(Profession.Ecologist)
            ? who.GetEcologistForageQuality()
            : _GetSourceMushroomQuality(propagator);
    }

    #endregion injected subroutines

}