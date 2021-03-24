using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using TheLion.Common.Extensions;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Manages dynamic subscribing and unsubscribing events for modded professions.</summary>
	internal class EventManager
	{
		private IModEvents _Listener { get; }
		private IMonitor _Monitor { get; }

		private List<BaseEvent> _subscribed = new();

		/// <summary>Construct an instance.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		internal EventManager(IModEvents listener, IMonitor monitor)
		{
			_Listener = listener;
			_Monitor = monitor;

			// hook static events
			Subscribe(new LevelChangedEvent(), new ReturnedToTitleEvent(), new SavedEvent(), new SaveLoadedEvent(this));
		}

		/// <summary>Subscribe new events to the event listener.</summary>
		/// <param name="events">Events to be subscribed.</param>
		internal void Subscribe(params BaseEvent[] events)
		{
			foreach (var e in events)
			{
				if (!_subscribed.ContainsType(e.GetType()))
				{
					e.Hook(_Listener);
					_subscribed.Add(e);
					_Monitor.Log($"Hooked {e.GetType().Name}.", LogLevel.Info);
				}
			}
		}

		/// <summary>Unsubscribe events from the event listener.</summary>
		/// <param name="eventTypes">The event types to be unsubscribed.</param>
		internal void Unsubscribe(params Type[] eventTypes)
		{
			foreach (var type in eventTypes)
			{
				if (_subscribed.RemoveType(type, out var removed)) removed.Unhook(_Listener);
				_Monitor.Log($"Unhooked {type.Name}.", LogLevel.Info);
			}
		}

		/// <summary>Subscribe the event listener to all events required by the local player's current professions.</summary>
		internal void SubscribeProfessionEventsForLocalPlayer()
		{
			_Monitor.Log($"Hooking all events for farmer {Game1.player.Name}.", LogLevel.Info);
			foreach (int professionIndex in Game1.player.professions)
			{
				if (professionIndex.AnyOf(Utility.ProfessionMap.Forward["brute"],
										  Utility.ProfessionMap.Forward["conservationist"],
										  Utility.ProfessionMap.Forward["demolitionist"],
										  Utility.ProfessionMap.Forward["gambit"],
										  Utility.ProfessionMap.Forward["oenologist"],
										  Utility.ProfessionMap.Forward["prospector"],
										  Utility.ProfessionMap.Forward["scavenger"],
										  Utility.ProfessionMap.Forward["spelunker"]
					)) SubscribeEventsForProfession(professionIndex);
			}
		}

		/// <summary>Subscribe the event listener to all events required by the local player's current professions.</summary>
		internal void UnsubscribeLocalPlayerEvents()
		{
			_Monitor.Log($"Unhooking local player events.", LogLevel.Info);
			List<Type> toRemove = new();
			for (int i = 4; i < _subscribed.Count; ++i) toRemove.Add(_subscribed[i].GetType());
			Unsubscribe(toRemove.ToArray());
		}

		/// <summary>Subscribe the event listener to all events required by a specific profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		internal void SubscribeEventsForProfession(int whichProfession)
		{
			if (Utility.ProfessionMap.Reverse[whichProfession] == "brute")
				Subscribe(new BruteUpdateTickedEvent(AwesomeProfessions.I18n), new BruteWarpedEvent());
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "conservationist")
				Subscribe(new ConservationistDayEndingEvent(), new ConservationistDayStartedEvent());
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "demolitionist")
				Subscribe(new DemolitionistUpdateTickedEvent(AwesomeProfessions.I18n));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "gambit")
				Subscribe(new GambitUpdateTickedEvent(AwesomeProfessions.I18n));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "oenologist")
				Subscribe(new OenologistDayEndingEvent());
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "prospector")
				Subscribe(new ProspectorDayStartedEvent(AwesomeProfessions.ProspectorHunt), new ProspectorWarpedEvent(AwesomeProfessions.ProspectorHunt), new TrackerButtonsChangedEvent(this));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "scavenger")
				Subscribe(new ScavengerDayStartedEvent(AwesomeProfessions.ScavengerHunt), new ScavengerWarpedEvent(AwesomeProfessions.ScavengerHunt), new TrackerButtonsChangedEvent(this));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "spelunker")
				Subscribe(new SpelunkerUpdateTickedEvent(AwesomeProfessions.I18n), new SpelunkerWarpedEvent());
		}

		/// <summary>Unsubscribe the event listener from all events required by a specific profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		internal void UnsubscribeEventsForProfession(int whichProfession)
		{
			if (Utility.ProfessionMap.Reverse[whichProfession] == "brute")
				Unsubscribe(typeof(BruteUpdateTickedEvent), typeof(BruteWarpedEvent));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "conservationist")
				Unsubscribe(typeof(ConservationistDayEndingEvent));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "demolitionist")
				Unsubscribe(typeof(DemolitionistUpdateTickedEvent));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "gambit")
				Unsubscribe(typeof(GambitUpdateTickedEvent));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "oenologist")
				Unsubscribe(typeof(OenologistDayEndingEvent));
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "prospector")
			{
				Unsubscribe(typeof(ProspectorDayStartedEvent), typeof(ProspectorWarpedEvent));
				if (!Utility.LocalPlayerHasProfession("scavenger"))
					Unsubscribe(typeof(TrackerButtonsChangedEvent));
			}
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "scavenger")
			{
				Unsubscribe(typeof(ScavengerDayStartedEvent), typeof(ScavengerWarpedEvent));
				if (!Utility.LocalPlayerHasProfession("prospector"))
					Unsubscribe(typeof(TrackerButtonsChangedEvent));
			}
			else if (Utility.ProfessionMap.Reverse[whichProfession] == "spelunker")
				Unsubscribe(typeof(SpelunkerUpdateTickedEvent), typeof(SpelunkerWarpedEvent));
		}

		/// <summary>Whether the event listener is subscribed to a given event type.</summary>
		/// <param name="eventType">The event type to check.</param>
		internal bool IsListening(Type eventType)
		{
			return _subscribed.ContainsType(eventType);
		}
	}
}
