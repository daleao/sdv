namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.CJBCheatsMenu;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class ProfessionsCheatSetProfessionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ProfessionsCheatSetProfessionPatch()
    {
        try
        {
            Target = "CJBCheatsMenu.Framework.Cheats.Skills.ProfessionsCheat".ToType().RequireMethod("SetProfession");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ProfessionsCheatSetProfessionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: case <defender_id>
        /// To: case <brute_id>

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender)
                )
                .SetOperand(Profession.Brute.Value);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving CJB Profession Cheat health bonus from Defender to Brute.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}