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
		internal static IModHelper ModHelper { get; set; }
		internal static ITranslationHelper I18n { get; set; }
		internal static ProfessionsConfig Config { get; set; }
		internal static ProfessionsData Data { get; set; }
		internal static EventManager EventManager { get; set; }
		internal static ProspectorHunt ProspectorHunt { get; set; }
		internal static ScavengerHunt ScavengerHunt { get; set; }

		internal static int demolitionistBuffMagnitude;
		internal static uint bruteKillStreak;
		internal static readonly List<Vector2> initialLadderTiles = new();

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// store reference to helper
			ModHelper = helper;

			// store reference to localized text
			I18n = helper.Translation;

			// get configs.json
			Config = helper.ReadConfig<ProfessionsConfig>();

			// initialize static references
			BaseEvent.Init(Config);
			BasePatch.Init(Config, Monitor);
			Utility.Init(Config);

			// get mod assets
			helper.Content.AssetEditors.Add(new AssetEditor(helper.Content, I18n));
			Utility.ArrowPointer.Texture = helper.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(Path.Combine("Assets", "cursor.png"));

			// apply patches
			new HarmonyPatcher(ModManifest.UniqueID).ApplyAll(
				new AnimalHouseAddNewHatchedAnimalPatch(),
				new BasicProjectileBehaviorOnCollisionWithMonsterPatch(helper.Reflection),
				new BasicProjectileCtorPatch(),
				new BobberBarCtorPatch(),
				new BushShakePatch(),
				new CaskPerformObjectDropInActionPatch(),
				new CrabPotCheckForActionPatch(),
				new CrabPotDayUpdatePatch(),
				new CrabPotDrawPatch(),
				new CraftingRecipeCtorPatch(),
				new CropHarvestPatch(),
				new FarmAnimalDayUpdatePatch(),
				new FarmAnimalGetSellPricePatch(),
				new FarmAnimalPetPatch(),
				new FarmerHasOrWillReceiveMailPatch(),
				new FarmerShowItemIntakePatch(),
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
				new LevelUpMenuAddProfessionDescriptionsPatch(I18n),
				new LevelUpMenuGetImmediateProfessionPerkPatch(),
				new LevelUpMenuGetProfessionNamePatch(),
				new LevelUpMenuGetProfessionTitleFromNumberPatch(I18n),
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

			// start event manager
			EventManager = new EventManager(helper.Events, Monitor);

			// generate unique buff ids
			int uniqueHash = (int)(Math.Abs(ModManifest.UniqueID.GetHashCode()) / Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(ModManifest.UniqueID.GetHashCode()))) - 8 + 1));
			Utility.SetProfessionBuffIDs(uniqueHash);
		}
	}
}
