namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Arsenal.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ArsenalSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (Game1.player.Read<bool>(DataFields.Cursed))
        {
            ModEntry.Events.Enable<CurseUpdateTickedEvent>();
        }

        if (Game1.player.Read<bool>(DataFields.ArsenalInitialized))
        {
            return;
        }

        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is not MeleeWeapon weapon || weapon.isScythe())
                {
                    return;
                }

                if (ModEntry.Config.Arsenal.InfinityPlusOne)
                {
                    weapon.AddIntrinsicEnchantments();
                }

                if (ModEntry.Config.Arsenal.Weapons.StabbySwords.Contains(weapon.Name))
                {
                    weapon.type.Value = MeleeWeapon.stabbingSword;
                }

                if (ModEntry.Config.Arsenal.Weapons.RebalancedWeapons)
                {
                    weapon.RefreshStats();
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

                if (ModEntry.Config.Arsenal.InfinityPlusOne)
                {
                    weapon.AddIntrinsicEnchantments();
                }

                if (ModEntry.Config.Arsenal.Weapons.StabbySwords.Contains(weapon.Name))
                {
                    weapon.type.Value = MeleeWeapon.stabbingSword;
                }

                if (ModEntry.Config.Arsenal.Weapons.RebalancedWeapons)
                {
                    weapon.RefreshStats();
                }
            }
        }

        Game1.player.Write(DataFields.ArsenalInitialized, true.ToString());
    }
}
