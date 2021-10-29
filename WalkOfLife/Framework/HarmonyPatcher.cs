using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private Harmony Harmony { get; }
		private Action<string, LogLevel> Log { get; }
		
		/// <summary>Construct an instance.</summary>
		internal HarmonyPatcher(Action<string, LogLevel> log, string uniqueID)
		{
			Log = log;
			Harmony = new(uniqueID);
		}

		/// <summary>Instantiate and apply one of every <see cref="BasePatch" /> class in the assembly using reflection.</summary>
		internal void ApplyAll()
		{
			var watch = Stopwatch.StartNew(); // benchmark patching

			Log("[HarmonyPatcher]: Gathering patches...", LogLevel.Trace);
			var patches = (
				from type in AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(BasePatch)))
				where type.IsSubclassOf(typeof(BasePatch))
				select type
			).ToList();
			Log($"[HarmonyPatcher]: Found { patches.Count} patch classes.", LogLevel.Trace);

			var stats = new Dictionary<string, int>
			{
				{ "patched", 0},
				{ "failed", 0},
				{ "ignored", 0},
				{ "prefixed", 0},
				{ "postfixed", 0},
				{ "transpiled", 0},
			};

			foreach (var patch in patches.Select(type => (BasePatch) type.Constructor()?.Invoke(Array.Empty<object>())))
			{
				var results = patch?.Apply(Harmony);
				if (results is null) continue;

				// aggregate patch results to total stats
				foreach (var key in stats.Keys)
					stats[key] += results[key];
			}

			watch.Stop();
			Log("[HarmonyPatcher]:" +
			             $"\nSuccessfully patched {stats["patched"]}/{stats["patched"] + stats["failed"] + stats["ignored"]} methods. Total patch tally:" +
			             $"\n\t- prefixes: {stats["prefixed"]}" +
			             $"\n\t- postfixes: {stats["postfixed"]}" +
			             $"\n\t- transpilers: {stats["transpiled"]}" + 
			             $"\n{stats["failed"]} patches failed to apply." + 
			             $"\n{stats["ignored"]} patches were ignored." +
			             $"\nExecution time: {watch.ElapsedMilliseconds} ms.", LogLevel.Trace);
		}
	}
}