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

		public int WineFameNeededForMaxValue { get; } = 5000;
		public int MaxStartingFriendshipForNewbornAnimals { get; } = 200;
		public int ForagesNeededForBestQuality { get; } = 500;
		public int MineralsNeededForBestQuality { get; } = 500;
		public float ChanceToStartTreasureHunt { get; } = 0.2f;
		public int TreasureHuntDurationMinutes { get; } = 30;
		public int TrashNeededForNextTaxLevel { get; } = 50;
	}
}
