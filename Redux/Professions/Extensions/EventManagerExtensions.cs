namespace DaLion.Redux.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Redux.Professions;
using DaLion.Redux.Professions.Events.Display;
using DaLion.Redux.Professions.Events.GameLoop;
using DaLion.Redux.Professions.Events.Input;
using DaLion.Redux.Professions.Events.Player;
using DaLion.Shared.Events;

#endregion using directives

/// <summary>Manages dynamic enabling and disabling of profession events.</summary>
internal static class EventManagerExtensions
{
    /// <summary>Look-up of event types required by each profession.</summary>
    private static readonly IReadOnlyDictionary<Profession, List<Type>> EventsByProfession = new Dictionary<Profession, List<Type>>()
    {
        { Profession.Brute, new List<Type> { typeof(BruteWarpedEvent) } },
        { Profession.Conservationist, new List<Type> { typeof(ConservationismDayEndingEvent) } },
        { Profession.Piper, new List<Type> { typeof(PiperWarpedEvent) } },
        { Profession.Prospector, new List<Type> { typeof(ProspectorHuntDayStartedEvent), typeof(ProspectorRenderedHudEvent), typeof(ProspectorWarpedEvent), typeof(TrackerButtonsChangedEvent), } },
        { Profession.Rascal, new List<Type> { typeof(RascalButtonPressedEvent), typeof(RascalButtonReleasedEvent) } },
        { Profession.Scavenger, new List<Type> { typeof(ScavengerHuntDayStartedEvent), typeof(ScavengerRenderedHudEvent), typeof(ScavengerWarpedEvent), typeof(TrackerButtonsChangedEvent), } },
        { Profession.Spelunker, new List<Type> { typeof(SpelunkerWarpedEvent) } },
    };

    /// <summary>Enables events for the local player's professions.</summary>
    /// <param name="manager">The <see cref="EventManager"/>.</param>
    internal static void EnableForLocalPlayer(this EventManager manager)
    {
        Log.D($"[EventManager]: Enabling profession events for {Game1.player.Name}...");
        foreach (var pid in Game1.player.professions)
        {
            try
            {
                if (Profession.TryFromValue(pid, out var profession))
                {
                    manager.EnableForProfession(profession);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Log.D($"[EventManager]: Unexpected profession index {pid} will be ignored.");
            }
        }

        Log.D($"[EventManager]: Done enabling event for {Game1.player.Name}.");
    }

    /// <summary>Enables all events required by the specified <paramref name="profession"/>.</summary>
    /// <param name="manager">The <see cref="EventManager"/>.</param>
    /// <param name="profession">A profession.</param>
    internal static void EnableForProfession(this EventManager manager, Profession profession)
    {
        if ((profession == Profession.Conservationist && !Context.IsMainPlayer) ||
            !EventsByProfession.TryGetValue(profession, out var events))
        {
            return;
        }

        Log.D($"[EventManager]: Enabling events for {profession}...");
        manager.Enable(events.ToArray());
    }

    /// <summary>Disables all events related to the specified <paramref name="profession"/>.</summary>
    /// <param name="manager">The <see cref="EventManager"/>.</param>
    /// <param name="profession">A profession.</param>
    internal static void DisableForProfession(this EventManager manager, Profession profession)
    {
        if ((profession == Profession.Conservationist &&
             Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist, out _))
            || !EventsByProfession.TryGetValue(profession, out var events))
        {
            return;
        }

        if (profession == Profession.Spelunker)
        {
            events.Add(typeof(SpelunkerUpdateTickedEvent));
        }

        List<Type> except = new();
        if ((profession == Profession.Prospector && Game1.player.HasProfession(Profession.Scavenger)) ||
            (profession == Profession.Scavenger && Game1.player.HasProfession(Profession.Prospector)))
        {
            except.Add(typeof(TrackerButtonsChangedEvent));
        }

        Log.D($"[EventManager]: Disabling {profession} events...");
        manager.Disable(events.Except(except).ToArray());
    }
}
