using HarmonyLib;
using StardewModdingAPI;
using System.Linq;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Patches;

namespace TheLion.Stardew.Professions.Framework
{
	/// <summary>Unified entry point for applying Harmony patches.</summary>
	internal class HarmonyPatcher
	{
		/// <summary>Iterate through and apply any number of patches.</summary>
		/// <param name="patches">A sequence of <see cref="BasePatch"/> instances.</param>
		internal void ApplyAll()
		{
			ModEntry.Log("Applying Harmony patches...", LogLevel.Trace);

			var patchTypes = from type in AccessTools.AllTypes()
							 where type.IsSubclassOf(typeof(BasePatch))
							 select type;

			var harmony = new Harmony(ModEntry.UniqueID);
			foreach (var type in patchTypes)
			{
				var patch = (BasePatch)type.Constructor()?.Invoke(new object[] { });
				patch?.Apply(harmony);
			}
		}
	}
}