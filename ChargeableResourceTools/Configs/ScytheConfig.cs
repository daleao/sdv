namespace TheLion.Stardew.Tools.Configs
{
	/// <summary>Configuration for the axe shockwave.</summary>
	public class ScytheConfig
	{
		/// <summary>Enables charging the Golden Scythe.</summary>
		public bool EnableScytheCharging { get; set; } = true;

		/// <summary>The bonus radius of the charged Golden Scythe.</summary>
		public float RadiusMultiplier { get; set; } = 1.5f;
	}
}