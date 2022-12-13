namespace DaLion.Overhaul.Modules.Professions.Patchers.Integrations;

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
[RequiresMod("CJBok.CheatsMenu")]
internal sealed class ProfessionsCheatSetProfessionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionsCheatSetProfessionPatcher"/> class.</summary>
    internal ProfessionsCheatSetProfessionPatcher()
    {
        this.Target = "CJBCheatsMenu.Framework.Cheats.Skills.ProfessionsCheat"
            .ToType()
            .RequireMethod("SetProfession");
    }

    #region harmony patches

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ProfessionsCheatSetProfessionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: case <defender_id>
        // To: case <brute_id>
        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender) })
                .SetOperand(Profession.Brute.Value);
        }
        catch (Exception ex)
        {
            Log.E(
                "Professions module failed moving CJB Profession Cheat health bonus from Defender to Brute." +
                "\n—-- Do NOT report this to CJB Cheats Menu's author. ---" +
                $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
