namespace DaLion.Ligo.Modules.Tools.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ToolUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ModEntry.State.Tools.Shockwave is not null;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.Config.Tools.TicksBetweenWaves > 0 && !e.IsMultipleOf(ModEntry.Config.Tools.TicksBetweenWaves))
        {
            return;
        }

        ModEntry.State.Tools.Shockwave!.Update(Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
    }
}
