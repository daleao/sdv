using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Linq;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	internal class GreenSlimeUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GreenSlimeUpdatePatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(GreenSlime), nameof(GreenSlime.update), new Type[] { typeof(GameTime), typeof(GameLocation) }),
				postfix: new HarmonyMethod(GetType(), nameof(GreenSlimeUpdatePostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for slimes to damage monsters around Slimemaster.</summary>
		protected static void GreenSlimeUpdatePostfix(ref GreenSlime __instance, GameLocation location)
		{
			if (!Utility.AnyPlayerInLocationHasProfession("slimemaster", location)) return;

			foreach (Monster monster in __instance.currentLocation.characters.Where(c => c is Monster && !(c is GreenSlime)))
			{
				if (!monster.IsInvisible && !monster.isInvincible() && monster.GetBoundingBox().Intersects(__instance.GetBoundingBox()))
				{
					int damageToMonster = Math.Max(1, __instance.DamageToFarmer + Game1.random.Next(-monster.DamageToFarmer / 4, monster.DamageToFarmer / 4));
					Vector2 trajectory = SUtility.getAwayFromPositionTrajectory(monster.GetBoundingBox(), __instance.Position) / 2f;
					monster.takeDamage(damageToMonster, (int)trajectory.X, (int)trajectory.Y, isBomb: false, 1.0, hitSound: "slime");
					monster.setInvincibleCountdown(225);
				}
			}
		}
		#endregion harmony patches
	}
}
