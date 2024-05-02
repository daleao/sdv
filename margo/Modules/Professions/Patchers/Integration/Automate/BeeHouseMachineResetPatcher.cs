namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.Automate;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Pathoschild.Automate", "Automate")]
internal sealed class BeeHouseMachineResetPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BeeHouseMachineResetPatcher"/> class.</summary>
    internal BeeHouseMachineResetPatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine"
            .ToType()
            .RequireMethod("Reset");
    }

    #region harmony patches

    /// <summary>Patch to increase production frequency of Producer Bee House.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: machine.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4);
        // To: machine.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, machine.DoesOwnerHaveProfession(<producer_id>)
        //     ? machine.DoesOwnerHaveProfession(100 + <producer_id>
        //         ? 1
        //         : 2
        //     : 4);
        try
        {
            var isNotProducer = generator.DefineLabel();
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_4),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Utility).RequireMethod(
                                nameof(Utility.CalculateMinutesUntilMorning),
                                new[] { typeof(int), typeof(int) })),
                    })
                .AddLabels(isNotProducer)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_0), // local 0 = SObject machine
                        new CodeInstruction(OpCodes.Ldc_I4_3), // 3 = Profession.Producer
                        new CodeInstruction(OpCodes.Ldc_I4_0), // false for not prestiged
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SObjectExtensions).RequireMethod(
                                nameof(SObjectExtensions.DoesOwnerHaveProfession),
                                new[] { typeof(SObject), typeof(int), typeof(bool) })),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotProducer),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldc_I4_3),
                        new CodeInstruction(OpCodes.Ldc_I4_1), // true for prestiged
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SObjectExtensions).RequireMethod(
                                nameof(SObjectExtensions.DoesOwnerHaveProfession),
                                new[] { typeof(SObject), typeof(int), typeof(bool) })),
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldc_I4_2),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move(-2)
                .AddLabels(isNotPrestiged)
                .Return()
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching automated Bee House production speed for Producers." +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
