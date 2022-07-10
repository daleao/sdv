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
internal sealed class ArsenalSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSaveLoadedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        Utility.iterateAllItems(item =>
        {
            if (item is not MeleeWeapon weapon || weapon.isScythe()) return;

            switch (weapon.InitialParentTileIndex)
            {
                case Constants.DARK_SWORD_INDEX_I:
                    weapon.enchantments.Add(new DemonicEnchantment());
                    break;
                case Constants.HOLY_BLADE_INDEX_I:
                    weapon.enchantments.Add(new HolyEnchantment());
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