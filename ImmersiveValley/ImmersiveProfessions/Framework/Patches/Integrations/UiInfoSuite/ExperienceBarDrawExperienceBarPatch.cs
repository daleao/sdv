namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.UiInfoSuite;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class ExperieneBarDrawExperienceBarPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ExperieneBarDrawExperienceBarPatch()
    {
        try
        {
            Target = "UIInfoSuite.UIElements.ExperienceBar".ToType().RequireMethod("DrawExperienceBar");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to move skill icon to the right.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ExperienceBarDrawExperienceBarTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(num + 54f, ...
        /// To: Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(num + 162f, ...

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_R4, 54f)
                )
                .SetOperand(174f);
        }
        catch (Exception ex)
        {
            Log.E($"Failed to budge Ui Info Suite experience bar skill icon.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}