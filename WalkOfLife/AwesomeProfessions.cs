using StardewModdingAPI;
using System;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod entry point.</summary>
	public partial class AwesomeProfessions : Mod
	{
		public static ProfessionsConfig Config { get; set; }
		public static ProfessionsData Data { get; set; }

		internal static int UniqueHash;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// get a unique 8-digit hash
			UniqueHash = (int)(Math.Abs(ModManifest.UniqueID.GetHashCode()) / Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(ModManifest.UniqueID.GetHashCode()))) - 8 + 1));

			// get configs.json
			Config = helper.ReadConfig<ProfessionsConfig>();

			// pimp events
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.Saved += OnSaved;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
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
	}
}
