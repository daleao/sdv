namespace DaLion.Overhaul.Modules.Combat.Events.Player.Warped;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class WeaponEnchantmentWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponEnchantmentWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponEnchantmentWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.Player.CurrentTool is not MeleeWeapon weapon)
        {
            this.Disable();
            return;
        }

        switch (weapon.InitialParentTileIndex)
        {
            case WeaponIds.HolyBlade:
                weapon.GetEnchantmentOfType<BlessedEnchantment>().OnWarp(e.Player, e.OldLocation, e.NewLocation);
                break;
            default:
                if (weapon.IsInfinityWeapon())
                {
                    weapon.GetEnchantmentOfType<InfinityEnchantment>().OnWarp(e.Player, e.OldLocation, e.NewLocation);
                }

                break;
        }
    }
}
