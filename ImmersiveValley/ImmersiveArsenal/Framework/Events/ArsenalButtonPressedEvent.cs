namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using System;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalButtonPressedEvent : ButtonPressedEvent
{
    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object sender, ButtonPressedEventArgs e)
    {
        if (!ModEntry.Config.WeaponsCostStamina || Game1.eventUp || !Game1.player.CanMove || Game1.player.UsingTool ||
            Game1.player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe()) return;

        if (e.Button.IsUseToolButton())
        {
            var multiplier = weapon.type.Value switch
            {
                MeleeWeapon.dagger => 0.5f,
                MeleeWeapon.club => 2f,
                _ => 1f,
            };

            Game1.player.Stamina -= (2 - Game1.player.CombatLevel * 0.1f) * multiplier;
        }
        else if (e.Button.IsActionButton() && weapon.type.Value is not (MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword))
        {
            var multiplier = weapon.type.Value switch
            {
                MeleeWeapon.dagger => 1f,
                MeleeWeapon.club => 4f,
                _ => throw new ArgumentOutOfRangeException()
            };

            Game1.player.Stamina -= (4 - Game1.player.CombatLevel * 0.1f) * multiplier;
        }
    }
}