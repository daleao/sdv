using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Manages treasure hunt events for Prospector profession.</summary>
	internal class ProspectorHunt : TreasureHunt
	{
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
			if (!base.TryStartNewHunt()) return;

			int i = Random.Next(location.Objects.Count());
			Vector2 v = location.Objects.Keys.ElementAt(i);
			var obj = location.Objects[v];
			if (Utility.IsStone(obj) && !Utility.IsResourceNode(obj))
			{
				TreasureTile = v;
				_timeLimit = (uint)location.Objects.Count() / 3;
				_elapsed = 0;
				AwesomeProfessions.EventManager.Subscribe(new ProspectorHuntUpdateTickedEvent(), new ProspectorHuntRenderedHudEvent());
				Game1.addHUDMessage(new HuntNotification(HuntStartedMessage, Icon));
			}
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
			if (!Game1.currentLocation.Objects.ContainsKey(TreasureTile.Value))
			{
				_GetStoneTreasure();
				End();
				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ProspectorHuntStreak");
			}
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
			int mineLevel = (Game1.currentLocation as MineShaft).mineLevel;
			Dictionary<int, int> treasuresAndQuantities = new();

			if (Random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
				treasuresAndQuantities.Add(890, Random.Next(1, 3) + Random.NextDouble() < 0.25 ? 2 : 0); // qi bean

			switch (Random.Next(2))
			{
				case 0:
					if (mineLevel > 120 && Random.NextDouble() < 0.03)
						treasuresAndQuantities.Add(386, Random.Next(1, 3)); // iridium ore

					List<int> possibles = new();
					if (mineLevel > 80) possibles.Add(384); // gold ore

					if (mineLevel > 40 && (possibles.Count == 0 || Random.NextDouble() < 0.6)) possibles.Add(380); // iron ore

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

				case 1:
					switch (Random.Next(3))
					{
						case 0:
							if (mineLevel > 80) treasuresAndQuantities.Add(537 + Random.NextDouble() < 0.4 ? Random.Next(-2, 0) : 0, Random.Next(1, 4)); // magma geode or worse
							else if (mineLevel > 40) treasuresAndQuantities.Add(536 + Random.NextDouble() < 0.4 ? -1 : 0, Random.Next(1, 4)); // frozen geode or worse
							else treasuresAndQuantities.Add(535, Random.Next(1, 4)); // regular geode

							if (Random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
							{
								var key = treasuresAndQuantities.Keys.Last();
								treasuresAndQuantities[key] *= 2;
							}

							break;

						case 1:
							if (mineLevel < 20)
							{
								treasuresAndQuantities.Add(382, Random.Next(1, 4)); // coal
								break;
							}

							if (mineLevel > 80) treasuresAndQuantities.Add(Random.NextDouble() < 0.3 ? 82 : Random.NextDouble() < 0.5 ? 64 : 60, Random.Next(1, 3)); // fire quartz else ruby or emerald
							else if (mineLevel > 40) treasuresAndQuantities.Add(Random.NextDouble() < 0.3 ? 84 : Random.NextDouble() < 0.5 ? 70 : 62, Random.Next(1, 3)); // frozen tear else jade or aquamarine
							else treasuresAndQuantities.Add(Random.NextDouble() < 0.3 ? 86 : Random.NextDouble() < 0.5 ? 66 : 68, Random.Next(1, 3)); // earth crystal else amethyst or topaz

							if (Random.NextDouble() < 0.028 * mineLevel / 12) treasuresAndQuantities.Add(72, 1); // diamond
							else treasuresAndQuantities.Add(80, Random.Next(1, 3)); // quartz

							break;

						case 2:
							double luckModifier = Math.Max(0, 1.0 + Game1.player.DailyLuck * mineLevel / 4);
							if (Random.NextDouble() < 0.05 * luckModifier) treasuresAndQuantities.Add(275, 1); // artifact trove

							if (Random.NextDouble() < 0.002 * (luckModifier * AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ProspectorHuntStreak", uint.Parse))) treasuresAndQuantities.Add(74, 1); // prismatic shard

							if (treasuresAndQuantities.Count == 1) treasuresAndQuantities.Add(72, 1); // consolation diamond

							break;
					}

					break;
			}

			if (treasuresAndQuantities.Count == 0) treasuresAndQuantities.Add(390, Random.Next(1, 4));

			foreach (var kvp in treasuresAndQuantities)
				Game1.createMultipleObjectDebris(kvp.Key, (int)TreasureTile.Value.X, (int)TreasureTile.Value.Y, kvp.Value, Game1.player.UniqueMultiplayerID, Game1.currentLocation);
		}
	}
}