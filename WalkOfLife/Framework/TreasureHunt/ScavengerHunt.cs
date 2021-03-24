using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerHunt : TreasureHunt
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The overal mod settings.</param>
		/// <param name="data">The mod persisted data.</param>
		/// <param name="manager">The event manager.</param>
		/// <param name="i18n">Provides localized text.</param>
		/// <param name="content">Interface for loading content assets.</param>
		internal ScavengerHunt(ProfessionsConfig config, ProfessionsData data, EventManager manager, ITranslationHelper i18n, IContentHelper content)
			: base(config, data, manager)
		{
			NewHuntMessage = i18n.Get("scavenger.huntstarted");
			FailedHuntMessage = i18n.Get("scavenger.huntfailed");
			Icon = content.Load<Texture2D>(Path.Combine("Assets", "scavenger.png"));
			TimeLimit = config.ScavengerHuntTimeLimitSeconds;
		}

		/// <summary>Try to start a new scavenger hunt at this location.</summary>
		/// <param name="location">The game location.</param>
		internal override void TryStartNewHunt(GameLocation location)
		{
			if (!base.TryStartNewHunt()) return;

			int x = Random.Next(location.Map.DisplayWidth / Game1.tileSize);
			int y = Random.Next(location.Map.DisplayHeight / Game1.tileSize);
			Vector2 v = new Vector2(x, y);
			if (Utility.IsTileValidForTreasure(v, location))
			{
				Utility.MakeTileDiggable(v, location);
				TreasureTile = v;
				_elapsed = 0;
				Manager.Subscribe(new ScavengerHuntUpdateTickedEvent(this), new ScavengerHuntRenderingHudEvent(this));
				Game1.addHUDMessage(new HuntNotification(NewHuntMessage, Icon));
			}
		}

		/// <summary>Reset treasure tile and unsubcribe treasure hunt update event.</summary>
		internal override void End()
		{
			Manager.Unsubscribe(typeof(ScavengerHuntUpdateTickedEvent), typeof(ProspectorHuntRenderingHudEvent));
			TreasureTile = null;
		}

		/// <summary>Check if the player has found the treasure tile.</summary>
		protected override void CheckForCompletion()
		{
			if (Game1.player.CurrentTool is Hoe && Game1.player.UsingTool)
			{
				Vector2 actionTile = new Vector2((int)(Game1.player.GetToolLocation().X / Game1.tileSize), (int)(Game1.player.GetToolLocation().Y / Game1.tileSize));
				if (actionTile == TreasureTile.Value)
				{
					End();
					DelayedAction getTreasure = new DelayedAction(200, () => _BeginFindTreasure());
					Game1.delayedActions.Add(getTreasure);
					++Data.ScavengerHuntStreak;
				}
			}
		}

		/// <summary>End the hunt unsuccessfully.</summary>
		protected override void Fail()
		{
			End();
			Game1.addHUDMessage(new HuntNotification(FailedHuntMessage));
			Data.ScavengerHuntStreak = 0;
		}

		/// <summary>Play treasure chest found animation.</summary>
		private void _BeginFindTreasure()
		{
			Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Path.Combine("LooseSprites", "Cursors"), new Rectangle(64, 1920, 32, 32), 500f, 1, 0, Game1.player.Position + new Vector2(-32f, -160f), flicker: false, flipped: false, Game1.player.getStandingY() / 10000f + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
			{
				motion = new Vector2(0f, -0.128f),
				timeBasedMotion = true,
				endFunction = _OpenChestEndFunction,
				extraInfoForEndBehavior = 0,
				alpha = 0f,
				alphaFade = -0.002f
			});
		}

		/// <summary>Play open treasure chest animation.</summary>
		/// <param name="extra">Not applicable.</param>
		private void _OpenChestEndFunction(int extra)
		{
			Game1.currentLocation.localSound("openChest");
			Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(Path.Combine("LooseSprites", "Cursors"), new Rectangle(64, 1920, 32, 32), 200f, 4, 0, Game1.player.Position + new Vector2(-32f, -228f), flicker: false, flipped: false, Game1.player.getStandingY() / 10000f + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
			{
				endFunction = _OpenTreasureMenuEndFunction,
				extraInfoForEndBehavior = 0
			});
		}

		/// <summary>Open the treasure chest menu.</summary>
		/// <param name="extra">Not applicable.</param>
		private void _OpenTreasureMenuEndFunction(int extra)
		{
			Game1.player.completelyStopAnimatingOrDoingAction();
			var treasures = _GetTreasureContents();
			Game1.activeClickableMenu = new ItemGrabMenu(treasures).setEssential(essential: true);
			(Game1.activeClickableMenu as ItemGrabMenu).source = 3;
		}

		/// <summary>Choose the contents of the treasure chest.</summary>
		/// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
		private List<Item> _GetTreasureContents()
		{
			List<Item> treasures = new();
			double chance = 1.0;
			while (Random.NextDouble() <= chance)
			{
				chance *= 0.4f;
				if (Game1.currentSeason.Equals("spring") && !(Game1.currentLocation is Beach) && Random.NextDouble() < 0.1)
					treasures.Add(new SObject(273, Random.Next(2, 6) + Random.NextDouble() < 0.25 ? 5 : 0)); // rice shoot

				if (Random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
					treasures.Add(new SObject(890, Random.Next(1, 3) + Random.NextDouble() < 0.25 ? 2 : 0)); // qi beans

				switch (Random.Next(4))
				{
					case 0:
						List<int> possibles = new();
						if (Random.NextDouble() < 0.4) possibles.Add(386); // iridium ore

						if (possibles.Count == 0 || Random.NextDouble() < 0.4) possibles.Add(384); // gold ore

						if (possibles.Count == 0 || Random.NextDouble() < 0.4) possibles.Add(380); // iron ore

						if (possibles.Count == 0 || Random.NextDouble() < 0.4) possibles.Add(378); // copper ore

						if (possibles.Count == 0 || Random.NextDouble() < 0.4) possibles.Add(388); // wood

						if (possibles.Count == 0 || Random.NextDouble() < 0.4) possibles.Add(390); // stone

						possibles.Add(382); // coal
						treasures.Add(new SObject(possibles.ElementAt(Random.Next(possibles.Count)), Random.Next(2, 7) * ((!(Random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.015)) ? 1 : 2)));
						if (Random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03) treasures.Last().Stack *= 2;

						break;
					case 1:
						if (Random.NextDouble() < 0.25 && Game1.player.craftingRecipes.ContainsKey("Wild Bait"))
							treasures.Add(new SObject(774, 5 + ((Random.NextDouble() < 0.25) ? 5 : 0))); // wild bait
						else treasures.Add(new SObject(685, 10)); // bait

						break;
					case 2:
						if (Random.NextDouble() < 0.1 && Game1.netWorldState.Value.LostBooksFound.Value < 21 && Game1.player.hasOrWillReceiveMail("lostBookFound"))
							treasures.Add(new SObject(102, 1)); // lost book
						else if (Game1.player.archaeologyFound.Count() > 0) // artifacts
						{
							if (Random.NextDouble() < 0.25) treasures.Add(new SObject(Random.Next(579, 585), 1));
							else if (Random.NextDouble() < 0.5) treasures.Add(new SObject(Random.NextDouble() < 0.25 ? Random.Next(100, 102) : Random.Next(120, 126), 1));
							else treasures.Add(new SObject(535, 1));
						}
						else treasures.Add(new SObject(382, Random.Next(1, 3))); // coal

						break;
					case 3:
						switch (Random.Next(3))
						{
							case 0:
								treasures.Add(new SObject(535 + Random.NextDouble() < 0.4 ? Random.Next(2) : 0, Random.Next(1, 4))); // geodes
								if (Random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03) treasures.Last().Stack *= 2;

								break;
							case 1:
								switch (Random.Next(4))
								{
									case 0: // fire quartz else ruby or emerald
										treasures.Add(new SObject(Random.NextDouble() < 0.3 ? 82 : Random.NextDouble() < 0.5 ? 64 : 60, Random.Next(1, 3)));
										break;
									case 1: // frozen tear else jade or aquamarine
										treasures.Add(new SObject(Random.NextDouble() < 0.3 ? 84 : Random.NextDouble() < 0.5 ? 70 : 62, Random.Next(1, 3)));
										break;
									case 2: // earth crystal else amethyst or topaz
										treasures.Add(new SObject(Random.NextDouble() < 0.3 ? 86 : Random.NextDouble() < 0.5 ? 66 : 68, Random.Next(1, 3)));
										break;
									case 3:
										if (Random.NextDouble() < 0.28) treasures.Add(new SObject(72, 1)); // diamond
										else treasures.Add(new SObject(80, Random.Next(1, 3))); // quartz
										break;
								}
								
								if (Random.NextDouble() < 0.05) treasures.Last().Stack *= 2;

								break;
							case 2:
								double luckModifier = 1.0 + Game1.player.DailyLuck * 10;
								if (Random.NextDouble() < 0.025 * luckModifier && !Game1.player.specialItems.Contains(60))
								{
									treasures.Add(new MeleeWeapon(15) { specialItem = true }); // forest sword
								}

								if (Random.NextDouble() < 0.025 * luckModifier && !Game1.player.specialItems.Contains(20))
								{
									treasures.Add(new MeleeWeapon(20) { specialItem = true }); // elf blade
								}

								if (Random.NextDouble() < 0.07 * luckModifier)
								{
									switch (Random.Next(3))
									{
										case 0:
											treasures.Add(new Ring(516 + Random.NextDouble() < Game1.player.LuckLevel / 11f ? 1 : 0)); // (small) glow ring
											break;
										case 1:
											treasures.Add(new Ring(518 + Random.NextDouble() < Game1.player.LuckLevel / 11f ? 1 : 0)); // (small) magnet ring
											break;
										case 2:
											treasures.Add(new Ring(Random.Next(529, 535))); // gemstone ring
											break;
									}
								}

								if (Random.NextDouble() < 0.02 * luckModifier) treasures.Add(new SObject(166, 1)); // treasure chest

								if (Random.NextDouble() < 0.001 * (luckModifier * Data.ScavengerHuntStreak)) treasures.Add(new SObject(74, 1));	// prismatic shard

								if (Random.NextDouble() < 0.01 * luckModifier) treasures.Add(new SObject(127, 1)); // strange doll

								if (Random.NextDouble() < 0.01 * luckModifier) treasures.Add(new SObject(126, 1)); // strange doll

								if (Random.NextDouble() < 0.01 * luckModifier) treasures.Add(new Ring(527)); // iridium band

								if (Random.NextDouble() < 0.01 * luckModifier) treasures.Add(new Boots(Random.Next(504, 514))); // boots

								if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && Random.NextDouble() < 0.01 * luckModifier) treasures.Add(new SObject(928, 1)); // golden egg

								if (treasures.Count == 1) treasures.Add(new SObject(72, 1)); // consolation diamond

								break;
						}

						break;
				}
			}

			if (treasures.Count == 0)
			{
				if (Random.NextDouble() < 0.5)
				{
					switch (Game1.currentSeason) // forage seeds
					{
						case "spring":
							treasures.Add(new SObject(495, 1));
							break;
						case "summer":
							treasures.Add(new SObject(496, 1));
							break;
						case "fall":
							treasures.Add(new SObject(496, 1));
							break;
						case "winter":
							treasures.Add(new SObject(496, 1));
							break;
					}
				}
				else treasures.Add(new SObject(770, Random.Next(1, 4) * 5)); // wild seeds
			}
			
			return treasures;
		}
	}
}