using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	public partial class AwesomeProfessions
	{
		/// <summary>Add specified professions to the local player.</summary>
		/// <param name="command">The console command.</param>
		/// <param name="args">The supplied arguments.</param>
		private void _AddProfessionsToLocalPlayer(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				Monitor.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			if (!args.Any())
			{
				Monitor.Log("You must specify the professions to add." + _GetCommandUsage(), LogLevel.Warn);
				return;
			}

			List<int> professionsToAdd = new();
			foreach (var arg in args)
			{
				if (arg.Equals("level"))
				{
					Monitor.Log($"Adding all professions for farmer {Game1.player.Name}'s current skill levels.", LogLevel.Info);

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
							_GetLevelPerk(skill, ++currentLevel);
					}

					Game1.player.newLevels.Clear();
					break;
				}

				if (arg.Equals("all"))
				{
					Monitor.Log($"Adding all professions to farmer {Game1.player.Name}.", LogLevel.Info);

					for (var professionIndex = 0; professionIndex < 30; ++professionIndex) professionsToAdd.Add(professionIndex);

					for (var skill = 0; skill < 5; ++skill)
					{
						var currentLevel = Game1.player.getEffectiveSkillLevel(skill);
						while (currentLevel < 10) _GetLevelPerk(skill, ++currentLevel);
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
					Monitor.Log($"Adding all {arg.FirstCharToUpper()} professions to farmer {Game1.player.Name}.", LogLevel.Info);
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
					while (currentLevel < 10) _GetLevelPerk(skill, ++currentLevel);

					if (Game1.player.newLevels.Count <= 0) continue;

					foreach (var level in Game1.player.newLevels.Where(level => level.X == skill))
						Game1.player.newLevels.Remove(level);
				}
				else if (Utility.ProfessionMap.Forward.TryGetValue(arg.FirstCharToUpper(), out var professionIndex) || int.TryParse(arg, out professionIndex))
				{
					Monitor.Log($"Adding {Utility.ProfessionMap.Reverse[professionIndex]} profession to farmer {Game1.player.Name}.", LogLevel.Info);

					professionsToAdd.Add(professionIndex);

					var skill = professionIndex / 6;
					var expectedLevel = professionIndex % 6 > 2 ? 10 : 5;
					var currentLevel = Game1.player.getEffectiveSkillLevel(skill);
					if (currentLevel < 5 && expectedLevel == 10)
						professionsToAdd.Add(skill * 6 + (professionIndex % 6 > 3 ? 1 : 0));

					while (currentLevel < expectedLevel) _GetLevelPerk(skill, ++currentLevel);

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
					Monitor.Log($"Ignoring unexpected argument {arg}.", LogLevel.Warn);
				}
			}

			foreach (var professionIndex in professionsToAdd.Distinct().Except(Game1.player.professions))
			{
				Game1.player.professions.Add(professionIndex);
				Utility.InitializeModData(professionIndex);
				EventManager.SubscribeEventsForProfession(professionIndex);
			}
		}

		/// <summary>Reset all skills and professions for the local player.</summary>
		/// <param name="command">The console command.</param>
		/// <param name="args">The supplied arguments.</param>
		private void _ResetLocalPlayerProfessions(string command, string[] args)
		{
			Game1.player.FarmingLevel = 0;
			Game1.player.FishingLevel = 0;
			Game1.player.ForagingLevel = 0;
			Game1.player.MiningLevel = 0;
			Game1.player.CombatLevel = 0;
			Game1.player.newLevels.Clear();
			Game1.player.professions.Clear();
			LevelUpMenu.RevalidateHealth(Game1.player);
		}

		/// <summary>Print the currently subscribed mod events to the console.</summary>
		/// <param name="command">The console command.</param>
		/// <param name="args">The supplied arguments (not applicable).</param>
		private void _PrintSubscribedEvents(string command, string[] args)
		{
			Monitor.Log("Currently subscribed events:");
			foreach (var s in EventManager.GetSubscribedEvents()) Monitor.Log($"{s}", LogLevel.Info);
		}

		/// <summary>Give the local player immediate perks for a skill level.</summary>
		/// <param name="skill">The skill index.</param>
		/// <param name="level">The skill level.</param>
		private void _GetLevelPerk(int skill, int level)
		{
			switch (skill)
			{
				case 4:
					Game1.player.maxHealth += 5;
					break;

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
			}
			Game1.player.health = Game1.player.maxHealth;
			Game1.player.Stamina = Game1.player.maxStamina.Value;
		}

		/// <summary>Print the current value of specified mod data fields to the console.</summary>
		/// <param name="command">The console command.</param>
		/// <param name="args">The supplied arguments (not applicable).</param>
		private void _PrintDataField(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				Monitor.Log("You must load a save first.", LogLevel.Warn);
				return;
			}

			if (!args.Any())
			{
				Monitor.Log("You must specify a data field to read." + _GetAvailableDataFields(), LogLevel.Warn);
				return;
			}

			foreach (var arg in args)
			{
				var value = Data.ReadField($"{UniqueID}/{arg}");
				if (!string.IsNullOrEmpty(value)) Monitor.Log($"{arg}: {value}", LogLevel.Info);
				else Monitor.Log($"Mod data does not contain an entry for {arg}.", LogLevel.Warn);
			}
		}

		/// <summary>Tell the dummies how to use the console command.</summary>
		private string _GetCommandUsage()
		{
			var result = "\n\nUsage: wol_getprofessions <argument1> <argument2> ... <argumentN>";
			result += "\nAvailable arguments:";
			result += "\n\t'level' - get all professions and level perks for the local player's current skill levels.";
			result += "\n\t'all' - get all professions, level perks and max out the local player's skills.";
			result += "\n\t'<skill>' - get all professions and perks for and max out the specified skill.";
			result += "\n\t'<profession>' - get the specified profession and level up the corresponding skill if necessary.";
			result += "\n\nExample:";
			result += "\n\tplayer_addprofessions farming fishing scavenger prospector slimecharmer";
			return result;
		}

		/// <summary>Tell the dummies the available mod data fields.</summary>
		private string _GetAvailableDataFields()
		{
			var result = "\n\nAvailable data fields:";
			result += "\n\tArtisanPointsAccrued - Fame points accrued as Artisan.";
			result += "\n\tArtisanAwardLevel - Highest tier of Artisan Fair's Seasonal Award won.";
			result += $"\n\tItemsForaged - Number of items foraged as Ecologist ({Config.ForagesNeededForBestQuality} needed for best quality).";
			result += $"\n\tMineralsCollected - Number of minerals collected as Gemologist ({Config.MineralsNeededForBestQuality} needed for best quality).";
			result += "\n\tScavengerStreak - Number of consecutive Scavenger Hunts completed (higher numbers improve your hunt reward).";
			result += "\n\tProspectorStreak - Number of consecutive Prospector Hunts completed (higher numbers improve your hunt reward).";
			result += "\n\tLowestMineLevelReached - The lowest mine level reached by the local player(higher numbers improve your chance to find ladders).";
			result += $"\n\tWaterTrashCollectedThisSeason - Number of junk items pulled out of water bodies as Conservationist in the current season ({Config.TrashNeededForNextTaxLevel} needed per tax bonus percent).";
			result += "\n\tActiveTaxBonusPercent - The active tax bonus this season as a result of last season's Conservationist activities.";
			return result;
		}
	}
}