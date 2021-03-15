using Harmony;
using StardewModdingAPI;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class LevelUpMenuGetProfessionNamePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal LevelUpMenuGetProfessionNamePatch(IMonitor monitor)
		: base(monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), name: "getProfessionName"),
				prefix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetProfessionNamePrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch to apply modded profession names.</summary>
		protected static bool LevelUpMenuGetProfessionNamePrefix(ref string __result, int whichProfession)
		{
			if (!Globals.ProfessionMap.Contains(whichProfession)) return true; // run original logic

			__result = Globals.ProfessionMap.Reverse[whichProfession];
			return false; // don't run original logic
		}
		#endregion harmony patches
	}

}
