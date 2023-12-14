namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige.Integration;

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
using SCPair = SpaceCore.Skills.Skill.ProfessionPair;
using SCSkill = SpaceCore.Skills.Skill;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class SkillLevelUpMenuDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillLevelUpMenuDrawPatcher"/> class.</summary>
    internal SkillLevelUpMenuDrawPatcher()
    {
        this.Target =
            this.RequireMethod<SkillLevelUpMenu>(nameof(SkillLevelUpMenu.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to increase the height of Level Up Menu to fit longer profession descriptions.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuDrawPrefix(
        SkillLevelUpMenu __instance,
        int ___currentLevel,
        string ___currentSkill,
        string ___title,
        List<int> ___professionsToChoose,
        List<string> ___leftProfessionDescription,
        List<TemporaryAnimatedSprite> ___littleStars,
        Color ___leftProfessionColor,
        int ___timerBeforeStart,
        SCPair? ___profPair,
        SpriteBatch b)
    {
        if (!__instance.isProfessionChooser || ___profPair is null)
        {
            return true; // run original logic
        }

        if (___currentLevel == 10)
        {
            __instance.height += 16;
        }

        if (___timerBeforeStart > 0 || ProfessionsModule.Config.PrestigeProgressionMode !=
            ProfessionConfig.PrestigeMode.Streamlined || ___professionsToChoose.Count != 1)
        {
            return true; // run original logic
        }

        #region copy

        var xPositionOnScreen = __instance.xPositionOnScreen;
        var yPositionOnScreen = __instance.yPositionOnScreen;
        var width = __instance.width;
        var height = __instance.height;
        b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
        foreach (var littleStar in ___littleStars)
        {
            littleStar.draw(b);
        }

        b.Draw(
            Game1.mouseCursors,
            new Vector2(
                xPositionOnScreen + (width / 2) - 116,
                yPositionOnScreen - 32 + 12),
            new Rectangle(363, 87, 58, 22),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            1f);
        if (!__instance.informationUp && __instance.isActive && __instance.starIcon != null)
        {
            __instance.starIcon.draw(b);
        }
        else
        {
            if (!__instance.informationUp)
            {
                return false;
            }

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, speaker: false, drawOnlyBox: true);
            Reflector
                .GetUnboundMethodDelegate<Action<SkillLevelUpMenu, SpriteBatch, int, bool, int, int, int>>(
                    __instance,
                    "drawHorizontalPartition").Invoke(__instance, b, yPositionOnScreen + 192, false, -1, -1, -1);

            var skillsByName = Reflector
                .GetStaticFieldGetter<Dictionary<string, SCSkill>>(typeof(SpaceCore.Skills), "SkillsByName")
                .Invoke();
            var icon = skillsByName[___currentSkill].Icon;
            if (icon is not null)
            {
                Utility.drawWithShadow(
                    b,
                    icon,
                    new Vector2(
                        xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth,
                        yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16),
                    icon.Bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flipped: false,
                    0.88f);
            }

            b.DrawString(
                Game1.dialogueFont,
                ___title,
                new Vector2(
                    xPositionOnScreen + (width / 2) - (Game1.dialogueFont.MeasureString(___title).X / 2f),
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16),
                Game1.textColor);

            if (icon is not null)
            {
                Utility.drawWithShadow(
                    b,
                    icon,
                    new Vector2(
                        xPositionOnScreen + __instance.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 64,
                        __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16),
                    icon.Bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flipped: false,
                    0.88f);
            }

            var chooseProfession = I18n.Prestige_LevelUp_Streamlined();
            b.DrawString(
                Game1.smallFont,
                chooseProfession,
                new Vector2(
                    xPositionOnScreen + (__instance.width / 2) -
                    (Game1.smallFont.MeasureString(chooseProfession).X / 2f),
                    __instance.yPositionOnScreen + 64 + IClickableMenu.spaceToClearTopBorder),
                Game1.textColor);

            b.DrawString(
                Game1.dialogueFont,
                ___leftProfessionDescription[0],
                new Vector2(
                    xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32 + (width / 4),
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160),
                ___leftProfessionColor);

            var scProfession = ___profPair.First.GetVanillaId() == ___professionsToChoose[0] ? ___profPair.First : ___profPair.Second;
            if (scProfession.Icon is not null)
            {
                b.Draw(
                    scProfession.Icon,
                    new Vector2(
                        xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + (width / 2) - 112 + (width / 4),
                        __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16),
                    scProfession.Icon.Bounds,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    1f);
            }

            for (var j = 1; j < ___leftProfessionDescription.Count; j++)
            {
                b.DrawString(
                    Game1.smallFont,
                    Game1.parseText(___leftProfessionDescription[j], Game1.smallFont, (__instance.width / 2) - 64),
                    new Vector2(
                        -4 + xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32 + (width / 4),
                        __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 128 + 8 + (64 * (j + 1))),
                    ___leftProfessionColor);
            }

            __instance.okButton.draw(b);
            __instance.drawMouse(b);
        }

        #endregion copy

        return false; // don't run original logic
    }

    /// <summary>Patch to draw Prestige tooltip during profession selection.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuDrawTranspiler(
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
                .MatchLabel((Label)isNotProfessionChooser)
                .Move(-1)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillLevelUpMenuDrawPatcher).RequireMethod(nameof(DrawSubroutine))),
                    });
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching level up menu prestige tooltip draw." +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony

    #region injected subroutines

    private static void DrawSubroutine(SkillLevelUpMenu menu, int currentLevel, SpriteBatch b)
    {
        if (!ProfessionsModule.EnableSkillReset || !menu.isProfessionChooser || currentLevel > 10)
        {
            return;
        }

        var professionsToChoose = Reflector
            .GetUnboundFieldGetter<SkillLevelUpMenu, List<int>>("professionsToChoose")
            .Invoke(menu);
        if (!CustomProfession.Loaded.TryGetValue(professionsToChoose[0], out var leftProfession) ||
            !CustomProfession.Loaded.TryGetValue(professionsToChoose[1], out var rightProfession))
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
                var hoverText = leftProfession.Level == 10
                    ? I18n.Prestige_LevelUp_Tooltip10()
                    : I18n.Prestige_LevelUp_Tooltip5();
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
                var hoverText = leftProfession.Level == 10
                    ? I18n.Prestige_LevelUp_Tooltip10()
                    : I18n.Prestige_LevelUp_Tooltip5();
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }
    }

    #endregion injected subroutines
}
