using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Events;
using TheLion.Stardew.Professions.Framework.Extensions;
using CollectionExtensions = TheLion.Stardew.Common.Extensions.CollectionExtensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
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
			IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// This injection chooses the correct level 10 profession choices based on the last selected level 5 profession
			/// From: else if (Game1.player.professions.Contains(currentSkill * 6))
			/// To: else if (Game1.player.CurrentBranchForSkill(currentSkill) == currentSkill * 6)

			try
			{
				helper
					.FindFirst( // find index of checking if the player has the the first level 5 profesion in the skill
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
							typeof(FarmerExtensions).MethodNamed(nameof(FarmerExtensions.CurrentBranchForSkill))),
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
				ModEntry.Log(
					$"Failed while patching level 10 profession choices to reflect last chosen level 5 profession. Helper returned {ex}",
					LogLevel.Error);
				return null;
			}

			/// From: Game1.player.professions.Add(professionsToChoose[i]);
			///		  getImmediateProfessionPerk(professionsToChoose[i]);
			/// To: if (!Game1.player.professions.AddOrReplace(professionsToChoose[i])) getImmediateProfessionPerk(professionsToChoose[i]);
			///		- and also -
			/// Injected: if (ShouldProposeFinalQuestion(professionsToChoose[i])) shouldProposeFinalQuestion = true;
			///			  if (ShouldCongratulateOnFullPrestige(professionsToChoose[i])) shouldCongratulateOnFullPrestige = true;
			/// Before: isActive = false;

			var dontGiveImmediatePerks = ilGenerator.DefineLabel();
			var chosenProfession = ilGenerator.DeclareLocal(typeof(int));
			var shouldProposeFinalQuestion = ilGenerator.DeclareLocal(typeof(bool));
			var shouldCongratulateOnFullPrestige = ilGenerator.DeclareLocal(typeof(bool));
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
						new CodeInstruction(OpCodes.Brtrue_S, dontGiveImmediatePerks)
					)
					.AdvanceUntil( // advance until an instruction signaling the end of instructions related to adding a new profession
						new CodeInstruction(OpCodes.Stfld, typeof(LevelUpMenu).Field(nameof(LevelUpMenu.isActive)))
					)
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldarg_0)
					)
					.Insert(
						// branch here if the player already had the chosen profession
						new[] {dontGiveImmediatePerks},
						// load the chosen profession onto the stack
						new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
						// check if should propose final question
						new CodeInstruction(OpCodes.Call,
							typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(ShouldProposeFinalQuestion))),
						// store the bool result for later
						new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion),
						// load the chosen profession onto the stack
						new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
						// check if should congratulate on full prestige
						new CodeInstruction(OpCodes.Call,
							typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(ShouldCongratulateOnFullPrestige))),
						// store the bool result for later
						new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateOnFullPrestige)
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while patching level up profession redundancy and injecting dialogues. Helper returned {ex}",
					LogLevel.Error);
				return null;
			}

			// repeat injection
			if (++i < 2) goto repeat1;

			/// Injected: if (shouldProposeFinalQuestion) ProposeFinalQuestion(chosenProfession)
			/// Aand: if (shouldCongratulateOnFullPrestige) CongratulateOnFullPrestige(chosenProfession)
			/// Before: if (!isActive || !informationUp)

			var dontProposeFinalQuestion = ilGenerator.DefineLabel();
			var dontCongratulateOnFullPrestige = ilGenerator.DefineLabel();
			var resumeExecution = ilGenerator.DefineLabel();
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
						new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateOnFullPrestige)
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
						new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateOnFullPrestige),
						new CodeInstruction(OpCodes.Call,
							typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(ProposeFinalQuestion))),
						new CodeInstruction(OpCodes.Br_S, resumeExecution)
					)
					.Insert(
						// branch here after checking for proposal
						new[] {dontProposeFinalQuestion},
						// check if should congratulate on full prestige
						new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateOnFullPrestige),
						new CodeInstruction(OpCodes.Brfalse_S, dontCongratulateOnFullPrestige),
						// if so, push the chosen profession onto the stack and call CongratulateOnFullPrestige()
						new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
						new CodeInstruction(OpCodes.Call,
							typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(CongratulateOnFullPrestige)))
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while injecting level up profession final question. Helper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
		
		#region private methods

		private static bool ShouldProposeFinalQuestion(int chosenProfession)
		{
			return ModEntry.Config.EnablePrestige && ModState.SuperModeIndex > 0 && chosenProfession is >= 26 and < 30 &&
			       ModState.SuperModeIndex != chosenProfession;
		}

		private static bool ShouldCongratulateOnFullPrestige(int chosenProfession)
		{
			return ModEntry.Config.EnableExtendedProgression && Game1.player.HasAllProfessionsInSkill(chosenProfession / 6);
		}

		private static void ProposeFinalQuestion(int chosenProfession, bool shouldCongratulateOnFullPrestige)
		{
			var oldProfessionKey = Utility.Professions.NameOf(ModState.SuperModeIndex).ToLower();
			var oldProfessionDisplayName = ModEntry.ModHelper.Translation.Get(oldProfessionKey + ".name.male");
			var oldBuff = ModEntry.ModHelper.Translation.Get(oldProfessionKey + ".buff");
			var newProfessionKey = Utility.Professions.NameOf(chosenProfession);
			var newProfessionDisplayName = ModEntry.ModHelper.Translation.Get(newProfessionKey + ".name.male");
			var newBuff = ModEntry.ModHelper.Translation.Get(newProfessionKey + ".buff");
			Game1.currentLocation.createQuestionDialogue(
				ModEntry.ModHelper.Translation.Get("prestige.levelup.question",
					new
					{
						oldProfession = oldProfessionDisplayName, oldBuff, newProfession = newProfessionDisplayName,
						newBuff
					}),
				Game1.currentLocation.createYesNoResponses(), delegate(Farmer _, string answer)
				{
					if (answer == "Yes") ModState.SuperModeIndex = chosenProfession;
					if (shouldCongratulateOnFullPrestige) CongratulateOnFullPrestige(chosenProfession);
				});
		}

		private static void CongratulateOnFullPrestige(int chosenProfession)
		{
			Game1.drawObjectDialogue(ModEntry.ModHelper.Translation.Get("prestige.levelup.unlocked",
				new {whichSkill = Farmer.getSkillDisplayNameFromIndex(chosenProfession / 6)}));

			if (!Game1.player.HasAllProfessions()) return;
			
			string name = ModEntry.ModHelper.Translation.Get("prestige.achievement.name." + (Game1.player.IsMale ? "male" : "female"));
			if (Game1.player.achievements.Contains(name.Hash())) return;

			ModEntry.Subscriber.Subscribe(new AchievementUnlockedDayStartedEvent());
		}

		#endregion private methods
	}
}