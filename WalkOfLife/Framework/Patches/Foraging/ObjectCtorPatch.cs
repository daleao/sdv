using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class ObjectCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal ObjectCtorPatch(IMonitor monitor)
		: base(monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Constructor(typeof(SObject), new Type[] { typeof(Vector2), typeof(int), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(bool) }),
				postfix: new HarmonyMethod(GetType(), nameof(ObjectCtorPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Ecologist wild berry recovery.</summary>
		protected static void ObjectCtorPostfix(ref SObject __instance)
		{
			Farmer who = Game1.getFarmer(__instance.owner.Value);
			if (_IsWildBerry(__instance) && Globals.SpecificPlayerHasProfession("ecologist", who))
				__instance.Edibility = (int)(__instance.Edibility * 1.5f);
		}
		#endregion harmony patches

		#region private methods
		/// <summary>Whether a given object is salmonberry or blackberry.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsWildBerry(SObject obj)
		{
			return obj?.ParentSheetIndex == 296 || obj?.ParentSheetIndex == 410;
		}
		#endregion private methods
	}
}
