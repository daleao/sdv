using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Linq;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class MonsterBehaviorAtGameTickPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal MonsterBehaviorAtGameTickPatch()
		{
			Original = typeof(Monster).MethodNamed(nameof(Monster.behaviorAtGameTick));
			Prefix = new HarmonyMethod(GetType(), nameof(MonsterBehaviorAtGameTickPrefix));
		}

		#region harmony patches

		/// <summary>Patch to force Slime aggro onto monsters.</summary>
		[HarmonyPrefix]
		private static bool MonsterBehaviorAtGameTickPrefix(Monster __instance, GameTime time)
		{
			try
			{
				if (__instance is not GreenSlime ||
					((!ModEntry.IsSuperModeActive || ModEntry.SuperModeIndex != Util.Professions.IndexOf("Piper")) &&
					 (!ModEntry.ActivePeerSuperModes.TryGetValue(Util.Professions.IndexOf("Piper"), out var peerIDs) ||
					  peerIDs.Count <= 0))) return true; // run original logic

				var monsters = __instance.currentLocation.characters.OfType<Monster>().Where(m => m is not GreenSlime).ToList();
				if (!monsters.Any()) return true; // run original logic

				if (__instance.timeBeforeAIMovementAgain > 0f)
					__instance.timeBeforeAIMovementAgain -= time.ElapsedGameTime.Milliseconds;

				Monster closestTarget = null;
				var distanceToClosestTarget = double.MaxValue;
				foreach (var monster in monsters)
				{
					var distanceToMonster = __instance.DistanceToCharacter(monster);
					if (distanceToMonster >= distanceToClosestTarget) continue;

					closestTarget = monster;
					distanceToClosestTarget = distanceToMonster;
				}

				if (closestTarget == null) return false; // don't run original logic

				if (Math.Abs(closestTarget.GetBoundingBox().Center.Y - __instance.GetBoundingBox().Center.Y) > 192)
				{
					if (closestTarget.GetBoundingBox().Center.X - __instance.GetBoundingBox().Center.X > 0)
						__instance.SetMovingLeft(b: true);
					else
						__instance.SetMovingRight(b: true);
				}
				else if (closestTarget.GetBoundingBox().Center.Y - __instance.GetBoundingBox().Center.Y > 0)
				{
					__instance.SetMovingUp(b: true);
				}
				else
				{
					__instance.SetMovingDown(b: true);
				}

				__instance.MovePosition(time, Game1.viewport, __instance.currentLocation);
				return false; // don't run original logic

				//var foundPlayer = ModEntry.Reflection.GetMethod(__instance, name: "findPlayer").Invoke<Farmer>();
				//return !foundPlayer.IsLocalPlayer || !ModEntry.IsSuperModeActive ||
				//       ModEntry.SuperModeIndex != Util.Professions.IndexOf("Hunter");
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // run original logic
			}
		}

		#endregion harmony patches
	}
}