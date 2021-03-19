using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace TheLion.AwesomeProfessions
{
	internal class TreeDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal TreeDayUpdatePatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Tree), nameof(Tree.dayUpdate)),
				prefix: new HarmonyMethod(GetType(), nameof(TreeDayUpdatePrefix)),
				postfix: new HarmonyMethod(GetType(), nameof(TreeDayUpdatePostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch to increase Abrorist tree growth odds.</summary>
		protected static bool TreeDayUpdatePrefix(ref Tree __instance, ref int __state)
		{
			__state = __instance.growthStage.Value;
			return true; // run original logic
		}

		/// <summary>Patch to increase Abrorist non-fruit tree growth odds.</summary>
		protected static void TreeDayUpdatePostfix(ref Tree __instance, int __state, GameLocation environment, Vector2 tileLocation)
		{
			bool isThereAnyArborist = Utility.AnyPlayerHasProfession("arborist", out int n);
			if (__instance.growthStage.Value > __state || !isThereAnyArborist) return;

			if (Utility.CanThisTreeGrow(__instance, environment, tileLocation))
			{
				if (__instance.treeType.Value == Tree.mahoganyTree)
				{
					if (Game1.random.NextDouble() < 0.075 * n || (__instance.fertilized.Value && Game1.random.NextDouble() < 0.3 * n))
						++__instance.growthStage.Value;
				}
				else if (Game1.random.NextDouble() < 0.1 * n) ++__instance.growthStage.Value;
			}
		}
		#endregion harmony patches
	}
}
