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

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class LevelUpMenuUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuUpdatePatch()
		{
			Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.update), new[] {typeof(GameTime)});
			Transpiler = new(AccessTools.Method(GetType(), nameof(LevelUpMenuUpdateTranspiler)));
		}

		#region harmony patches

		/// <summary>Patch to prevent duplicate profession acquisition.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> LevelUpMenuUpdateTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Injected: if (Game1.player.professions.Contains(professionsToChoose[0]));
			/// Before: Game1.player.professions.Add(professionsToChoose[0])
			///		- and also -
			/// Injected: if (ShouldProposeFinalQuestion(professionsToChoose[0])) finalQuestion = true;
			/// Before: isActive = false;

			var dontAddProfession = ilGenerator.DefineLabel();
			var chosenProfession = ilGenerator.DeclareLocal(typeof(int));
			var shouldProposeFinalQuestion = ilGenerator.DeclareLocal(typeof(bool));
			var i = 0;
			repeat:
			try
			{
				helper
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).PropertyGetter("Item")),
						new CodeInstruction(OpCodes.Callvirt, typeof(NetList<int, NetInt>).MethodNamed("Add"))
					)
					.RetreatUntil(
						new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player)))
					)
					.ToBufferUntil(
						new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).PropertyGetter("Item"))
					)
					.InsertBuffer()
					.Insert(
						new CodeInstruction(OpCodes.Callvirt, typeof(NetList<int, NetInt>).MethodNamed("Contains")),
						new CodeInstruction(OpCodes.Brtrue_S, dontAddProfession)
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Stfld, typeof(LevelUpMenu).Field(nameof(LevelUpMenu.isActive)))
					)
					.RetreatUntil(
						new CodeInstruction(OpCodes.Ldarg_0)
					)
					.InsertBuffer(2, 4)
					.Insert(
						new CodeInstruction(OpCodes.Dup),
						new CodeInstruction(OpCodes.Stloc_S, chosenProfession),
						new CodeInstruction(OpCodes.Call,
							typeof(LevelUpMenuUpdatePatch).MethodNamed(nameof(ShouldProposeFinalQuestion))),
						new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion)
					)
					.Return(2)
					.AddLabels(dontAddProfession);
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
					.Insert(
						new CodeInstruction(OpCodes.Ldc_I4_0),
						new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion))
					.FindLast(
						new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).Field(nameof(LevelUpMenu.isActive)))
					)
					.Retreat()
					.StripLabels(out var labels) // backup and remove branch labels
					.AddLabels(shouldNotProposeFinalQuestion)
					.Insert(
						new CodeInstruction(OpCodes.Ldloc_S, shouldProposeFinalQuestion),
						new CodeInstruction(OpCodes.Brfalse_S, shouldNotProposeFinalQuestion),
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
			return chosenProfession is >= 26 and < 30 && ModEntry.SuperModeIndex > 0;
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
					new {oldProfessionDisplayName, oldBuff, newProfessionDisplayName, newBuff}),
				Game1.currentLocation.createYesNoResponses(), delegate(Farmer _, string answer)
				{
					if (answer == "Yes") ModEntry.SuperModeIndex = chosenProfession;
				});
		}

		#endregion private methods
	}
}