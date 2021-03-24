using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod user-defined settings.</summary>
	public class ProfessionsConfig
	{
		/// <summary>Whether to replace vanilla profession icons with modded icons courtesy of IllogicalMoodSwing.</summary>
		public bool UseModdedProfessionIcons { get; set; } = true;
		/// <summary>Whether to use the alternative producer icon with processed animal products instead of base animal products.</summary>
		public bool UseAltProducerIcon { get; set; } = true;
		/// <summary>Mod key used by Prospector, Scavenger and Rascal professions.</summary>
		public KeybindList ModKey { get; set; } = KeybindList.ForSingle(SButton.LeftShift, SButton.LeftShoulder);
		/// <summary>You need this many points to reach double value of artisan beverages.</summary>
		public uint OenologyFameNeededForMaxValue { get; set; } = 5000;
		/// <summary>You must forage this many items before your forage becomes iridium-quality.</summary>
		public uint ForagesNeededForBestQuality { get; set; } = 200;
		/// <summary>You must mine this many minerals before your mined minerals become iridium-quality.</summary>
		public uint MineralsNeededForBestQuality { get; set; } = 200;
		/// <summary>You have this many seconds to locate and dig up the buried treasure.</summary>
		public uint ScavengerHuntTimeLimitSeconds { get; set; } = 43;
		/// <summary>You have this many seconds to locate and break the special rock.</summary>
		public uint ProspectorHuntTimeLimitSeconds { get; set; } = 22;
		/// <summary>The chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
		public double ChanceToStartTreasureHunt { get; set; } = 0.2;
		/// <summary>You must be this close to the treasure hunt target before the indicator appears.</summary>
		public float TreasureTileDetectionDistance { get; set; } = 3f;
		/// <summary>You must collect this many junk items from crab pots for every 1% of tax deduction next season.</summary>
		public uint TrashNeededForNextTaxLevel { get; set; } = 50;
	}
}
