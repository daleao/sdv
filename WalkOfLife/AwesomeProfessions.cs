using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod entry point.</summary>
	public class AwesomeProfessions : Mod
	{
		internal static IContentHelper Content { get; set; }
		internal static IModEvents Events { get; set; }
		internal static IReflectionHelper Reflection { get; set; }
		internal static ITranslationHelper I18n { get; set; }
		internal static ModDataDictionary Data { get; set; }
		internal static ProfessionsConfig Config { get; set; }
		internal static EventManager EventManager { get; set; }
		internal static ProspectorHunt ProspectorHunt { get; set; }
		internal static ScavengerHunt ScavengerHunt { get; set; }
		internal static string UniqueID { get; private set; }

		internal static int demolitionistBuffMagnitude;
		internal static uint bruteKillStreak;
		internal static readonly List<Vector2> initialLadderTiles = new();

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// get unique id and generate buff ids
			UniqueID = ModManifest.UniqueID;
			int uniqueHash = (int)(Math.Abs(UniqueID.GetHashCode()) / Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(UniqueID.GetHashCode()))) - 8 + 1));
			Utility.SetProfessionBuffIDs(uniqueHash);

			// store reference to content helper
			Content = helper.Content;

			// store reference to mod events
			Events = helper.Events;

			// store reference to reflection helper
			Reflection = helper.Reflection;

			// store reference to localized text
			I18n = helper.Translation;

			// get configs.json
			Config = helper.ReadConfig<ProfessionsConfig>();

			// patch profession icons
			helper.Content.AssetEditors.Add(new IconEditor());

			// apply patches
			BasePatch.Init(Monitor);
			new HarmonyPatcher().ApplyAll(
				new AnimalHouseAddNewHatchedAnimalPatch(),
				new BasicProjectileBehaviorOnCollisionWithMonsterPatch(),
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
				new GreenSlimeUpdatePatch(),
				new GreenSlimeGetExtraDropItemsPatch(),
				new HoeDirtApplySpeedIncreasesPatch(),
				new LevelUpMenuAddProfessionDescriptionsPatch(),
				new LevelUpMenuGetImmediateProfessionPerkPatch(),
				new LevelUpMenuGetProfessionNamePatch(),
				new LevelUpMenuGetProfessionTitleFromNumberPatch(),
				new LevelUpMenuRemoveImmediateProfessionPerkPatch(),
				new LevelUpMenuRevalidateHealthPatch(),
				new MeleeWeaponDoAnimateSpecialMovePatch(),
				new MineShaftCheckStoneForItemsPatch(),
				new ObjectCtorPatch(),
				new ObjectGetMinutesForCrystalariumPatch(),
				new ObjectGetPriceAfterMultipliersPatch(),
				new PondQueryMenuDrawPatch(),
				new ProjectileBehaviorOnCollisionPatch(),
				new QuestionEventSetUpPatch(),
				new SlingshotPerformFirePatch(),
				new TemporaryAnimatedSpriteCtorPatch(),
				new TreeDayUpdatePatch(),
				new TreeUpdateTapperProductPatch()
			);

			// start event manager
			EventManager = new EventManager(Monitor);
		}
	}
}