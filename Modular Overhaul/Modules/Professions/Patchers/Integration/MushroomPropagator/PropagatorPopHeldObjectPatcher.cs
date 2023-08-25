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
[ModRequirement("blueberry.MushroomPropagator", version: "2.2.0")]
internal sealed class PropagatorPopHeldObjectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PropagatorPopHeldObjectPatcher"/> class.</summary>
    internal PropagatorPopHeldObjectPatcher()
    {
        this.Target = "BlueberryMushroomMachine.Propagator"
            .ToType()
            .RequireMethod("PopHeldObject");
    }

    #region harmony patches

    /// <summary>Patch for Propagator output quantity (Harvester) and quality (Ecologist).</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? PropagatorPopExtraHeldMushroomsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (Game1.player.professions.Contains(13) && new Random().Next(5) == 0)
        // To: if (Game1.player.professions.Contains(13) && (randInt = new Random().Next(5) == 0 || Game1.player.professions.Contains(13+100) && randInt == 1))
        try
        {
            var doDoubleHarvest = generator.DefineLabel();
            var randInt = generator.DeclareLocal(typeof(int));
            helper
                .MatchProfessionCheck(Profession.Forager.Value) // find index of forager check
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .GetOperand(out var resumeExecution)
                .Match(new[] { new CodeInstruction(OpCodes.Brtrue_S) })
                .ReplaceWith(new CodeInstruction(OpCodes.Brfalse_S, doDoubleHarvest))
                .Move(-1)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Stloc_S, randInt),
                        new CodeInstruction(OpCodes.Ldloc_S, randInt),
                    })
                .Move(2)
                .AddLabels(doDoubleHarvest)
                .InsertProfessionCheck(Profession.Forager.Value + 100)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldloc_S, randInt),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching Blueberry's Mushroom Propagator output quantity." +
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
                            typeof(PropagatorPopHeldObjectPatcher)
                                .RequireMethod(nameof(PopExtraHeldMushroomsSubroutine))),
                    },
                    labels)
                .StripLabels();
        }
        catch (Exception ex)
        {
            Log.E($"Professions module failed patching Blueberry's Mushroom Propagator output quality." +
                  $"\nHelper returned {ex}");
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
