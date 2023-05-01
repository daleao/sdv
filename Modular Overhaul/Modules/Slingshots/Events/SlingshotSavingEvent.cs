namespace DaLion.Overhaul.Modules.Slingshots.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ToolsModule.Config.EnableAutoSelection;

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        if (SlingshotsModule.State.AutoSelectableSlingshot is not null)
        {
            Game1.player.Write(
                DataKeys.SelectableSlingshot,
                Game1.player.Items.IndexOf(SlingshotsModule.State.AutoSelectableSlingshot).ToString());
        }
    }
}
