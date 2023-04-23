namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class WeaponSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <summary>Gets the cache of weapons with intrinsic enchantments.</summary>
    /// <remarks>For recovery immediately after saving.</remarks>
    internal static List<MeleeWeapon> InstrinsicWeapons { get; } = new();

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        Utility.iterateAllItems(item =>
        {
            if (item is not MeleeWeapon weapon || !weapon.HasIntrinsicEnchantment())
            {
                return;
            }

            weapon.RemoveIntrinsicEnchantments();
            InstrinsicWeapons.Add(weapon);
        });

        if (WeaponsModule.State.AutoSelectableWeapon is not null)
        {
            Game1.player.Write(
                DataKeys.SelectableWeapon,
                Game1.player.Items.IndexOf(WeaponsModule.State.AutoSelectableWeapon).ToString());
        }
    }
}
