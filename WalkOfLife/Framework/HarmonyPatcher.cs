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
		public uint patchedCount;
		public uint failedCount;
		public uint ignoredCount;
		public uint prefixedCount;
		public uint postfixedCount;
		public uint transpiledCount;
		public uint totalPatchTargets;

		private readonly List<Type> _patches;
		private readonly Harmony _harmony;

		/// <summary>Construct an instance.</summary>
		internal HarmonyPatcher()
		{
			_harmony = new(ModEntry.UniqueID);

			ModEntry.Log("[HarmonyPatcher]: Initializing...", LogLevel.Trace);
			_patches = (from type in AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(BasePatch)))
				where type.IsSubclassOf(typeof(BasePatch))
				select type).ToList();
			totalPatchTargets = (uint) _patches.Count;
			ModEntry.Log($"[HarmonyPatcher]: Found {_patches.Count} patch classes.", LogLevel.Trace);
		}

		/// <summary>Apply all <see cref="BasePatch" /> instances in the assembly.</summary>
		internal void ApplyAll()
		{
			var watch = Stopwatch.StartNew(); // benchmark patching
			foreach (var patch in _patches.Select(type => (BasePatch) type.Constructor()?.Invoke(Array.Empty<object>())))
				patch?.Apply(_harmony);

			watch.Stop();
			ModEntry.Log($"[HarmonyPatcher]:" +
			             $"\nSuccessfully patched {patchedCount}/{totalPatchTargets} methods. Total patch tally:" +
			             $"\n\t- prefixes: {prefixedCount}" +
			             $"\n\t- postfixes: {postfixedCount}" +
			             $"\n\t- transpilers: {transpiledCount}" + 
			             $"\n{failedCount} patches failed to apply." + 
			             $"\n{ignoredCount} patches were ignored." +
			             $"\nExecution time: {watch.ElapsedMilliseconds} ms.", LogLevel.Trace);
		}
	}
}