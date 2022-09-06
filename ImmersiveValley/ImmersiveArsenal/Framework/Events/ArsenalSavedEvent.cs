namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Events;
using Enchantments;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalSavedEvent : SavedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSavedEvent(EventManager manager)
        : base(manager)
    {
        AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnSavedImpl(object? sender, SavedEventArgs e)
    {
        Utility.iterateAllItems(item =>
        {
            if (item is not MeleeWeapon weapon || weapon.isScythe()) return;

            switch (weapon.InitialParentTileIndex)
            {
                case Constants.DARK_SWORD_INDEX_I:
                    weapon.enchantments.Add(new CursedEnchantment());
                    break;
                case Constants.HOLY_BLADE_INDEX_I:
                    weapon.enchantments.Add(new BlessedEnchantment());
                    break;
                case Constants.INFINITY_BLADE_INDEX_I:
                case Constants.INFINITY_DAGGER_INDEX_I:
                case Constants.INFINITY_CLUB_INDEX_I:
                    weapon.enchantments.Add(new InfinityEnchantment());
                    break;
            }
        });
    }
}