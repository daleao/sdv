using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TheLion.AwesomeProfessions.Framework.Events.UpdateTicked;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	/// <summary>Manages dynamic subscribing and unsubscribing of events for modded professions.</summary>
	public class EventManager
	{
		private readonly List<IEvent> _subscribed = new();

		private IMonitor _Monitor { get; }

		private Dictionary<int, List<IEvent>> _EventsByProfession { get; } = new()
		{
			{ Farmer.artisan, new List<IEvent> { new ArtisanDayEndingEvent() } },
			{ Farmer.mariner, new List<IEvent> { new ConservationistDayEndingEvent(), new ConservationistDayStartedEvent() } },
			{ Farmer.tracker, new List<IEvent> { new ScavengerDayStartedEvent(), new ScavengerWarpedEvent(), new TrackerButtonsChangedEvent() } },
			{ Farmer.blacksmith, new List<IEvent> { new SpelunkerUpdateTickedEvent(), new SpelunkerWarpedEvent() } },
			{ Farmer.burrower, new List<IEvent> { new ProspectorDayStartedEvent(), new ProspectorWarpedEvent(), new TrackerButtonsChangedEvent() } },
			{ Farmer.excavator, new List<IEvent> { new DemolitionistUpdateTickedEvent() } },
			{ Farmer.brute, new List<IEvent> { new BruteUpdateTickedEvent(), new BruteWarpedEvent() } },
			{ Farmer.defender, new List<IEvent> { new GambitUpdateTickedEvent() } },
			{ Farmer.acrobat, new List<IEvent> { new SlimecharmerUpdateTickedEvent(), new SlimecharmerWarpedEvent() } }
		};

		/// <summary>Construct an instance.</summary>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal EventManager(IMonitor monitor)
		{
			_Monitor = monitor;

			// hook static events
			_Monitor.Log("Subscribing static events...");
			Subscribe(new StaticLevelChangedEvent(), new StaticReturnedToTitleEvent(), new StaticSaveLoadedEvent());
			if (AwesomeProfessions.ModRegistry.IsLoaded("alphablackwolf.skillPrestige") || AwesomeProfessions.ModRegistry.IsLoaded("cantorsdust.AllProfessions"))
				Subscribe(new StaticDayStartedEvent());
		}

		/// <summary>Subscribe new events to the event listener.</summary>
		/// <param name="events">Events to be subscribed.</param>
		internal void Subscribe(params IEvent[] events)
		{
			foreach (var e in events)
			{
				if (!_subscribed.ContainsType(e.GetType()))
				{
					e.Hook();
					_subscribed.Add(e);
					_Monitor.Log($"Subscribed to {e.GetType().Name}.");
				}
				else
				{
					_Monitor.Log($"Farmer already subscribed to {e.GetType().Name}.");
				}
			}
		}

		/// <summary>Unsubscribe events from the event listener.</summary>
		/// <param name="eventTypes">The event types to be unsubscribed.</param>
		internal void Unsubscribe(params Type[] eventTypes)
		{
			foreach (var type in eventTypes)
			{
				if (_subscribed.RemoveType(type, out var removed))
				{
					removed.Unhook();
					_Monitor.Log($"Unsubscribed from {type.Name}.");
				}
				else
				{
					_Monitor.Log($"Farmer not subscribed to {type.Name}.");
				}
			}
		}

		/// <summary>Subscribe the event listener to all events required by the local player's current professions.</summary>
		internal void SubscribeEventsForLocalPlayer()
		{
			_Monitor.Log($"Subscribing dynamic events for farmer {Game1.player.Name}...");
			foreach (var professionIndex in Game1.player.professions) SubscribeEventsForProfession(professionIndex);

			if (!Utility.LocalPlayerHasProfession("Artisan") || AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ArtisanAwardLevel", int.Parse) < 5)
				return;

			_Monitor.Log("Artisan perk already maxed out.");
			Unsubscribe(typeof(ArtisanDayEndingEvent));
		}

		/// <summary>Subscribe the event listener to all events required by the local player's current professions.</summary>
		internal void UnsubscribeLocalPlayerEvents()
		{
			_Monitor.Log($"Unsubscribing dynamic events...");
			List<Type> toRemove = new();
			for (var i = 4; i < _subscribed.Count; ++i) toRemove.Add(_subscribed[i].GetType());
			Unsubscribe(toRemove.ToArray());
		}

		/// <summary>Subscribe the event listener to all events required by a specific profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		internal void SubscribeEventsForProfession(int whichProfession)
		{
			if (!_EventsByProfession.TryGetValue(whichProfession, out var events)) return;
			foreach (var e in events) Subscribe(e);
		}

		/// <summary>Unsubscribe the event listener from all events required by a specific profession.</summary>
		/// <param name="whichProfession">The profession index.</param>
		internal void UnsubscribeProfessionEvents(int whichProfession)
		{
			if (!_EventsByProfession.TryGetValue(whichProfession, out var events)) return;

			List<IEvent> except = new();
			if (Utility.ProfessionMap.Reverse[whichProfession] == "Prospector" && Utility.LocalPlayerHasProfession("Scavenger") ||
			Utility.ProfessionMap.Reverse[whichProfession] == "Scavenger" && Utility.LocalPlayerHasProfession("Prospector"))
				except.Add(new TrackerButtonsChangedEvent());

			foreach (var e in events.Except(except)) Unsubscribe(e.GetType());
		}

		/// <summary>Verify if any events that should be subscribed are missing and if so subscribe those events.</summary>
		internal void SubscribeMissingEvents()
		{
			foreach (var professionIndex in Game1.player.professions)
			{
				if (!_EventsByProfession.TryGetValue(professionIndex, out var events)) continue;
				foreach (var e in events.Where(e => !IsSubscribed(e.GetType()))) Subscribe(e);
			}
		}

		/// <summary>Verify if there are any rogue events still subscribed and remove them.</summary>
		internal void CleanUpRogueEvents()
		{
			foreach (var e in _subscribed)
			{
				var prefix = Regex.Split(e.GetType().ToString(), @"(?<!^)(?=[A-Z])").First();
				if (Utility.ProfessionMap.Contains(prefix) && !Utility.LocalPlayerHasProfession(prefix)) Unsubscribe(e.GetType());
				else if (prefix.Equals("Tracker") && !(Utility.LocalPlayerHasProfession("Prospector") || Utility.LocalPlayerHasProfession("Scavenger"))) Unsubscribe(e.GetType());
			}
		}

		/// <summary>Whether the event listener is subscribed to a given event type.</summary>
		/// <param name="eventType">The event type to check.</param>
		internal bool IsSubscribed(Type eventType)
		{
			return _subscribed.ContainsType(eventType);
		}

		/// <summary>Get an enumerable of all currently subscribed events.</summary>
		internal IEnumerable<string> GetSubscribedEvents()
		{
			return _subscribed.Select(e => e.GetType().Name);
		}
	}
}