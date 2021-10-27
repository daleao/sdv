using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class BasicProjectileCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BasicProjectileCtorPatch()
		{
			Original = typeof(BasicProjectile).Constructor(new[] {
				typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(float), typeof(float),
				typeof(Vector2), typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(GameLocation),
				typeof(Character), typeof(bool), typeof(BasicProjectile.onCollisionBehavior)
			});
			Postfix = new(GetType(), nameof(BasicProjectileCtorPostfix));
		}

		#region harmony patches

		/// <summary>Patch for all classes to eliminate travel grace period.</summary>
		[HarmonyPostfix]
		private static void BasicProjectileCtorPostfix(BasicProjectile __instance, ref NetInt ___bouncesLeft,
			bool damagesMonsters, Character firer)
		{
			try
			{
				if (!damagesMonsters || firer is not Farmer farmer) return;
				__instance.ignoreTravelGracePeriod.Value = true;

				if (!farmer.HasProfession("Rascal") || !ModEntry.Config.Modkey.IsDown()) return;
				++___bouncesLeft.Value;
				__instance.damageToFarmer.Value = (int) (__instance.damageToFarmer.Value * 0.6);
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}