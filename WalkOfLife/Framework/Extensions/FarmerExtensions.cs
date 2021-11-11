using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Professions.Framework.Events;

namespace TheLion.Stardew.Professions.Framework.Extensions
{
	public static class FarmerExtensions
	{
		/// <summary>Whether the farmer has a particular profession.</summary>
		/// <param name="professionName">The name of the profession.</param>
		public static bool HasProfession(this Farmer farmer, string professionName)
		{
			return Utility.Professions.IndexByName.Forward.TryGetValue(professionName, out var professionIndex) &&
			       farmer.professions.Contains(professionIndex);
		}

		/// <summary>Whether the farmer has a particular profession.</summary>
		/// <param name="professionIndex">The index of the profession.</param>
		public static bool HasProfession(this Farmer farmer, int professionIndex)
		{
			return Utility.Professions.IndexByName.Contains(professionIndex) &&
			       farmer.professions.Contains(professionIndex);
		}

		/// <summary>Whether the farmer has any of the specified professions.</summary>
		/// <param name="professionNames">Sequence of profession names.</param>
		public static bool HasAnyOfProfessions(this Farmer farmer, params string[] professionNames)
		{
			return professionNames.Any(p =>
				Utility.Professions.IndexByName.Forward.TryGetValue(p, out var professionIndex) &&
				farmer.professions.Contains(professionIndex));
		}

		/// <summary>Whether the farmer has any of the specified professions.</summary>
		/// <param name="professionNames">Sequence of profession names.</param>
		public static bool HasAnyOfProfessions(this Farmer farmer, string[] professionNames, out string firstMatch)
		{
			firstMatch = professionNames.FirstOrDefault(p =>
				Utility.Professions.IndexByName.Forward.TryGetValue(p, out var professionIndex) &&
				farmer.professions.Contains(professionIndex));
			return firstMatch is not null;
		}

		/// <summary>Whether the farmer has acquired any other professions branching from the specified profession.</summary>
		/// <param name="professionIndex">A profession index (0 to 29).</param>
		/// <param name="otherProfessions">
		///     An array of acquired professions in the same branch as
		///     <paramref name="professionIndex">, excluding itself.
		/// </param>
		public static bool HasOtherProfessionsInBranch(this Farmer farmer, int professionIndex,
			out int[] otherProfessions)
		{
			var otherProfessionsInBranch = (professionIndex % 6) switch
			{
				0 => new[] {professionIndex + 1},
				1 => new[] {professionIndex - 1},
				2 => new[] {professionIndex + 1, professionIndex + 2, professionIndex + 3},
				3 => new[] {professionIndex - 1, professionIndex + 1, professionIndex + 2},
				4 => new[] {professionIndex - 2, professionIndex - 1, professionIndex + 1},
				5 => new[] {professionIndex - 3, professionIndex - 2, professionIndex - 1},
				_ => new int[] { }
			};

			otherProfessions = farmer.professions.Intersect(otherProfessionsInBranch).ToArray();
			return otherProfessions.Any();
		}

		/// <summary>Whether the farmer has acquired all professions branching from the specified profession.</summary>
		/// <param name="professionIndex">A profession index (0 to 29).</param>
		public static bool HasAllProfessionsInBranch(this Farmer farmer, int professionIndex)
		{
			return professionIndex % 6 == 0 && farmer.professions.Contains(professionIndex + 2) &&
			       farmer.professions.Contains(professionIndex + 3) ||
			       professionIndex % 6 == 1 && farmer.professions.Contains(professionIndex + 3) &&
			       farmer.professions.Contains(professionIndex + 4) ||
			       professionIndex % 6 > 1;
		}

		/// <summary>Get the last level 5 profession acquired by the farmer in the specified skill.</summary>
		/// <param name="skill">The skill index.</param>
		/// <returns>The last acquired profession, or -1 if none was found.</returns>
		public static int CurrentBranchForSkill(this Farmer farmer, int skill)
		{
			try
			{
				return farmer.professions.Reverse().First(p => p == skill * 6 || p == skill * 6 + 1);
			}
			catch
			{
				return -1;
			}
		}

		/// <summary>Whether the farmer can prestige the specified skill.</summary>
		/// <param name="whichSkill">A skill index (0 to 4).</param>
		public static bool CanPrestige(this Farmer farmer, SkillType whichSkill)
		{
			var isSkillLevelTen = whichSkill switch
			{
				SkillType.Farming => farmer.FarmingLevel >= 10,
				SkillType.Fishing => farmer.FishingLevel >= 10,
				SkillType.Foraging => farmer.ForagingLevel >= 10,
				SkillType.Mining => farmer.MiningLevel >= 10,
				SkillType.Combat => farmer.CombatLevel >= 10,
				SkillType.Luck => false,
				_ => false
			};

			var justLeveledUp = farmer.newLevels.Contains(new((int)whichSkill, 10));
			var hasAtLeastOneButNotAllProfessionsInSkill =
				farmer.GetProfessionsForSkill((int)whichSkill, true).Count() is > 0 and < 4;
			var alreadyPrestigedThisSkill = ModEntry.Subscriber.TryGet(typeof(PrestigeDayEndingEvent), out var prestigeDayEnding) &&
			                                ((PrestigeDayEndingEvent)prestigeDayEnding).SkillQueue.Contains(whichSkill);

			return isSkillLevelTen && !justLeveledUp && hasAtLeastOneButNotAllProfessionsInSkill &&
			       !alreadyPrestigedThisSkill;
		}

		/// <summary>Whether the farmer can prestige any skill.</summary>
		public static bool CanPrestigeAny(this Farmer farmer)
		{
			return farmer.CanPrestige(SkillType.Farming) || farmer.CanPrestige(SkillType.Fishing) ||
			       farmer.CanPrestige(SkillType.Foraging) || farmer.CanPrestige(SkillType.Mining) ||
			       farmer.CanPrestige(SkillType.Combat);
		}

		/// <summary>Resets a specific skill level, removing all associated recipes and bonuses but maintaining profession perks.</summary>
		/// <param name="whichSkill">The skill to reset.</param>
		public static void PrestigeSkill(this Farmer farmer, SkillType whichSkill)
		{
			// reset skill level
			switch (whichSkill)
			{
				case SkillType.Farming:
					farmer.FarmingLevel = 0;
					break;
				case SkillType.Fishing:
					farmer.FishingLevel = 0;
					break;
				case SkillType.Foraging:
					farmer.ForagingLevel = 0;
					break;
				case SkillType.Mining:
					farmer.MiningLevel = 0;
					break;
				case SkillType.Combat:
					farmer.CombatLevel = 0;
					break;
				case SkillType.Luck:
				default:
					return;
			}

			var toRemove = farmer.newLevels.Where(p => p.X == (int) whichSkill);
			foreach (var item in toRemove) farmer.newLevels.Remove(item);

			// reset skill experience
			farmer.experiencePoints[(int) whichSkill] = 0;

			if (ModEntry.Config.ForgetRecipesOnPrestige)
			{
				// remove associated crafting recipes
				foreach (var recipe in farmer.GetCraftingRecipesForSkill(whichSkill))
					farmer.craftingRecipes.Remove(recipe);

				// remove associated cooking recipes
				foreach (var recipe in farmer.GetCookingRecipesForSkill(whichSkill))
					farmer.cookingRecipes.Remove(recipe);
			}

			// revalidate health
			if (whichSkill == SkillType.Combat) LevelUpMenu.RevalidateHealth(farmer);
		}

		/// <summary>Get all the farmer's crafting recipes associated with a specific skill.</summary>
		/// <param name="whichSkill">The desired skill.</param>
		public static IEnumerable<string> GetCraftingRecipesForSkill(this Farmer farmer, SkillType whichSkill)
		{
			return CraftingRecipe.craftingRecipes.Where(r =>
					r.Value.Split('/')[4].Contains(whichSkill.ToString()) && farmer.craftingRecipes.ContainsKey(r.Key))
				.Select(recipe => recipe.Key);
		}

		/// <summary>Get all the farmer's cooking recipes associated with a specific skill.</summary>
		/// <param name="whichSkill">The desired skill.</param>
		public static IEnumerable<string> GetCookingRecipesForSkill(this Farmer farmer, SkillType whichSkill)
		{
			return CraftingRecipe.cookingRecipes.Where(r =>
					r.Value.Split('/')[3].Contains(whichSkill.ToString()) && farmer.cookingRecipes.ContainsKey(r.Key))
				.Select(recipe => recipe.Key);
		}

		/// <summary>Get all the farmer's professions associated with a specific skill.</summary>
		/// <param name="whichSkill">The desired skill.</param>
		public static IEnumerable<int> GetProfessionsForSkill(this Farmer farmer, int whichSkill,
			bool excludeTierOneProfessions = false)
		{
			return farmer.professions.Intersect(excludeTierOneProfessions
				? Enumerable.Range(whichSkill * 6 + 2, 4)
				: Enumerable.Range(whichSkill * 6, 6));
		}
	}
}