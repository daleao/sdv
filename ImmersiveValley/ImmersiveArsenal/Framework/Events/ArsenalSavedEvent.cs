namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Arsenal.Framework.Enchantments;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalSavedEvent : SavedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalSavedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSavedEvent(EventManager manager)
        : base(manager)
    {
        this.AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnSavedImpl(object? sender, SavedEventArgs e)
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
                    weapon.enchantments.Add(new CursedEnchantment());
                    break;
                case Constants.HolyBladeIndex:
                    weapon.enchantments.Add(new BlessedEnchantment());
                    break;
                case Constants.InfinityBladeIndex:
                case Constants.InfinityDaggerIndex:
                case Constants.InfinityClubIndex:
                    weapon.enchantments.Add(new InfinityEnchantment());
                    break;
            }
        });
    }
}
