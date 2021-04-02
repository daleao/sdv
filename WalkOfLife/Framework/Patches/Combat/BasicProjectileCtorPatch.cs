using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
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
				AccessTools.Constructor(typeof(BasicProjectile), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(GameLocation), typeof(Character), typeof(bool), typeof(BasicProjectile.onCollisionBehavior) }),
				postfix: new HarmonyMethod(GetType(), nameof(BasicProjectileCtorPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increase Desperado projectile velocity.</summary>
		//private static bool BasicProjectileCtorPrefix(ref float xVelocity, ref float yVelocity, Character firer)
		//{
		//	if (firer != null && firer is Farmer && Utility.SpecificFarmerHasProfession("desperado", firer as Farmer))
		//	{
		//		xVelocity *= 1.5f;
		//		yVelocity *= 1.5f;
		//	}

		//	return true; // run original logic
		//}

		/// <summary>Patch to allow Rascal to bounce slingshot projectile.</summary>
		private static void BasicProjectileCtorPostfix(ref BasicProjectile __instance, ref NetInt ___bouncesLeft, float xVelocity, float yVelocity, Character firer)
		{
			if (firer == null || !(firer is Farmer)) return;

			if (Utility.SpecificFarmerHasProfession("desperado", firer as Farmer))
			{
				AwesomeProfessions.Reflection.GetField<NetFloat>(__instance, name: "xVelocity").GetValue().Set(xVelocity * 1.5f);
				AwesomeProfessions.Reflection.GetField<NetFloat>(__instance, name: "yVelocity").GetValue().Set(yVelocity * 1.5f);
			}

			if (AwesomeProfessions.Config.ModKey.IsDown() && Utility.SpecificFarmerHasProfession("rascal", firer as Farmer))
				++___bouncesLeft.Value;
		}

		#endregion harmony patches
	}
}