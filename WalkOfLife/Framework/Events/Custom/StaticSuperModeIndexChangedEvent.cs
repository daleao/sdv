using Microsoft.Xna.Framework;
using System;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeIndexChangedEventHandler(int newIndex);

	public class StaticSuperModeIndexChangedEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeIndexChanged += OnSuperModeIndexChanged;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeIndexChanged -= OnSuperModeIndexChanged;
		}

		/// <summary>Raised when SuperModeIndex is set to a new value.</summary>
		public void OnSuperModeIndexChanged(int newIndex)
		{
			ModEntry.Subscriber.UnsubscribeSuperModeEvents();
			ModEntry.SuperModeCounter = 0;

			ModEntry.Data.WriteField("SuperModeIndex", ModEntry.SuperModeIndex.ToString());
			if (ModEntry.SuperModeIndex < 0) return;

			var whichSuperMode = Util.Professions.NameOf(newIndex);
			if (!whichSuperMode.AnyOf("Brute", "Poacher", "Desperado", "Piper")) throw new ArgumentException($"Unexpected super mode {whichSuperMode}");

			switch (whichSuperMode)
			{
				case "Brute":
					ModEntry.SuperModeGlowColor = Color.OrangeRed;
					ModEntry.SuperModeOverlayColor = Color.OrangeRed;
					ModEntry.SuperModeSfx = "brute_rage";
					break;
				case "Poacher":
					ModEntry.SuperModeGlowColor = Color.GhostWhite;
					ModEntry.SuperModeOverlayColor = Color.Black;
					ModEntry.SuperModeSfx = "poacher_ambush";
					ModEntry.MonstersStolenFrom ??= new();
					break;
				case "Desperado":
					ModEntry.SuperModeGlowColor = Color.DarkGoldenrod;
					ModEntry.SuperModeOverlayColor = Color.SandyBrown;
					ModEntry.SuperModeSfx = "desperado_cockgun";
					break;
				case "Piper":
					ModEntry.SuperModeGlowColor = Color.LightSeaGreen;
					ModEntry.SuperModeOverlayColor = Color.Green;
					ModEntry.SuperModeSfx = "piper_provoke";
					ModEntry.PipedSlimesScales ??= new();
					break;
			}

			ModEntry.SuperModeBarAlpha = 1f;
			ModEntry.ShouldShakeSuperModeBar = false;

			ModEntry.Subscriber.SubscribeSuperModeEvents();
		}
	}
}
