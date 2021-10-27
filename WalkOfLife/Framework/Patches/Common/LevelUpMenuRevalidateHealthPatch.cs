using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuRevalidateHealthPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuRevalidateHealthPatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed(nameof(LevelUpMenu.RevalidateHealth));
			Prefix = new(GetType(), nameof(LevelUpMenuRevalidateHealthPrefix));
		}

		#region harmony patches

		/// <summary>
		///     Patch revalidate player health after changes to the combat skill + revalidate fish pond capacity after changes
		///     to the fishing skill.
		/// </summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuRevalidateHealthPrefix(Farmer farmer)
		{
			var expectedMaxHealth = 100;
			if (farmer.mailReceived.Contains("qiCave")) expectedMaxHealth += 25;

			for (var i = 1; i <= farmer.GetUnmodifiedSkillLevel(4); ++i)
				if (!farmer.newLevels.Contains(new(4, i)) && i != 5 && i != 10)
					expectedMaxHealth += 5;

			if (Game1.player.HasProfession("Fighter")) expectedMaxHealth += 15;
			if (Game1.player.HasProfession("Brute")) expectedMaxHealth += 25;

			if (farmer.maxHealth != expectedMaxHealth)
			{
				Log(
					$"Fixing max health of {farmer.Name}.\nCurrent: {farmer.maxHealth}\nExpected: {expectedMaxHealth}",
					LogLevel.Warn);
				farmer.maxHealth = expectedMaxHealth;
				farmer.health = Math.Min(farmer.maxHealth, farmer.health);
			}

			try
			{
				// revalidate fish pond capacity
				foreach (var b in Game1.getFarm().buildings.Where(b =>
					(b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) && b is FishPond &&
					!b.isUnderConstruction()))
				{
					var pond = (FishPond) b;
					pond.UpdateMaximumOccupancy();
					pond.currentOccupants.Value = Math.Min(pond.currentOccupants.Value, pond.maxOccupants.Value);
				}
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
				return false; // don't run original logic
			}

			return false; // don't run original logic
		}

		#endregion harmony patches
	}
}