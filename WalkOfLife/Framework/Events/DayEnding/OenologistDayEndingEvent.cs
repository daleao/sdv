using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class OenologistDayEndingEvent : DayEndingEvent
	{
		private const uint _AwardLevelMax = 5;

		/// <inheritdoc/>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			// get oenology fame points for the day
			foreach (SObject obj in Game1.getFarm().getShippingBin(Game1.player).Where(item => Utility.IsWine(item)))
			{
				switch (obj.Quality)
				{
					case SObject.bestQuality:
						AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/OenologyFameAccrued", (uint)obj.Stack * 3);
						break;

					case SObject.highQuality:
						AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/OenologyFameAccrued", (uint)obj.Stack);
						break;

					case SObject.lowQuality:
						AwesomeProfessions.Data.DecrementField($"{AwesomeProfessions.UniqueID}/OenologyFameAccrued", (uint)obj.Stack);
						break;
				}
			}

			// check if should level up
			uint oenologyFameAccrued;
			if (Game1.dayOfMonth == 7 && (oenologyFameAccrued = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/OenologyFameAccrued", uint.Parse)) > 0)
			{
				uint fameNeededForNextLevel = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/FameNeededForNextAwardLevel", uint.Parse);
				if (oenologyFameAccrued >= fameNeededForNextLevel)
				{
					AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/OenologyAwardLevel");
					AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/OenologyFameAccrued", (oenologyFameAccrued - fameNeededForNextLevel).ToString());
					uint currentLevel = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/OenologyAwardLevel", uint.Parse);
					fameNeededForNextLevel = AwesomeProfessions.Config.OenologyLevelUpDifficulty * (10 * currentLevel * currentLevel - 20 * currentLevel + 50);
					AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/FameNeededForNextAwardLevel", fameNeededForNextLevel.ToString());
					Game1.addMailForTomorrow($"{AwesomeProfessions.UniqueID}/OenologistAwardNotice{currentLevel}");
				}
			}
		}
	}
}