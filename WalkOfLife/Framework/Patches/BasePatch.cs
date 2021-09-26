using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	/// <summary>Harmony patch base class.</summary>
	internal abstract class BasePatch
	{
		protected static ILHelper Helper { get; private set; }

		protected MethodBase Original { get; set; }
		protected HarmonyMethod Prefix { get; set; }
		protected HarmonyMethod Transpiler { get; set; }
		protected HarmonyMethod Postfix { get; set; }

		/// <summary>Initialize the ILHelper.</summary>
		internal static void Init(string modPath)
		{
			Helper = new ILHelper(ModEntry.Log, ModEntry.Config.EnableILCodeExport, modPath);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		public virtual void Apply(Harmony harmony)
		{
			try
			{
				ModEntry.Log($"Applying {GetType().Name} to {Original.DeclaringType}::{Original.Name}.", LogLevel.Trace);
				harmony.Patch(Original, Prefix, Postfix, Transpiler);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed to patch {Original.DeclaringType}::{Original.Name}.\nHarmony returned {ex}", LogLevel.Error);
			}
		}
	}
}