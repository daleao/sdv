namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class BeeHouseMachineResetPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BeeHouseMachineResetPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine".ToType().RequireMethod("Reset");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to increase production frequency of Producer Bee House.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ObjectDayUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: machine.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4);
        /// To: machine.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, machine.DoesOwnerHaveProfession(<producer_id>)
        ///     ? machine.DoesOwnerHaveProfession(100 + <producer_id>
        ///         ? 1
        ///         : 2
        ///     : 4);

        var isNotProducer = generator.DefineLabel();
        var isNotPrestiged = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_4),
                    new CodeInstruction(OpCodes.Call,
                        typeof(StardewValley.Utility).RequireMethod(nameof(StardewValley.Utility.CalculateMinutesUntilMorning),
                            new[] { typeof(int), typeof(int) }))
                )
                .AddLabels(isNotProducer)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_0), // local 0 = SObject machine
                    new CodeInstruction(OpCodes.Ldc_I4_3), // 3 = Profession.Producer
                    new CodeInstruction(OpCodes.Ldc_I4_0), // false for not prestiged
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.DoesOwnerHaveProfession))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotProducer),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldc_I4_3),
                    new CodeInstruction(OpCodes.Ldc_I4_1), // true for prestiged
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.DoesOwnerHaveProfession))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldc_I4_2),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Retreat(2)
                .AddLabels(isNotPrestiged)
                .Return()
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching automated Bee House production speed for Producers.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}