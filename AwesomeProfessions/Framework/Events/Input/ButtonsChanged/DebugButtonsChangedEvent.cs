using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Events.Display.RenderedActiveMenu;

namespace TheLion.Stardew.Professions.Framework.Events.Input.ButtonsChanged;

[UsedImplicitly]
internal class DebugButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    public override void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (!ModEntry.Config.DebugKey.IsDown() ||
            !e.Pressed.Any(b => b is SButton.MouseRight or SButton.MouseLeft)) return;
        ModEntry.Log($"{e.Cursor.GetScaledScreenPixels()}", LogLevel.Debug);

        if (DebugRenderedActiveMenuEvent.FocusedComponent is not null)
        {
            var component = DebugRenderedActiveMenuEvent.FocusedComponent;
            var name = string.IsNullOrEmpty(component.name) ? "Anon" : component.name;
            var message = $"[{component.myID}]: {name} ({component.GetType().Name})";
            message = component.GetType().GetFields().Where(f => !f.Name.IsAnyOf("myID", "name")).Aggregate(message,
                (current, field) => current + $"\n\t- {field.Name}: {field.GetValue(component)}");
            ModEntry.Log(message, LogLevel.Debug);
        }
        else
        {
            if (Game1.currentLocation.Objects.TryGetValue(e.Cursor.Tile, out var o))
            {
                var message = $"[{o.ParentSheetIndex}]: {o.Name} ({o.GetType().Name})";
                message = o.GetType().GetFields().Where(f => !f.Name.IsAnyOf("ParentSheetIndex", "Name"))
                    .Aggregate(message, (current, field) => current + $"\n\t- {field.Name}: {field.GetValue(o)}");
                ModEntry.Log(message, LogLevel.Debug);
            }
            else
            {
                foreach (var c in Game1.currentLocation.characters.Cast<Character>()
                             .Concat(Game1.currentLocation.farmers))
                {
                    if (c.getTileLocation() != e.Cursor.Tile) continue;

                    var message = $"{c.Name} ({c.GetType()})";
                    message = c.GetType().GetFields().Where(f => f.Name != "Name").Aggregate(message,
                        (current, field) => current + $"\n\t- {field.Name}: {field.GetValue(c)}");

                    if (c is Farmer)
                    {
                        message += "\n\n\tModData:";
                        message = c.modData.Pairs.Aggregate(message,
                            (current, pair) => current + $"\n\t\t{pair.Key}: {pair.Value}");
                    }

                    ModEntry.Log(message, LogLevel.Debug);

                    break;
                }
            }
        }
    }
}