using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Events;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GameLocationAnswerDialogueActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationAnswerDialogueActionPatch()
		{
			Original = RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
		}

		#region harmony patches

		/// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
		[HarmonyPrefix]
		private static bool GameLocationAnswerDialogueActionPrefix(GameLocation __instance, string questionAndAnswer)
		{
			if (!ModEntry.Config.EnablePrestige || questionAndAnswer != "dogStatue_Yes" &&
				!questionAndAnswer.Contains("professionForget_"))
				return true; // run original logic

			try
			{
				if (questionAndAnswer == "dogStatue_Yes")
				{
					var skillResponses = new List<Response>();
					if (Game1.player.CanResetSkill(SkillType.Farming))
					{
						var costVal = Utility.Prestige.GetPrestigeCost(SkillType.Farming);
						var costStr = costVal > 0
							? " (" +
							  ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal}) + ')'
							: string.Empty;
						skillResponses.Add(new("farming",
							Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604") + costStr));
					}

					if (Game1.player.CanResetSkill(SkillType.Fishing))
					{
						var costVal = Utility.Prestige.GetPrestigeCost(SkillType.Fishing);
						var costStr = costVal > 0
							? " (" +
							  ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal}) + ')'
							: string.Empty;
						skillResponses.Add(new("fishing",
							Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607") + costStr));
					}

					if (Game1.player.CanResetSkill(SkillType.Foraging))
					{
						var costVal = Utility.Prestige.GetPrestigeCost(SkillType.Foraging);
						var costStr = costVal > 0
							? " (" +
							  ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal}) + ')'
							: string.Empty;
						skillResponses.Add(new("foraging",
							Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606") + costStr));
					}

					if (Game1.player.CanResetSkill(SkillType.Mining))
					{
						var costVal = Utility.Prestige.GetPrestigeCost(SkillType.Mining);
						var costStr = costVal > 0
							? " (" +
							  ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal}) + ')'
							: string.Empty;
						skillResponses.Add(new("mining",
							Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605") + costStr));
					}

					if (Game1.player.CanResetSkill(SkillType.Combat))
					{
						var costVal = Utility.Prestige.GetPrestigeCost(SkillType.Combat);
						var costStr = costVal > 0
							? " (" +
							  ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal}) + ')'
							: string.Empty;
						skillResponses.Add(new("combat",
							Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608") + costStr));
					}

					skillResponses.Add(new("cancel",
						Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
					__instance.createQuestionDialogue(ModEntry.ModHelper.Translation.Get("prestige.dogstatue.which"),
						skillResponses.ToArray(), "professionForget");
				}
				else if (questionAndAnswer.Contains("professionForget_"))
				{
					// if cancel do nothing
					var skillName = questionAndAnswer.Split('_')[1];
					if (skillName == "cancel") return false; // don't run original logic

					// get skill type
#pragma warning disable 8509
					var skillType = skillName switch
#pragma warning restore 8509
					{
						"farming" => SkillType.Farming,
						"fishing" => SkillType.Fishing,
						"foraging" => SkillType.Foraging,
						"mining" => SkillType.Mining,
						"combat" => SkillType.Combat
					};

					var cost = Utility.Prestige.GetPrestigeCost(skillType);
					if (cost > 0)
					{
						// check for funds and deduct cost
						if (Game1.player.Money < cost)
						{
							Game1.drawObjectDialogue(
								Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
							return false; // don't run original logic
						}

						Game1.player.Money = Math.Max(0, Game1.player.Money - cost);
					}

					// prepare to prestige at night
					if (ModEntry.Subscriber.TryGet(typeof(PrestigeDayEndingEvent), out var prestigeDayEnding))
						((PrestigeDayEndingEvent) prestigeDayEnding).SkillsToReset.Enqueue(skillType);
					else
						ModEntry.Subscriber.Subscribe(new PrestigeDayEndingEvent(skillType));

					// play sound effect
					try
					{
						if (ModEntry.SoundFX.SoundByName.TryGetValue("prestige", out var sfx))
							sfx.Play(Game1.options.soundVolumeLevel, 0f, 0f);
						else throw new ContentLoadException();
					}
					catch (Exception ex)
					{
						ModEntry.Log($"Couldn't play file 'assets/sfx/prestige.wav'. Make sure the file exists. {ex}",
							LogLevel.Error);
					}

					// tell the player
					Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

					// woof woof
					DelayedAction.playSoundAfterDelay("dog_bark", 1300);
					DelayedAction.playSoundAfterDelay("dog_bark", 1900);
				}

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