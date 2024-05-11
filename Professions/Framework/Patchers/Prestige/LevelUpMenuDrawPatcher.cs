// ReSharper disable PossibleLossOfFraction
namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal LevelUpMenuDrawPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    /// <summary>Patch to increase the height of Level Up Menu to fit longer profession descriptions.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuDrawPrefix(
        LevelUpMenu __instance,
        int ___currentLevel,
        string ___title,
        List<int> ___professionsToChoose,
        List<string> ___leftProfessionDescription,
        List<TemporaryAnimatedSprite> ___littleStars,
        Color ___leftProfessionColor,
        Rectangle ___sourceRectForLevelIcon,
        int ___timerBeforeStart,
        SpriteBatch b)
    {
        if (!__instance.isProfessionChooser)
        {
            return true; // run original logic
        }

        if (___currentLevel == 10)
        {
            __instance.height += 32;
        }

        if (___timerBeforeStart > 0 || ___professionsToChoose.Count != 1)
        {
            return true; // run original logic
        }

        #region choose single profession

        var xPositionOnScreen = __instance.xPositionOnScreen;
        var yPositionOnScreen = __instance.yPositionOnScreen;
        var width = __instance.width;
        var height = __instance.height;
        b.Draw(
            Game1.fadeToBlackRect,
            new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height),
            Color.Black * 0.5f);
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

            Game1.drawDialogueBox(
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                speaker: false,
                drawOnlyBox: true);
            Reflector
                .GetUnboundMethodDelegate<Action<LevelUpMenu, SpriteBatch, int, bool, int, int, int>>(
                    __instance,
                    "drawHorizontalPartition").Invoke(__instance, b, yPositionOnScreen + 192, false, -1, -1, -1);

            Utility.drawWithShadow(
                b,
                Game1.buffsIcons,
                new Vector2(
                    xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth,
                    yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16),
                ___sourceRectForLevelIcon,
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                flipped: false,
                0.88f);

            b.DrawString(
                Game1.dialogueFont,
                ___title,
                new Vector2(
                    xPositionOnScreen + (width / 2) - (Game1.dialogueFont.MeasureString(___title).X / 2f),
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16),
                Game1.textColor);

            Utility.drawWithShadow(
                b,
                Game1.buffsIcons,
                new Vector2(
                    xPositionOnScreen + __instance.width - IClickableMenu.spaceToClearSideBorder -
                    IClickableMenu.borderWidth - 64,
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16),
                ___sourceRectForLevelIcon,
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                flipped: false,
                0.88f);

            var chooseProfession = I18n.Prestige_LevelUp_Single();
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

            b.Draw(
                Game1.mouseCursors,
                new Vector2(
                    xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + (width / 2) - 112 + (width / 4),
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16),
                new Rectangle(
                    (___professionsToChoose[0] % 6) * 16, 624 + (___professionsToChoose[0] / 6 * 16), 16, 16),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                1f);
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
            if (!Game1.options.SnappyMenus || __instance.hasMovedSelection)
            {
                Game1.mouseCursorTransparency = 1f;
                __instance.drawMouse(b);
            }
        }

        #endregion choose single profession

        return false; // don't run original logic
    }

    /// <summary>Patch to draw acquired profession tooltips during profession selection.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: string chooseProfession = Game1.content.LoadString("Strings\\UI:LevelUp_ChooseProfession");
        // To: string chooseProfession = GetChooseProfessionText(this);
        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_1)])
                .PatternMatch([new CodeInstruction(OpCodes.Ldsfld)], ILHelper.SearchOption.Previous)
                .CountUntil([new CodeInstruction(OpCodes.Callvirt)], out var count)
                .Remove(count)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(LevelUpMenuDrawPatcher).RequireMethod(nameof(GetChooseProfessionText))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching level up menu choose profession text.\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawSubroutine(this, b);
        // Before: else if (!isProfessionChooser)
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(LevelUpMenu).RequireField(nameof(LevelUpMenu.isProfessionChooser))),
                    ],
                    ILHelper.SearchOption.First)
                .Move()
                .GetOperand(out var isNotProfessionChooser)
                .LabelMatch((Label)isNotProfessionChooser)
                .Move(-1)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(LevelUpMenuDrawPatcher).RequireMethod(nameof(DrawSubroutine))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching level up menu prestige tooltip draw.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static string GetChooseProfessionText(int currentLevel)
    {
        return currentLevel > 10
            ? I18n.Prestige_LevelUp_Choose()
            : Game1.content.LoadString("Strings\\UI:LevelUp_ChooseProfession");
    }

    private static void DrawSubroutine(LevelUpMenu menu, int currentLevel, SpriteBatch b)
    {
        if (!EnableSkillReset || !menu.isProfessionChooser || currentLevel > 10)
        {
            return;
        }

        var professionsToChoose = Reflector
            .GetUnboundFieldGetter<LevelUpMenu, List<int>>("professionsToChoose")
            .Invoke(menu);
        if (!Profession.TryFromValue(professionsToChoose[0], out var leftProfession) ||
            !Profession.TryFromValue(professionsToChoose[1], out var rightProfession))
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
                var hoverText = leftProfession % 6 <= 1
                    ? I18n.Prestige_LevelUp_Tooltip5()
                    : I18n.Prestige_LevelUp_Tooltip10();
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
                var hoverText = leftProfession % 6 <= 1
                    ? I18n.Prestige_LevelUp_Tooltip5()
                    : I18n.Prestige_LevelUp_Tooltip10();
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }
    }

    #endregion injections
}
