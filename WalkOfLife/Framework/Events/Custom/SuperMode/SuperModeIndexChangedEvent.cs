using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeIndexChangedEventHandler(int newIndex);

	internal class SuperModeIndexChangedEvent : BaseEvent
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
			if (ModEntry.SuperModeIndex < 0)
			{
				ModEntry.Log("Unregistered Super Mode.", LogLevel.Info);
				return;
			}

			if (newIndex is < 26 or >= 30)
			{
				ModEntry.SuperModeIndex = -1;
				throw new ArgumentException($"Unexpected super mode index {newIndex}.");
			}

			var whichSuperMode = Utility.Professions.NameOf(newIndex);
			switch (whichSuperMode)
			{
				case "Brute":
					ModEntry.SuperModeGlowColor = Color.OrangeRed;
					ModEntry.SuperModeOverlayColor = Color.OrangeRed;
					ModEntry.SuperModeSFX = "brute_rage";
					break;

				case "Poacher":
					ModEntry.SuperModeGlowColor = Color.MediumPurple;
					ModEntry.SuperModeOverlayColor = Color.MidnightBlue;
					ModEntry.SuperModeSFX = "poacher_ambush";
					ModEntry.MonstersStolenFrom ??= new();
					break;

				case "Desperado":
					ModEntry.SuperModeGlowColor = Color.DarkGoldenrod;
					ModEntry.SuperModeOverlayColor = Color.SandyBrown;
					ModEntry.SuperModeSFX = "desperado_cockgun";
					break;

				case "Piper":
					ModEntry.SuperModeGlowColor = Color.LimeGreen;
					ModEntry.SuperModeOverlayColor = Color.DarkGreen;
					ModEntry.SuperModeSFX = "piper_provoke";
					ModEntry.PipedSlimeScales ??= new();
					break;
			}

			ModEntry.SuperModeBarAlpha = 1f;
			ModEntry.ShouldShakeSuperModeBar = false;
			ModEntry.Subscriber.SubscribeSuperModeEvents();

			var key = whichSuperMode.ToLower();
			var professionDisplayName = ModEntry.ModHelper.Translation.Get(key + ".name.male");
			var buffName = ModEntry.ModHelper.Translation.Get(key + ".buff");
			ModEntry.Log($"Registered to {professionDisplayName}'s {buffName}.", LogLevel.Info);
		}
	}
}