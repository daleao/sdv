namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Events;
using Enchantments;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalSavingEvent : SavingEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSavingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        Utility.iterateAllItems(item =>
        {
            if (item is not MeleeWeapon weapon || weapon.isScythe()) return;

            switch (weapon.InitialParentTileIndex)
            {
                case Constants.DARK_SWORD_INDEX_I:
                    weapon.RemoveEnchantment(weapon.GetEnchantmentOfType<DemonicEnchantment>());
                    break;
                case Constants.HOLY_BLADE_INDEX_I:
                    weapon.RemoveEnchantment(weapon.GetEnchantmentOfType<HolyEnchantment>());
                    break;
                case Constants.INFINITY_BLADE_INDEX_I:
                case Constants.INFINITY_DAGGER_INDEX_I:
                case Constants.INFINITY_CLUB_INDEX_I:
                    weapon.RemoveEnchantment(weapon.GetEnchantmentOfType<InfinityEnchantment>());
                    break;
            }
        });
    }
}