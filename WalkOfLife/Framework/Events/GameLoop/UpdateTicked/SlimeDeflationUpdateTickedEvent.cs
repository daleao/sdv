using StardewModdingAPI.Events;
using System;
using System.Linq;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			for (int i = ModEntry.PipedSlimes.Count() - 1; i >= 0; --i)
			{
				ModEntry.PipedSlimes.ElementAt(i).Scale = Math.Max(ModEntry.PipedSlimes.ElementAt(i).Scale / 1.1f, 1f);
				if (ModEntry.PipedSlimes.ElementAt(i).Scale <= 1f) ModEntry.PipedSlimes.RemoveAt(i);
			}

			if (!ModEntry.PipedSlimes.Any()) ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}