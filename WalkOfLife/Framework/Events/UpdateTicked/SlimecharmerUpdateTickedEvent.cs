using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.AwesomeProfessions.Framework.Events.UpdateTicked
{
	internal class SlimecharmerUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (AwesomeProfessions.slimeHealTimer > 0 && Game1.shouldTimePass())
				--AwesomeProfessions.slimeHealTimer;
		}
	}
}