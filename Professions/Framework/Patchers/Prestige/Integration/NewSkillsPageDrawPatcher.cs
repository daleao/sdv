namespace DaLion.Professions.Framework.Patchers.Prestige.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.Interface;
using StardewValley.Menus;
using static StardewValley.LocalizedContentManager;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewSkillsPageDrawPatcher : HarmonyPatcher
{
    internal static Dictionary<ISkill, Rectangle> RibbonTargetRectBySkill = [];
    internal static int RibbonXOffset = 0;

    /// <summary>Initializes a new instance of the <see cref="NewSkillsPageDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal NewSkillsPageDrawPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<NewSkillsPage>(nameof(NewSkillsPage.draw), [typeof(SpriteBatch)]);
        this.Transpiler!.before = ["Shockah.XPDisplay"];
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    [HarmonyBefore("Shockah.XPDisplay")]
    private static IEnumerable<CodeInstruction>? NewSkillsPageDrawTranspiler(
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
                        typeof(LocalizedContentManager).RequirePropertyGetter(nameof(LocalizedContentManager
                            .CurrentLanguageCode))),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_0)])
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillsPageDrawPatcher).RequireMethod(
                            nameof(SkillsPageDrawPatcher.AdjustForRibbonWidth))),
                    new CodeInstruction(OpCodes.Sub),
                ]);
        }
        catch (Exception ex)
        {
            Log.E("Professions mod failed adjusting localized skill page content position." +
                  $"\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawExtendedLevelBars(levelIndex, skillindex, x, y, addedX, skillLevel, b)
        // Before: if (i == 9) draw level number ...
        // Note: local variable indices correspond to SpaceCore v1.23.0
        var levelIndex = helper.Locals[14];
        var skillIndex = helper.Locals[15];
        var skillLevel = helper.Locals[19];
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
                        new CodeInstruction(OpCodes.Ldloc_3), // load xOffset
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                        new CodeInstruction(OpCodes.Ldarg_1), // load b
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(NewSkillsPageDrawPatcher).RequireMethod(nameof(DrawExtendedLevelBars))),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E("Professions mod failed patching to draw SpaceCore skills page extended level bars." +
                  $"\nHelper returned {ex}");
            return null;
        }

        // From: (addedSkill ? Color.LightGreen : Color.Cornsilk)
        // To: (addedSkill ? Color.LightGreen : skillLevel == 20 ? Color.Grey : Color.SandyBrown)
        try
        {
            var isSkillLevel20 = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Color).RequirePropertyGetter(nameof(Color.SandyBrown))),
                ])
                .Insert([
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
            Log.E("Professions mod failed patching to draw max skill level with different color." +
                  $"\nHelper returned {ex}");
            return null;
        }

        // repeat previous 2 injections now for custom skills

        var levelIndex2 = helper.Locals[25];
        var skillLevel2 = helper.Locals[26];
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldloc_S, levelIndex2),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                        new CodeInstruction(OpCodes.Bne_Un),
                    ],
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldloc_S, levelIndex2),
                        new CodeInstruction(OpCodes.Ldloc_2), // local 2 = int indexWithLuckSkill
                        new CodeInstruction(OpCodes.Ldloc_0), // load x
                        new CodeInstruction(OpCodes.Ldloc_1), // load y
                        new CodeInstruction(OpCodes.Ldloc_3), // load xOffset
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel2),
                        new CodeInstruction(OpCodes.Ldarg_1), // load b
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(NewSkillsPageDrawPatcher).RequireMethod(nameof(DrawExtendedLevelBars))),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E(
                "Professions mod failed patching to draw SpaceCore skills page extended level bars for custom skills." +
                $"\nHelper returned {ex}");
            return null;
        }

        try
        {
            var isSkillLevel20 = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Color).RequirePropertyGetter(nameof(Color.SandyBrown))),
                ])
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_S, skillLevel2),
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
            Log.E("Professions mod failed patching to draw max custom skill level with different color." +
                  $"\nHelper returned {ex}");
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
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(NewSkillsPage).RequireField("skillScrollOffset")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(NewSkillsPageDrawPatcher).RequireMethod(nameof(DrawExtras))),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E("Professions mod failed patching to draw skills page prestige ribbons." +
                  $"\nHelper returned {ex}");
            return null;
        }

        // From: levelIndex < skill.ExperienceCurve.Length;
        // To: levelIndex < 10;
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(SCSkill).RequirePropertyGetter(nameof(SCSkill.ExperienceCurve))),
                    ],
                    ILHelper.SearchOption.First)
                .Remove(3)
                .Move(-1)
                .ReplaceWith(new CodeInstruction(OpCodes.Ldc_I4_S, 10));
        }
        catch (Exception ex)
        {
            Log.E("Professions mod failed patching to draw skills page prestige ribbons." +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static void DrawExtendedLevelBars(
        int levelIndex, int indexWithLuckSkill, int x, int y, int addedX, int skillLevel, SpriteBatch b)
    {
        if (!Config.Masteries.EnablePrestigeLevels)
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
                new Vector2(x + addedX + (levelIndex * 36), y + (indexWithLuckSkill * 56) - 4),
                new Rectangle(0, 0, 8, 9),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                1f);
        }
    }

    private static void DrawExtras(NewSkillsPage page, SpriteBatch b, int skillScrollOffset)
    {
        var x = CurrentLanguageCode == LanguageCode.ru
            ? (page.xPositionOnScreen + page.width - 448 - 48)
            : page.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 - 8;
        var y = page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 8;

        const int verticalSpacing = 56;
        y -= skillScrollOffset * verticalSpacing;
        for (var i = 0; i < 5; i++)
        {
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i),
            };

            if (!skill.CanGainPrestigeLevels() || skill < skillScrollOffset)
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

        if (!ShouldEnableSkillReset)
        {
            return;
        }

        var position =
            new Vector2(
                page.xPositionOnScreen + page.width + Textures.PROGRESSION_HORIZONTAL_OFFSET + RibbonXOffset - 12,
                page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth +
                Textures.PROGRESSION_VERTICAL_OFFSET + 12);
        var lastVisibleSkillIndex =
            Reflector.GetUnboundPropertyGetter<NewSkillsPage, int>("LastVisibleSkillIndex").Invoke(page);
        for (var i = 0; i < 5; i++)
        {
            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i),
            };

            // hide if scrolled and out of bounds
            if (i < skillScrollOffset || i > lastVisibleSkillIndex)
            {
                RibbonTargetRectBySkill[skill] = Rectangle.Empty;
                continue;
            }

            position.Y += verticalSpacing;
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

        if (CustomSkill.Loaded.Count == 0)
        {
            return;
        }

        var customSkills = SpaceCoreIntegration.Api
            .GetCustomSkills()
            .Select(name => CustomSkill.Loaded[name]);
        var customSkillIndex =
            Reflector.GetUnboundPropertyGetter<NewSkillsPage, int>("GameSkillCount").Invoke(page);
        foreach (var skill in customSkills)
        {
            // hide if scrolled and out of bounds
            if (customSkillIndex < skillScrollOffset || customSkillIndex > lastVisibleSkillIndex)
            {
                RibbonTargetRectBySkill[skill] = Rectangle.Empty;
                customSkillIndex++;
                continue;
            }

            position.Y += 56;
            var count = Game1.player.GetProfessionsForSkill(skill, true).Length;
            if (count == 0)
            {
                customSkillIndex++;
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
            customSkillIndex++;
        }
    }

    #endregion injections
}
