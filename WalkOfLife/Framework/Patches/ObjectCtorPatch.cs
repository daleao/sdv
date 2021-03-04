using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class ObjectCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal ObjectCtorPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(SObject), new Type[] { typeof(Vector2), typeof(int), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(bool) }),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectCtorPostfix))
			);
		}

		/// <summary>Patch for cheaper tapper recipe for Tapper.</summary>
		protected static void ObjectCtorPostfix(ref SObject __instance)
		{
			if (IsWildBerry(__instance) && Utils.PlayerHasProfession("ecologist"))
			{
				__instance.Edibility = (int)(__instance.Edibility * 1.5f);
			}
		}

		/// <summary>Whether a given object is salmonberry or blackberry.</summary>
		/// <param name="obj">The given object.</param>
		protected static bool IsWildBerry(SObject obj)
		{
			return obj.ParentSheetIndex == 296 || obj.ParentSheetIndex == 410;
		}
	}
}
