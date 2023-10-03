namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Debug]
[AlwaysEnabledEvent]
internal sealed class DebugButtonsChangedEvent : ButtonsChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebugButtonsChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DebugButtonsChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override async void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        if (Config.DebugKey.JustPressed())
        {
            State.DebugMode = !State.DebugMode;
            return;
        }

        if (!State.DebugMode || !e.Pressed.Any(b => b is SButton.MouseRight or SButton.MouseLeft))
        {
            return;
        }

        if (Game1.activeClickableMenu is not null && DebugCursorMovedEvent.FocusedComponent is not null)
        {
            var component = DebugCursorMovedEvent.FocusedComponent;
            var name = string.IsNullOrEmpty(component.name) ? "Anon" : component.name;
            var message = $"[{component.myID}]: {name} ({component.GetType().Name})";
            message = component
                .GetType()
                .GetFields()
                .Where(f => !f.Name.IsIn("myID", "name"))
                .Aggregate(
                    message,
                    (current, field) => current + $"\n\t- {field.Name}: {field.GetValue(component)}");
            Log.D(message);
        }
        else if (Context.IsWorldReady)
        {
            if (Game1.currentLocation.Objects.TryGetValue(e.Cursor.Tile, out var o))
            {
                var message = $"[{o.ParentSheetIndex}]: {o.Name} ({o.GetType().Name})";
                message = o
                    .GetType()
                    .GetFields()
                    .Where(f => !f.Name.IsIn("ParentSheetIndex", "Name"))
                    .Aggregate(
                        message,
                        (current, field) => current + $"\n\t- {field.Name}: {field.GetValue(o)}");
                Log.D(message);
            }
            else
            {
                foreach (var c in Game1.currentLocation.characters.Cast<Character>()
                             .Concat(Game1.currentLocation.farmers))
                {
                    if (c.getTileLocation() != e.Cursor.Tile)
                    {
                        continue;
                    }

                    var message = string.Empty;
                    Farmer? who = null;
                    if (c is Farmer farmer)
                    {
                        who = farmer;
                        message += $"[{who.UniqueMultiplayerID}]: ";
                    }

                    message += $"{c.Name} ({c.GetType()})";
                    message = c
                        .GetType()
                        .GetFields()
                        .Where(f => !f.Name.IsIn("UniqueMultiplayerID", "Name"))
                        .Aggregate(
                            message,
                            (m, f) => m + $"\n\t- {f.Name}: {f.GetValue(c)}");

                    message +=
                        $"\n\n\tCurrent location: {c.currentLocation.NameOrUniqueName} ({c.currentLocation.GetType().Name})";

                    if (who is not null)
                    {
                        message += "\n\n\tMod data:";
                        message = Game1.MasterPlayer.modData.Pairs
                            .Where(p => p.Key.StartsWith(Manifest.UniqueID) &&
                                        p.Key.Contains(who.UniqueMultiplayerID.ToString()))
                            .Aggregate(
                                message,
                                (m, p) => m + $"\n\t\t- {p.Key}: {p.Value}");

                        var events = string.Empty;
                        if (who.IsLocalPlayer)
                        {
                            events = this.Manager.Enabled.Aggregate(
                                string.Empty,
                                (current, next) => current + "\n\t\t- " + next.GetType().Name);
                        }
                        else if (Context.IsMultiplayer && who.isActive())
                        {
                            var peer = ModHelper.Multiplayer.GetConnectedPlayer(who.UniqueMultiplayerID);
                            events = peer is { IsSplitScreen: true, ScreenID: not null }
                                ? this.Manager.EnabledForScreen(peer.ScreenID.Value).Aggregate(
                                    string.Empty,
                                    (current, next) => current + "\n\t\t- " + next.GetType().Name)
                                : await Broadcaster.RequestAsync(
                                    "EventsEnabled",
                                    "Debug/Request",
                                    who.UniqueMultiplayerID);
                        }

                        if (!string.IsNullOrEmpty(events))
                        {
                            message += "\n\n\tEvents:" + events;
                        }
                        else
                        {
                            message += "\n\nCouldn't read player's enabled events.";
                        }
                    }

                    Log.D(message);
                    break;
                }
            }
        }
    }
}
