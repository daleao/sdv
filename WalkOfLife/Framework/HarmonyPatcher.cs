using Harmony;
using StardewModdingAPI;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Unified entry point for applying multiple patches.</summary>
	internal class HarmonyPatcher
	{
		private readonly string _uniqueId;

		/// <summary>Construct an instance.</summary>
		/// <param name="uniqueId">The unique id for this mod.</param>
		internal HarmonyPatcher(string uniqueId)
		{
			_uniqueId = uniqueId;
		}

		/// <summary>Iterate through and apply any number of patches.</summary>
		/// <param name="patches">A sequence of base patch instances.</param>
		internal void ApplyAll(params BasePatch[] patches)
		{
			var harmony = HarmonyInstance.Create(_uniqueId);
			foreach (var patch in patches) patch.Apply(harmony);
		}
	}
}
