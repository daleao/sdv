namespace DaLion.Overhaul.Modules.Combat.Events.Player.Warped;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BlessedEnchantmentWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="BlessedEnchantmentWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BlessedEnchantmentWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.Player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: WeaponIds.HolyBlade } holyBlade)
        {
            this.Disable();
            return;
        }

        holyBlade.GetEnchantmentOfType<BlessedEnchantment>().OnWarp(e.Player, e.OldLocation, e.NewLocation);
    }
}
