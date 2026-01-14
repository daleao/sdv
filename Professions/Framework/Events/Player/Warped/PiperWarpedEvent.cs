namespace DaLion.Professions.Framework.Events.Player.Warped;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicket;
using DaLion.Professions.Framework.UI;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using Input.CursorMoved;
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
        var toDangerZone = newLocation.IsEnemyArea() || newLocation.Name.ContainsAnyOf("Mine", "SkullCave");
        if (!toDangerZone)
        {
            this.Manager.Enable<PipedSelfDestructOneSecondUpdateTickedEvent>();
            State.PipedMinionMenu?.Dispose();
            State.PipedMinionMenu = null;
            if (!newLocation.IsOutdoors && newLocation is not SlimeHutch)
            {
                return;
            }

            foreach (var (_, piped) in GreenSlime_Piped.Values)
            {
                if (!piped.IsDismissed)
                {
                    piped.WarpToPiper();
                }
            }

            this.Manager.Enable<PiperVisionCursorMovedEvent>();
            return;
        }

        var oldLocation = e.OldLocation;
        var fromDangerZone = oldLocation.IsEnemyArea() || oldLocation.Name.ContainsAnyOf("Mine", "SkullCave");
        if (!fromDangerZone)
        {
            var numberRaised = piper.CountRaisedSlimes();
            if (numberRaised == 0)
            {
                return;
            }

            var numberToSpawn = ((numberRaised - 1) / 10) + 1;
            if (numberToSpawn == 0)
            {
                return;
            }

            piper.SpawnMinions(numberToSpawn);
        }

        if (!GreenSlime_Piped.Values.Any())
        {
            return;
        }

        foreach (var (_, piped) in GreenSlime_Piped.Values)
        {
            if (!ReferenceEquals(piped.Slime.currentLocation, newLocation) && !piped.IsDismissed)
            {
                piped.WarpToPiper();
            }
        }

        State.PipedMinionMenu = new PipedMinionHud();
    }
}
