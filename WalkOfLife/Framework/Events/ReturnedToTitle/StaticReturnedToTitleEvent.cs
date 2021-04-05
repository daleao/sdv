using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class StaticReturnedToTitleEvent : ReturnedToTitleEvent
	{
		/// <summary>Raised after the game returns to the title screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public override void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			AwesomeProfessions.EventManager.UnsubscribeLocalPlayerEvents();
		}
	}
}