using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.TreasureHunt
{
	/// <summary>Manages treasure hunt events for Prospector profession.</summary>
	public class ProspectorHunt : TreasureHunt
	{
		/// <summary>Construct an instance.</summary>
		internal ProspectorHunt()
		{
			HuntStartedMessage = ModEntry.ModHelper.Translation.Get("prospector.huntstarted");
			HuntFailedMessage = ModEntry.ModHelper.Translation.Get("prospector.huntfailed");
			IconSourceRect = new Rectangle(48, 672, 16, 16);
		}

		/// <summary>Try to start a new prospector hunt at this location.</summary>
		/// <param name="location">The game location.</param>
		internal override void TryStartNewHunt(GameLocation location)
		{
			if (!location.Objects.Any() || !base.TryStartNewHunt()) return;

			var v = location.Objects.Keys.ElementAtOrDefault(Random.Next(location.Objects.Keys.Count()));
			if (!location.Objects.TryGetValue(v, out var obj) || !obj.IsStone() || obj.IsResourceNode()) return;

			TreasureTile = v;
			TimeLimit = (uint)(location.Objects.Count() * ModEntry.Config.TreasureHuntHandicap);
			Elapsed = 0;
			ModEntry.Subscriber.Subscribe(new Events.ArrowPointerUpdateTickedEvent(),
				new Events.ProspectorHuntUpdateTickedEvent(), new Events.ProspectorHuntRenderedHudEvent());
			Game1.addHUDMessage(new HuntNotification(HuntStartedMessage, IconSourceRect));
		}

		/// <summary>Check if the player has found the treasure tile.</summary>
		protected override void CheckForCompletion()
		{
			if (TreasureTile == null || Game1.currentLocation.Objects.ContainsKey(TreasureTile.Value)) return;

			GetStoneTreasure();
			End();
			ModEntry.Data.IncrementField<uint>("ProspectorHuntStreak");
		}

		/// <summary>End the hunt unsuccessfully.</summary>
		protected override void Fail()
		{
			End();
			Game1.addHUDMessage(new HuntNotification(HuntFailedMessage));
			ModEntry.Data.WriteField("ProspectorHuntStreak", "0");
		}

		/// <summary>Reset treasure tile and unsubscribe treasure hunt update event.</summary>
		internal override void End()
		{
			ModEntry.Subscriber.Unsubscribe(typeof(Events.ProspectorHuntUpdateTickedEvent),
				typeof(Events.ProspectorHuntRenderedHudEvent));
			TreasureTile = null;
		}

		/// <summary>Spawn hunt spoils as debris.</summary>
		/// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
		private void GetStoneTreasure()
		{
			if (TreasureTile == null) return;

			var mineLevel = ((MineShaft)Game1.currentLocation).mineLevel;
			Dictionary<int, int> treasuresAndQuantities = new();

			if (Random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
				treasuresAndQuantities.Add(890, Random.Next(1, 3) + (Random.NextDouble() < 0.25 ? 2 : 0)); // qi beans

			switch (Random.Next(3))
			{
				case 0:
					if (mineLevel > 120 && Random.NextDouble() < 0.06)
						treasuresAndQuantities.Add(386, Random.Next(1, 3)); // iridium ore

					List<int> possibles = new();
					if (mineLevel > 80) possibles.Add(384); // gold ore

					if (mineLevel > 40 && (possibles.Count == 0 || Random.NextDouble() < 0.6))
						possibles.Add(380); // iron ore

					if (possibles.Count == 0 || Random.NextDouble() < 0.6) possibles.Add(378); // copper ore

					possibles.Add(382); // coal
					treasuresAndQuantities.Add(possibles.ElementAt(Random.Next(possibles.Count)),
						Random.Next(2, 7) * Random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.015 ? 2 : 1);
					if (Random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
					{
						var key = treasuresAndQuantities.Keys.Last();
						treasuresAndQuantities[key] *= 2;
					}

					break;

				case 1:
					if (Game1.player.archaeologyFound.Any() && Random.NextDouble() < 0.5) // artifacts
						treasuresAndQuantities.Add(Random.NextDouble() < 0.5 ? Random.Next(579, 586) : 535, 1);
					else
						treasuresAndQuantities.Add(382, Random.Next(1, 4)); // coal

					break;

				case 2:
					switch (Random.Next(3))
					{
						case 0: // geodes
							switch (mineLevel)
							{
								case > 80:
									treasuresAndQuantities.Add(
										537 + (Random.NextDouble() < 0.4 ? Random.Next(-2, 0) : 0),
										Random.Next(1, 4)); // magma geode or worse
									break;

								case > 40:
									treasuresAndQuantities.Add(536 + (Random.NextDouble() < 0.4 ? -1 : 0),
										Random.Next(1, 4)); // frozen geode or worse
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

						case 1: // minerals
							if (mineLevel < 20)
							{
								treasuresAndQuantities.Add(382, Random.Next(1, 4)); // coal
								break;
							}

							switch (mineLevel)
							{
								case > 80:
									treasuresAndQuantities.Add(
										Random.NextDouble() < 0.3 ? 82 : Random.NextDouble() < 0.5 ? 64 : 60,
										Random.Next(1, 3)); // fire quartz else ruby or emerald
									break;

								case > 40:
									treasuresAndQuantities.Add(
										Random.NextDouble() < 0.3 ? 84 : Random.NextDouble() < 0.5 ? 70 : 62,
										Random.Next(1, 3)); // frozen tear else jade or aquamarine
									break;

								default:
									treasuresAndQuantities.Add(
										Random.NextDouble() < 0.3 ? 86 : Random.NextDouble() < 0.5 ? 66 : 68,
										Random.Next(1, 3)); // earth crystal else amethyst or topaz
									break;
							}

							if (Random.NextDouble() < 0.028 * mineLevel / 12)
								treasuresAndQuantities.Add(72, 1); // diamond
							else treasuresAndQuantities.Add(80, Random.Next(1, 3)); // quartz

							break;

						case 2: // special items
							var luckModifier = Math.Max(0, 1.0 + Game1.player.DailyLuck * mineLevel / 4);
							var streak = ModEntry.Data.ReadField<uint>("ProspectorHuntStreak");
							if (Random.NextDouble() < 0.025 * luckModifier && !Game1.player.specialItems.Contains(31))
								treasuresAndQuantities.Add(-1, 1); // femur

							if (Random.NextDouble() < 0.010 * luckModifier && !Game1.player.specialItems.Contains(60))
								treasuresAndQuantities.Add(-2, 1); // ossified blade

							if (Random.NextDouble() < 0.02 * luckModifier * Math.Pow(2, streak))
								treasuresAndQuantities.Add(74, 1); // prismatic shard

							if (treasuresAndQuantities.Count == 0)
								treasuresAndQuantities.Add(72, 1); // consolation diamond

							break;
					}

					break;
			}

			//if (treasuresAndQuantities.Count == 0) treasuresAndQuantities.Add(382, Random.Next(1, 4)); // coal

			foreach (var p in treasuresAndQuantities)
				switch (p.Key)
				{
					case -1:
						Game1.createItemDebris(new MeleeWeapon(31) { specialItem = true },
							new Vector2(TreasureTile.Value.X, TreasureTile.Value.Y) + new Vector2(32f, 32f),
							Random.Next(4), Game1.currentLocation);
						break;

					case -2:
						Game1.createItemDebris(new MeleeWeapon(60) { specialItem = true },
							new Vector2(TreasureTile.Value.X, TreasureTile.Value.Y) + new Vector2(32f, 32f),
							Random.Next(4), Game1.currentLocation);
						break;

					default:
						Game1.createMultipleObjectDebris(p.Key, (int)TreasureTile.Value.X, (int)TreasureTile.Value.Y,
							p.Value, Game1.player.UniqueMultiplayerID, Game1.currentLocation);
						break;
				}
		}
	}
}