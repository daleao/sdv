using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Linq;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	internal class GreenSlimeUpdatePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GreenSlime), nameof(GreenSlime.update), new[] { typeof(GameTime), typeof(GameLocation) }),
				postfix: new HarmonyMethod(GetType(), nameof(GreenSlimeUpdatePostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for slimes to damage monsters around Slimecharmer.</summary>
		private static void GreenSlimeUpdatePostfix(ref GreenSlime __instance, GameLocation location)
		{
			try
			{
				if (!Utility.AnyPlayerInLocationHasProfession("Slimecharmer", location)) return;

				foreach (var npc in __instance.currentLocation.characters.Where(npc => npc.IsMonster && npc is not GreenSlime))
				{
					var monster = (Monster)npc;
					var monsterBox = monster.GetBoundingBox();
					if (monster.IsInvisible || monster.isInvincible() || monster.isGlider.Value || !monsterBox.Intersects(__instance.GetBoundingBox()))
						continue;

					var damageToMonster = Math.Max(1, __instance.DamageToFarmer + Game1.random.Next(-__instance.DamageToFarmer / 4, __instance.DamageToFarmer / 4));
					var trajectory = SUtility.getAwayFromPositionTrajectory(monsterBox, __instance.Position) / 2f;
					monster.takeDamage(damageToMonster, (int)trajectory.X, (int)trajectory.Y, isBomb: false, 1.0, hitSound: "slime");
					monster.setInvincibleCountdown(225);
					monster.currentLocation.debris.Add(new Debris(damageToMonster, new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y), new Color(255, 130, 0), 1f, monster));
				}
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(GreenSlimeUpdatePostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}