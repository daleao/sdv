using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches.Foraging
{
	[UsedImplicitly]
	internal class Game1ShouldTimePassPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal Game1ShouldTimePassPatch()
		{
			Original = RequireMethod<Game1>(nameof(Game1.shouldTimePass));
		}

		#region harmony patches

		/// <summary>Patch to freeze time during prestiged treasure hunts.</summary>
		[HarmonyPrefix]
		private static bool Game1ShouldTimePassPrefix(ref bool __result)
		{
			if (ModState.ProspectorHunt is not null && ModState.ProspectorHunt.IsActive &&
			    Game1.player.HasPrestigedProfession("Prospector") ||
			    ModState.ScavengerHunt is not null && ModState.ScavengerHunt.IsActive &&
			    Game1.player.HasPrestigedProfession("Scavenger"))
			{
				__result = false;
				return false; // don't run original logic
			}

			return true; // run original logic
		}

		#endregion harmony patches
	}
}