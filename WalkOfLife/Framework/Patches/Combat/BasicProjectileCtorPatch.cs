using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class BasicProjectileCtorPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Constructor(typeof(BasicProjectile), new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(GameLocation), typeof(Character), typeof(bool), typeof(BasicProjectile.onCollisionBehavior) }),
				postfix: new HarmonyMethod(GetType(), nameof(BasicProjectileCtorPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increase Desperado projectile velocity + allow Rascal projectile bounce.</summary>
		private static void BasicProjectileCtorPostfix(ref BasicProjectile __instance, ref NetInt ___bouncesLeft, float xVelocity, float yVelocity, Character firer)
		{
			if (firer is not Farmer) return;
			
			try
			{
				if (Utility.SpecificPlayerHasProfession("Desperado", (Farmer)firer))
				{
					AwesomeProfessions.Reflection.GetField<NetFloat>(__instance, name: "xVelocity").GetValue().Set(xVelocity * 1.5f);
					AwesomeProfessions.Reflection.GetField<NetFloat>(__instance, name: "yVelocity").GetValue().Set(yVelocity * 1.5f);
				}

				if (AwesomeProfessions.Config.ModKey.IsDown() && Utility.SpecificPlayerHasProfession("Rascal", (Farmer)firer))
					++___bouncesLeft.Value;
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(BasicProjectileCtorPostfix)}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}