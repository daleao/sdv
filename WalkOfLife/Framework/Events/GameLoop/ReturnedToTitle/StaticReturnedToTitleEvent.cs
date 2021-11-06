using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	[UsedImplicitly]
	internal class StaticReturnedToTitleEvent : ReturnedToTitleEvent
	{
		/// <inheritdoc />
		public override void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
		{
			// release mod data
			ModEntry.Data.Unload();

			// unsubscribe events
			ModEntry.Subscriber.UnsubscribeLocalPlayerEvents();

			// reset super mode
			if (ModEntry.SuperModeIndex > 0) ModEntry.SuperModeIndex = -1;
		}
	}
}