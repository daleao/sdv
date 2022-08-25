namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.Harmony;
using DaLion.Common.Integrations.Automate;
using Extensions;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly, RequiresMod("Pathoschild.Automate")]
internal sealed class BushMachineGetOutputPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BushMachineGetOutputPatch()
    {
        Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType()
            .RequireMethod("GetOutput");
    }

    #region harmony patches

    /// <summary>Patch for automated Berry Bush quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BushMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: int quality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : 0);
        /// To: int quality = GetOutputSubroutine(this);

        try
        {
            helper
                .FindProfessionCheck(Profession.Ecologist.Value) // find index of ecologist check
                .Retreat()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        "Pathoschild.Stardew.Automate.Framework.BaseMachine`1".ToType().MakeGenericType(typeof(SObject))
                            .RequirePropertyGetter("Machine")),
                    new CodeInstruction(OpCodes.Call,
                        typeof(BushMachineGetOutputPatch).RequireMethod(nameof(GetOutputSubroutine)))
                )
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4)
                )
                .RemoveLabels();
        }
        catch (Exception ex)
        {
            Log.E("Immersive Professions failed while patching automated Berry Bush quality." +
                  "\n—-- Do NOT report this to Automate's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int GetOutputSubroutine(Bush machine)
    {
        var chest = ExtendedAutomateAPI.GetClosestContainerTo(machine);
        var user = ModEntry.Config.LaxOwnershipRequirements ? Game1.player : chest?.GetOwner() ?? Game1.MasterPlayer;
        return user.HasProfession(Profession.Ecologist) ? user.GetEcologistForageQuality() : SObject.lowQuality;
    }

    #endregion injected subroutines
}