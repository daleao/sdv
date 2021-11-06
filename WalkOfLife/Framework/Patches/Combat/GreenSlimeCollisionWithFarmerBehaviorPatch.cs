using System;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GreenSlimeCollisionWithFarmerBehaviorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GreenSlimeCollisionWithFarmerBehaviorPatch()
		{
			Original = RequireMethod<GreenSlime>(nameof(GreenSlime.collisionWithFarmerBehavior));
			Postfix = new(AccessTools.Method(GetType(), nameof(GreenSlimeCollisionWithFarmerBehaviorPostfix)));
		}

		#region harmony patches

		/// <summary>Patch to increment Piper Eubstance counter and heal on contact with slime.</summary>
		[HarmonyPostfix]
		private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
		{
			var who = __instance.Player;
			if (!who.IsLocalPlayer || ModEntry.SuperModeIndex != Utility.Professions.IndexOf("Piper") ||
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
			__instance.currentLocation.debris.Add(new(healed,
				new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));

			if (!ModEntry.IsSuperModeActive) ModEntry.SuperModeCounter += Game1.random.Next(1, 10);

			ModEntry.SlimeContactTimer = 60;
		}

		#endregion harmony patches
	}
}