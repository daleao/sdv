using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class FruitTreeDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal FruitTreeDayUpdatePatch(ModConfig config, IMonitor monitor)
		: base(config, monitor) { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FruitTree), nameof(FruitTree.dayUpdate)),
				postfix: new HarmonyMethod(GetType(), nameof(FruitTreeDayUpdatePostfix))
			);
		}

		/// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
		protected static void FruitTreeDayUpdatePostfix(ref FruitTree __instance, GameLocation environment, Vector2 tileLocation)
		{
			bool isThereAnyArborist = Utils.AnyPlayerHasProfession("arborist", out int n);
			if (isThereAnyArborist && __instance.daysUntilMature.Value % 7 == 0)
			{
				__instance.daysUntilMature.Value -= n;
			}
		}
	}
}
