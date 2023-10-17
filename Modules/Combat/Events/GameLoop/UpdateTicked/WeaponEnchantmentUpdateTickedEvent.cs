namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class WeaponEnchantmentUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponEnchantmentUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponEnchantmentUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon weapon)
        {
            this.Disable();
            return;
        }

        switch (weapon.InitialParentTileIndex)
        {
            case WeaponIds.HolyBlade:
                weapon.GetEnchantmentOfType<BlessedEnchantment>().Update(e.Ticks, Game1.player);
                break;
            default:
                if (weapon.IsInfinityWeapon())
                {
                    weapon.GetEnchantmentOfType<InfinityEnchantment>().Update(e.Ticks, Game1.player);
                }

                break;
        }
    }
}
