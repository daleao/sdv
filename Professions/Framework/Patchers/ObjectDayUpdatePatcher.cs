namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectDayUpdatePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.DayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to prevent quantum bombs when detonating manually + record Arborist-planted trees.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ObjectPlacementActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var notHatchedByPiper = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldstr, QIDs.SlimeIncubator)])
                .Move(2)
                .GetOperand(out var caseLabel)
                .LabelMatch((Label)caseLabel)
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[21]),
                    new CodeInstruction(OpCodes.Brfalse),
                ])
                .Move(2)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[21]),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ObjectDayUpdatePatcher).RequireMethod(nameof(SetPiperFlag))),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Piper flag to hatched Slimes.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static void SetPiperFlag(SObject incubator, GreenSlime slime)
    {
        if (incubator.GetOwner().HasProfession(Profession.Piper))
        {
            Data.Write(slime, DataKeys.HatchedByPiper, "true".ToString());
        }
    }

    #endregion injected
}
