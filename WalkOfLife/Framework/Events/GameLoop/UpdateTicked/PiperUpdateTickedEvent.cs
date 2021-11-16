using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class PiperUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc />
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (ModState.SlimeContactTimer > 0 && Game1.shouldTimePass()) --ModState.SlimeContactTimer;
		}
	}
}