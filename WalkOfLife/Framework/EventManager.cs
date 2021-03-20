using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Manages dynamic subscribing and unsubscribing events for modded professions.</summary>
	public class EventManager
	{
		private IModEvents _listener;
		private IMonitor _monitor;
		private List<IEvent> _subscribed = new();

		/// <summary>Construct an instance.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public EventManager(IModEvents listener, IMonitor monitor)
		{
			_listener = listener;
			_monitor = monitor;

			// hook static events
			Subscribe(new LevelChangedEvent(), new ReturnedToTitleEvent(), new SavedEvent(), new SaveLoadedEvent());
		}

		/// <summary>Subscribe new events to the event listener.</summary>
		/// <param name="events">Events to be subscribed.</param>
		public void Subscribe(params IEvent[] events)
		{
			foreach (var e in events)
			{
				if (!_subscribed.ContainsType(e.GetType()))
				{
					e.Hook(_listener);
					_subscribed.Add(e);
					_monitor.Log($"Hooked {e.GetType().Name}.", LogLevel.Info);
				}
			}
		}

		/// <summary>Unsubscribe events from the event listener.</summary>
		/// <param name="eventType">The type of events to be unsubscribed.</param>
		public void Unsubscribe(params Type[] eventType)
		{
			foreach (var type in eventType)
			{
				if (_subscribed.RemoveType(type, out var removed))
					removed.Unhook(_listener);
					_monitor.Log($"Unhooked {type.Name}.", LogLevel.Info);
			}
		}

		/// <summary>Subscribe the event listener to all events required by the local player's current professions.</summary>
		public void SubscribeProfessionEventsForLocalPlayer()
		{
			_monitor.Log($"Hooking all events for farmer {Game1.player.Name}.", LogLevel.Info);
			foreach (int professionIndex in Game1.player.professions)
			{
				if (professionIndex.AnyOf(Utility.ProfessionMap.Forward["brute"],
										  Utility.ProfessionMap.Forward["conservationist"],
										  Utility.ProfessionMap.Forward["demolitionist"],
										  Utility.ProfessionMap.Forward["gambit"],
										  Utility.ProfessionMap.Forward["prospector"],
										  Utility.ProfessionMap.Forward["scavenger"],
										  Utility.ProfessionMap.Forward["spelunker"]
					)) SubscribeEventsForProfession(professionIndex);
			}
		}

		/// <summary>Subscribe the event listener to all events required by the local player's current professions.</summary>
		public void UnsubscribeLocalPlayerEvents()
		{
			_monitor.Log($"Unhooking local player events.", LogLevel.Info);
			List<Type> toRemove = new();
			for (int i = 4; i < _subscribed.Count; ++i) toRemove.Add(_subscribed[i].GetType());
			Unsubscribe(toRemove.ToArray());
		}

		/// <summary>Subscribe the event listener to all events required by a specific profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public void SubscribeEventsForProfession(int whichProfession)
		{
			if (Utility.ProfessionMap.Reverse[whichProfession] == "brute")
				Subscribe(new BruteUpdateTickedEvent(), new BruteWarpedEvent());
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "conservationist")
				Subscribe(new ConservationistDayStartedEvent());
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "demolitionist")
				Subscribe(new DemolitionistUpdateTickedEvent());
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "gambit")
				Subscribe(new GambitUpdateTickedEvent());
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "prospector")
				Subscribe(new ArrowPointerUpdateTickedEvent(), new ProspectorWarpedEvent(), new TreasureHuntRenderingHudEvent());
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "scavenger")
				Subscribe(new ArrowPointerUpdateTickedEvent(), new ScavengerWarpedEvent(), new TreasureHuntRenderingHudEvent());
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "spelunker")
				Subscribe(new SpelunkerUpdateTickedEvent(), new SpelunkerWarpedEvent());
		}

		/// <summary>Unsubscribe the event listener from all events required by a specific profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		public void UnsubscribeEventsForProfession(int whichProfession)
		{
			if (Utility.ProfessionMap.Reverse[whichProfession] == "brute")
				Unsubscribe(typeof(BruteUpdateTickedEvent), typeof(BruteWarpedEvent));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "conservationist")
				Unsubscribe(typeof(ConservationistDayStartedEvent));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "demolitionist")
				Unsubscribe(typeof(DemolitionistUpdateTickedEvent));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "gambit")
				Unsubscribe(typeof(GambitUpdateTickedEvent));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "prospector")
			{
				Unsubscribe(typeof(ProspectorWarpedEvent));
				if (!Utility.LocalPlayerHasProfession("scavenger"))
					Unsubscribe(typeof(ArrowPointerUpdateTickedEvent), typeof(TreasureHuntRenderingHudEvent));
			}
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "scavenger")
			{
				Unsubscribe(typeof(ScavengerWarpedEvent));
				if (!Utility.LocalPlayerHasProfession("prospector"))
					Unsubscribe(typeof(ArrowPointerUpdateTickedEvent), typeof(TreasureHuntRenderingHudEvent));
			}
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "spelunker")
				Unsubscribe(typeof(SpelunkerUpdateTickedEvent), typeof(SpelunkerWarpedEvent));
		}
	}
}
