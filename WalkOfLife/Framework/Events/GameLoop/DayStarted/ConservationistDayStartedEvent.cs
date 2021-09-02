using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using SUtility = StardewValley.Utility;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ConservationistDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc/>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			foreach (var location in Game1.locations)
			{
				foreach (var obj in location.Objects.Values)
				{
					if (obj is not CrabPot crabpot || !Game1.getFarmer(obj.owner.Value).IsLocalPlayer || !Util.Objects.IsTrash(crabpot.heldObject.Value)) continue;
					
					ModEntry.Data.IncrementField<uint>("WaterTrashCollectedThisSeason");
					if (ModEntry.Data.ReadField<uint>("WaterTrashCollectedThisSeason") % ModEntry.Config.TrashNeededPerFriendshipPoint == 0)
						SUtility.improveFriendshipWithEveryoneInRegion(Game1.player, 1, 2);
				}
			}
		}
	}
}