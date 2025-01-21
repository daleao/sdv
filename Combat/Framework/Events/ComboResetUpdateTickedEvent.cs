namespace DaLion.Combat.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ComboResetUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ComboResetUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ComboResetUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        var player = Game1.player;
        if (player?.CurrentTool is not MeleeWeapon weapon || State.QueuedHitStep == ComboHitStep.Idle)
        {
            this.Disable();
            return;
        }

        State.ComboCooldown = (int)(400f * weapon.GetTotalSpeedModifier(player));
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is not null)
        {
            return;
        }

        State.ComboCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (State.ComboCooldown > 0)
        {
            return;
        }

        State.QueuedHitStep = ComboHitStep.Idle;
        State.CurrentHitStep = ComboHitStep.Idle;
        this.Disable();
    }
}
