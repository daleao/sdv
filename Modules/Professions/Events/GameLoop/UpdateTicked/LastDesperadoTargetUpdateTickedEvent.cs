namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class LastDesperadoTargetUpdateTickedEvent : UpdateTickedEvent
{
    private int _timer;

    /// <summary>Initializes a new instance of the <see cref="LastDesperadoTargetUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LastDesperadoTargetUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._timer = 60;
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        ProfessionsModule.State.LastDesperadoTarget = null;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.player.CurrentTool is not Slingshot || --this._timer <= 0)
        {
            this.Disable();
        }
    }
}
