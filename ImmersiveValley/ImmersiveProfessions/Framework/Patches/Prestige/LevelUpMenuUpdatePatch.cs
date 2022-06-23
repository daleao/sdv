namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Menus;

using DaLion.Common;
using DaLion.Common.Data;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Events.GameLoop;
using Extensions;
using Ultimates;

using CollectionExtensions = DaLion.Common.Extensions.Collections.CollectionExtensions;
using Localization = Utility.Localization;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuUpdatePatch()
    {
        Target = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.update), new[] {typeof(GameTime)});
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    private static void LevelUpMenuUpdatePrefix(LevelUpMenu __instance, int ___currentLevel, List<int> ___professionsToChoose)
    {
        if (!__instance.isProfessionChooser || !__instance.hasUpdatedProfessions ||
            !ShouldSuppressClick(___professionsToChoose[0], ___currentLevel) ||
            !ShouldSuppressClick(___professionsToChoose[1], ___currentLevel)) return;

        __instance.isActive = false;
        __instance.informationUp = false;
        __instance.isProfessionChooser = false;
    }

    /// <summary>Patch to prevent duplicate profession acquisition + display end of level up dialogues.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> LevelUpMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (currentLevel == 5)
        /// To: if (currentLevel is 5 or 15)

        var isLevel5 = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Ldc_I4_5),
                    new CodeInstruction(OpCodes.Bne_Un_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Bne_Un_S)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Beq_S, isLevel5),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 15)
                )
                .Advance()
                .AddLabels(isLevel5);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level 15 profession choices. Helper returned {ex}");
            return null;
        }

        /// This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        /// From: else if (Game1.player.professions.Contains(currentSkill * 6))
        /// To: else if (GetCurrentBranchForSkill(currentSkill) == currentSkill * 6)

        try
        {
            helper
                .FindFirst( // find index of checking if the player has the the first level 5 profession in the skill
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentSkill")),
                    new CodeInstruction(OpCodes.Ldc_I4_6),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains)))
                )
                .GetLabels(out var labels)
                .Remove(2) // remove loading the local player's professions
                .AddLabels(labels)
                .Advance(2)
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuUpdatePatch).RequireMethod(nameof(GetCurrentBranchForSkill))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentSkill"))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains)))
                )
                .Remove() // remove Callvirt Nelist<int, NetInt>.Contains()
                .SetOpCode(OpCodes.Bne_Un_S); // was Brfalse_S
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching 2nd-tier profession choices to reflect last chosen 1st-tier profession. Helper returned {ex}");
            return null;
        }

        /// From: Game1.player.professions.Add(professionsToChoose[i]);
        ///		  getImmediateProfessionPerk(professionsToChoose[i]);
        /// To: if (!Game1.player.professions.AddOrReplace(professionsToChoose[i])) getImmediateProfessionPerk(professionsToChoose[i]);
        ///     if (currentLevel > 10) Game1.player.professions.Add(100 + professionsToChoose[i]);
        ///		- and also -
        /// Injected: if (ShouldProposeFinalQuestion(professionsToChoose[i])) shouldProposeFinalQuestion = true;
        ///			  if (ShouldCongratulateOnFullPrestige(currentLevel, professionsToChoose[i])) shouldCongratulateOnFullPrestige = true;
        /// Before: isActive = false;

        var dontGetImmediatePerks = generator.DefineLabel();
        var isNotPrestigeLevel = generator.DefineLabel();
        var chosenProfession = generator.DeclareLocal(typeof(int));
        var shouldProposeFinalQuestion = generator.DeclareLocal(typeof(bool));
        var shouldCongratulateFullSkillMastery = generator.DeclareLocal(typeof(bool));
        var i = 0;
        repeat1:
        try
        {
            helper
                .FindNext( // find index of adding a profession to the player's list of professions
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Callvirt, typeof(NetList<int, NetInt>).RequireMethod("Add"))
                )
                .Advance()
                .Insert(
                    // duplicate chosen profession
                    new CodeInstruction(OpCodes.Dup),
                    // store it for later
                    new CodeInstruction(OpCodes.Stloc_S, chosenProfession)
                )
                .ReplaceWith( // replace Add() with AddOrReplace()
                    new(OpCodes.Call,
                        typeof(CollectionExtensions).RequireMethod(nameof(CollectionExtensions.AddOrReplace))
                            .MakeGenericMethod(typeof(int)))
                )
                .Advance()
                .Insert(
                    // skip adding perks if player already has them
                    new CodeInstruction(OpCodes.Brfalse_S, dontGetImmediatePerks)
                )
                .AdvanceUntil( // find isActive = false section
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stfld)
                )
                .InsertWithLabels(
                    // branch here if the player already had the chosen profession
                    new[] {dontGetImmediatePerks},
                    // check if current level is above 10 (i.e. prestige level)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                    new CodeInstruction(OpCodes.Ble_Un_S, isNotPrestigeLevel), // branch out if not
                    // add chosenProfession + 100 to player's professions
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 100),
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Add)))
                )
                .InsertWithLabels(
                    // branch here if was not prestige level
                    new[] {isNotPrestigeLevel},
                    // load the chosen profession onto the stack
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    // check if should propose final question
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuUpdatePatch).RequireMethod(nameof(ShouldProposeFinalQuestion))),
                    // store the bool result for later
                    new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion),
                    // load the current level onto the stack
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    // load the chosen profession onto the stack
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    // check if should congratulate on full prestige
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuUpdatePatch).RequireMethod(nameof(ShouldCongratulateOnFullSkillMastery))),
                    // store the bool result for later
                    new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level up profession redundancy and injecting dialogues. Helper returned {ex}");
            return null;
        }

        // repeat injection
        if (++i < 2) goto repeat1;

        /// Injected: if (shouldProposeFinalQuestion) ProposeFinalQuestion(chosenProfession)
        /// Aand: if (shouldCongratulateOnFullPrestige) CongratulateOnFullPrestige(chosenProfession)
        /// Before: if (!isActive || !informationUp)

        var dontProposeFinalQuestion = generator.DefineLabel();
        var dontCongratulateOnFullPrestige = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .GoTo(0)
                .Insert(
                    // initialize shouldProposeFinalQuestion local variable to false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion),
                    // initialize shouldCongratulateOnFullPrestige local variable to false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery)
                )
                .FindLast( // find index of the section that checks for a return (once LevelUpMenu is no longer needed)
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField(nameof(LevelUpMenu.isActive)))
                )
                .Retreat() // retreat to the start of this section
                .StripLabels(out var labels) // backup and remove branch labels
                .AddLabels(dontCongratulateOnFullPrestige,
                    resumeExecution) // branch here after checking for congratulate or after proposing final question
                .InsertWithLabels(
                    // restore backed-up labels
                    labels,
                    // check if should propose the final question
                    new CodeInstruction(OpCodes.Ldloc_S, shouldProposeFinalQuestion),
                    new CodeInstruction(OpCodes.Brfalse_S, dontProposeFinalQuestion),
                    // if so, push the chosen profession onto the stack and call ProposeFinalQuestion()
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuUpdatePatch).RequireMethod(nameof(ProposeFinalQuestion))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .InsertWithLabels(
                    // branch here after checking for proposal
                    new[] {dontProposeFinalQuestion},
                    // check if should congratulate on full prestige
                    new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                    new CodeInstruction(OpCodes.Brfalse_S, dontCongratulateOnFullPrestige),
                    // if so, push the chosen profession onto the stack and call CongratulateOnFullPrestige()
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuUpdatePatch).RequireMethod(nameof(CongratulateOnFullSkillMastery)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while injecting level up profession final question. Helper returned {ex}");
            return null;
        }

        /// Injected: if (!ShouldSuppressClick(chosenProfession[i], currentLevel))
        /// Before: leftProfessionColor = Color.Green; (x2)

        var skip = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("professionsToChoose")),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Call, typeof(LevelUpMenuUpdatePatch).RequireMethod(nameof(ShouldSuppressClick))),
                    new CodeInstruction(OpCodes.Brtrue, skip)
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("professionsToChoose")),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Call, typeof(LevelUpMenuUpdatePatch).RequireMethod(nameof(ShouldSuppressClick))),
                    new CodeInstruction(OpCodes.Brtrue, skip)
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4, 512)
                )
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level up menu choice suppression. Helper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int GetCurrentBranchForSkill(int currentSkill)
    {
        return Game1.player.GetCurrentBranchForSkill(Skill.FromValue(currentSkill));
    }

    internal static bool ShouldProposeFinalQuestion(int chosenProfession)
    {
        return ModEntry.Config.EnablePrestige && chosenProfession is >= 26 and < 30 &&
               ModEntry.PlayerState.RegisteredUltimate is not null &&
               (int) ModEntry.PlayerState.RegisteredUltimate.Index != chosenProfession;
    }

    internal static bool ShouldCongratulateOnFullSkillMastery(int currentLevel, int chosenProfession)
    {
        if (currentLevel != 10 || !Skill.TryFromValue(chosenProfession / 6, out var skill) ||
            skill == Skill.Luck) return false;

        var hasAllProfessions = Game1.player.HasAllProfessionsInSkill(skill);
        Log.D($"Farmer {Game1.player.Name} " + (hasAllProfessions
            ? $" has acquired all professions in the {skill} skill and may now gain extended levels."
            : $" does not yet have all professions in the {skill} skill."));
        if (hasAllProfessions) return true;

        var missingProfessions = Game1.player.GetMissingProfessionsInSkill(skill);
        var missingProfessionNames = string.Join(',', missingProfessions.Select(p => p.GetDisplayName()));
        Log.D($"Missing professions: {missingProfessionNames}");
        return false;
    }

    internal static void ProposeFinalQuestion(int chosenProfession, bool shouldCongratulateFullSkillMastery)
    {
        var oldProfession = Profession.FromValue((int) ModEntry.PlayerState.RegisteredUltimate.Index);
        var newProfession = Profession.FromValue(chosenProfession);
        var pronoun = Localization.GetBuffPronoun();
        Game1.currentLocation.createQuestionDialogue(
            ModEntry.i18n.Get("prestige.levelup.question",
                new
                {
                    pronoun,
                    oldProfession = oldProfession.GetDisplayName(Game1.player.IsMale),
                    oldUlti = ModEntry.i18n.Get(oldProfession.StringId.ToLower() + ".ulti"),
                    newProfession = newProfession.GetDisplayName(Game1.player.IsMale),
                    newUlti = ModEntry.i18n.Get(newProfession.StringId.ToLower() + ".ulti")
                }),
            Game1.currentLocation.createYesNoResponses(), delegate(Farmer _, string answer)
            {
                if (answer == "Yes")
                {
                    var newIndex = (UltimateIndex) chosenProfession;
                    ModEntry.PlayerState.RegisteredUltimate =
#pragma warning disable CS8509
                        ModEntry.PlayerState.RegisteredUltimate = newIndex switch
#pragma warning restore CS8509
                        {
                            UltimateIndex.BruteFrenzy => new UndyingFrenzy(),
                            UltimateIndex.PoacherAmbush => new Ambush(),
                            UltimateIndex.PiperPandemic => new Enthrall(),
                            UltimateIndex.DesperadoBlossom => new DeathBlossom()
                        };
                    ModDataIO.WriteData(Game1.player, ModData.UltimateIndex.ToString(), newIndex.ToString());
                }

                if (shouldCongratulateFullSkillMastery) CongratulateOnFullSkillMastery(chosenProfession);
            });
    }

    internal static void CongratulateOnFullSkillMastery(int chosenProfession)
    {
        Game1.drawObjectDialogue(ModEntry.i18n.Get("prestige.levelup.unlocked",
            new {whichSkill = Skill.FromValue(chosenProfession / 6).DisplayName}));

        if (!Game1.player.HasAllProfessions()) return;

        string title =
            ModEntry.i18n.Get("prestige.achievement.name" +
                                               (Game1.player.IsMale ? ".male" : ".female"));
        if (Game1.player.achievements.Contains(title.GetDeterministicHashCode())) return;

        ModEntry.EventManager.Hook<AchievementUnlockedDayStartedEvent>();
    }

    private static bool ShouldSuppressClick(int hovered, int currentLevel)
    {
        return Profession.TryFromValue(hovered, out var profession) &&
               (currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession) ||
                currentLevel == 10 && Game1.player.HasProfession(profession));
    }

    #endregion injected subroutines
}