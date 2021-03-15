using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Linq;
using TheLion.AwesomeProfessions.Framework;
using TheLion.AwesomeProfessions.Framework.Patches;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod entry point.</summary>
	public class AwesomeProfessions : Mod
	{
		#region public properties
		public static ProfessionsConfig Config { get; set; }
		public static ProfessionsData Data { get; set; }
		#endregion public properties

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// get configs.json
			Config = helper.ReadConfig<ProfessionsConfig>();

			// add event hooks
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.Saved += OnSaved;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.Player.LevelChanged += OnLevelChanged;
			helper.Events.Player.Warped += OnWarped;

			// edit game assets
			helper.Content.AssetEditors.Add(new AssetEditor(helper.Content));

			// apply patches
			new HarmonyPatcher(ModManifest.UniqueID).ApplyAll(
				new AnimalHouseAddNewHatchedAnimalPatch(Monitor),
				new BasicProjectileBehaviorOnCollisionWithMonsterPatch(Monitor, helper.Reflection),
				new BasicProjectileCtorPatch(Monitor),
				new BobberBarCtorPatch(Monitor),
				new BushShakePatch(Monitor),
				new CaskPerformObjectDropInActionPatch(Monitor),
				new CrabPotCheckForActionPatch(Monitor),
				new CrabPotDayUpdatePatch(Monitor),
				new CraftingRecipeCtorPatch(Monitor),
				new CropHarvestPatch(Monitor),
				new FarmAnimalDayUpdatePatch(Monitor),
				new FarmAnimalGetSellPricePatch(Monitor),
				new FarmAnimalPetPatch(Monitor),
				new FishingRodStartMinigameEndFunctionPatch(Monitor),
				new FishPondUpdateMaximumOccupancyPatch(Monitor),
				new FruitTreeDayUpdatePatch(Monitor),
				new Game1CreateItemDebrisPatch(Monitor),
				new Game1CreateObjectDebrisPatch(Monitor),
				new Game1DrawHUDPatch(Monitor),
				new GameLocationBreakStonePatch(Monitor),
				new GameLocationCheckActionPatch(Monitor),
				new GameLocationDamageMonsterPatch(Monitor),
				new GameLocationGetFishPatch(Monitor),
				new GameLocationExplodePatch(Monitor),
				new GameLocationOnStoneDestroyedPatch(Monitor),
				new HoeDirtApplySpeedIncreasesPatch(Monitor),
				new LevelUpMenuAddProfessionDescriptionsPatch(Monitor, helper.Translation),
				new LevelUpMenuGetImmediateProfessionPerkPatch(Monitor),
				new LevelUpMenuGetProfessionNamePatch(Monitor),
				new LevelUpMenuGetProfessionTitleFromNumberPatch(Monitor, helper.Translation),
				new LevelUpMenuRemoveImmediateProfessionPerkPatch(Monitor),
				new LevelUpMenuRevalidateHealthPatch(Monitor),
				new MeleeWeaponDoAnimateSpecialMovePatch(Monitor),
				new MineShaftCheckStoneForItemsPatch(Monitor),
				new ObjectCtorPatch(Monitor),
				new ObjectGetMinutesForCrystalariumPatch(Monitor),
				new ObjectGetPriceAfterMultipliersPatch(Monitor),
				new PondQueryMenuDrawPatch(Monitor, helper.Reflection),
				new ProjectileBehaviorOnCollisionPatch(Monitor),
				new QuestionEventSetUpPatch(Monitor),
				new SlingshotPerformFirePatch(Monitor),
				new TemporaryAnimatedSpriteCtorPatch(Monitor),
				new TreeDayUpdatePatch(Monitor),
				new TreeUpdateTapperProductPatch(Monitor)
			);
		}

		#region event hooks
		/// <summary>Raised after loading a save (including the first day after creating a new save), or connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			Data = Helper.Data.ReadSaveData<ProfessionsData>("thelion.AwesomeProfessions") ?? new ProfessionsData();
		}

		/// <summary>Raised after the game writes data to save file (except the initial save creation).</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnSaved(object sender, SavedEventArgs e)
		{
			Helper.Data.WriteSaveData("thelion.AwesomeProfessions", Data);
		}

		/// <summary>Raised after the game state is updated.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			// handle Spelunker buff
			if (Globals.LocalPlayerHasProfession("spelunker") && Game1.currentLocation is MineShaft)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Globals.SpelunkerBuffID);
				if (buff == null)
				{
					Game1.buffsDisplay.addOtherBuff(
						buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: 1, 0, 0, minutesDuration: 1, source: "spelunker", displaySource: Helper.Translation.Get("spelunker.name"))
						{
							which = Globals.SpelunkerBuffID,
						}
					);
				}
			}

			// handle Demolitionist buff
			if (Globals.DemolitionistBuffMagnitude > 0)
			{
				if (e.Ticks % 30 == 0)
				{
					int buffDecay = Globals.DemolitionistBuffMagnitude > 4 ? 2 : 1;
					Globals.DemolitionistBuffMagnitude = Math.Max(0, Globals.DemolitionistBuffMagnitude - buffDecay);
				}

				int buffId = Globals.DemolitionistBuffID + Globals.DemolitionistBuffMagnitude;
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
				if (buff == null)
				{
					Game1.buffsDisplay.addOtherBuff(
						buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: Globals.DemolitionistBuffMagnitude, 0, 0, minutesDuration: 1, source: "demolitionist", displaySource: Helper.Translation.Get("demolitionist.name"))
						{
							which = buffId,
							millisecondsDuration = 50,
							description = Helper.Translation.Get("demolitionist.buffdescription")
						}
					);
				}
			}

			// handle Brute buff
			if (Globals.BruteKillStreak > 0)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Globals.BruteBuffID);
				if (buff != null)
					Game1.buffsDisplay.removeOtherBuff(Globals.BruteBuffID);

				Game1.buffsDisplay.addOtherBuff(
					buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "brute", displaySource: Helper.Translation.Get("brute.name"))
					{
						which = Globals.BruteBuffID,
						sheetIndex = 20,
						millisecondsDuration = 50,
						description = Helper.Translation.Get("brute.buffdescription", new { buffMagnitude = Math.Truncate(Globals.BruteKillStreak * 5.0) / 10 })
					}
				);
			}

			// handle Gambit buff
			double healthPercent;
			if (Globals.LocalPlayerHasProfession("gambit") && (healthPercent = (double)Game1.player.health / Game1.player.maxHealth) < 1)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Globals.GambitBuffID);
				if (buff != null)
					Game1.buffsDisplay.removeOtherBuff(Globals.GambitBuffID);

				Game1.buffsDisplay.addOtherBuff(
					buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "gambit", displaySource: Helper.Translation.Get("gambit.name"))
					{
						which = Globals.GambitBuffID,
						sheetIndex = 20,
						millisecondsDuration = 50,
						description = Helper.Translation.Get("gambit.buffdescription", new { buffMagnitude = Math.Truncate(200.0 / (healthPercent + 0.2) - 200.0 / 1.2) / 10 })
					}
				);
			}
		}

		/// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			// reset Conservationist collected trash
			if (Game1.dayOfMonth == 1) Data.OceanTrashCollectedThisSeason = 0;
		}

		/// <summary>Raised after a player's skill level changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnLevelChanged(object sender, LevelChangedEventArgs e)
		{
			// ensure immediate perks get removed on skill level reset
			if (e.IsLocalPlayer && e.Skill.AnyOf(SkillType.Combat, SkillType.Fishing) && e.NewLevel == 0) LevelUpMenu.RevalidateHealth(e.Player);
		}

		/// <summary>Raised after the current player moves to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			Globals.InitialLadderTiles.Clear();
			if (e.NewLocation is MineShaft)
			{
				// find ladder positions for Prospector
				if (Globals.SpecificPlayerHasProfession("prospector", e.Player))
					foreach (var tile in Globals.GetLadderTiles(e.NewLocation as MineShaft)) Globals.InitialLadderTiles.Add(tile);

				// record lowest level reached for local player
				uint currentMineLevel = (uint)(e.NewLocation as MineShaft).mineLevel;
				if (currentMineLevel > Data.LowestMineLevelReached) Data.LowestMineLevelReached = currentMineLevel;
			}

			// reset Brute buff
			if (Globals.BruteKillStreak > 0 && e.NewLocation.GetType() != e.OldLocation.GetType()) Globals.BruteKillStreak = 0;
		}
		#endregion event hooks
	}
}
