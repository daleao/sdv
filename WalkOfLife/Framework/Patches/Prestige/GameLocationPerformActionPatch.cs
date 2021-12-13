﻿using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
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
				    ModEntry.Subscriber.IsSubscribed(typeof(PrestigeDayEndingEvent)))
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