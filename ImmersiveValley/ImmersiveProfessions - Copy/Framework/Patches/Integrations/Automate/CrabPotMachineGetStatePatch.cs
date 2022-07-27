namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly, RequiresMod("Pathoschild.Automate")]
internal sealed class CrabPotMachineGetStatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CrabPotMachineGetStatePatch()
    {
        Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrabPotMachine".ToType()
            .RequireMethod("GetState");
    }

    #region harmony patches

    /// <summary>Patch for conflicting Luremaster and Conservationist automation rules.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CrabPotMachineGetStateTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Removed: || !this.PlayerNeedsBait()

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Brtrue_S)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Call, "CrabPotMachine".ToType().RequireMethod("PlayerNeedsBait"))
                )
                .SetOpCode(OpCodes.Brfalse_S);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching bait conditions for automated Crab Pots.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}