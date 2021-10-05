using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GreenSlimeCollisionWithFarmerBehaviorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GreenSlimeCollisionWithFarmerBehaviorPatch()
		{
			Original = typeof(GreenSlime).MethodNamed(nameof(GreenSlime.collisionWithFarmerBehavior));
			Postfix = new HarmonyMethod(GetType(), nameof(GreenSlimeCollisionWithFarmerBehaviorPostfix));
		}

		#region harmony patches

		/// <summary>Patch to increment Piper Eubstance counter and heal on contact with slime.</summary>
		[HarmonyPostfix]
		private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
		{
			try
			{
				var who = __instance.Player;
				if (!who.IsLocalPlayer || ModEntry.SuperModeIndex != Util.Professions.IndexOf("Piper") ||
				    ModEntry.SlimeContactTimer > 0) return;

				int healed;
				if (ModEntry.IsSuperModeActive)
				{
					healed = __instance.DamageToFarmer / 2;
					healed += Game1.random.Next(Math.Min(-1, -healed / 8), Math.Max(1, healed / 8));
				}
				else
				{
					healed = 1;
				}

				who.health = Math.Min(who.health + healed, who.maxHealth);
				__instance.currentLocation.debris.Add(new Debris(healed,
					new Vector2(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));

				if (!ModEntry.IsSuperModeActive) ModEntry.SuperModeCounter += Game1.random.Next(1, 10);

				ModEntry.SlimeContactTimer = 60;
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}