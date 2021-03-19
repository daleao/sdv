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
		public uint ForagesNeededForBestQuality { get; } = 500;
		public uint MineralsNeededForBestQuality { get; } = 500;
		public uint TreasureHuntTimeLimitSeconds { get; } = 90;
		public float TreasureDetectionDistance { get; } = 4f;
		public uint TrashNeededForNextTaxLevel { get; } = 50;
	}
}
