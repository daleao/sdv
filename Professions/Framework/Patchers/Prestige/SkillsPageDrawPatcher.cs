namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using static StardewValley.LocalizedContentManager;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPageDrawPatcher : HarmonyPatcher
{
    internal static Dictionary<ISkill, Rectangle> RibbonTargetRectBySkill = [];
    internal static int RibbonXOffset;
    internal static bool ShouldDrawRibbons;

    /// <summary>Initializes a new instance of the <see cref="SkillsPageDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SkillsPageDrawPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SkillsPage>(nameof(SkillsPage.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? SkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Inject: x -= AdjustForRibbonWidth()
        // After: x = ...
        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(LocalizedContentManager).RequirePropertyGetter(nameof(CurrentLanguageCode))),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_0)])
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillsPageDrawPatcher).RequireMethod(nameof(AdjustForRibbonWidth))),
                    new CodeInstruction(OpCodes.Sub),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adjusting localized skill page content position.\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawExtendedLevelBars(levelIndex, skillIndex, x, y, addedX, skillLevel, b)
        // Before: if (i == 9) draw level number ...
        var levelIndex = helper.Locals[10];
        var skillIndex = helper.Locals[11];
        var skillLevel = helper.Locals[15];
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldloc_S, levelIndex),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                        new CodeInstruction(OpCodes.Bne_Un),
                    ],
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldloc_S, levelIndex),
                        new CodeInstruction(OpCodes.Ldloc_S, skillIndex),
                        new CodeInstruction(OpCodes.Ldloc_0), // load x
                        new CodeInstruction(OpCodes.Ldloc_1), // load y
                        new CodeInstruction(OpCodes.Ldloc_2), // load addedX
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel), // load skillLevel,
                        new CodeInstruction(OpCodes.Ldarg_1), // load b
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillsPageDrawPatcher).RequireMethod(nameof(DrawExtendedLevelBars))),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching to draw skills page extended level bars.\nHelper returned {ex}");
            return null;
        }

        // From: (addedSkill ? Color.LightGreen : Color.Cornsilk)
        // To: (addedSkill ? Color.LightGreen : skillLevel == 20 ? Color.Grey : Color.SandyBrown)
        try
        {
            var isSkillLevel20 = generator.DefineLabel();
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Color).RequirePropertyGetter(nameof(Color.SandyBrown))),
                    ])
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 20),
                        new CodeInstruction(OpCodes.Beq_S, isSkillLevel20),
                    ])
                .Move()
                .GetOperand(out var resumeExecution)
                .Move()
                .Insert(
                    [
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Color).RequirePropertyGetter(nameof(Color.Cornsilk))),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    ],
                    [isSkillLevel20]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching to draw max skill level with different color.\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawExtrasSubroutine(b);
        // Before: if (hoverText.Length > 0)
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillsPage).RequireField("hoverText")),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(string).RequirePropertyGetter(nameof(string.Length))),
                    ],
                    ILHelper.SearchOption.Last)
                .StripLabels(out var labels) // backup and remove branch labels
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillsPageDrawPatcher).RequireMethod(nameof(DrawExtras))),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching to draw skills page prestige ribbons.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Harmony-injected subroutine shared by a SpaceCore patch.")]
    internal static int AdjustForRibbonWidth()
    {
        return ShouldEnableSkillReset && ShouldDrawRibbons ? Textures.STARS_WIDTH : 0;
    }

    private static void DrawExtendedLevelBars(
        int levelIndex, int skillIndex, int x, int y, int addedX, int skillLevel, SpriteBatch b)
    {
        if (!ShouldEnablePrestigeLevels)
        {
            return;
        }

        var drawBlue = skillLevel > levelIndex + 10;
        if (!drawBlue)
        {
            return;
        }

        // this will draw only the blue bars
        if ((levelIndex + 1) % 5 != 0)
        {
            b.Draw(
                Textures.SkillBars,
                new Vector2(x + addedX + (levelIndex * 36), y + (skillIndex * 68) - 4),
                new Rectangle(0, 0, 8, 9),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                1f);
        }
    }

    private static void DrawExtras(SkillsPage page, SpriteBatch b)
    {
        var x = CurrentLanguageCode is LanguageCode.ru or LanguageCode.it
            ? page.xPositionOnScreen + page.width - 448 - 48
            : page.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 - 8;
        var y = page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 8;
        const int verticalSpacing = 68;
        for (var i = 0; i < 5; i++)
        {
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i),
            };

            if (!skill.IsMastered())
            {
                continue;
            }

            b.Draw(
                Textures.MasteredSkillIcons,
                new Vector2(x - 52, y - 4 + (i * verticalSpacing)),
                skill.SourceSheetRect,
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                0.87f);
        }

        if (!ShouldEnableSkillReset || !ShouldDrawRibbons)
        {
            return;
        }

        var position =
            new Vector2(
                page.xPositionOnScreen + page.width + Textures.PROGRESSION_HORIZONTAL_OFFSET + RibbonXOffset,
                page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Textures.PROGRESSION_VERTICAL_OFFSET);
        for (var i = 0; i < 5; i++)
        {
            position.Y += verticalSpacing;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i),
            };

            var count = Game1.player.GetProfessionsForSkill(skill, true).Length;
            if (count == 0)
            {
                continue;
            }

            var sourceRect = new Rectangle(0, (count - 1) * 8, (count + 1) * 4, 8);
            b.Draw(
                Textures.PrestigeRibbons,
                position,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                Textures.STARS_SCALE,
                SpriteEffects.None,
                1f);

            RibbonTargetRectBySkill[skill] = new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)(sourceRect.Width * Textures.STARS_SCALE),
                (int)(sourceRect.Height * Textures.STARS_SCALE));
        }
    }

    #endregion injections
}
