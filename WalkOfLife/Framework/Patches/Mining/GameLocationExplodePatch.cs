using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class GameLocationExplodePatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.explode)),
				postfix: new HarmonyMethod(GetType(), nameof(GameLocationExplodePostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch for Blaster double coal chance + Demolitionist speed burst.</summary>
		private static void GameLocationExplodePostfix(ref GameLocation __instance, Vector2 tileLocation, int radius, Farmer who)
		{
			try
			{
				var isBlaster = Utility.SpecificPlayerHasProfession("Blaster", who);
				var isDemolitionist = Utility.SpecificPlayerHasProfession("Demolitionist", who);
				if (!isBlaster && !isDemolitionist) return;

				var grid = new CircleTileGrid(tileLocation, radius);
				foreach (var tile in grid)
				{
					if (!__instance.objects.TryGetValue(tile, out var tileObj) || !Utility.IsStone(tileObj)) continue;

					if (isBlaster)
					{
						if (!__instance.Name.StartsWith("UndergroundMine"))
						{
							var chanceModifier = who.DailyLuck / 2.0 + who.LuckLevel * 0.001 + who.MiningLevel * 0.005;
							var r = new Random((int)tile.X * 1000 + (int)tile.Y + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
							if (tileObj.ParentSheetIndex == 343 || tileObj.ParentSheetIndex == 450)
							{
								if (r.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1)
									Game1.createObjectDebris(SObject.coal, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
							}
							else if (r.NextDouble() < 0.05 * (1.0 + chanceModifier))
							{
								Game1.createObjectDebris(SObject.coal, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
							}
						}
						else
						{
							var r = new Random((int)tile.X * 1000 + (int)tile.Y + ((MineShaft)__instance).mineLevel + (int)Game1.uniqueIDForThisGame / 2);
							if (r.NextDouble() < 0.25)
							{
								Game1.createObjectDebris(382, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
								AwesomeProfessions.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer")
									.GetValue()
									.broadcastSprites(__instance,
										new TemporaryAnimatedSprite(25,
											new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize), Color.White,
											8,
											Game1.random.NextDouble() < 0.5, 80f, 0, -1, -1f, 128));
							}
						}
					}

					if (isDemolitionist && Game1.random.NextDouble() < 0.20)
					{
						if (Utility.ResourceFromStoneId.TryGetValue(tileObj.ParentSheetIndex, out var resourceIndex))
						{
							Game1.createObjectDebris(resourceIndex, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
						}
						else switch (tileObj.ParentSheetIndex)
						{
							case 44: // gem node
							{
								Game1.createObjectDebris(Game1.random.Next(1, 8) * 2, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
								break;
							}
							case 46: // mystic stone
							{
								switch (Game1.random.NextDouble())
								{
									case < 0.25:
										Game1.createObjectDebris(74, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance); // drop prismatic shard
										break;

									case < 0.6:
										Game1.createObjectDebris(765, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance); // drop iridium ore
										break;

									default:
										Game1.createObjectDebris(764, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance); // drop gold ore
										break;
								}
								break;
							}
							default:
							{
								if (845 <= tileObj.ParentSheetIndex & tileObj.ParentSheetIndex <= 847 && Game1.random.NextDouble() < 0.005)
									Game1.createObjectDebris(827, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
								break;
							}
						}
					}
				}

				if (!who.IsLocalPlayer || !isDemolitionist) return;

				// get excited speed buff
				var distanceFromEpicenter = (int)(tileLocation - who.getTileLocation()).Length();
				if (distanceFromEpicenter < radius * 2 + 1) AwesomeProfessions.demolitionistBuffMagnitude = 4;
				if (distanceFromEpicenter < radius + 1) AwesomeProfessions.demolitionistBuffMagnitude += 2;
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(GameLocationExplodePostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}