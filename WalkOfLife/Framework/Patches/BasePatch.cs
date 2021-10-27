using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	/// <summary>Harmony patch base class.</summary>
	public abstract class BasePatch
	{
		protected static ILHelper Helper { get; private set; }
		protected static Action<string, LogLevel> Log { get; private set; }

		protected MethodBase Original { get; set; }
		protected HarmonyMethod Prefix { get; set; }
		protected HarmonyMethod Transpiler { get; set; }
		protected HarmonyMethod Postfix { get; set; }

		/// <summary>Initialize the ILHelper.</summary>
		internal static void Init(Action<string, LogLevel> log, bool enableILCodeExport, string modPath)
		{
			Helper = new(log, enableILCodeExport, modPath);
			Log = log;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		/// <returns>Returns an array of bools representation of patch results.</returns>
		public virtual Dictionary<string, int> Apply(Harmony harmony)
		{
			var results = new Dictionary<string, int>
			{
				{ "patched", 0},
				{ "failed", 0},
				{ "ignored", 0},
				{ "prefixed", 0},
				{ "postfixed", 0},
				{ "transpiled", 0},
			};
			
			if (Original == null)
			{
				ModEntry.Log($"[Patch]: Ignoring {GetType().Name}. The patch target was not found.", LogLevel.Trace);
				++results["ignored"];
				return results;
			}

			try
			{
				ModEntry.Log($"[Patch]: Applying {GetType().Name} to {Original.DeclaringType}::{Original.Name}.",
					LogLevel.Trace);
				harmony.Patch(Original, Prefix, Postfix, Transpiler);
				if (Prefix != null) ++results["prefixed"];
				if (Postfix != null) ++results["postfixed"];
				if (Transpiler != null) ++results["transpiled"];
				++results["patched"];
			}
			catch (Exception ex)
			{
				ModEntry.Log($"[Patch]: Failed to patch {Original.DeclaringType}::{Original.Name}.\nHarmony returned {ex}",
					LogLevel.Error);
				++results["failed"];
			}

			return results;
		}
	}
}