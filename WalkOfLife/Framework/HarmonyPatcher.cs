using HarmonyLib;
using StardewModdingAPI;
using System.Linq;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Patches;

namespace TheLion.Stardew.Professions.Framework
{
	/// <summary>Unified entry point for applying Harmony patches.</summary>
	internal class HarmonyPatcher
	{
		/// <summary>Apply all <see cref="BasePatch"/> instances in the assembly using reflection.</summary>
		internal void ApplyAll()
		{
			ModEntry.Log("[HarmonyPatcher]: Applying Harmony patches...", LogLevel.Trace);
			var patchTypes = (from type in AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(BasePatch)))
							 where type.IsSubclassOf(typeof(BasePatch))
							 select type).ToList();

			var harmony = new Harmony(ModEntry.UniqueID);
			foreach (var patch in patchTypes.Select(type => (BasePatch)type.Constructor()?.Invoke(new object[] { })))
				patch?.Apply(harmony);
			
			ModEntry.Log($"[HarmonyPatcher]: Successfully applied {patchTypes.Count} Harmony patches.", LogLevel.Trace);
		}
	}
}