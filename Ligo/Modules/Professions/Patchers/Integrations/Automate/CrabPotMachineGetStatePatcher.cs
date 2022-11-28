namespace DaLion.Ligo.Modules.Professions.Patchers.Integrations;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
internal sealed class CrabPotMachineGetStatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotMachineGetStatePatcher"/> class.</summary>
    internal CrabPotMachineGetStatePatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrabPotMachine"
            .ToType()
            .RequireMethod("GetState");
    }

    #region harmony patches

    /// <summary>Patch for conflicting Luremaster and Conservationist automation rules.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CrabPotMachineGetStateTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed: || !this.PlayerNeedsBait()
        try
        {
            helper
                .FindFirst(new CodeInstruction(OpCodes.Brtrue_S))
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Call, "CrabPotMachine"
                        .ToType()
                        .RequireMethod("PlayerNeedsBait")))
                .SetOpCode(OpCodes.Brfalse_S);
        }
        catch (Exception ex)
        {
            Log.E("LIGO Professions module failed while patching bait conditions for automated Crab Pots." +
                  "\n—-- Do NOT report this to Automate's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
