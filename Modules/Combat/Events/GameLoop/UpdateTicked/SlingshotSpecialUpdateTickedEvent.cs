namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotSpecialUpdateTickedEvent : UpdateTickedEvent
{
    private Slingshot? _slingshot;

    /// <summary>Initializes a new instance of the <see cref="SlingshotSpecialUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotSpecialUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        if (Game1.player.CurrentTool is not Slingshot slingshot)
        {
            this.Disable();
            return;
        }

        this._slingshot = slingshot;
        CombatModule.State.SlingshotGatlingTimer = slingshot.GetSpecialDuration();
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        this._slingshot ??= new Slingshot();
        var triggerCooldown = CombatModule.State.SlingshotGatlingTimer <= this._slingshot.GetSpecialDuration() - (this._slingshot.GetAutoFireRate() * 1000);
        CombatModule.State.SlingshotGatlingTimer = 0;
        this._slingshot?.EndSpecialMove(triggerCooldown);
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        CombatModule.State.SlingshotGatlingTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (CombatModule.State.SlingshotGatlingTimer > 0)
        {
            return;
        }

        this.Disable();
    }
}
