using Harmony;
using StardewModdingAPI;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuGetProfessionTitleFromNumberPatch : BasePatch
	{
		private static ITranslationHelper _I18n { get; set; }

		/// <summary>Construct an instance.</summary>
		/// <param name="i18n">Provides localized text.</param>
		internal LevelUpMenuGetProfessionTitleFromNumberPatch(ITranslationHelper i18n)
		{
			_I18n = i18n;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.getProfessionTitleFromNumber)),
				prefix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetProfessionTitleFromNumberPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to apply modded profession names.</summary>
		protected static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
		{
			if (!Utility.ProfessionMap.Contains(whichProfession)) return true; // run original logic

			__result = _I18n.Get(Utility.ProfessionMap.Reverse[whichProfession] + ".name");
			return false; // don't run original logic
		}
		#endregion harmony patches
	}
}
