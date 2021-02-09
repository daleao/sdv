namespace TheLion.AwesomeTools.Framework.Configs
{
	/// <summary>Configuration for the axe shockwave.</summary>
	public class AxeConfig
	{
		/// <summary>Enables charging the Axe.</summary>
		public bool EnableAxeCharging { get; set; } = true;

		/// <summary>The radius of affected tiles at each upgrade level.</summary>
		public int[] RadiusAtEachLevel { get; set; } = new int[] { 1, 2, 3, 4, 5 };

		/// <summary>Whether to clear fruit tree seeds.</summary>
		public bool ClearFruitTreeSeeds { get; set; } = false;

		/// <summary>Whether to clear fruit trees that aren't fully grown.</summary>
		public bool ClearFruitTreeSaplings { get; set; } = false;

		/// <summary>Whether to cut down fully-grown fruit trees.</summary>
		public bool CutGrownFruitTrees { get; set; } = false;

		/// <summary>Whether to clear non-fruit tree seeds.</summary>
		public bool ClearTreeSeeds { get; set; } = false;

		/// <summary>Whether to clear non-fruit trees that aren't fully grown.</summary>
		public bool ClearTreeSaplings { get; set; } = false;

		/// <summary>Whether to cut down full-grown non-fruit trees.</summary>
		public bool CutGrownTrees { get; set; } = false;

		/// <summary>Whether to cut down non-fruit trees that have a tapper.</summary>
		public bool CutTappedTrees { get; set; } = false;

		/// <summary>Whether to harvest giant crops.</summary>
		public bool CutGiantCrops { get; set; } = false;

		/// <summary>Whether to clear bushes.</summary>
		public bool ClearBushes { get; set; } = true;

		/// <summary>Whether to clear live crops.</summary>
		public bool ClearLiveCrops { get; set; } = false;

		/// <summary>Whether to clear dead crops.</summary>
		public bool ClearDeadCrops { get; set; } = true;

		/// <summary>Whether to clear debris like twigs, giant stumps, fallen logs and weeds.</summary>
		public bool ClearDebris { get; set; } = true;
	}
}
