namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using System.Linq;
using DaLion.Common.Events;
using DaLion.Stardew.Arsenal.Framework.Enchantments;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
        this.AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon weapon && !weapon.isScythe())
                {
                    this.AddEnchantments(weapon);
                }
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                if (!weapon.isScythe())
                {
                    this.AddEnchantments(weapon);
                }
            }
        }
    }

    private void AddEnchantments(MeleeWeapon weapon)
    {
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
    }
}
