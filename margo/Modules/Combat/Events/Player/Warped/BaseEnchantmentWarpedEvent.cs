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
internal sealed class BaseEnchantmentWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BaseEnchantmentWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        switch (e.Player.CurrentTool)
        {
            case MeleeWeapon weapon:
                switch (weapon.InitialParentTileIndex)
                {
                    case WeaponIds.HolyBlade:
                        weapon.GetEnchantmentOfType<BlessedEnchantment>()
                            .OnWarp(e.Player, e.OldLocation, e.NewLocation);
                        break;
                    default:
                        if (weapon.IsInfinityWeapon())
                        {
                            weapon.GetEnchantmentOfType<InfinityEnchantment>()
                                .OnWarp(e.Player, e.OldLocation, e.NewLocation);
                        }

                        break;
                }

                break;

            case Slingshot slingshot:
                switch (slingshot.InitialParentTileIndex)
                {
                    case WeaponIds.InfinitySlingshot:
                        slingshot.GetEnchantmentOfType<InfinityEnchantment>()
                            .OnWarp(e.Player, e.OldLocation, e.NewLocation);
                        break;
                }

                break;

            default:
                this.Disable();
                return;
        }
    }
}
