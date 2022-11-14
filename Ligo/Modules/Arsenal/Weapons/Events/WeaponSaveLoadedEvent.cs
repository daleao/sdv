namespace DaLion.Ligo.Modules.Arsenal.Weapons.Events;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class WeaponSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon weapon || !weapon.isScythe())
                {
                    return;
                }

                this.AddEnchantments(weapon);
                if (weapon.type.Value == MeleeWeapon.defenseSword &&
                    ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords &&
                    ModEntry.Config.Arsenal.Weapons.StabbySwords.Contains(weapon.Name))
                {
                    weapon.type.Value = MeleeWeapon.stabbingSword;
                }
            });
        }
        else
        {
            foreach (var weapon in Game1.player.Items.OfType<MeleeWeapon>())
            {
                if (weapon.isScythe())
                {
                    continue;
                }

                this.AddEnchantments(weapon);
                if (weapon.type.Value == MeleeWeapon.defenseSword &&
                    ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords &&
                    ModEntry.Config.Arsenal.Weapons.StabbySwords.Contains(weapon.Name))
                {
                    weapon.type.Value = MeleeWeapon.stabbingSword;
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
