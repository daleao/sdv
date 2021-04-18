using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;

namespace TheLion.AwesomeProfessions.Framework.Patches.Combat
{
	internal class FarmerTakeDamagePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Farmer), nameof(Farmer.takeDamage)),
				postfix: new HarmonyMethod(GetType(), nameof(FarmerTakeDamagePostfix))
			);
		}

		/// <summary>Patch to heal Slimecharmer on contact with slime.</summary>
		private static void FarmerTakeDamagePostfix(ref Farmer __instance, int damage, Monster damager)
		{
			try
			{
				if (AwesomeProfessions.slimeHealTimer > 0 || __instance.temporaryInvincibilityTimer > 0 || !Utility.SpecificPlayerHasProfession("Slimecharmer", __instance) || damager is not GreenSlime)
					return;

				damage += Game1.random.Next(Math.Min(-1, -damage / 8), Math.Max(1, damage / 8));
				damage /= 2;
				__instance.health = Math.Min(__instance.health + damage, __instance.maxHealth);
				__instance.currentLocation.debris.Add(new Debris(damage,
					new Vector2(__instance.getStandingX() + 8, __instance.getStandingY()), Color.Lime, 1f, __instance));
				AwesomeProfessions.slimeHealTimer = 72;
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(FarmerTakeDamagePostfix)}:\n{ex}");
			}
		}
	}
}