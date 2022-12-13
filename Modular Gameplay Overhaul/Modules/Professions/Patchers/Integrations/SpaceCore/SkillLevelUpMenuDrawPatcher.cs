namespace DaLion.Overhaul.Modules.Professions.Patchers.Integrations;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.Interface;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class SkillLevelUpMenuDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillLevelUpMenuDrawPatcher"/> class.</summary>
    internal SkillLevelUpMenuDrawPatcher()
    {
        this.Target =
            this.RequireMethod<SkillLevelUpMenu>(nameof(SkillLevelUpMenu.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to draw Prestige tooltip during profession selection.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SkillLevelUpMenuDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: DrawSubroutine(this, b);
        // Before: else if (!isProfessionChooser)
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            "SkillLevelUpMenu"
                                .ToType()
                                .RequireField(nameof(LevelUpMenu.isProfessionChooser))),
                    })
                .Move()
                .GetOperand(out var isNotProfessionChooser)
                .FindLabel((Label)isNotProfessionChooser)
                .Move(-1)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillLevelUpMenuDrawPatcher).RequireMethod(nameof(DrawSubroutine))),
                    });
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching level up menu prestige tooltip draw." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony

    #region injected subroutines

    private static void DrawSubroutine(IClickableMenu menu, int currentLevel, SpriteBatch b)
    {
        var isProfessionChooser = Reflector
            .GetUnboundFieldGetter<IClickableMenu, bool>(menu, "isProfessionChooser")
            .Invoke(menu);
        if (!ProfessionsModule.Config.EnablePrestige || !isProfessionChooser || currentLevel > 10)
        {
            return;
        }

        var professionsToChoose = Reflector
            .GetUnboundFieldGetter<IClickableMenu, List<int>>(menu, "professionsToChoose")
            .Invoke(menu);
        if (!SCProfession.Loaded.TryGetValue(professionsToChoose[0], out var leftProfession) ||
            !SCProfession.Loaded.TryGetValue(professionsToChoose[1], out var rightProfession))
        {
            return;
        }

        Rectangle selectionArea;
        if (Game1.player.HasProfession(leftProfession) &&
            Game1.player.HasAllProfessionsBranchingFrom(leftProfession))
        {
            selectionArea = new Rectangle(
                menu.xPositionOnScreen + 32,
                menu.yPositionOnScreen + 232,
                (menu.width / 2) - 40,
                menu.height - 264);
            b.Draw(Game1.staminaRect, selectionArea, new Color(Color.Black, 0.3f));

            if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                string hoverText = I18n.Get(leftProfession.Id % 6 <= 1
                    ? "prestige.levelup.tooltip:5"
                    : "prestige.levelup.tooltip:10");
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }

        if (Game1.player.HasProfession(rightProfession) &&
            Game1.player.HasAllProfessionsBranchingFrom(rightProfession))
        {
            selectionArea = new Rectangle(
                menu.xPositionOnScreen + (menu.width / 2) + 8,
                menu.yPositionOnScreen + 232,
                (menu.width / 2) - 40,
                menu.height - 264);
            b.Draw(Game1.staminaRect, selectionArea, new Color(Color.Black, 0.3f));

            if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                string hoverText = I18n.Get(leftProfession.Id % 6 <= 1
                    ? "prestige.levelup.tooltip:5"
                    : "prestige.levelup.tooltip:10");
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }
    }

    #endregion injected subroutines
}
