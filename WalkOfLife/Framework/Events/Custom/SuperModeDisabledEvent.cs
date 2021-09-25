using StardewValley;
using StardewValley.Monsters;
using System.Linq;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeDisabledEventHandler();

	public class SuperModeDisabledEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeDisabled += OnSuperModeDisabled;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeDisabled -= OnSuperModeDisabled;
		}

		/// <summary>Raised when IsSuperModeActive is set to false.</summary>
		public void OnSuperModeDisabled()
		{
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeCountdownUpdateTickedEvent));

			Game1.player.stopGlowing();

			var buffID = (ModEntry.UniqueID + ModEntry.SuperModeIndex).Hash();
			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == ++buffID);
			if (buff != null) Game1.buffsDisplay.removeOtherBuff(buffID);

			// notify peers
			ModEntry.Multiplayer.SendMessage(message: ModEntry.SuperModeIndex, messageType: "SuperModeDectivated", modIDs: new[] { ModEntry.UniqueID });

			// remove permanent effects
			if (ModEntry.SuperModeIndex == Util.Professions.IndexOf("Piper"))
			{
				ModEntry.PipedSlimes = Game1.currentLocation.characters.OfType<GreenSlime>().Where(s => s.Scale > 1f).ToList();
				ModEntry.Subscriber.Subscribe(new SlimeDeflationUpdateTickedEvent());
			}
		}
	}
}
