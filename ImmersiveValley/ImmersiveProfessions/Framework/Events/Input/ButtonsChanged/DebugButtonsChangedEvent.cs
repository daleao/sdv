namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using Common;
using Common.Attributes;
using Common.Events;
using Common.Extensions;
using Display;
using LinqFasterer;
using StardewModdingAPI.Events;
using System.Linq;

#endregion using directives

[UsedImplicitly, DebugOnly]
internal sealed class DebugButtonsChangedEvent : ButtonsChangedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugButtonsChangedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysEnabled = true;
    }

    /// <inheritdoc />
    public override bool Enable() => false;

    /// <inheritdoc />
    public override bool Disable() => false;

    /// <inheritdoc />
    protected override async void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.DebugKey.JustPressed())
            ModEntry.Events.EnableWithAttribute<DebugOnlyAttribute>();
        else if (ModEntry.Config.DebugKey.GetState() == SButtonState.Released)
            ModEntry.Events.DisableWithAttribute<DebugOnlyAttribute>();

        if (!ModEntry.Config.DebugKey.IsDown() ||
            !e.Pressed.Any(b => b is SButton.MouseRight or SButton.MouseLeft)) return;

        if (DebugRenderedActiveMenuEvent.FocusedComponent is not null)
        {
            var component = DebugRenderedActiveMenuEvent.FocusedComponent;
            var name = string.IsNullOrEmpty(component.name) ? "Anon" : component.name;
            var message = $"[{component.myID}]: {name} ({component.GetType().Name})";
            message = component.GetType().GetFields().WhereF(f => !f.Name.IsIn("myID", "name")).AggregateF(message,
                (current, field) => current + $"\n\t- {field.Name}: {field.GetValue(component)}");
            Log.D(message);
        }
        else if (Context.IsWorldReady)
        {
            if (Game1.currentLocation.Objects.TryGetValue(e.Cursor.Tile, out var o))
            {
                var message = $"[{o.ParentSheetIndex}]: {o.Name} ({o.GetType().Name})";
                message = o.GetType().GetFields().WhereF(f => !f.Name.IsIn("ParentSheetIndex", "Name"))
                    .AggregateF(message, (current, field) => current + $"\n\t- {field.Name}: {field.GetValue(o)}");
                Log.D(message);
            }
            else
            {
                foreach (var c in Game1.currentLocation.characters.Cast<Character>()
                             .Concat(Game1.currentLocation.farmers))
                {
                    if (c.getTileLocation() != e.Cursor.Tile) continue;

                    var message = string.Empty;
                    Farmer? who = null;
                    if (c is Farmer farmer)
                    {
                        who = farmer;
                        message += $"[{who.UniqueMultiplayerID}]: ";
                    }

                    message += $"{c.Name} ({c.GetType()})";
                    message = c.GetType().GetFields().Where(f => !f.Name.IsIn("UniqueMultiplayerID", "Name"))
                        .Aggregate(message, (m, f) => m + $"\n\t- {f.Name}: {f.GetValue(c)}");

                    message +=
                        $"\n\n\tCurrent location: {c.currentLocation.NameOrUniqueName} ({c.currentLocation.GetType().Name})";

                    if (who is not null)
                    {
                        message += "\n\n\tMod data:";
                        message = Game1.MasterPlayer.modData.Pairs
                            .Where(p => p.Key.StartsWith(ModEntry.Manifest.UniqueID) &&
                                        p.Key.Contains(who.UniqueMultiplayerID.ToString()))
                            .Aggregate(message,
                                (m, p) => m + $"\n\t\t- {p.Key}: {p.Value}");

                        var events = "";
                        if (who.IsLocalPlayer)
                        {
                            events = Manager.Enabled.Aggregate("",
                                (current, next) => current + "\n\t\t- " + next.GetType().Name);
                        }
                        else if (Context.IsMultiplayer && who.isActive())
                        {
                            var peer = ModEntry.ModHelper.Multiplayer.GetConnectedPlayer(who.UniqueMultiplayerID);
                            if (peer is { IsSplitScreen: true, ScreenID: not null })
                            {
                                events = ModEntry.Events.EnabledForScreen(peer.ScreenID.Value).Aggregate("",
                                    (current, next) => current + "\n\t\t- " + next.GetType().Name);
                            }
                            else
                            {
                                events = await ModEntry.Broadcaster.RequestAsync("EventsEnabled", "Debug/Request",
                                    who.UniqueMultiplayerID);
                            }
                        }

                        if (!string.IsNullOrEmpty(events)) message += "\n\n\tEvents:" + events;
                        else message += "\n\nCouldn't read player's enabled events.";
                    }

                    Log.D(message);
                    break;
                }
            }
        }
    }
}