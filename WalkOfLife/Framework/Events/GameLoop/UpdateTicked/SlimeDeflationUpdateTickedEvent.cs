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
			var undeflatedSlimes = ModEntry.PipedSlimesScales.Keys.ToList();
			while (undeflatedSlimes.Any())
			{
				for (var i = undeflatedSlimes.Count - 1; i >= 0; --i)
				{
					undeflatedSlimes[i].Scale = Math.Max(undeflatedSlimes[i].Scale / 1.1f, ModEntry.PipedSlimesScales[undeflatedSlimes[i]]);
					if (undeflatedSlimes[i].Scale <= ModEntry.PipedSlimesScales[undeflatedSlimes[i]])
					{
						undeflatedSlimes[i].willDestroyObjectsUnderfoot = false;
						undeflatedSlimes.RemoveAt(i);
					}
				}
			}

			ModEntry.PipedSlimesScales.Clear();
			ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}