using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Manages treasure hunt events for Prospector profession.</summary>
	public class ProspectorHunt : TreasureHunt
	{
		private readonly IEnumerable<int> _artifactsThatCanBeFound = new HashSet<int>
		{
			100, // chipped amphora
			101, // arrowhead
			102, // lost book
			103, // ancient doll
			109, // ancient sword
			113, // chicken statue
			114, // ancient seed
			115, // prehistoric tool
			119, // bone flute
			120, // prehistoric handaxe
			123, // ancient drum
			124, // golden mask
			125, // golden relic
			126, // strange doll
			127, // strange doll
			588, // palm fossil
		};

		/// <summary>Construct an instance.</summary>
		internal ProspectorHunt(string huntStartedMessage, string huntFailedMessage, Texture2D icon)
		{
			HuntStartedMessage = huntStartedMessage;
			HuntFailedMessage = huntFailedMessage;
			Icon = icon;
		}

		/// <summary>Try to start a new prospector hunt at this location.</summary>
		/// <param name="location">The game location.</param>
		internal override void TryStartNewHunt(GameLocation location)
		{
			if (!location.Objects.Any() || !base.TryStartNewHunt()) return;

			var v = location.Objects.Keys.ElementAtOrDefault(Random.Next(location.Objects.Keys.Count()));
			var obj = location.Objects[v];
			if (!Utility.IsStone(obj) || Utility.IsResourceNode(obj)) return;

			TreasureTile = v;
			_timeLimit = (uint)location.Objects.Count();
			_elapsed = 0;
			AwesomeProfessions.EventManager.Subscribe(new ArrowPointerUpdateTickedEvent(), new ProspectorHuntUpdateTickedEvent(), new ProspectorHuntRenderedHudEvent());
			Game1.addHUDMessage(new HuntNotification(HuntStartedMessage, Icon));
		}

		/// <summary>Reset treasure tile and unsubscribe treasure hunt update event.</summary>
		internal override void End()
		{
			AwesomeProfessions.EventManager.Unsubscribe(typeof(ProspectorHuntUpdateTickedEvent), typeof(ProspectorHuntRenderedHudEvent));
			TreasureTile = null;
		}

		/// <summary>Check if the player has found the treasure tile.</summary>
		protected override void CheckForCompletion()
		{
			if (TreasureTile == null || Game1.currentLocation.Objects.ContainsKey(TreasureTile.Value)) return;

			_GetStoneTreasure();
			End();
			AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ProspectorHuntStreak", amount: 1);
		}

		/// <summary>End the hunt unsuccessfully.</summary>
		protected override void Fail()
		{
			End();
			Game1.addHUDMessage(new HuntNotification(HuntFailedMessage));
			AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/ProspectorHuntStreak", "0");
		}

		/// <summary>Spawn hunt spoils as debris.</summary>
		/// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
		private void _GetStoneTreasure()
		{
			if (TreasureTile == null) return;

			var mineLevel = ((MineShaft)Game1.currentLocation).mineLevel;
			Dictionary<int, int> treasuresAndQuantities = new();

			if (Random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
				treasuresAndQuantities.Add(890, Random.Next(1, 3) + (Random.NextDouble() < 0.25 ? 2 : 0)); // qi bean

			switch (Random.Next(3))
			{
				case 0:
				{
					if (mineLevel > 120 && Random.NextDouble() < 0.03)
						treasuresAndQuantities.Add(386, Random.Next(1, 3)); // iridium ore

					List<int> possibles = new();
					if (mineLevel > 80) possibles.Add(384); // gold ore

					if (mineLevel > 40 && (possibles.Count == 0 || Random.NextDouble() < 0.6))
						possibles.Add(380); // iron ore

					if (possibles.Count == 0 || Random.NextDouble() < 0.6) possibles.Add(378); // copper ore

					if (possibles.Count == 0 || Random.NextDouble() < 0.6) possibles.Add(390); // stone

					possibles.Add(382); // coal
					treasuresAndQuantities.Add(possibles.ElementAt(Random.Next(possibles.Count)), Random.Next(2, 7));
					if (Random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
					{
						var key = treasuresAndQuantities.Keys.Last();
						treasuresAndQuantities[key] *= 2;
					}

					break;
				}
				case 1:
				{
					if (Game1.player.archaeologyFound.Any()) // artifacts
						treasuresAndQuantities.Add(Random.NextDouble() < 0.5 ? _artifactsThatCanBeFound.ElementAt(Random.Next(_artifactsThatCanBeFound.Count())) : Random.NextDouble() < 0.25 ? Random.Next(579, 586) : 535, 1);
					else
						treasuresAndQuantities.Add(382, Random.Next(1, 3));

					break;
				}
				case 2:
				{
					switch (Random.Next(3))
					{
						case 0:
						{
							switch (mineLevel)
							{
								case > 80:
									treasuresAndQuantities.Add(537 + (Random.NextDouble() < 0.4 ? Random.Next(-2, 0) : 0), Random.Next(1, 4)); // magma geode or worse
									break;

								case > 40:
									treasuresAndQuantities.Add(536 + (Random.NextDouble() < 0.4 ? -1 : 0), Random.Next(1, 4)); // frozen geode or worse
									break;

								default:
									treasuresAndQuantities.Add(535, Random.Next(1, 4)); // regular geode
									break;
							}

							if (Random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
							{
								var key = treasuresAndQuantities.Keys.Last();
								treasuresAndQuantities[key] *= 2;
							}

							break;
						}
						case 1:
						{
							if (mineLevel < 20)
							{
								treasuresAndQuantities.Add(382, Random.Next(1, 4)); // coal
								break;
							}

							switch (mineLevel)
							{
								case > 80:
									treasuresAndQuantities.Add(Random.NextDouble() < 0.3 ? 82 : Random.NextDouble() < 0.5 ? 64 : 60, Random.Next(1, 3)); // fire quartz else ruby or emerald
									break;

								case > 40:
									treasuresAndQuantities.Add(Random.NextDouble() < 0.3 ? 84 : Random.NextDouble() < 0.5 ? 70 : 62, Random.Next(1, 3)); // frozen tear else jade or aquamarine
									break;

								default:
									treasuresAndQuantities.Add(Random.NextDouble() < 0.3 ? 86 : Random.NextDouble() < 0.5 ? 66 : 68, Random.Next(1, 3)); // earth crystal else amethyst or topaz
									break;
							}

							if (Random.NextDouble() < 0.028 * mineLevel / 12) treasuresAndQuantities.Add(72, 1); // diamond
							else treasuresAndQuantities.Add(80, Random.Next(1, 3)); // quartz

							break;
						}
						case 2:
						{
							var luckModifier = Math.Max(0, 1.0 + Game1.player.DailyLuck * mineLevel / 4);
							var streak = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ProspectorHuntStreak", uint.Parse);
							if (Random.NextDouble() < 0.025 * luckModifier && !Game1.player.specialItems.Contains(31))
								treasuresAndQuantities.Add(31, 1); // femur

							if (Random.NextDouble() < 0.010 * luckModifier && !Game1.player.specialItems.Contains(60))
								treasuresAndQuantities.Add(60, 1); // ossified blade

							if (Random.NextDouble() < 0.002 * luckModifier * Math.Pow(2, streak)) treasuresAndQuantities.Add(74, 1); // prismatic shard

							if (treasuresAndQuantities.Count == 1) treasuresAndQuantities.Add(72, 1); // consolation diamond

							break;
						}
					}

					break;
				}
			}

			if (treasuresAndQuantities.Count == 0) treasuresAndQuantities.Add(390, Random.Next(1, 4));

			foreach (var kvp in treasuresAndQuantities)
			{
				if (kvp.Key.AnyOf(31, 60))
					Game1.createItemDebris(new MeleeWeapon(kvp.Key) { specialItem = true }, new Vector2(TreasureTile.Value.X, TreasureTile.Value.Y) + new Vector2(32f, 32f), Random.Next(4), Game1.currentLocation);
				else
					Game1.createMultipleObjectDebris(kvp.Key, (int)TreasureTile.Value.X, (int)TreasureTile.Value.Y, kvp.Value, Game1.player.UniqueMultiplayerID, Game1.currentLocation);
			}
		}
	}
}