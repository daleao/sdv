namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ToolsModule.Config.EnableAutoSelection;

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        if (ArsenalModule.State.SelectableArsenal is not null)
        {
            Game1.player.Write(
                DataKeys.SelectableArsenal,
                Game1.player.Items.IndexOf(ArsenalModule.State.SelectableArsenal).ToString());
        }
    }
}
