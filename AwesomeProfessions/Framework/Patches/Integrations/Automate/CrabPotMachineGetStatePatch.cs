using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using DaLion.Stardew.Common.Harmony;

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

[UsedImplicitly]
internal class CrabPotMachineGetStatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CrabPotMachineGetStatePatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrabPotMachine".ToType()
                .MethodNamed("GetState");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for conflicting Luremaster and Conservationist automation rules.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CrabPotMachineGetStateTranspiler(
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
                    new CodeInstruction(OpCodes.Call, "CrabPotMachine".ToType().MethodNamed("PlayerNeedsBait"))
                )
                .SetOpCode(OpCodes.Brfalse_S);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching bait conditions for automated Crab Pots.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}