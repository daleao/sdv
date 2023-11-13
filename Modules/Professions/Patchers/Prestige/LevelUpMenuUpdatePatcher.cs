namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayStarted;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuUpdatePatcher"/> class.</summary>
    internal LevelUpMenuUpdatePatcher()
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.update), new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    private static void LevelUpMenuUpdatePrefix(
        LevelUpMenu __instance, int ___currentLevel, List<int> ___professionsToChoose)
    {
        if (!__instance.isProfessionChooser || !__instance.hasUpdatedProfessions ||
            !ShouldSuppressClick(___professionsToChoose[0], ___currentLevel) ||
            !ShouldSuppressClick(___professionsToChoose[1], ___currentLevel))
        {
            return;
        }

        __instance.isActive = false;
        __instance.informationUp = false;
        __instance.isProfessionChooser = false;
    }

    /// <summary>Patch to prevent duplicate profession acquisition + display end of level up dialogues.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (currentLevel == 5)
        // To: if (currentLevel is 5 or 15)
        try
        {
            var isLevel5 = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(OpCodes.Ldc_I4_5),
                        new CodeInstruction(OpCodes.Bne_Un_S),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Bne_Un_S) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Beq_S, isLevel5),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 15),
                    })
                .Move()
                .AddLabels(isLevel5);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching level 15 profession choices.\nHelper returned {ex}");
            return null;
        }

        // This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        // From: else if (Game1.player.professions.Contains(currentSkill * 6))
        // To: else if (GetCurrentBranchForSkill(currentSkill) == currentSkill * 6)
        try
        {
            helper
                .Match(
                    new[]
                    {
                        // find index of checking if the player has the the first level 5 profession in the skill
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentSkill")),
                        new CodeInstruction(OpCodes.Ldc_I4_6),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))),
                    },
                    ILHelper.SearchOption.First)
                .GetLabels(out var labels)
                .Remove(2) // remove loading the local player's professions
                .AddLabels(labels)
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(GetCurrentBranchForSkill))),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentSkill")),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))),
                    })
                .Remove() // remove Callvirt Nelist<int, NetInt>.Contains()
                .SetOpCode(OpCodes.Bne_Un_S); // was Brfalse_S
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed patching 2nd-tier profession choices to reflect last chosen 1st-tier profession.\nHelper returned {ex}");
            return null;
        }

        var chosenProfession = generator.DeclareLocal(typeof(int));
        var shouldProposeFinalQuestion = generator.DeclareLocal(typeof(bool));
        var shouldCongratulateFullSkillMastery = generator.DeclareLocal(typeof(bool));

        // From: Game1.player.professions.Add(professionsToChoose[i]);
        //		  getImmediateProfessionPerk(professionsToChoose[i]);
        // To: if (!Game1.player.professions.AddOrReplace(professionsToChoose[i])) getImmediateProfessionPerk(professionsToChoose[i]);
        //     if (currentLevel > 10) Game1.player.professions.Add(100 + professionsToChoose[i]);
        //		- and also -
        // Injected: if (ShouldProposeFinalQuestion(professionsToChoose[i])) shouldProposeFinalQuestion = true;
        //			  if (ShouldCongratulateOnFullPrestige(currentLevel, professionsToChoose[i])) shouldCongratulateOnFullPrestige = true;
        // Before: isActive = false;
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
                                        typeof(LevelUpMenu).RequireField("currentLevel")),
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
                                    // load the chosen profession onto the stack
                                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                                    // check if should propose final question
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(LevelUpMenuUpdatePatcher).RequireMethod(
                                            nameof(ShouldProposeChangeUltimate))),
                                    // store the bool result for later
                                    new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion),
                                    // load the current level onto the stack
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(LevelUpMenu).RequireField("currentLevel")),
                                    // load the chosen profession onto the stack
                                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                                    // check if should congratulate on full prestige
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(LevelUpMenuUpdatePatcher).RequireMethod(
                                            nameof(HasAcquiredLastProfession))),
                                    // store the bool result for later
                                    new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery),
                                },
                                // branch here if was not prestige level
                                new[] { isNotPrestigeLevel });
                    });
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed patching level up profession redundancy and injecting dialogues.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (shouldProposeFinalQuestion) ProposeFinalQuestion(chosenProfession)
        // And: if (shouldCongratulateOnFullPrestige) CongratulateOnFullPrestige(chosenProfession)
        // Before: if (!isActive || !informationUp)
        try
        {
            var dontProposeFinalQuestion = generator.DefineLabel();
            var dontCongratulateOnFullPrestige = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .GoTo(0)
                .Insert(
                    new[]
                    {
                        // initialize shouldProposeFinalQuestion local variable to false
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion),
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
                            typeof(LevelUpMenu).RequireField(nameof(LevelUpMenu.isActive))),
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
                        // check if should propose the final question
                        new CodeInstruction(OpCodes.Ldloc_S, shouldProposeFinalQuestion),
                        new CodeInstruction(OpCodes.Brfalse_S, dontProposeFinalQuestion),
                        // if so, push the chosen profession onto the stack and call ProposeFinalQuestion()
                        new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                        new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(ProposeChangeUltimate))),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    // restore backed-up labels
                    labels)
                .Insert(
                    new[]
                    {
                        // check if should congratulate on full prestige
                        new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                        new CodeInstruction(OpCodes.Brfalse_S, dontCongratulateOnFullPrestige),
                        // if so, push the chosen profession onto the stack and call CongratulateOnFullPrestige()
                        new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(CongratulateForAcquiringLastProfession))),
                    },
                    // branch here after checking for proposal
                    new[] { dontProposeFinalQuestion });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting level up profession final question.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (!ShouldSuppressClick(chosenProfession[i], currentLevel))
        // Before: leftProfessionColor = Color.Green; (x2)
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
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("professionsToChoose")),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(ShouldSuppressClick))),
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
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("professionsToChoose")),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(ShouldSuppressClick))),
                        new CodeInstruction(OpCodes.Brtrue, skip),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldc_I4, 512),
                    })
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching level up menu choice suppression.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool ShouldProposeChangeUltimate(int chosenProfession)
    {
        return ProfessionsModule.Config.EnablePrestige && chosenProfession is >= 26 and < 30 &&
               Game1.player.Get_Ultimate() is not null && Game1.player.Get_Ultimate()!.Value != chosenProfession;
    }

    private static bool HasAcquiredLastProfession(int currentLevel, int chosenProfession)
    {
        if (!ProfessionsModule.Config.EnablePrestige || currentLevel != 10 ||
            !Skill.TryFromValue(chosenProfession / 6, out var skill) || skill == Farmer.luckSkill)
        {
            return false;
        }

        var hasAllProfessions = Game1.player.HasAllProfessionsInSkill(skill);
        Log.D($"[Prestige]: Farmer {Game1.player.Name} " + (hasAllProfessions
            ? $" has acquired all professions in the {skill} skill and may now gain extended levels."
            : $" does not yet have all professions in the {skill} skill."));
        if (hasAllProfessions)
        {
            return true;
        }

#if DEBUG
        var missing = string.Join(
            ',',
            Game1.player
                .GetMissingProfessionsInSkill(skill)
                .Select(p => p.Title));
        Log.D($"[Prestige]: Missing professions: {missing}");
#endif
        return false;
    }

    private static void ProposeChangeUltimate(int chosenProfession, bool shouldCongratulateFullSkillMastery)
    {
        var ulti = Game1.player.Get_Ultimate()!;
        var oldProfession = Profession.FromValue(ulti);
        var newProfession = Profession.FromValue(chosenProfession);
        Game1.currentLocation.createQuestionDialogue(
            I18n.Prestige_LevelUp_Question(
                    oldProfession.Title,
                    Ultimate.FromValue(oldProfession).DisplayName,
                    newProfession.Title,
                    Ultimate.FromValue(newProfession).DisplayName),
            Game1.currentLocation.createYesNoResponses(),
            (_, answer) =>
            {
                if (answer == "Yes")
                {
                    Game1.player.Set_Ultimate(Ultimate.FromValue(chosenProfession));
                }

                if (shouldCongratulateFullSkillMastery)
                {
                    CongratulateForAcquiringLastProfession(chosenProfession);
                }
            });
    }

    private static void CongratulateForAcquiringLastProfession(int chosenProfession)
    {
        if (ProfessionsModule.Config.EnableExtendedProgression)
        {
            Game1.drawObjectDialogue(I18n.Prestige_LevelUp_Unlocked(Skill.FromValue(chosenProfession / 6).DisplayName));
        }

        if (!Game1.player.HasAllProfessions())
        {
            return;
        }

        string title = _I18n.Get("prestige.achievement.title" + (Game1.player.IsMale ? ".male" : ".female"));
        if (!Game1.player.achievements.Contains(title.GetDeterministicHashCode()))
        {
            EventManager.Enable<AchievementUnlockedDayStartedEvent>();
        }
    }

    private static int GetCurrentBranchForSkill(int currentSkill)
    {
        return Game1.player.GetCurrentBranchForSkill(Skill.FromValue(currentSkill));
    }

    private static bool ShouldSuppressClick(int hovered, int currentLevel)
    {
        return Profession.TryFromValue(hovered, out var profession) &&
               ((currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession)) ||
                (currentLevel == 10 && Game1.player.HasProfession(profession)));
    }

    #endregion injected subroutines
}
