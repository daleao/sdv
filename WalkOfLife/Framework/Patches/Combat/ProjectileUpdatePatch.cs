using System;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Projectiles;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class ProjectileUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ProjectileUpdatePatch()
		{
			Original = RequireMethod<Projectile>(nameof(Projectile.update));
			Postfix = new(AccessTools.Method(GetType(), nameof(ProjectileUpdatePostfix)));
		}

		#region harmony patches

		/// <summary>Patch for increased Desperado bullet cross-section.</summary>
		[HarmonyPostfix]
		private static void ProjectileUpdatePostfix(Projectile __instance, ref bool __result, NetPosition ___position,
			NetCharacterRef ___theOneWhoFiredMe, NetFloat ___xVelocity, NetFloat ___yVelocity, GameLocation location)
		{
			// check if is BasicProjectile
			if (__instance is not BasicProjectile projectile) return;

			// check if damages monsters
			var damagesMonsters = ModEntry.ModHelper.Reflection.GetField<NetBool>(__instance, "damagesMonsters")
				.GetValue().Value;
			if (!damagesMonsters) return;

			// check if firer is has Desperado Super Mode
			var firer = ___theOneWhoFiredMe.Get(Game1.currentLocation) is Farmer farmer ? farmer : Game1.player;
			if (!firer.IsLocalPlayer || ModEntry.SuperModeIndex != Utility.Professions.IndexOf("Desperado")) return;

			// check for powered bullet
			var bulletPower = Utility.Professions.GetDesperadoBulletPower() - 1f;
			if (bulletPower <= 0f) return;

			// check if already collided
			if (__result)
			{
				if (!ModEntry.DidBulletPierceEnemy) return;

				projectile.damageToFarmer.Value = (int) (projectile.damageToFarmer.Value * 0.6f);
				ModEntry.DidBulletPierceEnemy = false;
				__result = false;
				return;
			}

			// get collision angle
			var velocity = new Vector2(___xVelocity.Value, ___yVelocity.Value);
			var angle = velocity.AngleWithHorizontal();
			if (angle > 180) angle -= 360;

			// check for extended collision
			var bulletHitbox = __instance.getBoundingBox();
			var isBulletTravelingVertically = Math.Abs(angle) is >= 45 and <= 135;
			if (isBulletTravelingVertically)
				bulletHitbox.Inflate((int) (bulletHitbox.Width * bulletPower), 0);
			else
				bulletHitbox.Inflate(0, (int) (bulletHitbox.Height * bulletPower));

			if (location.doesPositionCollideWithCharacter(bulletHitbox) is not Monster monster) return;

			// do deal damage
			var actualDistance = isBulletTravelingVertically
				? Math.Abs(monster.getStandingX() - __instance.getBoundingBox().Center.X)
				: Math.Abs(monster.getStandingY() - __instance.getBoundingBox().Center.Y);
			var monsterRadius = isBulletTravelingVertically
				? monster.GetBoundingBox().Width / 2
				: monster.GetBoundingBox().Height / 2;
			var actualBulletRadius = isBulletTravelingVertically
				? __instance.getBoundingBox().Width / 2
				: __instance.getBoundingBox().Height / 2;
			var extendedBulletRadius =
				isBulletTravelingVertically ? bulletHitbox.Width / 2 : bulletHitbox.Height / 2;

			var lerpFactor = (actualDistance - (actualBulletRadius + monsterRadius)) /
			                 (extendedBulletRadius - actualBulletRadius);
			var multiplier = MathHelper.Lerp(1f, 0f, lerpFactor);
			var damage = (int) (projectile.damageToFarmer.Value * multiplier);
			location.damageMonster(monster.GetBoundingBox(), damage, damage + 1, false, multiplier + bulletPower, 0,
				0f, 1f, true, firer);
		}

		#endregion harmony patches
	}
}