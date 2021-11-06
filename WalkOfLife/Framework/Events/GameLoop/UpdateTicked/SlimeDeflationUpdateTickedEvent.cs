using System;
using System.Linq;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc />
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			var undeflatedSlimes = ModEntry.PipedSlimeScales.Keys.ToList();
			for (var i = undeflatedSlimes.Count - 1; i >= 0; --i)
			{
				undeflatedSlimes[i].Scale = Math.Max(undeflatedSlimes[i].Scale / 1.1f,
					ModEntry.PipedSlimeScales[undeflatedSlimes[i]]);
				if (!(undeflatedSlimes[i].Scale <= ModEntry.PipedSlimeScales[undeflatedSlimes[i]])) continue;

				undeflatedSlimes[i].willDestroyObjectsUnderfoot = false;
				undeflatedSlimes.RemoveAt(i);
			}

			if (undeflatedSlimes.Any()) return;

			ModEntry.PipedSlimeScales.Clear();
			ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}