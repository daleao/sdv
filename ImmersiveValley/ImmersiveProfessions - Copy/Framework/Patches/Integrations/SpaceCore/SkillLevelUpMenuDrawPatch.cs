namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly, RequiresMod("spacesechase0.SpaceCore")]
internal sealed class SkillLevelUpMenuDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static Func<IClickableMenu, bool>? _GetIsProfessionChooser;

    private static Func<IClickableMenu, List<int>>? _GetProfessionsToChoose;

    /// <summary>Construct an instance.</summary>
    internal SkillLevelUpMenuDrawPatch()
    {
        Target = "SpaceCore.Interface.SkillLevelUpMenu".ToType().RequireMethod("draw", new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to draw Prestige tooltip during profession selection.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SkillLevelUpMenuDrawTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: DrawSubroutine(this, b);
        /// Before: else if (!isProfessionChooser)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld,
                        "SkillLevelUpMenu".ToType().RequireField(nameof(LevelUpMenu.isProfessionChooser)))
                )
                .Advance()
                .GetOperand(out var isNotProfessionChooser)
                .FindLabel((Label)isNotProfessionChooser)
                .Retreat()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SkillLevelUpMenuDrawPatch).RequireMethod(nameof(DrawSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level up menu prestige tooltip draw.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony

    #region injected subroutines

    private static void DrawSubroutine(IClickableMenu menu, int currentLevel, SpriteBatch b)
    {
        _GetIsProfessionChooser ??= "SpaceCore.Interface.SkillLevelUpMenu".ToType().RequireField("isProfessionChooser")
            .CompileUnboundFieldGetterDelegate<Func<IClickableMenu, bool>>();
        if (!ModEntry.Config.EnablePrestige || !_GetIsProfessionChooser(menu) ||
            currentLevel > 10) return;

        _GetProfessionsToChoose ??= "SpaceCore.Interface.SkillLevelUpMenu".ToType().RequireField("professionsToChoose")
            .CompileUnboundFieldGetterDelegate<Func<IClickableMenu, List<int>>>();
        var professionsToChoose = _GetProfessionsToChoose(menu);
        if (!ModEntry.CustomProfessions.TryGetValue(professionsToChoose[0], out var leftProfession) ||
            !ModEntry.CustomProfessions.TryGetValue(professionsToChoose[1], out var rightProfession)) return;

        Rectangle selectionArea;
        if (Game1.player.HasProfession(leftProfession) &&
            Game1.player.HasAllProfessionsBranchingFrom(leftProfession))
        {
            selectionArea = new(menu.xPositionOnScreen + 32, menu.yPositionOnScreen + 232,
                menu.width / 2 - 40, menu.height - 264);
            b.Draw(Game1.staminaRect, selectionArea, new(Color.Black, 0.3f));

            if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                var hoverText = ModEntry.i18n.Get(leftProfession.Id % 6 <= 1
                    ? "prestige.levelup.tooltip:5"
                    : "prestige.levelup.tooltip:10");
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }

        if (Game1.player.HasProfession(rightProfession) &&
            Game1.player.HasAllProfessionsBranchingFrom(rightProfession))
        {
            selectionArea = new(menu.xPositionOnScreen + menu.width / 2 + 8,
                menu.yPositionOnScreen + 232,
                menu.width / 2 - 40, menu.height - 264);
            b.Draw(Game1.staminaRect, selectionArea, new(Color.Black, 0.3f));

            if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                var hoverText = ModEntry.i18n.Get(leftProfession.Id % 6 <= 1
                    ? "prestige.levelup.tooltip:5"
                    : "prestige.levelup.tooltip:10");
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }
    }

    #endregion injected subroutines
}