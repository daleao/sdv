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
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class LevelUpMenuUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuUpdatePatch()
		{
			Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.update), new[] {typeof(GameTime)});
			Transpiler = new(GetType().MethodNamed(nameof(LevelUpMenuUpdateTranspiler)));
		}

		#region harmony patches

		/// <summary>Patch to prevent duplicate profession acquisition.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> LevelUpMenuUpdateTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

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
						new CodeInstruction(OpCodes.Call, typeof(FarmerExtensions).MethodNamed(nameof(FarmerExtensions.CurrentBranchForSkill))),
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
			/// Injected: if (ShouldProposeFinalQuestion(professionsToChoose[i])) finalQuestion = true;
			/// Before: isActive = false;

			var dontGiveImmediatePerks = ilGenerator.DefineLabel();
			var chosenProfession = ilGenerator.DeclareLocal(typeof(int));
			var shouldProposeFinalQuestion = ilGenerator.DeclareLocal(typeof(bool));
			var i = 0;
			repeat:
			try
			{
				helper
					.FindNext( // find index of adding a profession to the player's list of professions
						new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).PropertyGetter("Item")),
						new CodeInstruction(OpCodes.Callvirt, typeof(NetList<int, NetInt>).MethodNamed("Add"))
					)
					.Advance()
					.ReplaceWith( // replace Add() with AddOrReplace()
						new CodeInstruction(OpCodes.Call,
							typeof(Common.Extensions.CollectionExtensions)
								.MethodNamed(nameof(Common.Extensions.CollectionExtensions.AddOrReplace))
								.MakeGenericMethod(typeof(int)))
					)
					.Advance()
					.Insert(
						// skip adding perks if player already has them
						new CodeInstruction(OpCodes.Brtrue_S, dontGiveImmediatePerks)
					)
					.Advance()
					.ToBufferUntil( // copy instructions to push chosen profession onto the stack
						stripLabels: false,
						advance: true,
						new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).PropertyGetter("Item"))
					)
					.AdvanceUntil( // advance until an instruction signaling the end of instructions related to adding a new profession
						new CodeInstruction(OpCodes.Ldarg_0)
					)
					.InsertBuffer() // paste instructions to push the chosen profession onto the stack
					.Insert(
						// duplicate the chosen profession
						new CodeInstruction(OpCodes.Dup),
						// store the first copy to a local variable for later
						new CodeInstruction(OpCodes.Stloc_S, chosenProfession),
						// the second copy will be consumed by the ShouldProposeFinalQuestion() check
						new CodeInstruction(OpCodes.Call,
							typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(ShouldProposeFinalQuestion))),
						// store the bool result onto another local variable
						new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion)
					)
					.Return(2) // return to the start of the section which calls ShouldProposeFinalQuestion
					.AddLabels(dontGiveImmediatePerks); // branch here if the player already had the chosen profession 
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while patching level up profession redundancy and injecting final question decision. Helper returned {ex}",
					LogLevel.Error);
				return null;
			}

			// repeat injection
			if (++i < 2) goto repeat;

			/// Injected: ProposeFinalQuestion(chosenProfession)
			/// Before: if (!isActive || !informationUp)

			var shouldNotProposeFinalQuestion = ilGenerator.DefineLabel();
			try
			{
				helper
					.ReturnToFirst()
					.Insert( // initialize shouldProposeFinalQuestion local variable to false
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion))
					.FindLast( // find index of the section that checks for a return (once LevelUpMenu is no longer needed)
						new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).Field(nameof(LevelUpMenu.isActive)))
					)
					.Retreat() // retreat to the start of this section
					.StripLabels(out var labels) // backup and remove branch labels
					.AddLabels(shouldNotProposeFinalQuestion) // branch here if should not propose the final question
					.Insert(
						// check if should propose the final question
						new CodeInstruction(OpCodes.Ldloc_S, shouldProposeFinalQuestion),
						new CodeInstruction(OpCodes.Brfalse_S, shouldNotProposeFinalQuestion),
						// if so, push the chosen profession onto the stack and call ProposeFinalQuestion()
						new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
						new CodeInstruction(OpCodes.Call,
							typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(ProposeFinalQuestion)))
					)
					.Return()
					.AddLabels(labels);
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
			return ModEntry.SuperModeIndex > 0 && chosenProfession is >= 26 and < 30 && ModEntry.SuperModeIndex != chosenProfession;
		}

		private static void ProposeFinalQuestion(int chosenProfession)
		{
			var oldProfessionKey = Utility.Professions.NameOf(ModEntry.SuperModeIndex).ToLower();
			var oldProfessionDisplayName = ModEntry.ModHelper.Translation.Get(oldProfessionKey + ".name.male");
			var oldBuff = ModEntry.ModHelper.Translation.Get(oldProfessionKey + ".buff");
			var newProfessionKey = Utility.Professions.NameOf(chosenProfession);
			var newProfessionDisplayName = ModEntry.ModHelper.Translation.Get(newProfessionKey + ".name.male");
			var newBuff = ModEntry.ModHelper.Translation.Get(newProfessionKey + ".buff");
			Game1.currentLocation.createQuestionDialogue(
				ModEntry.ModHelper.Translation.Get("prestige.levelup.question",
					new {oldProfession = oldProfessionDisplayName, oldBuff = oldBuff, newProfession = newProfessionDisplayName, newBuff = newBuff}),
				Game1.currentLocation.createYesNoResponses(), delegate(Farmer _, string answer)
				{
					if (answer == "Yes") ModEntry.SuperModeIndex = chosenProfession;
				});
		}

		#endregion private methods
	}
}