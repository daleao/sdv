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

		public uint WineFameNeededForMaxValue { get; } = 5000;
		public uint ForagesNeededForBestQuality { get; } = 200;
		public uint MineralsNeededForBestQuality { get; } = 200;
		public uint TreasureHuntTimeLimitSeconds { get; } = 43;
		public double ChanceToStartTreasureHunt { get; } = 0.1;
		public float TreasureTileDetectionDistance { get; } = 3f;
		public uint TrashNeededForNextTaxLevel { get; } = 50;
	}
}
