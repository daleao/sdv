namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ArsenalSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        Utility.iterateAllItems(item =>
        {
            if (item is not MeleeWeapon weapon || weapon.isScythe())
            {
                return;
            }

            switch (weapon.InitialParentTileIndex)
            {
                case Constants.DarkSwordIndex:
                    weapon.RemoveEnchantment(weapon.GetEnchantmentOfType<CursedEnchantment>());
                    break;
                case Constants.HolyBladeIndex:
                    weapon.RemoveEnchantment(weapon.GetEnchantmentOfType<BlessedEnchantment>());
                    break;
                case Constants.InfinityBladeIndex:
                case Constants.InfinityDaggerIndex:
                case Constants.InfinityClubIndex:
                    weapon.RemoveEnchantment(weapon.GetEnchantmentOfType<InfinityEnchantment>());
                    break;
            }
        });
    }
}
