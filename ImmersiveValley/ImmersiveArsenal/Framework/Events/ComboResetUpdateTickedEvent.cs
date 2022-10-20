namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common.Events;
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
        if (--ModEntry.State.WeaponSwingCooldown > 0)
        {
            return;
        }

        ModEntry.State.ComboHitStep = ComboHitStep.Idle;
        ModEntry.State.QueuedHits = 0;
        Log.D($"Combo hit step: {ModEntry.State.ComboHitStep}");
        this.Disable();
    }
}
