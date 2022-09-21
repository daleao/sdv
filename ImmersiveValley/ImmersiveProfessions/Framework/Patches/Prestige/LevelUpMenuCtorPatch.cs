namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuCtorPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuCtorPatch"/> class.</summary>
    internal LevelUpMenuCtorPatch()
    {
        this.Target = this.RequireConstructor<LevelUpMenu>(typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>Patch to prevent duplicate profession acquisition + display end of level up dialogues.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuCtorTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: if ((currentLevel == 5 || currentLevel == 10) && currentSkill != 5)
        // To: if (currentLevel % 5 == 0 && currentSkill != 5)
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Ldc_I4_5),
                    new CodeInstruction(OpCodes.Beq_S))
                .Advance(3)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Rem_Un),
                    new CodeInstruction(OpCodes.Ldc_I4_0))
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_S, 10));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching profession choices above level 10.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
