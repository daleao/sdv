using StardewModdingAPI;
using TheLion.AwesomeProfessions.Framework;
using TheLion.AwesomeProfessions.Framework.Patches;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static ModConfig Config { get; set; }
		public static ITranslationHelper I18n { get; set; }

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// get configs.json
			Config = Helper.ReadConfig<ModConfig>();

			// get localized content
			I18n = Helper.Translation;

			// apply patches
			new Patcher(ModManifest.UniqueID).ApplyAll(
				new AnimalHouseAddNewHatchedAnimalPatch(Config, Monitor),
				new CaskGetAgingMultiplierForItemPatch(Config, Monitor),
				new CropHarvestPatch(Config, Monitor),
				new FarmAnimalDayUpdatePatch(Config, Monitor),
				new FarmAnimalGetSellPricePatch(Config, Monitor),
				new FarmAnimalPetPatch(Config, Monitor),
				new GameLocationBreakStonePatch(Config, Monitor),
				new GameLocationExplodePatch(Config, Monitor),
				new GameLocationOnStoneDestroyedPatch(Config, Monitor),
				new LevelUpMenuAddProfessionDescriptionsPatch(Config, Monitor, I18n),
				new LevelUpMenuGetProfessionNamePatch(Config, Monitor),
				new ObjectGetPriceAfterMultipliersPatch(Config, Monitor),
				new QuestionEventSetUpPatch(Config, Monitor),
				new TreeDayUpdatePatch(Config, Monitor),
				new TreeUpdateTapperProductPatch(Config, Monitor)
			);
		}
	}
}
