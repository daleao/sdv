using System;
using StardewModdingAPI.Events;
using StardewValley.Locations;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class SpelunkerWarpedEvent : WarpedEvent
	{
		private static readonly SpelunkerBuffDisplayUpdateTickedEvent SpelunkerUpdateTickedEvent = new();

		/// <inheritdoc />
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			if (e.NewLocation is MineShaft)
			{
				++ModState.SpelunkerLadderStreak;
				e.Player.health = Math.Min(e.Player.health + (int) (e.Player.maxHealth * 0.05f), e.Player.maxHealth);
				e.Player.Stamina = Math.Min(e.Player.Stamina + e.Player.MaxStamina * 0.05f, e.Player.MaxStamina);
				ModEntry.Subscriber.Subscribe(SpelunkerUpdateTickedEvent);
			}
			else
			{
				ModState.SpelunkerLadderStreak = 0;
				ModEntry.Subscriber.Unsubscribe(SpelunkerUpdateTickedEvent.GetType());
			}
		}
	}
}