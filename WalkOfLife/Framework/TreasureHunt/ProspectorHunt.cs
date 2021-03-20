using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions
{
	public class ProspectorHunt : TreasureHunt
	{
		private readonly string _newHuntMessage;
		private readonly string _failedHuntMessage;
		private readonly Texture2D _icon;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The overal mod settings.</param>
		/// <param name="i18n">Provides localized text.</param>
		/// <param name="content">Interface for loading content assets.</param>
		public ProspectorHunt(ProfessionsConfig config, ProfessionsData data, ITranslationHelper i18n, IContentHelper content)
			: base(config, data)
		{
			_newHuntMessage = i18n.Get("scavenger.huntstarted");
			_failedHuntMessage = i18n.Get("scavenger.huntfailed");
			_icon = content.Load<Texture2D>(Path.Combine("Assets", "scavenger.png"));
		}

		/// <summary>Try to start a new prospector hunt at this location.</summary>
		/// <param name="location">The game location.</param>
		public override void TryStartNewHunt(GameLocation location)
		{
			if (!TryStartNewHunt()) return;

			int i = random.Next(location.Objects.Count());
			Vector2 v = location.Objects.Keys.ElementAt(i);
			var obj = location.Objects[v];
			if (Utility.IsStone(obj) && !Utility.IsResourceNode(obj))
			{
				TreasureTile = v;
				elapsed = 0;
				AwesomeProfessions.EventManager.Subscribe(new ProspectorHuntUpdateTickedEvent());
				Game1.addHUDMessage(new HuntNotification(_newHuntMessage, _icon));
			}
		}

		/// <summary>Check if the player has found the treasure tile.</summary>
		protected override void CheckForCompletion()
		{
			if (!Game1.currentLocation.Objects.ContainsKey(TreasureTile.Value))
			{
				_GetStoneTreasure();
				End();
				++_data.ProspectorHuntStreak;
			}
		}

		/// <summary>End the hunt unsuccessfully.</summary>
		protected override void Fail()
		{
			End();
			_data.ProspectorHuntStreak = 0;
			Game1.addHUDMessage(new HuntNotification(_failedHuntMessage));
		}

		/// <summary>Reset treasure tile and unsubcribe treasure hunt update event.</summary>
		protected override void End()
		{
			AwesomeProfessions.EventManager.Unsubscribe(typeof(ProspectorHuntUpdateTickedEvent));
			TreasureTile = null;
		}

		/// <summary>Spawn hunt spoils as debris.</summary>
		/// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
		private void _GetStoneTreasure()
		{
			int mineLevel = (Game1.currentLocation as MineShaft).mineLevel;
			Dictionary<int, int> treasuresAndQuantities = new();

			if (random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
				treasuresAndQuantities.Add(890, random.Next(1, 3) + random.NextDouble() < 0.25 ? 2 : 0);	// qi bean

			switch (random.Next(2))
			{
				case 0:
					if (mineLevel > 120 && random.NextDouble() < 0.03)
						treasuresAndQuantities.Add(386, random.Next(1, 3));	// iridium ore

					List<int> possibles = new();
					if (mineLevel > 80) possibles.Add(384);	// gold ore

					if (mineLevel > 40 && (possibles.Count == 0 || random.NextDouble() < 0.6)) possibles.Add(380);	// iron ore

					if (possibles.Count == 0 || random.NextDouble() < 0.6) possibles.Add(378);	// copper ore

					if (possibles.Count == 0 || random.NextDouble() < 0.6) possibles.Add(390);	// stone

					possibles.Add(382);	// coal
					treasuresAndQuantities.Add(possibles.ElementAt(random.Next(possibles.Count)), random.Next(2, 7));
					if (random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
					{
						var key = treasuresAndQuantities.Keys.Last();
						treasuresAndQuantities.Replace(key, treasuresAndQuantities[key] * 2);
					}

					break;
				case 1:
					switch (random.Next(3))
					{
						case 0:
							if (mineLevel > 80) treasuresAndQuantities.Add(537 + random.NextDouble() < 0.4 ? random.Next(-2, 0) : 0, random.Next(1, 4));	// magma geode or worse
							else if (mineLevel > 40) treasuresAndQuantities.Add(536 + random.NextDouble() < 0.4 ? -1 : 0, random.Next(1, 4));	// frozen geode or worse
							else treasuresAndQuantities.Add(535, random.Next(1, 4));	// regular geode

							if (random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
							{
								var key = treasuresAndQuantities.Keys.Last();
								treasuresAndQuantities.Replace(key, treasuresAndQuantities[key] * 2);
							}

							break;
						case 1:
							if (mineLevel < 20)
							{
								treasuresAndQuantities.Add(382, random.Next(1, 4));	// coal
								break;
							}

							if (mineLevel > 80) treasuresAndQuantities.Add(random.NextDouble() < 0.3 ? 82 : random.NextDouble() < 0.5 ? 64 : 60, random.Next(1, 3));	// fire quartz else ruby or emerald
							else if (mineLevel > 40) treasuresAndQuantities.Add(random.NextDouble() < 0.3 ? 84 : random.NextDouble() < 0.5 ? 70 : 62, random.Next(1, 3));	// frozen tear else jade or aquamarine
							else treasuresAndQuantities.Add(random.NextDouble() < 0.3 ? 86 : random.NextDouble() < 0.5 ? 66 : 68, random.Next(1, 3));	// earth crystal else amethyst or topaz

							if (random.NextDouble() < 0.028 * mineLevel / 12) treasuresAndQuantities.Add(72, 1);	// diamond
							else treasuresAndQuantities.Add(80, random.Next(1, 3));	// quartz

							break;
						case 2:
							double luckModifier = Math.Max(0, 1.0 + Game1.player.DailyLuck * mineLevel / 4);
							if (random.NextDouble() < 0.05 * luckModifier)
								treasuresAndQuantities.Add(275, 1);	// artifact trove

							if (random.NextDouble() < 0.002 * (luckModifier + _data.ProspectorHuntStreak / 10))
								treasuresAndQuantities.Add(74, 1);	// prismatic shard


							if (treasuresAndQuantities.Count == 1)
								treasuresAndQuantities.Add(72, 1);	// consolation diamond

							break;
					}

					break;
			}

			if (treasuresAndQuantities.Count == 0)
				treasuresAndQuantities.Add(390, random.Next(1, 4));

			foreach (var kvp in treasuresAndQuantities)
				Game1.createMultipleObjectDebris(kvp.Key, (int)TreasureTile.Value.X, (int)TreasureTile.Value.Y, kvp.Value, Game1.player.UniqueMultiplayerID, Game1.currentLocation);
		}
	}
}
