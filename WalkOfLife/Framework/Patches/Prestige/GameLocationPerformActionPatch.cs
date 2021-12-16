using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Events;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GameLocationPerformActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationPerformActionPatch()
		{
			Original = RequireMethod<GameLocation>(nameof(GameLocation.performAction));
		}

		#region harmony patches

		/// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
		[HarmonyPrefix]
		private static bool GameLocationPerformActionPrefix(GameLocation __instance, string action, Farmer who)
		{
			if (!ModEntry.Config.EnablePrestige || action is null || action.Split(' ')[0] != "DogStatue" ||
			    !who.IsLocalPlayer)
				return true; // run original logic

			try
			{
				string message;
				if (!ModEntry.Config.AllowMultipleResetsPerDay &&
				    ModEntry.Subscriber.IsSubscribed(typeof(PrestigeDayEndingEvent)) || ModState.ChangedSuperModeToday)
				{
					message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.dismiss");
					Game1.drawObjectDialogue(message);
					return false; // don't run original logic
				}

				if (who.CanResetAnySkill())
				{
					message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.first");
					if (ModEntry.Config.ForgetRecipesOnSkillReset)
						message += ModEntry.ModHelper.Translation.Get("prestige.dogstatue.forget");
					message += ModEntry.ModHelper.Translation.Get("prestige.dogstatue.offer");

					__instance.createQuestionDialogue(message, __instance.createYesNoResponses(), "dogStatue");
					return false; // don't run original logic
				}
				
				if (who.HasAllProfessions() && !ModState.ChangedSuperModeToday)
				{
					var currentProfessionKey = Utility.Professions.NameOf(ModState.SuperModeIndex).ToLower();
					var currentProfessionDisplayName = ModEntry.ModHelper.Translation.Get(currentProfessionKey + ".name.male");
					var currentBuff = ModEntry.ModHelper.Translation.Get(currentProfessionKey + ".buff");
					var pronoun = Utility.Professions.GetBuffPronoun();
					message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.replace",
						new {pronoun, currentProfession = currentProfessionDisplayName, currentBuff});

					var choices = (
						from superMode in who.GetUnchosenSuperModes()
						orderby superMode
						let choiceProfessionKey = Utility.Professions.NameOf(superMode).ToLower()
						let choiceProfessionDisplayName =
							ModEntry.ModHelper.Translation.Get(choiceProfessionKey + ".name.male")
						let choiceBuff = ModEntry.ModHelper.Translation.Get(choiceProfessionKey + ".buff")
						let choice =
							ModEntry.ModHelper.Translation.Get("prestige.dogstatue.choice",
								new {choiceProfession = choiceProfessionDisplayName, choiceBuff})
						select new Response("Choice_" + superMode, choice)).ToList();

					choices.Add(new Response("Cancel", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))
						.SetHotKey(Keys.Escape));

					__instance.createQuestionDialogue(message, choices.ToArray(), delegate (Farmer _, string choice)
					{
						if (choice == "Cancel") return;
						var newIndex = int.Parse(choice.Split("_")[1]);
						ModState.SuperModeIndex = newIndex;

						ModEntry.SoundBox.Play("prestige");

						var choiceProfessionKey = Utility.Professions.NameOf(newIndex).ToLower();
						var choiceProfessionDisplayName =
							ModEntry.ModHelper.Translation.Get(choiceProfessionKey +
							                                   (who.IsMale ? ".name.male" : ".name.female"));
						pronoun = ModEntry.ModHelper.Translation.Get("pronoun.indefinite" + (who.IsMale ? ".male" : ".female"));
						Game1.drawObjectDialogue(ModEntry.ModHelper.Translation.Get("prestige.dogstatue.fledged",
							new {pronoun, choiceProfession = choiceProfessionDisplayName}));

						ModState.ChangedSuperModeToday = true;
					});
					return false; // don't run original logic
				}

				message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.first");
				Game1.drawObjectDialogue(message);
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}