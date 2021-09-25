using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SlimeInflationUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			for (int i = ModEntry.PipedSlimes.Count() - 1; i >= 0; --i)
			{
				ModEntry.PipedSlimes.ElementAt(i).Scale = Math.Min(ModEntry.PipedSlimes.ElementAt(i).Scale * 1.1f, 2f);
				if (Game1.random.NextDouble() < 0.1 || ModEntry.PipedSlimes.ElementAt(i).Scale >= 2f) ModEntry.PipedSlimes.RemoveAt(i);
			}

			if (!ModEntry.PipedSlimes.Any()) ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}