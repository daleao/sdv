namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;
using DaLion.Professions.Framework.Events.Input.CursorMoved;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PiperWarpedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PiperWarpedEvent(EventManager? manager = null)
    : WarpedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Piper);

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        var piper = e.Player;
        var newLocation = e.NewLocation;
        var toDangerZone = newLocation.IsEnemyArea();
        if (!toDangerZone)
        {
            this.Manager.Enable<PipedDismissOneSecondUpdateTickedEvent>();
            if ((!newLocation.IsOutdoors && newLocation is not SlimeHutch) || !PipedSlime.TheHatSlimeIsUponUs)
            {
                return;
            }

            PipedSlime.HatSlime.WarpToPiper();
            this.Manager.Enable<PiperVisionCursorMovedEvent>();
            return;
        }

        if (!GreenSlime_Piped.Values.Any())
        {
            return;
        }

        foreach (var (_, piped) in GreenSlime_Piped.Values)
        {
            if (!ReferenceEquals(piped.Slime.currentLocation, newLocation))
            {
                piped.WarpToPiper();
            }
        }
    }
}
