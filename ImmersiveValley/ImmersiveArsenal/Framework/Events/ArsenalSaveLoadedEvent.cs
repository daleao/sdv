namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Events;
using Enchantments;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
        AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon weapon && !weapon.isScythe()) AddEnchantments(weapon);
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>()) if (!weapon.isScythe()) AddEnchantments(weapon);
        }
    }

    private void AddEnchantments(MeleeWeapon weapon)
    {
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
    }
}