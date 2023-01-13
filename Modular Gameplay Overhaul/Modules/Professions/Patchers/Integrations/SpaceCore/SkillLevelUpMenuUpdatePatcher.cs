namespace DaLion.Overhaul.Modules.Professions.Patchers.Integrations;

#region using directives

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using SpaceCore;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class SkillLevelUpMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillLevelUpMenuUpdatePatcher"/> class.</summary>
    internal SkillLevelUpMenuUpdatePatcher()
    {
        this.Target = this.RequireMethod<SkillLevelUpMenu>(nameof(SkillLevelUpMenu.update), new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    private static void SkillLevelUpMenuUpdatePrefix(
        int ___currentLevel,
        bool ___hasUpdatedProfessions,
        ref bool ___informationUp,
        ref bool ___isActive,
        ref bool ___isProfessionChooser,
        List<int> ___professionsToChoose)
    {
        if (!___isProfessionChooser || !___hasUpdatedProfessions ||
            !ShouldSuppressClick(___professionsToChoose[0], ___currentLevel) ||
            !ShouldSuppressClick(___professionsToChoose[1], ___currentLevel))
        {
            return;
        }

        ___isActive = false;
        ___informationUp = false;
        ___isProfessionChooser = false;
    }

    /// <summary>Patch to prevent duplicate profession acquisition.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SkillLevelUpMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (pair2.Level == currentLevel)
        // To: if (pair2.Level == currentLevel || pair2.Level == currentLevel + 10)
        try
        {
            var isTheRightLevel = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_3),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Skills.Skill.ProfessionPair).RequirePropertyGetter(nameof(Skills.Skill.ProfessionPair.Level))),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Bne_Un_S) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Beq_S, isTheRightLevel), new CodeInstruction(OpCodes.Ldloc_3),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Skills.Skill.ProfessionPair).RequirePropertyGetter(nameof(Skills.Skill.ProfessionPair.Level))),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 10), new CodeInstruction(OpCodes.Add),
                    })
                .Move()
                .AddLabels(isTheRightLevel);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching level 15 and 20 profession choices.\nHelper returned {ex}");
            return null;
        }

        // This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        // From: profPair = null; foreach ( ... )
        // To: profPair = ChooseProfessionPair(skill);
        try
        {
            helper
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldnull) }, ILHelper.SearchOption.First) // find index of initializing profPair to null
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(ChooseProfessionPair))))
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_1),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentSkill")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                    })
                .Move(2)
                .Match(new[] { new CodeInstruction(OpCodes.Endfinally) }, out var count)
                .Remove(count); // remove the entire loop
        }
        catch (Exception ex)
        {
            Log.E(
                "Professions module failed patching 2nd-tier profession choices to reflect last chosen 1st-tier profession." +
                "\n—-- Do NOT report this to SpaceCore's author. ---" +
                $"\nHelper returned {ex}");
            return null;
        }

        var chosenProfession = generator.DeclareLocal(typeof(int));
        var shouldCongratulateFullSkillMastery = generator.DeclareLocal(typeof(bool));

        // From: Game1.player.professions.Add(professionsToChoose[i]);
        // To: if (!Game1.player.professions.AddOrReplace(professionsToChoose[i]))
        try
        {
            helper
                .Repeat(
                    2,
                    _ =>
                    {
                        var dontGetImmediatePerks = generator.DefineLabel();
                        var isNotPrestigeLevel = generator.DefineLabel();
                        helper
                            .Match(
                                new[]
                                {
                                    // find index of adding a profession to the player's list of professions
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(List<int>).RequirePropertyGetter("Item")),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(NetList<int, NetInt>).RequireMethod("Add")),
                                })
                            .Move()
                            .Insert(
                                new[]
                                {
                                    // duplicate chosen profession
                                    new CodeInstruction(OpCodes.Dup),
                                    // store it for later
                                    new CodeInstruction(OpCodes.Stloc_S, chosenProfession),
                                })
                            .ReplaceWith(
                                // replace Add() with AddOrReplace()
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(Shared.Extensions.Collections.CollectionExtensions)
                                        .RequireMethod(
                                            nameof(Shared.Extensions.Collections.CollectionExtensions
                                                .AddOrReplace))
                                        .MakeGenericMethod(typeof(int))))
                            .Move()
                            .Insert(
                                new[]
                                {
                                    // skip adding perks if player already has them
                                    new CodeInstruction(OpCodes.Brfalse_S, dontGetImmediatePerks),
                                })
                            .Match(
                                new[]
                                {
                                    // find isActive = false section
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(OpCodes.Ldc_I4_0),
                                    new CodeInstruction(OpCodes.Stfld),
                                })
                            .Insert(
                                new[]
                                {
                                    // check if current level is above 10 (i.e. prestige level)
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                                    new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                                    new CodeInstruction(OpCodes.Ble_Un_S, isNotPrestigeLevel), // branch out if not
                                    // add chosenProfession + 100 to player's professions
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(Farmer).RequireField(nameof(Farmer.professions))),
                                    new CodeInstruction(OpCodes.Ldc_I4_S, 100),
                                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                                    new CodeInstruction(OpCodes.Add),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Add))),
                                },
                                // branch here if the player already had the chosen profession
                                new[] { dontGetImmediatePerks })
                            .Insert(
                                new[]
                                {
                                    // load the current level onto the stack
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                                    // load the current skill id
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentSkill")),
                                    // check if should congratulate on full prestige
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(
                                            nameof(ShouldCongratulateOnFullSkillMastery))),
                                    // store the bool result for later
                                    new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery),
                                },
                                // branch here if was not prestige level
                                new[] { isNotPrestigeLevel });
                    });
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching level up profession redundancy." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        // Injected: if (shouldCongratulateOnFullPrestige) CongratulateOnFullPrestige(chosenProfession)
        // Before: if (!isActive || !informationUp)
        try
        {
            var dontCongratulateOnFullPrestige = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .GoTo(0)
                .Insert(
                    new[]
                    {
                        // initialize shouldCongratulateOnFullPrestige local variable to false
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery),
                    })
                .Match(
                    new[]
                    {
                        // find index of the section that checks for a return (once LevelUpMenu is no longer needed)
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(SkillLevelUpMenu).RequireField(nameof(SkillLevelUpMenu.isActive))),
                    },
                    ILHelper.SearchOption.Last)
                .Move(-1) // retreat to the start of this section
                .StripLabels(out var labels) // backup and remove branch labels
                .AddLabels(
                    dontCongratulateOnFullPrestige,
                    resumeExecution) // branch here after checking for congratulate or after proposing final question
                .Insert(
                    new[]
                    {
                        // check if should congratulate on full prestige
                        new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                        new CodeInstruction(OpCodes.Brfalse_S, dontCongratulateOnFullPrestige),
                        // if so, push the current skill id onto the stack and call CongratulateOnFullPrestige()
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentSkill")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(CongratulateOnFullSkillMastery))),
                    },
                    // restore backed-up labels
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting level up profession final question.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (!ShouldSuppressClick(chosenProfession[i], currentLevel))
        // Before: leftProfessionColor = Color.Green;
        try
        {
            var skip = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green))),
                    },
                    ILHelper.SearchOption.First)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(SkillLevelUpMenu).RequireField("professionsToChoose")),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(ShouldSuppressClick))),
                        new CodeInstruction(OpCodes.Brtrue, skip),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(SkillLevelUpMenu).RequireField("professionsToChoose")),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(ShouldSuppressClick))),
                        new CodeInstruction(OpCodes.Brtrue, skip),
                    })
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldc_I4, 512), })
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching level up menu choice suppression." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static object? ChooseProfessionPair(object skillInstance, string skillId, int currentLevel)
    {
        if (currentLevel is not (5 or 10) || !SCSkill.Loaded.ContainsKey(skillId))
        {
            return null;
        }

        var professionPairs = Reflector
            .GetUnboundPropertyGetter<object, IEnumerable>(skillInstance, "ProfessionsForLevels")
            .Invoke(skillInstance)
            .Cast<object>()
            .ToList();
        var levelFivePair = professionPairs[0];
        if (currentLevel == 5)
        {
            return levelFivePair;
        }

        var first = Reflector
            .GetUnboundPropertyGetter<object, object>(levelFivePair, "First")
            .Invoke(levelFivePair);
        var second = Reflector
            .GetUnboundPropertyGetter<object, object>(levelFivePair, "Second")
            .Invoke(levelFivePair);
        var firstStringId = Reflector
            .GetUnboundPropertyGetter<object, string>(first, "Id")
            .Invoke(first);
        var secondStringId = Reflector
            .GetUnboundPropertyGetter<object, string>(second, "Id")
            .Invoke(second);
        var firstId = SpaceCoreIntegration.Instance!.ModApi!.GetProfessionId(skillId, firstStringId);
        var secondId = SpaceCoreIntegration.Instance.ModApi.GetProfessionId(skillId, secondStringId);
        var branch = Game1.player.GetMostRecentProfession(firstId.Collect(secondId));
        return branch == firstId ? professionPairs[1] : professionPairs[2];
    }

    private static bool ShouldCongratulateOnFullSkillMastery(int currentLevel, string skillId)
    {
        if (currentLevel != 10 || !SCSkill.Loaded.TryGetValue(skillId, out var scSkill))
        {
            return false;
        }

        var hasAllProfessions = Game1.player.HasAllProfessionsInSkill(scSkill);
        Log.D($"[Prestige]:: Farmer {Game1.player.Name} " + (hasAllProfessions
            ? $" has acquired all professions in the {scSkill} skill and may now gain extended levels."
            : $" does not yet have all professions in the {scSkill} skill."));
        if (hasAllProfessions)
        {
            return true;
        }

        var missingProfessionNames = string.Join(
            ',',
            Game1.player.GetMissingProfessionsInSkill(scSkill)
                .Select(p => p.Title));
        Log.D($"[Prestige]:: Missing professions: {missingProfessionNames}");
        return false;
    }

    private static void CongratulateOnFullSkillMastery(string skillId)
    {
        Game1.drawObjectDialogue(I18n.Get(
            "prestige.levelup.unlocked",
            new { skill = SCSkill.Loaded[skillId].DisplayName }));

        if (!Game1.player.HasAllProfessions(true))
        {
            return;
        }

        string title = I18n.Get("prestige.achievement.title" + (Game1.player.IsMale ? ".male" : ".female"));
        if (!Game1.player.achievements.Contains(title.GetDeterministicHashCode()))
        {
            EventManager.Enable<AchievementUnlockedDayStartedEvent>();
        }
    }

    private static bool ShouldSuppressClick(int hovered, int currentLevel)
    {
        return SCProfession.Loaded.TryGetValue(hovered, out var profession) &&
               ((currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession)) ||
                (currentLevel == 10 && Game1.player.HasProfession(profession)));
    }

    #endregion injected subroutines
}
