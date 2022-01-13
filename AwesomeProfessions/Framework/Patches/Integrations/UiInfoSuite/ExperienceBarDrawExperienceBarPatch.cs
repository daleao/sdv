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
internal class ExperieneBarDrawExperienceBarPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ExperieneBarDrawExperienceBarPatch()
    {
        try
        {
            Original = "UIInfoSuite.UIElements.ExperienceBar".ToType().MethodNamed("DrawExperienceBar");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to move skill icon to the right.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ExperienceBarDrawExperienceBarTranspiler(
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
            ModEntry.Log(
                $"Failed while patching to budge Ui Info Suite experience bar skill icon. Helper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}