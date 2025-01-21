namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal LevelUpMenuCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<LevelUpMenu>(typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>Patch to allow choosing professions above level 10.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? LevelUpMenuCtorTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: isProfessionChooser = (level == 5 || level == 10) && skill != 5)
        // To: isProfessionChooser = level % 5 == 0 && skill != 5)
        try
        {
            helper
                .PatternMatch([
                        new CodeInstruction(OpCodes.Stfld, typeof(LevelUpMenu).RequireField(nameof(LevelUpMenu.isProfessionChooser)))
                    ])
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Bne_Un_S),
                    ],
                    ILHelper.SearchOption.Previous)
                .Move(-1)
                .ReplaceWith(new CodeInstruction(OpCodes.Ldc_I4_0))
                .Move(-1)
                .ReplaceWith(new CodeInstruction(OpCodes.Rem_Un))
                .Move(-1)
                .Remove();
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching profession choices above level 10.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
