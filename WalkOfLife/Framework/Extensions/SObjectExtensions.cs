using StardewValley;
using System.Linq;
using TheLion.Stardew.Common.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Extensions
{
	public static class SObjectExtensions
	{
		/// <summary>Whether a given object is an artisan good.</summary>
		public static bool IsArtisanGood(this SObject obj)
		{
			return obj?.Category == SObject.artisanGoodsCategory;
		}

		/// <summary>Whether a given object is an artisan good.</summary>
		public static bool IsArtisanMachine(this SObject obj)
		{
			return Util.Objects.ArtisanMachines.Contains(obj?.Name);
		}

		/// <summary>Whether a given object is an animal produce or derived artisan good.</summary>
		public static bool IsAnimalProduct(this SObject obj)
		{
			return obj != null &&
				   (obj.Category.AnyOf(SObject.EggCategory, SObject.MilkCategory, SObject.sellAtPierresAndMarnies)
					|| Util.Objects.AnimalDerivedProductIDs.Contains(obj.ParentSheetIndex));
		}

		/// <summary>Whether a given object is salmonberry or blackberry.</summary>
		public static bool IsWildBerry(this SObject obj)
		{
			return obj?.ParentSheetIndex == 296 || obj?.ParentSheetIndex == 410;
		}

		/// <summary>Whether a given object is a spring onion.</summary>
		public static bool IsSpringOnion(this SObject obj)
		{
			return obj?.ParentSheetIndex == 399;
		}

		/// <summary>Whether a given object is a gem or mineral.</summary>
		public static bool IsGemOrMineral(this SObject obj)
		{
			return obj?.Category.AnyOf(SObject.GemCategory, SObject.mineralsCategory) == true;
		}

		/// <summary>Whether a given object is a foraged mineral.</summary>
		public static bool IsForagedMineral(this SObject obj)
		{
			return obj.Name.AnyOf("Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz");
		}

		/// <summary>Whether a given object is a resource node or foraged mineral.</summary>
		public static bool IsResourceNode(this SObject obj)
		{
			return IsStone(obj) && Util.Objects.ResourceNodeIDs.Contains(obj.ParentSheetIndex);
		}

		/// <summary>Whether a given object is a stone.</summary>
		public static bool IsStone(this SObject obj)
		{
			return obj?.Name == "Stone";
		}

		/// <summary>Whether a given object is a fish caught with a fishing rod.</summary>
		public static bool IsFish(this SObject obj)
		{
			return obj?.Category == SObject.FishCategory;
		}

		/// <summary>Whether a given object is a crab pot fish.</summary>
		public static bool IsTrapFish(this SObject obj)
		{
			return obj.IsFish() && obj.ParentSheetIndex is > 714 and < 724;
		}

		/// <summary>Whether a given object is a trash.</summary>
		public static bool IsAlgae(this SObject obj)
		{
			return obj?.ParentSheetIndex.AnyOf(152, 152, 157) == true;
		}

		/// <summary>Whether a given object is a trash.</summary>
		public static bool IsTrash(this SObject obj)
		{
			return obj?.Category == SObject.junkCategory;
		}

		/// <summary>Whether a given object is typically found in pirate treasure.</summary>
		public static bool IsPirateTreasure(this SObject obj)
		{
			return Util.Objects.TrapperPirateTreasureTable.ContainsKey(obj.ParentSheetIndex);
		}

		/// <summary>Whether the player should track a given object.</summary>
		public static bool ShouldBeTracked(this SObject obj)
		{
			return Game1.player.HasProfession("Scavenger") &&
				   (obj.IsSpawnedObject && !obj.IsForagedMineral() || obj.ParentSheetIndex == 590)
				   || Game1.player.HasProfession("Prospector") && (obj.IsResourceNode() || obj.IsForagedMineral());
		}
	}
}