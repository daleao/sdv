namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using NetFabric.Hyperlinq;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="ToolSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ToolsModule.Config.EnableAutoSelection;

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        var slots = string.Join(
            ',',
            ToolsModule.State.SelectableTools
                .AsValueEnumerable()
                .Select(tool => Game1.player.Items.IndexOf(tool)));
        Game1.player.Append(DataFields.SelectableSlots, slots);
    }
}
