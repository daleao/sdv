using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	[UsedImplicitly]
	internal class StaticLevelChangedEvent : LevelChangedEvent
	{
		/// <inheritdoc />
		public override void OnLevelChanged(object sender, LevelChangedEventArgs e)
		{
			if (!e.IsLocalPlayer || e.NewLevel != 0) return;

			// clean up rogue events and data on skill prestige or reset
			ModEntry.Subscriber.CleanUpRogueEvents();
			ModEntry.Data.CleanUpRogueData();
		}
	}
}