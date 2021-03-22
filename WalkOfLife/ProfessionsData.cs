namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod persisted data.</summary>
	public class ProfessionsData
	{
		internal uint OenologyFameAccrued { get; set; } = 0;
		internal uint HighestOenologyAwardEarned { get; set; } = 0;
		internal uint LowestMineLevelReached { get; set; } = 0;
		internal uint ItemsForaged { get; set; } = 0;
		internal uint MineralsCollected { get; set; } = 0;
		internal uint ScavengerHuntStreak { get; set; } = 0;
		internal uint ProspectorHuntStreak { get; set; } = 0;
		internal uint WaterTrashCollectedThisSeason { get; set; } = 0;
		internal float ConservationistTaxBonusThisSeason { get; set; } = 0;
	}
}
