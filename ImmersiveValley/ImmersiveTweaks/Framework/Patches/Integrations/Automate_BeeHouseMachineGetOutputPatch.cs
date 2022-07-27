namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common;
using Common.Attributes;
using Common.Extensions.Reflection;
using Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly, RequiresMod("Pathoschild.Automate")]
internal sealed class BeeHouseMachineGetOutputPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BeeHouseMachineGetOutputPatch()
    {
        Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.BeeHouseMachine".ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Adds aging quality to automated bee houses.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BeeHouseMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (ModEntry.Config.AgeImprovesBeeHouses) object.Quality = @object.GetQualityFromAge();
        /// Before: StardewValley.Object result = @object;

        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[4])
                )
                .AddLabels(resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.AgeImprovesBeeHouses))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        "Pathoschild.Stardew.Automate.Framework.BaseMachine`1".ToType().MakeGenericType(typeof(SObject))
                            .RequirePropertyGetter("Machine")),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).RequireMethod(nameof(SObjectExtensions.GetQualityFromAge))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SObject).RequirePropertySetter(nameof(SObject.Quality)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed improving automated honey quality with age.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}