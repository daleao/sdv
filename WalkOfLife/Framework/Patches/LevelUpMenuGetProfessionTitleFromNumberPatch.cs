using Harmony;
using StardewModdingAPI;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class LevelUpMenuGetProfessionTitleFromNumberPatch : BasePatch
	{
		private static ITranslationHelper _i18n;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal LevelUpMenuGetProfessionTitleFromNumberPatch(ModConfig config, IMonitor monitor, ITranslationHelper i18n)
		: base(config, monitor)
		{
			_i18n = i18n;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), name: "getProfessionName"),
				prefix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetProfessionNamePrefix))
			);
		}

		/// <summary>Patch to apply modded profession names.</summary>
		protected static bool LevelUpMenuGetProfessionNamePrefix(ref string __result, int whichProfession)
		{
			if (!Utils.ProfessionMap.Contains(whichProfession))
				return true; // run original logic

			__result = _i18n.Get(Utils.ProfessionMap.Reverse[whichProfession] + ".name");
			return false; // don't run original logic
		}
	}

}
