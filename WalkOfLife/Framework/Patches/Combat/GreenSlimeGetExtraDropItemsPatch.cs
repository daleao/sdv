using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class GreenSlimeGetExtraDropItemsPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(GreenSlime), nameof(GreenSlime.getExtraDropItems)),
				postfix: new HarmonyMethod(GetType(), nameof(GreenSlimeGetExtraDropItemsPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to increase drop rate of slime eggs for Slimemaster.</summary>
		private static void GreenSlimeGetExtraDropItemsPostfix(ref GreenSlime __instance, ref List<Item> __result)
		{
			if (!Utility.AnyFarmerInLocationHasProfession("slimemaster", __instance.currentLocation) || !Game1.MasterPlayer.mailReceived.Contains("slimeHutchBuilt") || Game1.random.NextDouble() > 0.02) return;

			switch (__instance.Name)
			{
				case "Green Slime":
					__result.Add(new SObject(680, 1));
					break;

				case "Frost Jelly":
					__result.Add(new SObject(413, 1));
					break;

				case "Tiger Slime":
					__result.Add(new SObject(857, 1));
					break;

				case "Sludge":
					if (__instance.color.Value.Equals(Color.BlueViolet)) __result.Add(new SObject(439, 1));
					else __result.Add(new SObject(437, 1));
					break;
			}
		}

		#endregion harmony patches
	}
}