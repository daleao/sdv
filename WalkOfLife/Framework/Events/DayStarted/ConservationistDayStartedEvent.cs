using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	internal class ConservationistDayStartedEvent : DayStartedEvent
	{
		/// <summary>Construct an instance.</summary>
		internal ConservationistDayStartedEvent() { }

		/// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world. Count water trash collected.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			foreach (var location in Game1.locations)
			{
				foreach (var obj in location.Objects.Values)
				{
					if (obj is CrabPot && Game1.getFarmer(obj.owner.Value).IsLocalPlayer && Utility.IsTrash((obj as CrabPot).heldObject.Value))
					{
						if (++Data.WaterTrashCollectedThisSeason % 10 == 0)
							SUtility.improveFriendshipWithEveryoneInRegion(Game1.player, 1, 2);
					}
				}
				
			}
		}
	}
}
