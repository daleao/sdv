using System.Collections.Generic;

namespace TheLion.AwesomeTools
{
	/// <summary>Configuration for the pickaxe shockwave.</summary>
	public class PickaxeConfig
	{
		/// <summary>Enables charging the Pickaxe.</summary>
		public bool EnablePickaxeCharging { get; set; } = true;

		/// <summary>Pickaxe must be at least this level to charge.</summary>
		public int RequiredUpgradeForCharging { get; set; } = 1;

		/// <summary>The radius of affected tiles at each upgrade level.</summary>
		public List<int> RadiusAtEachPowerLevel { get; set; } = new List<int>() { 1, 2, 3, 4 };

		/// <summary>Whether to show affected tiles overlay while charging.</summary>
		public bool ShowPickaxeAffectedTiles { get; set; } = true;

		/// <summary>Whether to break boulders and meteorites.</summary>
		public bool BreakBouldersAndMeteorites { get; set; } = true;

		/// <summary>Whether to harvest spawned items in the mines.</summary>
		public bool HarvestMineSpawns { get; set; } = true;

		/// <summary>Whether to break containers in the mine.</summary>
		public bool BreakMineContainers { get; set; } = true;

		/// <summary>Whether to clear placed objects.</summary>
		public bool ClearObjects { get; set; } = false;

		/// <summary>Whether to clear placed paths & flooring.</summary>
		public bool ClearFlooring { get; set; } = false;

		/// <summary>Whether to clear tilled dirt.</summary>
		public bool ClearDirt { get; set; } = true;

		/// <summary>Whether to clear bushes.</summary>
		public bool ClearBushes { get; set; } = true;

		/// <summary>Whether to clear live crops.</summary>
		public bool ClearLiveCrops { get; set; } = false;

		/// <summary>Whether to clear dead crops.</summary>
		public bool ClearDeadCrops { get; set; } = true;

		/// <summary>Whether to clear debris like stones, boulders and weeds.</summary>
		public bool ClearDebris { get; set; } = true;
	}
}