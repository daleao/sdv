namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Buffs;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DesperadoQuickshotUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class DesperadoQuickshotUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    private int _timer;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._timer = 48;
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        State.LastDesperadoTarget = null;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player.CurrentTool is not Slingshot || --this._timer <= 0)
        {
            this.Disable();
            return;
        }

        player.applyBuff(new DesperadoQuickshotBuff());
    }
}
