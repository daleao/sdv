using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions
{
	internal static class ConsoleCommands
	{
		#region command handlers

		/// <summary>List the current professions of the local player.</summary>
		internal static void PrintLocalPlayerProfessions(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			foreach (var professionsIndex in Game1.player.professions)
			{
				try
				{
					ModEntry.Log($"{Framework.Util.Professions.NameOf(professionsIndex)}", LogLevel.Info);
				}
				catch (IndexOutOfRangeException)
				{
					ModEntry.Log($"Unknown profession index {professionsIndex}", LogLevel.Info);
				}
			}
		}

		/// <summary>Add specified professions to the local player.</summary>
		internal static void AddProfessionsToLocalPlayer(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			if (!args.Any())
			{
				ModEntry.Log("You must specify the professions to add." + GetUsageForAddProfessions(), LogLevel.Warn);
				return;
			}

			List<int> professionsToAdd = new();
			foreach (var arg in args)
			{
				if (arg == "level")
				{
					ModEntry.Log($"Adding all professions for farmer {Game1.player.Name}'s current skill levels.", LogLevel.Info);

					for (var skill = 0; skill < 5; ++skill)
					{
						var currentLevel = Game1.player.getEffectiveSkillLevel(skill);
						if (currentLevel >= 5)
						{
							professionsToAdd.Add(skill * 6);
							professionsToAdd.Add(skill * 6 + 1);
						}

						if (currentLevel >= 10)
						{
							professionsToAdd.Add(skill * 6 + 2);
							professionsToAdd.Add(skill * 6 + 3);
							professionsToAdd.Add(skill * 6 + 4);
							professionsToAdd.Add(skill * 6 + 5);
						}

						while (currentLevel < Game1.player.GetUnmodifiedSkillLevel(skill))
							GetLevelPerk(skill, ++currentLevel);
					}

					Game1.player.newLevels.Clear();
					break;
				}

				if (arg == "all")
				{
					ModEntry.Log($"Adding all professions to farmer {Game1.player.Name}.", LogLevel.Info);

					for (var professionIndex = 0; professionIndex < 30; ++professionIndex) professionsToAdd.Add(professionIndex);

					for (var skill = 0; skill < 5; ++skill)
					{
						var currentLevel = Game1.player.getEffectiveSkillLevel(skill);
						while (currentLevel < 10) GetLevelPerk(skill, ++currentLevel);
					}

					Game1.player.FarmingLevel = 10;
					Game1.player.FishingLevel = 10;
					Game1.player.ForagingLevel = 10;
					Game1.player.MiningLevel = 10;
					Game1.player.CombatLevel = 10;

					Game1.player.newLevels.Clear();
					break;
				}

				if (arg.AnyOf("farming", "fishing", "foraging", "mining", "combat"))
				{
					ModEntry.Log($"Adding all {arg.FirstCharToUpper()} professions to farmer {Game1.player.Name}.", LogLevel.Info);
					var skill = -1;
					switch (arg)
					{
						case "farming":
							skill = 0;
							Game1.player.FarmingLevel = 10;
							break;

						case "fishing":
							skill = 1;
							Game1.player.FishingLevel = 10;
							break;

						case "foraging":
							skill = 2;
							Game1.player.ForagingLevel = 10;
							break;

						case "mining":
							skill = 3;
							Game1.player.MiningLevel = 10;
							break;

						case "combat":
							skill = 4;
							Game1.player.CombatLevel = 10;
							break;
					}

					if (skill <= 0) continue;

					for (var professionIndex = 6 * skill; professionIndex < 6 * (skill + 1); ++professionIndex)
						professionsToAdd.Add(professionIndex);

					var currentLevel = Game1.player.getEffectiveSkillLevel(skill);
					while (currentLevel < 10) GetLevelPerk(skill, ++currentLevel);

					if (Game1.player.newLevels.Count <= 0) continue;

					foreach (var level in Game1.player.newLevels.Where(level => level.X == skill))
						Game1.player.newLevels.Remove(level);
				}
				else if (Framework.Util.Professions.IndexByName.Forward.TryGetValue(arg.FirstCharToUpper(), out var professionIndex) || int.TryParse(arg, out professionIndex))
				{
					ModEntry.Log($"Adding {Framework.Util.Professions.NameOf(professionIndex)} profession to farmer {Game1.player.Name}.", LogLevel.Info);

					professionsToAdd.Add(professionIndex);

					var skill = professionIndex / 6;
					var expectedLevel = professionIndex % 6 >= 2 ? 10 : 5;
					var currentLevel = Game1.player.getEffectiveSkillLevel(skill);
					if (currentLevel < 5 && expectedLevel == 10)
						professionsToAdd.Add(skill * 6 + (professionIndex % 6 >= 4 ? 1 : 0));

					while (currentLevel < expectedLevel) GetLevelPerk(skill, ++currentLevel);

					switch (skill)
					{
						case 0:
							Game1.player.FarmingLevel = expectedLevel;
							break;

						case 1:
							Game1.player.FishingLevel = expectedLevel;
							break;

						case 2:
							Game1.player.ForagingLevel = expectedLevel;
							break;

						case 3:
							Game1.player.MiningLevel = expectedLevel;
							break;

						case 4:
							Game1.player.CombatLevel = expectedLevel;
							break;
					}

					if (Game1.player.newLevels.Count <= 0) continue;
					foreach (var level in Game1.player.newLevels.Where(level => level.X == skill && level.Y <= expectedLevel))
						Game1.player.newLevels.Remove(level);
				}
				else
				{
					ModEntry.Log($"Ignoring unexpected argument {arg}.", LogLevel.Warn);
				}
			}

			LevelUpMenu levelUpMenu = new();
			foreach (var professionIndex in professionsToAdd.Distinct().Except(Game1.player.professions))
			{
				Game1.player.professions.Add(professionIndex);
				levelUpMenu.getImmediateProfessionPerk(professionIndex);
			}

			LevelUpMenu.RevalidateHealth(Game1.player);
		}

		/// <summary>Reset all skills and professions for the local player.</summary>
		internal static void ResetLocalPlayerProfessions(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			Game1.player.FarmingLevel = 0;
			Game1.player.FishingLevel = 0;
			Game1.player.ForagingLevel = 0;
			Game1.player.MiningLevel = 0;
			Game1.player.CombatLevel = 0;
			Game1.player.newLevels.Clear();

			foreach (var professionIndex in Game1.player.professions) LevelUpMenu.removeImmediateProfessionPerk(professionIndex);
			Game1.player.professions.Clear();

			LevelUpMenu.RevalidateHealth(Game1.player);
		}

		/// <summary>Set <see cref="ModEntry.SuperModeCounter"/> to the max value.</summary>
		internal static void SetSuperModeCounter(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			if (!args.Any() || args.Length > 1)
			{
				ModEntry.Log("You must specify a single value.", LogLevel.Warn);
				return;
			}

			if (int.TryParse(args[0], out var value))
				ModEntry.SuperModeCounter = value;
			else
				ModEntry.Log("You must specify an integer value.", LogLevel.Warn);
		}

		/// <summary>Set <see cref="ModEntry.SuperModeCounter"/> to the desired value.</summary>
		internal static void ReadySuperMode(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			ModEntry.SuperModeCounter = ModEntry.SuperModeCounterMax;
		}

		/// <summary>Set all farm animals owned by the local player to the max friendship value.</summary>
		internal static void MaxAnimalFriendship(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			foreach (var animal in Game1.getFarm().getAllFarmAnimals().Where(a => a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer))
				animal.friendshipTowardFarmer.Value = 1000;
		}

		/// <summary>Set all farm animals owned by the local player to the max mood value.</summary>
		internal static void MaxAnimalMood(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			foreach (var animal in Game1.getFarm().getAllFarmAnimals().Where(a => a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer))
				animal.happiness.Value = 255;
		}

		/// <summary>Check current fishing progress.</summary>
		internal static void PrintFishCaughtAudit(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			var fishData = Game1.content.Load<Dictionary<int, string>>(Path.Combine("Data", "Fish")).Where(p => !p.Key.AnyOf(152, 152, 157) && !p.Value.Contains("trap")).ToDictionary(p => p.Key, p => p.Value);
			int numLegendariesCaught = 0, numMaxSizedCaught = 0;
			var caughtFishNames = new List<string>();
			var nonMaxSizedCaught = new Dictionary<string, Tuple<int, int>>();
			var result = "";
			foreach (var p in Game1.player.fishCaught.Pairs)
			{
				if (!fishData.TryGetValue(p.Key, out var specificFishData)) continue;

				var dataFields = specificFishData.Split('/');
				if (Framework.Util.Objects.LegendaryFishNames.Contains(dataFields[0]))
				{
					++numLegendariesCaught;
				}
				else
				{
					if (p.Value[1] >= Convert.ToInt32(dataFields[4]))
						++numMaxSizedCaught;
					else
						nonMaxSizedCaught.Add(dataFields[0], new Tuple<int, int>(p.Value[1], Convert.ToInt32(dataFields[4])));
				}

				caughtFishNames.Add(dataFields[0]);
			}

			var priceMultiplier = Game1.player.professions.Contains(Farmer.angler) ? (numMaxSizedCaught + numMaxSizedCaught * 5).ToString() + '%' : "Zero. You're not an Angler.";
			result += $"Species caught: {Game1.player.fishCaught.Count()}/{fishData.Count}\nMax-sized: {numMaxSizedCaught}/{Game1.player.fishCaught.Count()}\nLegendaries: {numLegendariesCaught}/10\nTotal Angler price bonus: {priceMultiplier}\n\nThe following caught fish are not max-sized:";
			foreach (var fish in nonMaxSizedCaught.Keys) result += $"\n- {fish} (current: {nonMaxSizedCaught[fish].Item1}, max: {nonMaxSizedCaught[fish].Item2})";

			var seasonFish = from specificFishData in fishData.Values
							 where specificFishData.Split('/')[6].Contains(Game1.currentSeason)
							 select specificFishData.Split('/')[0];

			result += "\n\nThe following fish can be caught this season:";
			result = seasonFish.Except(caughtFishNames).Aggregate(result, (current, fish) => current + $"\n- {fish}");

			ModEntry.Log(result, LogLevel.Info);
		}

		/// <summary>Print the current value of every mod data field to the console.</summary>
		internal static void PrintModData(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			var fields = new[] { "ItemsForaged", "MineralsCollected", "ProspectorStreak", "ScavengerStreak", "WaterTrashCollectedThisSeason", "ActiveTaxBonusPercent" };
			foreach (var field in fields)
			{
				var value = ModEntry.Data.ReadField($"{field}");
				if (field == "ActiveTaxBonusPercent" && float.TryParse(value, out var pct)) value = (pct * 100).ToString(CultureInfo.InvariantCulture) + '%';

				ModEntry.Log(
					!string.IsNullOrEmpty(value)
						? $"{field}: {value}"
						: $"Mod data does not contain an entry for {field}.", LogLevel.Info);
			}
		}

		/// <summary>Set a new value to the ItemsForaged data field.</summary>
		internal static void SetItemsForaged(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			if (!args.Any() || args.Length > 1)
			{
				ModEntry.Log("You must specify a single value.", LogLevel.Warn);
				return;
			}

			if (!int.TryParse(args[0], out var value) || value < 0)
			{
				ModEntry.Log("You must specify a positive integer value.", LogLevel.Warn);
				return;
			}

			ModEntry.Data.WriteField("ItemsForaged", args[0]);
			ModEntry.Log($"ItemsForaged set to {args[0]}.", LogLevel.Info);
		}

		/// <summary>Set a new value to the MineralsCollected data field.</summary>
		internal static void SetMineralsCollected(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			if (!args.Any() || args.Length > 1)
			{
				ModEntry.Log("You must specify a single value.", LogLevel.Warn);
				return;
			}

			if (!int.TryParse(args[0], out var value) || value < 0)
			{
				ModEntry.Log("You must specify a positive integer value.", LogLevel.Warn);
				return;
			}

			ModEntry.Data.WriteField("MineralsCollected", args[0]);
			ModEntry.Log($"MineralsCollected set to {args[0]}.", LogLevel.Info);
		}

		/// <summary>Set a new value to the ProspectorStreak data field.</summary>
		internal static void SetProspectorStreak(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			if (!args.Any() || args.Length > 1)
			{
				ModEntry.Log("You must specify a single value.", LogLevel.Warn);
				return;
			}

			if (!int.TryParse(args[0], out var value) || value < 0)
			{
				ModEntry.Log("You must specify a positive integer value.", LogLevel.Warn);
				return;
			}

			ModEntry.Data.WriteField("ProspectorStreak", args[0]);
			ModEntry.Log($"ProspectorStreak set to {args[0]}.", LogLevel.Info);
		}

		/// <summary>Set a new value to the ScavengerStreak data field.</summary>
		internal static void SetScavengerStreak(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			if (!args.Any() || args.Length > 1)
			{
				ModEntry.Log("You must specify a single value.", LogLevel.Warn);
				return;
			}

			if (!int.TryParse(args[0], out var value) || value < 0)
			{
				ModEntry.Log("You must specify a positive integer value.", LogLevel.Warn);
				return;
			}

			ModEntry.Data.WriteField("ScavengerStreak", args[0]);
			ModEntry.Log($"ScavengerStreak set to {args[0]}.", LogLevel.Info);
		}

		/// <summary>Set a new value to the WaterTrashCollectedThisSeason data field.</summary>
		internal static void SetWaterTrashCollectedThisSeason(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			if (!args.Any() || args.Length > 1)
			{
				ModEntry.Log("You must specify a single value.", LogLevel.Warn);
				return;
			}

			if (!int.TryParse(args[0], out var value) || value < 0)
			{
				ModEntry.Log("You must specify a positive integer value.", LogLevel.Warn);
				return;
			}

			ModEntry.Data.WriteField("WaterTrashCollectedThisSeason", args[0]);
			ModEntry.Log($"WaterTrashCollectedThisSeason set to {args[0]}.", LogLevel.Info);
		}

		/// <summary>Print the currently subscribed mod events to the console.</summary>
		internal static void PrintSubscribedEvents(string command, string[] args)
		{
			ModEntry.Log("Currently subscribed events:", LogLevel.Info);
			foreach (var s in ModEntry.Subscriber.SubscribedEvents) ModEntry.Log($"{s}", LogLevel.Info);
		}

		#endregion command handlers

		#region private methods

		/// <summary>Give the local player immediate perks for a skill level.</summary>
		/// <param name="skill">The skill index.</param>
		/// <param name="level">The skill level.</param>
		private static void GetLevelPerk(int skill, int level)
		{
			switch (skill)
			{
				case 1:
					switch (level)
					{
						case 2:
							if (!Game1.player.hasOrWillReceiveMail("fishing2"))
								Game1.addMailForTomorrow("fishing2");
							break;

						case 6:
							if (!Game1.player.hasOrWillReceiveMail("fishing6"))
								Game1.addMailForTomorrow("fishing6");
							break;
					}
					break;

				case 4:
					if (level != 5 && level != 10) Game1.player.maxHealth += 5;
					break;
			}

			Game1.player.health = Game1.player.maxHealth;
			Game1.player.Stamina = Game1.player.maxStamina.Value;
		}

		/// <summary>Tell the dummies how to use the console command.</summary>
		public static string GetUsageForAddProfessions()
		{
			var result = "\n\nUsage: player_addprofessions <argument1> <argument2> ... <argumentN>";
			result += "\nAvailable arguments:";
			result += "\n\t'level' - get all professions and level perks for the local player's current skill levels.";
			result += "\n\t'all' - get all professions, level perks and max out the local player's skills.";
			result += "\n\t'<skill>' - get all professions and perks for and max out the specified skill.";
			result += "\n\t'<profession>' - get the specified profession and level up the corresponding skill if necessary.";
			result += "\n\nExample:";
			result += "\n\tplayer_addprofessions farming fishing scavenger prospector piper";
			return result;
		}

		#endregion private methods
	}
}