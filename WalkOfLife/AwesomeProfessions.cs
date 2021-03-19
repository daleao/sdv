using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod entry point.</summary>
	public class AwesomeProfessions : Mod
	{
		public static IModHelper ModHelper { get; set; }
		public static ITranslationHelper I18n { get; set; }
		public static ProfessionsConfig Config { get; set; }
		public static ProfessionsData Data { get; set; }
		public static EventManager EventManager { get; set; }
		public static ProspectorHunt ProspectorHunt { get; set; }
		public static ScavengerHunt ScavengerHunt { get; set; }

		public static int DemolitionistBuffMagnitude { get; set; } = 0;
		public static uint BruteKillStreak { get; set; } = 0;
		public static List<Vector2> InitialLadderTiles { get; } = new();

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// store a reference to helper
			ModHelper = helper;

			// store a reference to localized text
			I18n = helper.Translation;

			// get configs.json
			Config = helper.ReadConfig<ProfessionsConfig>();

			// get mod assets
			helper.Content.AssetEditors.Add(new AssetEditor(helper.Content));
			Utility.ArrowPointer.Texture = helper.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(Path.Combine("Assets", "cursor.png"));

			// apply patches
			BasePatch.Init(Config, Data, Monitor);
			new HarmonyPatcher(ModManifest.UniqueID).ApplyAll(
				new AnimalHouseAddNewHatchedAnimalPatch(),
				new BasicProjectileBehaviorOnCollisionWithMonsterPatch(helper.Reflection),
				new BasicProjectileCtorPatch(),
				new BobberBarCtorPatch(),
				new BushShakePatch(),
				new CaskPerformObjectDropInActionPatch(),
				new CrabPotCheckForActionPatch(),
				new CrabPotDayUpdatePatch(),
				new CraftingRecipeCtorPatch(),
				new CropHarvestPatch(),
				new FarmAnimalDayUpdatePatch(),
				new FarmAnimalGetSellPricePatch(),
				new FarmAnimalPetPatch(),
				new FishingRodStartMinigameEndFunctionPatch(),
				new FishPondUpdateMaximumOccupancyPatch(),
				new FruitTreeDayUpdatePatch(),
				new Game1CreateItemDebrisPatch(),
				new Game1CreateObjectDebrisPatch(),
				new Game1DrawHUDPatch(),
				new GameLocationBreakStonePatch(),
				new GameLocationCheckActionPatch(),
				new GameLocationDamageMonsterPatch(),
				new GameLocationGetFishPatch(),
				new GameLocationExplodePatch(),
				new GameLocationOnStoneDestroyedPatch(),
				new HoeDirtApplySpeedIncreasesPatch(),
				new LevelUpMenuAddProfessionDescriptionsPatch(helper.Translation),
				new LevelUpMenuGetImmediateProfessionPerkPatch(),
				new LevelUpMenuGetProfessionNamePatch(),
				new LevelUpMenuGetProfessionTitleFromNumberPatch(helper.Translation),
				new LevelUpMenuRemoveImmediateProfessionPerkPatch(),
				new LevelUpMenuRevalidateHealthPatch(),
				new MeleeWeaponDoAnimateSpecialMovePatch(),
				new MineShaftCheckStoneForItemsPatch(),
				new ObjectCtorPatch(),
				new ObjectGetMinutesForCrystalariumPatch(),
				new ObjectGetPriceAfterMultipliersPatch(),
				new PondQueryMenuDrawPatch(helper.Reflection),
				new ProjectileBehaviorOnCollisionPatch(),
				new QuestionEventSetUpPatch(),
				new SlingshotPerformFirePatch(),
				new TemporaryAnimatedSpriteCtorPatch(),
				new TreeDayUpdatePatch(),
				new TreeUpdateTapperProductPatch()
			);

			// generate unique buff ids
			int uniqueHash = (int)(Math.Abs(ModManifest.UniqueID.GetHashCode()) / Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(ModManifest.UniqueID.GetHashCode()))) - 8 + 1));
			Utility.SetProfessionBuffIDs(uniqueHash);

			// start event manager
			EventManager = new EventManager(helper.Events);

			// start treasure hunt managers
			ProspectorHunt = new ProspectorHunt(Config, Monitor, I18n, helper.Content, uniqueHash);
			ScavengerHunt = new ScavengerHunt(Config, Monitor, I18n, helper.Content, uniqueHash);
		}
	}
}
