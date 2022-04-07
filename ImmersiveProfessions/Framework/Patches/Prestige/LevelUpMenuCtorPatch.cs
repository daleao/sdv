namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Menus;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class LevelUpMenuCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuCtorPatch()
    {
        Original = RequireConstructor<LevelUpMenu>(typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>Patch to prevent duplicate profession acquisition + display end of level up dialogues.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> LevelUpMenuCtorTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if ((currentLevel == 5 || currentLevel == 10) && currentSkill != 5)
        /// To: if (currentLevel % 5 == 0 && currentSkill != 5)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Ldc_I4_5),
                    new CodeInstruction(OpCodes.Beq_S)
                )
                .Advance(3)
                .Insert(
                    new CodeInstruction(OpCodes.Rem_Un),
                    new CodeInstruction(OpCodes.Ldc_I4_0)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_S, 10)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching profession choices above level 10. Helper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}