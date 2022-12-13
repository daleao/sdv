namespace DaLion.Overhaul.Modules.Professions.Patchers.Integrations;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Patchers.Prestige;
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
internal sealed class NewSkillsPageDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewSkillsPageDrawPatcher"/> class.</summary>
    internal NewSkillsPageDrawPatcher()
    {
        this.Target = this.RequireMethod<NewSkillsPage>(nameof(NewSkillsPage.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? NewSkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Inject: x -= ArsenalModule.Config.Slingshots.PrestigeProgressionStyle == ProgressionStyle.Stars ? Textures.STARS_WIDTH_I : Textures.RIBBON_WIDTH_I;
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
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ProfessionsConfig).RequirePropertyGetter(nameof(ProfessionsConfig.EnablePrestige))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ProfessionsConfig).RequirePropertyGetter(
                                nameof(ProfessionsConfig.PrestigeProgressionStyle))),
                        new CodeInstruction(OpCodes.Ldc_I4_0), new CodeInstruction(OpCodes.Beq_S, notRibbons),
                        new CodeInstruction(
                            OpCodes.Ldc_I4_S,
                            (int)((Textures.RibbonWidth + 5) * Textures.RibbonScale)),
                        new CodeInstruction(OpCodes.Sub), new CodeInstruction(OpCodes.Br_S, resumeExecution),
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
            Log.E("Immersive Professions failed adjusing localized skill page content position." +
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
                        new CodeInstruction(OpCodes.Ldloc_S, levelIndex), new CodeInstruction(OpCodes.Ldc_I4_S, 9),
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
            Log.E("Immersive Professions Failed patching to draw SpaceCore skills page extended level bars." +
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
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel), new CodeInstruction(OpCodes.Ldc_I4_S, 20),
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
            Log.E("Immersive Professions Failed patching to draw max skill level with different color." +
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
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillsPageDrawPatcher).RequireMethod(nameof(SkillsPageDrawPatcher.DrawRibbons))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E("Immersive Professions Failed patching to draw skills page prestige ribbons." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
