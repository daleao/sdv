namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige.Integration;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Overhaul.Modules.Professions.Patchers.Prestige;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using SpaceCore.Interface;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewSkillsPageDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewSkillsPageDrawPatcher"/> class.</summary>
    internal NewSkillsPageDrawPatcher()
    {
        this.Target = this.RequireMethod<NewSkillsPage>(nameof(NewSkillsPage.draw), new[] { typeof(SpriteBatch) });
        this.Transpiler!.before = new[] { "Shockah.XPDisplay" };
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    [HarmonyBefore("Shockah.XPDisplay")]
    private static IEnumerable<CodeInstruction>? NewSkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Inject: x -= ProfessionsModule.Config.PrestigeProgressionStyle == ProgressionStyle.Stars ? Textures.STARS_WIDTH_I : Textures.RIBBON_WIDTH_I;
        // After: x = ...
        try
        {
            var notRibbons = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Br_S) })
                .GetOperand(out var resumeExecution)
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_0) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.EnablePrestige))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(
                                nameof(Config.ProgressionStyle))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Beq_S, notRibbons),
                        new CodeInstruction(
                            OpCodes.Ldc_I4_S,
                            (int)((Textures.RibbonWidth + 5) * Textures.RibbonScale)),
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldc_I4_S,
                            (int)((Textures.StarsWidth + 4) * Textures.StarsScale)),
                        new CodeInstruction(OpCodes.Sub),
                    },
                    new[] { notRibbons });
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed adjusing localized skill page content position." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawExtendedLevelBars(levelIndex, skillindex, x, y, addedX, skillLevel, b)
        // Before: if (i == 9) draw level number ...
        // Note: local variable indices correspond to SpaceCore v1.8.0
        var levelIndex = helper.Locals[8];
        var skillIndex = helper.Locals[9];
        var skillLevel = helper.Locals[13];
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, levelIndex),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                        new CodeInstruction(OpCodes.Bne_Un),
                    },
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, levelIndex),
                        new CodeInstruction(OpCodes.Ldloc_S, skillIndex),
                        new CodeInstruction(OpCodes.Ldloc_0), // load x
                        new CodeInstruction(OpCodes.Ldloc_1), // load y
                        new CodeInstruction(OpCodes.Ldloc_3), // load xOffset
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                        new CodeInstruction(OpCodes.Ldarg_1), // load b
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillsPageDrawPatcher).RequireMethod(nameof(SkillsPageDrawPatcher
                                .DrawExtendedLevelBars))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching to draw SpaceCore skills page extended level bars." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        // From: (addedSkill ? Color.LightGreen : Color.Cornsilk)
        // To: (addedSkill ? Color.LightGreen : skillLevel == 20 ? Color.Grey : Color.SandyBrown)
        try
        {
            var isSkillLevel20 = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Color).RequirePropertyGetter(nameof(Color.SandyBrown))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 20),
                        new CodeInstruction(OpCodes.Beq_S, isSkillLevel20),
                    })
                .Move()
                .GetOperand(out var resumeExecution)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Color).RequirePropertyGetter(nameof(Color.Cornsilk))),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    new[] { isSkillLevel20 });
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching to draw max skill level with different color." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        // repeat previous 2 injections now for custom skills

        var levelIndex2 = helper.Locals[19];
        var skillLevel2 = helper.Locals[20];
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, levelIndex2),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                        new CodeInstruction(OpCodes.Bne_Un),
                    },
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
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
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching to draw SpaceCore skills page extended level bars for custom skills." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        try
        {
            var isSkillLevel20 = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Color).RequirePropertyGetter(nameof(Color.SandyBrown))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel2),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 20),
                        new CodeInstruction(OpCodes.Beq_S, isSkillLevel20),
                    })
                .Move()
                .GetOperand(out var resumeExecution)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Color).RequirePropertyGetter(nameof(Color.Cornsilk))),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    new[] { isSkillLevel20 });
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching to draw max custom skill level with different color." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawRibbonsSubroutine(b);
        // Before: if (hoverText.Length > 0)
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillsPage).RequireField("hoverText")),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(string).RequirePropertyGetter(nameof(string.Length))),
                    },
                    ILHelper.SearchOption.Last)
                .StripLabels(out var labels) // backup and remove branch labels
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(NewSkillsPage).RequireField("skillScrollOffset")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(NewSkillsPageDrawPatcher).RequireMethod(nameof(DrawRibbons))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching to draw skills page prestige ribbons." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        // From: levelIndex < skill.ExperienceCurve.Length;
        // To: levelIndex < 10;
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Skills.Skill).RequirePropertyGetter(nameof(Skills.Skill.ExperienceCurve))),
                    },
                    ILHelper.SearchOption.First)
                .Remove(3)
                .Move(-1)
                .ReplaceWith(new CodeInstruction(OpCodes.Ldc_I4_S, 10));
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching to draw skills page prestige ribbons." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Harmony-injected subroutine shared by a SpaceCore patch.")]
    internal static void DrawExtendedLevelBars(
        int levelIndex, int indexWithLuckSkill, int x, int y, int addedX, int skillLevel, SpriteBatch b)
    {
        if (!ProfessionsModule.Config.EnablePrestige)
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
                Textures.SkillBarsTx,
                new Vector2(addedX + x + (levelIndex * 36), y - 4 + (indexWithLuckSkill * 56)),
                new Rectangle(0, 0, 8, 9),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                1f);
        }
    }

    internal static void DrawRibbons(NewSkillsPage page, SpriteBatch b, int skillScrollOffset)
    {
        if (!ProfessionsModule.Config.EnablePrestige)
        {
            return;
        }

        var position =
            new Vector2(
                page.xPositionOnScreen + page.width + Textures.ProgressionHorizontalOffset,
                page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Textures.ProgressionVerticalOffset);
        if (ProfessionsModule.Config.ProgressionStyle == Config.PrestigeProgressionStyle.StackedStars)
        {
            position.X -= 22;
            position.Y -= 4;
        }

        var lastVisibleSkillIndex =
            Reflector.GetUnboundPropertyGetter<NewSkillsPage, int>(page, "LastVisibleSkillIndex").Invoke(page);
        for (var i = 0; i < 5; i++)
        {
            // hide if scrolled and out of bounds
            if (i < skillScrollOffset || i > lastVisibleSkillIndex)
            {
                continue;
            }

            position.Y += 56;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skillIndex = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i),
            };

            var count = Game1.player.GetProfessionsForSkill(skillIndex, true).Length;
            if (count == 0)
            {
                continue;
            }

            Rectangle sourceRect;
            switch (ProfessionsModule.Config.ProgressionStyle)
            {
                case Config.PrestigeProgressionStyle.Gen3Ribbons:
                case Config.PrestigeProgressionStyle.Gen4Ribbons:
                    sourceRect = new Rectangle(
                        i * Textures.RibbonWidth,
                        (count - 1) * Textures.RibbonWidth,
                        Textures.RibbonWidth,
                        Textures.RibbonWidth);
                    break;
                case Config.PrestigeProgressionStyle.StackedStars:
                    sourceRect = new Rectangle(0, (count - 1) * 16, Textures.StarsWidth, 16);
                    break;
                default:
                    sourceRect = Rectangle.Empty;
                    break;
            }

            b.Draw(
                Textures.PrestigeSheetTx,
                position,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                ProfessionsModule.Config.ProgressionStyle == Config.PrestigeProgressionStyle.StackedStars
                    ? Textures.StarsScale
                    : Textures.RibbonScale,
                SpriteEffects.None,
                1f);
        }

        if (SCSkill.Loaded.Count == 0)
        {
            return;
        }

        if (ProfessionsModule.Config.ProgressionStyle is Config.PrestigeProgressionStyle.Gen3Ribbons
            or Config.PrestigeProgressionStyle.Gen4Ribbons)
        {
            position.X += 2; // not sure why but custom skill ribbons render with a small offset
        }

        var customSkills = SpaceCoreIntegration.Instance!.ModApi!
            .GetCustomSkills()
            .Select(name => SCSkill.Loaded[name]);
        if (LuckSkill.Instance is not null)
        {
            // luck skill must be enumerated first
            customSkills = customSkills.Prepend(LuckSkill.Instance);
        }

        var indexWithLuckSkill =
            Reflector.GetUnboundPropertyGetter<NewSkillsPage, int>(page, "GameSkillCount").Invoke(page) - 1;
        foreach (var skill in customSkills)
        {
            // hide if scrolled and out of bounds
            if (indexWithLuckSkill < skillScrollOffset || indexWithLuckSkill > lastVisibleSkillIndex)
            {
                indexWithLuckSkill++;
                continue;
            }

            position.Y += 56;

            var count = Game1.player.GetProfessionsForSkill(skill, true).Length;
            if (count == 0)
            {
                indexWithLuckSkill++;
                continue;
            }

            var sourceRect = ProfessionsModule.Config.ProgressionStyle switch
            {
                Config.PrestigeProgressionStyle.Gen3Ribbons or Config.PrestigeProgressionStyle.Gen4Ribbons => new Rectangle(
                    skill.StringId == "blueberry.LoveOfCooking.CookingSkill" ? 111 : 133,
                    (count - 1) * Textures.RibbonWidth,
                    Textures.RibbonWidth,
                    Textures.RibbonWidth),
                Config.PrestigeProgressionStyle.StackedStars =>
                    new Rectangle(0, (count - 1) * 16, Textures.StarsWidth, 16),
                _ => Rectangle.Empty,
            };

            b.Draw(
                Textures.PrestigeSheetTx,
                position,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                ProfessionsModule.Config.ProgressionStyle == Config.PrestigeProgressionStyle.StackedStars
                    ? Textures.StarsScale
                    : Textures.RibbonScale,
                SpriteEffects.None,
                1f);

            indexWithLuckSkill++;
        }
    }

    #endregion injected subroutines
}
