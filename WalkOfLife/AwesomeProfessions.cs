using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;
using System;
using TheLion.AwesomeProfessions.Framework;
using TheLion.AwesomeProfessions.Framework.Patches;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod entry point.</summary>
	public class AwesomeProfessions : Mod
	{
		public static ProfessionsConfig Config { get; set; }
		public static ProfessionsData Data { get; set; }

		public static int UniqueBuffID { get; } = Common.Utils.GetDigitsFromHash("thelion.AwesomeProfessions", 8);
		public static int SpelunkerBuffID { get; } = UniqueBuffID + Utils.ProfessionMap.Forward["spelunker"];
		public static int DemolitionistBuffID { get; } = UniqueBuffID + Utils.ProfessionMap.Forward["demolitionist"];
		public static int BruteBuffID { get; } = UniqueBuffID - Utils.ProfessionMap.Forward["brute"];
		public static int GambitBuffID { get; } = UniqueBuffID - Utils.ProfessionMap.Forward["gambit"];

		public static int DemolitionistBuffMagnitude { get; set; } = 0;
		public static uint BruteKillStreak { get; set; } = 0;

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
			helper.Events.Player.Warped += OnWarped;

			// edit game assets
			helper.Content.AssetEditors.Add(new AssetEditor(helper.Content));

			// apply patches
			new HarmonyPatcher(ModManifest.UniqueID).ApplyAll(
				new TestPatch(Config, Monitor),
				new AnimalHouseAddNewHatchedAnimalPatch(Config, Monitor),
				new BasicProjectileBehaviorOnCollisionWithMonsterPatch(Config, Monitor, helper.Reflection),
				new BobberBarCtorPatch(Config, Monitor),
				new BushShakePatch(Config, Monitor),
				new CaskPerformObjectDropInActionPatch(Config, Monitor),
				new CrabPotCheckForActionPatch(Config, Monitor),
				new CrabPotDayUpdatePatch(Config, Monitor),
				new CraftingRecipeCtorPatch(Config, Monitor),
				new CropHarvestPatch(Config, Monitor),
				new FarmAnimalDayUpdatePatch(Config, Monitor),
				new FarmAnimalGetSellPricePatch(Config, Monitor),
				new FarmAnimalPetPatch(Config, Monitor),
				new FishingRodStartMinigameEndFunctionPatch(Config, Monitor),
				new FishPondUpdateMaximumOccupancyPatch(Config, Monitor),
				new FruitTreeDayUpdatePatch(Config, Monitor),
				new Game1CreateItemDebrisPatch(Config, Monitor),
				new Game1CreateObjectDebrisPatch(Config, Monitor),
				new Game1DrawHUDPatch(Config, Monitor),
				new GameLocationBreakStonePatch(Config, Monitor),
				new GameLocationCheckActionPatch(Config, Monitor),
				new GameLocationDamageMonsterPatch(Config, Monitor, helper.Translation),
				new GameLocationGetFishPatch(Config, Monitor),
				new GameLocationExplodePatch(Config, Monitor),
				new GameLocationOnStoneDestroyedPatch(Config, Monitor),
				new HoeDirtApplySpeedIncreasesPatch(Config, Monitor),
				new LevelUpMenuAddProfessionDescriptionsPatch(Config, Monitor, helper.Translation),
				new LevelUpMenuGetImmediateProfessionPerkPatch(Config, Monitor),
				new LevelUpMenuGetProfessionNamePatch(Config, Monitor),
				new LevelUpMenuGetProfessionTitleFromNumberPatch(Config, Monitor, helper.Translation),
				new LevelUpMenuRemoveImmediateProfessionPerkPatch(Config, Monitor),
				new LevelUpMenuRevalidateHealthPatch(Config, Monitor),
				new MeleeWeaponDoAnimateSpecialMovePatch(Config, Monitor),
				new MineShaftCheckStoneForItemsPatch(Config, Monitor),
				new ObjectCtorPatch(Config, Monitor),
				new ObjectGetMinutesForCrystalariumPatch(Config, Monitor),
				new ObjectGetPriceAfterMultipliersPatch(Config, Monitor),
				new PondQueryMenuDrawPatch(Config, Monitor, helper.Reflection),
				new ProjectileBehaviorOnCollisionPatch(Config, Monitor),
				new QuestionEventSetUpPatch(Config, Monitor),
				new SlingshotPerformFirePatch(Config, Monitor),
				new TemporaryAnimatedSpriteCtorPatch(Config, Monitor),
				new TreeDayUpdatePatch(Config, Monitor),
				new TreeUpdateTapperProductPatch(Config, Monitor)
			);
		}

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
			if (Utils.LocalPlayerHasProfession("spelunker") && Game1.currentLocation is MineShaft)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == SpelunkerBuffID);
				if (buff == null)
				{
					Game1.buffsDisplay.addOtherBuff(
						buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: 1, 0, 0, minutesDuration: 1, source: "spelunker", displaySource: Helper.Translation.Get("spelunker.name"))
						{
							which = SpelunkerBuffID,
						}
					);
				}
			}

			if (DemolitionistBuffMagnitude > 0)
			{
				if (e.Ticks % 30 == 0)
				{
					int buffDecay = DemolitionistBuffMagnitude > 4 ? 2 : 1;
					DemolitionistBuffMagnitude = Math.Max(0, DemolitionistBuffMagnitude - buffDecay);
				}

				int buffId = DemolitionistBuffID + DemolitionistBuffMagnitude;
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
				if (buff == null)
				{
					Game1.buffsDisplay.addOtherBuff(
						buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: DemolitionistBuffMagnitude, 0, 0, minutesDuration: 1, source: "demolitionist", displaySource: Helper.Translation.Get("demolitionist.name"))
						{
							which = buffId,
							millisecondsDuration = 50,
							description = Helper.Translation.Get("demolitionist.buffdescription")
						}
					);
				}
			}

			if (BruteKillStreak > 0)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == BruteBuffID);
				if (buff != null)
					Game1.buffsDisplay.removeOtherBuff(BruteBuffID);

				Game1.buffsDisplay.addOtherBuff(
					buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "brute", displaySource: Helper.Translation.Get("brute.name"))
					{
						which = BruteBuffID,
						sheetIndex = 20,
						millisecondsDuration = 50,
						description = Helper.Translation.Get("brute.buffdescription", new { buffMagnitude = Math.Truncate(BruteKillStreak * 5.0) / 10 })
					}
				);
			}

			double healthPercent;
			if (Utils.LocalPlayerHasProfession("gambit") && (healthPercent = (double)Game1.player.health / Game1.player.maxHealth) < 1)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == GambitBuffID);
				if (buff != null)
					Game1.buffsDisplay.removeOtherBuff(GambitBuffID);

				Game1.buffsDisplay.addOtherBuff(
					buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "gambit", displaySource: Helper.Translation.Get("gambit.name"))
					{
						which = GambitBuffID,
						sheetIndex = 20,
						millisecondsDuration = 50,
						description = Helper.Translation.Get("gambit.buffdescription", new { buffMagnitude = Math.Truncate(200.0 / (healthPercent + 0.2) - 200.0 / 1.2) / 10 })
					}
				);

				if (e.IsOneSecond)
				{
					Monitor.Log($"{healthPercent}", LogLevel.Info);
				}
			}
		}

		/// <summary>Raised after the current player moves to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.NewLocation is MineShaft)
			{
				int currentMineLevel = (e.NewLocation as MineShaft).mineLevel;
				if (currentMineLevel > Data.LowestMineLevelReached)
					Data.LowestMineLevelReached = currentMineLevel;
			}

			if (e.NewLocation.GetType() != e.OldLocation.GetType())
				BruteKillStreak = 0;
		}
	}
}
