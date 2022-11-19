namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Extensions;
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
        var who = Game1.player;
        if (who.CurrentTool is not MeleeWeapon weapon || ModEntry.State.Arsenal.ComboHitStep == ComboHitStep.Idle)
        {
            return;
        }

        ModEntry.State.Arsenal.WeaponSwingCooldown = (int)(400 * who.GetTotalSwingSpeedModifier(weapon));
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Arsenal.WeaponSwingCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (ModEntry.State.Arsenal.WeaponSwingCooldown > 0)
        {
            return;
        }

        ModEntry.State.Arsenal.ComboHitStep = ComboHitStep.Idle;
        this.Disable();
    }
}
