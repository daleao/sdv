namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
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
        if (player.CurrentTool is not MeleeWeapon weapon || player.Get_CurrentHitStep() == ComboHitStep.Idle)
        {
            return;
        }

        player.Set_ComboCooldown((int)(400 * player.GetTotalSwingSpeedModifier(weapon)));
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Game1.player.Decrement_ComboCooldown(Game1.currentGameTime.ElapsedGameTime.Milliseconds);
        if (Game1.player.Get_ComboCooldown() > 0)
        {
            return;
        }

        Game1.player.Set_CurrentHitStep(ComboHitStep.Idle);
        this.Disable();
    }
}
