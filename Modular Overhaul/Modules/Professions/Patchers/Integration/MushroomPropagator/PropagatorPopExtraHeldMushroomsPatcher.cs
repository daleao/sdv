namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("blueberry.MushroomPropagator")]
internal sealed class PropagatorPopExtraHeldMushroomsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PropagatorPopExtraHeldMushroomsPatcher"/> class.</summary>
    internal PropagatorPopExtraHeldMushroomsPatcher()
    {
        this.Target = "BlueberryMushroomMachine.Propagator"
            .ToType()
            .RequireMethod("PopExtraHeldMushrooms");
    }

    #region harmony patches

    /// <summary>Patch for Propagator output quantity (Harvester) and quality (Ecologist).</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? PropagatorPopExtraHeldMushroomsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: ceq 0
        // To: Game1.player.professions.Contains(<forager_id>) ? !cgt 0 : clt 0
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Profession.Forager.Value) // find index of forager check
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_0) })
                .SetOpCode(OpCodes.Ldc_I4_1)
                .Move()
                .InsertProfessionCheck(Profession.Forager.Value + 100)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                        new CodeInstruction(OpCodes.Cgt_Un),
                        new CodeInstruction(OpCodes.Not),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Insert(
                    new[] { new CodeInstruction(OpCodes.Clt_Un) },
                    new[] { isNotPrestiged })
                .Remove()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching Blueberry's Mushroom Propagator output quantity." +
                  "\n—-- Do NOT report this to Mushroom Propagator's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        // From: int popQuality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : SourceMushroomQuality);
        // To: int popQuality = PopExtraHeldMushroomsSubroutine(this);
        try
        {
            helper
                .MatchProfessionCheck(Profession.Ecologist.Value) // find index of ecologist check
                .Move(-1)
                .GetLabels(out var labels)
                .CountUntil(new[] { new CodeInstruction(OpCodes.Ldc_I4_4) }, out var count)
                .Remove(count)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(PropagatorPopExtraHeldMushroomsPatcher)
                                .RequireMethod(nameof(PopExtraHeldMushroomsSubroutine))),
                    },
                    labels)
                .StripLabels();
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching Blueberry's Mushroom Propagator output quality.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int PopExtraHeldMushroomsSubroutine(SObject propagator)
    {
        var who = ProfessionsModule.Config.LaxOwnershipRequirements ? Game1.player : propagator.GetOwner();
        return who.HasProfession(Profession.Ecologist)
            ? who.GetEcologistForageQuality()
            : Reflector.GetUnboundFieldGetter<SObject, int>(propagator, "SourceMushroomQuality").Invoke(propagator);
    }

    #endregion injected subroutines
}
