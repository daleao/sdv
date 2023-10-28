namespace DaLion.Overhaul.Modules.Professions.Events.Player.Warped;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[ModRequirement("FlashShifter.StardewValleyExpandedCP", "Stardew Valley Expanded")]
[AlwaysEnabledEvent]
internal sealed class GaldoraHudThemeWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="GaldoraHudThemeWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal GaldoraHudThemeWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.GetType() == e.OldLocation.GetType())
        {
            return;
        }

        if (e.NewLocation.NameOrUniqueName.IsAnyOf(
                "Custom_CastleVillageOutpost",
                "Custom_CrimsonBadlands",
                "Custom_IridiumQuarry",
                "Custom_TreasureCave"))
        {
            ModHelper.GameContent.InvalidateCache($"{Manifest.UniqueID}/UltimateMeter");
        }
    }
}
