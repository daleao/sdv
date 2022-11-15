namespace DaLion.Ligo.Modules.Arsenal.Weapons.Events;

#region using directives

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

        ModEntry.State.Arsenal.WeaponSwingCooldown = 400 - (weapon.speed.Value * 20);
        ModEntry.State.Arsenal.WeaponSwingCooldown = (int)(ModEntry.State.Arsenal.WeaponSwingCooldown * (1f - who.weaponSpeedModifier));
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Arsenal.WeaponSwingCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (ModEntry.State.Arsenal.WeaponSwingCooldown > 0)
        {
            Log.D($"{ModEntry.State.Arsenal.WeaponSwingCooldown}");
            return;
        }

        ModEntry.State.Arsenal.ComboHitStep = ComboHitStep.Idle;
        this.Disable();
    }
}
