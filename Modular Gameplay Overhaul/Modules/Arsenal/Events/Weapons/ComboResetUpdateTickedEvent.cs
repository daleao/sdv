namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
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
        if (player.CurrentTool is not MeleeWeapon weapon || ArsenalModule.State.ComboHitQueued == ComboHitStep.Idle)
        {
            return;
        }

        ArsenalModule.State.ComboCooldown = (int)(400 * player.GetTotalSwingSpeedModifier(weapon));
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        ArsenalModule.State.ComboCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (ArsenalModule.State.ComboCooldown > 0)
        {
            return;
        }

        ArsenalModule.State.ComboHitQueued = ComboHitStep.Idle;
        ArsenalModule.State.ComboHitStep = ComboHitStep.Idle;
        this.Disable();
    }
}
