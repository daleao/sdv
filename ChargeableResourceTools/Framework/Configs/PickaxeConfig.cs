namespace TheLion.AwesomeTools.Framework.Configs
{
	/// <summary>Configuration for the pickaxe shockwave.</summary>
	public class PickaxeConfig
	{
		/// <summary>Enables charging the Axe.</summary>
		public bool EnablePickaxeCharging { get; set; } = true;

		/// <summary>The radius of affected tiles at each upgrade level.</summary>
		public int[] RadiusAtEachLevel { get; set; } = new int[] { 1, 2, 3, 4, 5 };

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
