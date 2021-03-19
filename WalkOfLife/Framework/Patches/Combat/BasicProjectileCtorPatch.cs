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
		/// <summary>Construct an instance.</summary>
		internal BasicProjectileCtorPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(BasicProjectile), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(float), typeof(float), typeof(Vector2), typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(GameLocation), typeof(Character), typeof(bool), typeof(BasicProjectile.onCollisionBehavior) }),
				postfix: new HarmonyMethod(GetType(), nameof(BasicProjectileCtorPostfix))
			);
		}

		#region harmony patches
		protected static void BasicProjectileCtorPostfix(ref NetInt ___bouncesLeft, Character firer)
		{
			if (AwesomeProfessions.Config.Modkey.IsDown() && Utility.SpecificPlayerHasProfession("rascal", firer as Farmer))
				++___bouncesLeft.Value;
		}
		#endregion harmony patches
	}
}
