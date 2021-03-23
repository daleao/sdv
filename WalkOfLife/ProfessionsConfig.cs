using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod user-defined settings.</summary>
	public class ProfessionsConfig
	{
		public bool UseModdedProfessionIcons { get; set; } = true;
		public bool UseAltProducerIcon { get; set; } = true;
		public KeybindList Modkey { get; set; } = KeybindList.ForSingle(SButton.LeftShift);

		public uint OenologyFameNeededForMaxValue { get; } = 5000;
		public uint ForagesNeededForBestQuality { get; } = 200;
		public uint MineralsNeededForBestQuality { get; } = 200;
		public uint ScavengerHuntTimeLimitSeconds { get; } = 43;
		public uint ProspectorHuntTimeLimitSeconds { get; } = 22;
		public double ChanceToStartTreasureHunt { get; } = 0.2;
		public float TreasureTileDetectionDistance { get; } = 3f;
		public uint TrashNeededForNextTaxLevel { get; } = 50;
	}
}
