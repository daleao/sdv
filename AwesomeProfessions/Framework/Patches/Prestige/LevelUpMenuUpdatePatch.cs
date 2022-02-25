namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Menus;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Events.GameLoop;
using Extensions;
using SuperMode;

using CollectionExtensions = Stardew.Common.Extensions.CollectionExtensions;
using Localization = Utility.Localization;

#endregion using directives

[UsedImplicitly]
internal class LevelUpMenuUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuUpdatePatch()
    {
        Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.update), new[] {typeof(GameTime)});
    }

    #region harmony patches

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
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).Field("currentLevel")),
                    new CodeInstruction(OpCodes.Ldc_I4_5),
                    new CodeInstruction(OpCodes.Bne_Un_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Bne_Un_S)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Beq_S, isLevel5),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).Field("currentLevel")),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 15)
                )
                .Advance()
                .AddLabels(isLevel5);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level 15 profession choices. Helper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        /// From: else if (Game1.player.professions.Contains(currentSkill * 6))
        /// To: else if (Game1.player.CurrentBranchForSkill(currentSkill) == currentSkill * 6)

        try
        {
            helper
                .FindFirst( // find index of checking if the player has the the first level 5 profession in the skill
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).Field(nameof(Farmer.professions))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).Field("currentSkill")),
                    new CodeInstruction(OpCodes.Ldc_I4_6),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains)))
                )
                .Remove() // remove Ldfld Farmer.professions
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_6)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions).MethodNamed(nameof(FarmerExtensions.GetCurrentBranchForSkill))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).Field("currentSkill"))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Contains)))
                )
                .Remove() // remove Callvirt Nelist<int, NetInt>.Contains()
                .SetOpCode(OpCodes.Bne_Un_S); // was Brfalse_S
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching 2nd-tier profession choices to reflect last chosen 1st-tier profession. Helper returned {ex}");
            transpilationFailed = true;
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
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).PropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Callvirt, typeof(NetList<int, NetInt>).MethodNamed("Add"))
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
                        typeof(CollectionExtensions)
                            .MethodNamed(nameof(CollectionExtensions.AddOrReplace))
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
                .Insert(
                    // branch here if the player already had the chosen profession
                    new[] {dontGetImmediatePerks},
                    // check if current level is above 10 (i.e. prestige level)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).Field("currentLevel")),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                    new CodeInstruction(OpCodes.Ble_Un_S, isNotPrestigeLevel), // branch out if not
                    // add chosenProfession + 100 to player's professions
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).Field(nameof(Farmer.professions))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 100),
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(NetList<int, NetInt>).MethodNamed(nameof(NetList<int, NetInt>.Add)))
                )
                .Insert(
                    // branch here if was not prestige level
                    new[] {isNotPrestigeLevel},
                    // load the chosen profession onto the stack
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    // check if should propose final question
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(ShouldProposeFinalQuestion))),
                    // store the bool result for later
                    new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion),
                    // load the current level onto the stack
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).Field("currentLevel")),
                    // load the chosen profession onto the stack
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    // check if should congratulate on full prestige
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(ShouldCongratulateOnFullSkillMastery))),
                    // store the bool result for later
                    new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level up profession redundancy and injecting dialogues. Helper returned {ex}");
            transpilationFailed = true;
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
                .ReturnToFirst()
                .Insert(
                    // initialize shouldProposeFinalQuestion local variable to false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion),
                    // initialize shouldCongratulateOnFullPrestige local variable to false
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery)
                )
                .FindLast( // find index of the section that checks for a return (once LevelUpMenu is no longer needed)
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).Field(nameof(LevelUpMenu.isActive)))
                )
                .Retreat() // retreat to the start of this section
                .StripLabels(out var labels) // backup and remove branch labels
                .AddLabels(dontCongratulateOnFullPrestige,
                    resumeExecution) // branch here after checking for congratulate or after proposing final question
                .Insert(
                    // restore backed-up labels
                    labels,
                    // check if should propose the final question
                    new CodeInstruction(OpCodes.Ldloc_S, shouldProposeFinalQuestion),
                    new CodeInstruction(OpCodes.Brfalse_S, dontProposeFinalQuestion),
                    // if so, push the chosen profession onto the stack and call ProposeFinalQuestion()
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(ProposeFinalQuestion))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Insert(
                    // branch here after checking for proposal
                    new[] {dontProposeFinalQuestion},
                    // check if should congratulate on full prestige
                    new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                    new CodeInstruction(OpCodes.Brfalse_S, dontCongratulateOnFullPrestige),
                    // if so, push the chosen profession onto the stack and call CongratulateOnFullPrestige()
                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(CongratulateOnFullSkillMastery)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while injecting level up profession final question. Helper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool ShouldProposeFinalQuestion(int chosenProfession)
    {
        return ModEntry.Config.EnablePrestige && chosenProfession is >= 26 and < 30 &&
               ModEntry.PlayerState.Value.SuperMode is not null &&
               (int) ModEntry.PlayerState.Value.SuperMode.Index != chosenProfession;
    }

    private static bool ShouldCongratulateOnFullSkillMastery(int currentLevel, int chosenProfession)
    {
        return currentLevel == 10 && Game1.player.HasAllProfessionsInSkill(chosenProfession / 6);
    }

    private static void ProposeFinalQuestion(int chosenProfession, bool shouldCongratulateFullSkillMastery)
    {
        var oldProfessionKey = ModEntry.PlayerState.Value.SuperMode.Index.ToString().ToLower();
        var oldProfessionDisplayName = ModEntry.ModHelper.Translation.Get(oldProfessionKey + ".name.male");
        var oldBuff = ModEntry.ModHelper.Translation.Get(oldProfessionKey + ".buff");
        var newProfessionKey = chosenProfession.ToProfessionName();
        var newProfessionDisplayName = ModEntry.ModHelper.Translation.Get(newProfessionKey + ".name.male");
        var newBuff = ModEntry.ModHelper.Translation.Get(newProfessionKey + ".buff");
        var pronoun = Localization.GetBuffPronoun();
        Game1.currentLocation.createQuestionDialogue(
            ModEntry.ModHelper.Translation.Get("prestige.levelup.question",
                new
                {
                    pronoun,
                    oldProfession = oldProfessionDisplayName,
                    oldBuff,
                    newProfession = newProfessionDisplayName,
                    newBuff
                }),
            Game1.currentLocation.createYesNoResponses(), delegate(Farmer _, string answer)
            {
                if (answer == "Yes")
                {
                    var newIndex = (SuperModeIndex) chosenProfession;
                    ModEntry.PlayerState.Value.SuperMode =
#pragma warning disable CS8509
                        ModEntry.PlayerState.Value.SuperMode = newIndex switch
#pragma warning restore CS8509
                        {
                            SuperModeIndex.Brute => new BruteFury(),
                            SuperModeIndex.Poacher => new PoacherColdBlood(),
                            SuperModeIndex.Piper => new PiperEubstance(),
                            SuperModeIndex.Desperado => new DesperadoTemerity()
                        };
                    Game1.player.WriteData(DataField.SuperModeIndex, newIndex.ToString());
                }

                if (shouldCongratulateFullSkillMastery) CongratulateOnFullSkillMastery(chosenProfession);
            });
    }

    private static void CongratulateOnFullSkillMastery(int chosenProfession)
    {
        Game1.drawObjectDialogue(ModEntry.ModHelper.Translation.Get("prestige.levelup.unlocked",
            new {whichSkill = Farmer.getSkillDisplayNameFromIndex(chosenProfession / 6)}));

        if (!Game1.player.HasAllProfessions()) return;

        string name =
            ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                               (Game1.player.IsMale ? "male" : "female"));
        if (Game1.player.achievements.Contains(name.GetDeterministicHashCode())) return;

        EventManager.Enable(typeof(AchievementUnlockedDayStartedEvent));
    }

    #endregion injected subroutines
}