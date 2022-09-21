namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Arsenal.Framework.Enchantments;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSavingEvent(EventManager manager)
        : base(manager)
    {
        this.AlwaysEnabled = true;
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
