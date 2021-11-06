using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal abstract class GameLaunchedEvent : BaseEvent
	{
		/// <inheritdoc />
		public override void Hook()
		{
			ModEntry.ModHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		/// <inheritdoc />
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.GameLoop.GameLaunched -= OnGameLaunched;
		}

		/// <summary>Raised after the game is launched, right before the first update tick.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		public abstract void OnGameLaunched(object sender, GameLaunchedEventArgs e);
	}
}