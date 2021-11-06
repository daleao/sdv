using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Patches;

namespace TheLion.Stardew.Professions.Framework
{
	/// <summary>Unified entry point for applying Harmony patches.</summary>
	internal class HarmonyPatcher
	{
		/// <summary>Construct an instance.</summary>
		internal HarmonyPatcher(string uniqueID)
		{
			Harmony = new(uniqueID);
		}

		private Harmony Harmony { get; }

		/// <summary>Instantiate and apply one of every <see cref="IPatch" /> class in the assembly using reflection.</summary>
		internal void ApplyAll()
		{
			ModEntry.Log("[HarmonyPatcher]: Gathering patches...", LogLevel.Trace);
			var patches = AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(IPatch)))
				.Where(t => typeof(IPatch).IsAssignableFrom(t) && !t.IsAbstract).ToList();
			ModEntry.Log($"[HarmonyPatcher]: Found {patches.Count} patch classes.", LogLevel.Trace);

			foreach (var patch in patches.Select(t => (IPatch) t.Constructor().Invoke(new object[] { })))
				patch.Apply(Harmony);
		}
	}
}