using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches.Compatibility;

[UsedImplicitly]
internal class ProfessionsCheatSetProfessionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ProfessionsCheatSetProfessionPatch()
    {
        try
        {
            Original = "CJBCheatsMenu.Framework.Cheats.Skills.ProfessionsCheat".ToType().MethodNamed("SetProfession");
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
                .SetOperand(Utility.Professions.IndexOf("Brute"));
        }
        catch (Exception ex)
        {
            ModEntry.Log(
                $"Failed while moving CJB Profession Cheat health bonus from Defender to Brute.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}