using System;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using TheLion.Stardew.Common.Classes;
using TheLion.Stardew.Professions.Framework.Events;
using TheLion.Stardew.Professions.Framework.Extensions;
using TheLion.Stardew.Professions.Framework.Utility;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GameLocationExplodePatch : BasePatch
	{
		private static readonly DemolitionistBuffDisplayUpdateTickedEvent DemolitionistUpdateTickedEvent = new();

		/// <summary>Construct an instance.</summary>
		internal GameLocationExplodePatch()
		{
			Original = RequireMethod<GameLocation>(nameof(GameLocation.explode));
		}

		#region harmony patches

		/// <summary>Patch for Blaster double coal chance + Demolitionist speed burst.</summary>
		[HarmonyPostfix]
		private static void GameLocationExplodePostfix(GameLocation __instance, Vector2 tileLocation, int radius,
			Farmer who)
		{
			var isBlaster = who.HasProfession("Blaster");
			var isDemolitionist = who.HasProfession("Demolitionist");
			if (!isBlaster && !isDemolitionist) return;

			var grid = new CircleTileGrid(tileLocation, radius);
			foreach (var tile in grid)
			{
				if (!__instance.objects.TryGetValue(tile, out var tileObj) || !tileObj.IsStone()) continue;

				if (isBlaster)
				{
					var isPrestigedBlaster = who.HasPrestigedProfession("Blaster");
					if (!__instance.Name.StartsWith("UndergroundMine"))
					{
						var chanceModifier = who.DailyLuck / 2.0 + who.LuckLevel * 0.001 + who.MiningLevel * 0.005;
						//var r = new Random((int) tile.X * 1000 + (int) tile.Y + (int) Game1.stats.DaysPlayed +
						//                   (int) Game1.uniqueIDForThisGame / 2);
						var r = new Random(Guid.NewGuid().GetHashCode());
						if (tileObj.ParentSheetIndex is 343 or 450)
						{
							if ((r.NextDouble() < 0.035 || isPrestigedBlaster && r.NextDouble() < 0.035) &&
							    Game1.stats.DaysPlayed > 1)
							{
								Game1.createObjectDebris(SObject.coal, (int) tile.X, (int) tile.Y,
									who.UniqueMultiplayerID, __instance);
								if (isPrestigedBlaster)
									Game1.createObjectDebris(SObject.coal, (int) tile.X, (int) tile.Y,
										who.UniqueMultiplayerID, __instance);
							}
						}
						else if (r.NextDouble() < 0.05 * (1.0 + chanceModifier) ||
						         isPrestigedBlaster && r.NextDouble() < 0.05 * (1.0 + chanceModifier))
						{
							Game1.createObjectDebris(SObject.coal, (int) tile.X, (int) tile.Y,
								who.UniqueMultiplayerID, __instance);
							if (isPrestigedBlaster)
								Game1.createObjectDebris(SObject.coal, (int) tile.X, (int) tile.Y,
									who.UniqueMultiplayerID, __instance);
						}
					}
					else
					{
						//var r = new Random((int) tile.X * 1000 + (int) tile.Y + ((MineShaft) __instance).mineLevel +
						//                   (int) Game1.uniqueIDForThisGame / 2);
						var r = new Random(Guid.NewGuid().GetHashCode());
						if (r.NextDouble() < 0.25 || isPrestigedBlaster && r.NextDouble() < 0.25)
						{
							Game1.createObjectDebris(SObject.coal, (int) tile.X, (int) tile.Y, who.UniqueMultiplayerID,
								__instance);
							if (isPrestigedBlaster)
								Game1.createObjectDebris(SObject.coal, (int) tile.X, (int) tile.Y,
									who.UniqueMultiplayerID, __instance);
							ModEntry.ModHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer")
								.GetValue()
								.broadcastSprites(__instance,
									new TemporaryAnimatedSprite(25,
										new(tile.X * Game1.tileSize, tile.Y * Game1.tileSize), Color.White,
										8,
										Game1.random.NextDouble() < 0.5, 80f, 0, -1, -1f, 128));
						}
					}
				}

				if (!isDemolitionist) continue;

				var isPrestigedDemolitionist = who.HasPrestigedProfession("Demolitionist");
				if (Game1.random.NextDouble() >= 0.20 &&
				    (!isPrestigedDemolitionist || Game1.random.NextDouble() >= 0.20)) continue;

				if (Objects.ResourceFromStoneId.TryGetValue(tileObj.ParentSheetIndex, out var resourceIndex))
				{
					Game1.createObjectDebris(resourceIndex, (int) tile.X, (int) tile.Y, who.UniqueMultiplayerID,
						__instance);
					if (isPrestigedDemolitionist)
						Game1.createObjectDebris(resourceIndex, (int) tile.X, (int) tile.Y, who.UniqueMultiplayerID,
							__instance);
				}
				else
				{
					switch (tileObj.ParentSheetIndex)
					{
						case 44: // gem node
							Game1.createObjectDebris(Game1.random.Next(1, 8) * 2, (int) tile.X, (int) tile.Y,
								who.UniqueMultiplayerID, __instance);
							if (isPrestigedDemolitionist)
								Game1.createObjectDebris(Game1.random.Next(1, 8) * 2, (int) tile.X, (int) tile.Y,
									who.UniqueMultiplayerID, __instance);
							break;

						case 46: // mystic stone
							switch (Game1.random.NextDouble())
							{
								case < 0.25:
									Game1.createObjectDebris(74, (int) tile.X, (int) tile.Y,
										who.UniqueMultiplayerID, __instance); // drop prismatic shard
									if (isPrestigedDemolitionist)
										Game1.createObjectDebris(74, (int) tile.X, (int) tile.Y,
											who.UniqueMultiplayerID, __instance); // drop prismatic shard
									break;

								case < 0.6:
									Game1.createObjectDebris(765, (int) tile.X, (int) tile.Y,
										who.UniqueMultiplayerID, __instance); // drop iridium ore
									if (isPrestigedDemolitionist)
										Game1.createObjectDebris(765, (int) tile.X, (int) tile.Y,
											who.UniqueMultiplayerID, __instance); // drop iridium ore
									break;

								default:
									Game1.createObjectDebris(764, (int) tile.X, (int) tile.Y,
										who.UniqueMultiplayerID, __instance); // drop gold ore
									if (isPrestigedDemolitionist)
										Game1.createObjectDebris(764, (int) tile.X, (int) tile.Y,
											who.UniqueMultiplayerID, __instance); // drop gold ore
									break;
							}

							break;

						default:
							if ((845 <= tileObj.ParentSheetIndex) & (tileObj.ParentSheetIndex <= 847) &&
							    Game1.random.NextDouble() < 0.005)
							{
								Game1.createObjectDebris(827, (int) tile.X, (int) tile.Y, who.UniqueMultiplayerID,
									__instance);
								if (isPrestigedDemolitionist)
									Game1.createObjectDebris(827, (int) tile.X, (int) tile.Y, who.UniqueMultiplayerID,
										__instance);
							}

							break;
					}
				}
			}

			if (!who.IsLocalPlayer || !isDemolitionist) return;

			if (!ModEntry.Config.EnableGetExcited) return;

			// get excited speed buff
			var distanceFromEpicenter = (int) (tileLocation - who.getTileLocation()).Length();
			if (distanceFromEpicenter < radius * 2 + 1) ModState.DemolitionistExcitedness = 6;
			if (distanceFromEpicenter < radius + 1) ModState.DemolitionistExcitedness += 2;
			ModEntry.Subscriber.Subscribe(DemolitionistUpdateTickedEvent);
		}

		#endregion harmony patches
	}
}