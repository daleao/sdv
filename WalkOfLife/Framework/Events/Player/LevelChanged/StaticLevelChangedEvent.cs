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
			if (!e.IsLocalPlayer) return;

			if (e.NewLevel == 0)
			{
				// clean up rogue events and data on skill reset
				ModEntry.Subscriber.CleanUpRogueEvents();
				ModEntry.Data.CleanUpRogueData();
			}
			else
			{
				ModEntry.Subscriber.Subscribe(new RestoreForgottenRecipesDayStartedEvent());
			}
		}
	}
}