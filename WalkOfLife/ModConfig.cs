using StardewModdingAPI.Utilities;

namespace TheLion.Stardew.Professions
{
	/// <summary>The mod user-defined settings.</summary>
	public class ModConfig
	{
		/// <summary>Mod key used by Prospector and Scavenger professions.</summary>
		public KeybindList ModKey { get; set; } = KeybindList.Parse("LeftShift, LeftShoulder");

		/// <summary>Mod key used by Prospector and Scavenger professions.</summary>
		public KeybindList SuperModeKey { get; set; } = KeybindList.Parse("LeftShift, LeftShoulder");

		/// <summary>Whether Super Mode is activated on <see cref="SuperModeKey"/> hold (as opposed to press).</summary>
		public bool HoldKeyToActivateSuperMode { get; set; } = true;

		/// <summary>How long <see cref="SuperModeKey"/> should be held to activate Super Mode, in seconds.</summary>
		public int SuperModeActivationDelay { get; set; } = 1;

		/// <summary>You must forage this many items before your forage becomes iridium-quality.</summary>
		public uint ForagesNeededForBestQuality { get; set; } = 500;

		/// <summary>You must mine this many minerals before your mined minerals become iridium-quality.</summary>
		public uint MineralsNeededForBestQuality { get; set; } = 500;

		/// <summary>The chance that a scavenger or prospector hunt will trigger in the right conditions.</summary>
		public double ChanceToStartTreasureHunt { get; set; } = 0.2;

		/// <summary>Increase this multiplier if you find that treasure hunts end too quickly.</summary>
		public float TreasureHuntHandicap { get; set; } = 1f;

		/// <summary>You must be this close to the treasure hunt target before the indicator appears.</summary>
		public float TreasureDetectionDistance { get; set; } = 3f;

		/// <summary>You must collect this many junk items from crab pots for every 1% of tax deduction next season.</summary>
		public uint TrashNeededForNextTaxLevel { get; set; } = 100;

		/// <summary>You must collect this many junk items from crab pots for every 1% of tax deduction next season.</summary>
		public uint TrashNeededPerFriendshipPoint { get; set; } = 10;

		/// <summary>The maximum tax deduction percentage allowed by the Ferngill Revenue Service.</summary>
		public float TaxBonusCeiling { get; set; } = 0.25f;

		/// <summary>If a Harmony transpiler is failing to patch, enabling this option will export the original IL code for easier debugging.</summary>
		public bool EnableILCodeExport { get; set; } = true;
	}
}