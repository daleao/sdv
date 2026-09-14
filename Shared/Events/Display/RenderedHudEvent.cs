namespace DaLion.Shared.Events;

#region using directives

using System.Collections.Concurrent;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.RenderedHud"/> allowing dynamic enabling / disabling.</summary>
public abstract class RenderedHudEvent : ManagedEvent
{
    private static BlockingCollection<RenderedHudEvent> Events = new();

    private static BlockingCollection<RenderedHudEvent> PriorityEvents = new();

    /// <summary>Initializes a new instance of the <see cref="RenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected RenderedHudEvent(EventManager manager, bool priority = false)
        : base(manager)
    {
        if (priority) {
            Log.D($"New Priority RenderedHudEvent {this.GetType().Name} for {Game1.player.Name}");
            PriorityEvents.Add(this);
        } else {
            Log.D($"New Normal RenderedHudEvent {this.GetType().Name} for {Game1.player.Name}");
            Events.Add(this);
        }
        if ((Events.Count + PriorityEvents.Count) == 1) {
            Log.D($"Add OnRenderedHudStatic to event list");
            this.Manager.ModEvents.Display.RenderedHud += OnRenderedHudStatic;
        }
    }

    public static void OnRenderedHudStatic(object? sender, RenderedHudEventArgs args) {
        foreach (var e in Events) {
            e.OnRenderedHud(sender, args);
        }
        foreach (var e in PriorityEvents) {
            e.OnRenderedHud(sender, args);
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        var self = this;
        if (Events.TryTake(out self)) {
            Log.D($"Removed Normal RenderedHudEvent {self.GetType().Name} for {Game1.player.Name}");
        }
        if (PriorityEvents.TryTake(out self)) {
            Log.D($"Removed Priority RenderedHudEvent {self.GetType().Name} for {Game1.player.Name}");
        }
        if (!Events.Any() && !PriorityEvents.Any()) {
            Log.D($"Remove OnRenderedHudStatic to event list");
            this.Manager.ModEvents.Display.RenderedHud -= OnRenderedHudStatic;
        }
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="OnRenderedHud"/>
    protected abstract void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e);

    /// <inheritdoc cref="IDisplayEvents.RenderedHud"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnRenderedHudImpl(sender, e);
        }
    }
}
