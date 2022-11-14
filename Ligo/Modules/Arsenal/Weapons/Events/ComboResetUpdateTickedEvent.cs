namespace DaLion.Ligo.Modules.Arsenal.Weapons.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

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
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (--ModEntry.State.Arsenal.WeaponSwingCooldown > 0)
        {
            return;
        }

        ModEntry.State.Arsenal.ComboHitStep = ComboHitStep.Idle;
        this.Disable();
    }
}
