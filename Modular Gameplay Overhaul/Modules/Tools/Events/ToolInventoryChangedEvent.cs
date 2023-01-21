namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolInventoryChangedEvent : InventoryChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ToolInventoryChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolInventoryChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ToolsModule.Config.EnableAutoSelection;

    /// <inheritdoc />
    protected override void OnInventoryChangedImpl(object? sender, InventoryChangedEventArgs e)
    {
        if (!e.IsLocalPlayer || ToolsModule.State.SelectableToolByType.Count == 0)
        {
            return;
        }

        foreach (var removed in e.Removed)
        {
            var type = removed.GetType();
            if (removed is Tool && ToolsModule.State.SelectableToolByType.TryGetValue(type, out var selectable) &&
                selectable?.Tool == removed)
            {
                ToolsModule.State.SelectableToolByType[type] = null;
            }
        }
    }
}
