namespace DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;

#region using directives

using DaLion.Core;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PiperOneSecondUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PiperOneSecondUpdateTickedEvent(EventManager? manager = null)
    : OneSecondUpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (Game1.currentLocation is SlimeHutch || !CoreMod.State.AreEnemiesNearby)
        {
            return;
        }

        if (State.AlliedSlimes[0] is not null &&
            (State.AlliedSlimes[1] is not null || !Game1.player.HasProfession(Profession.Piper, true)))
        {
            return;
        }

        foreach (var slime in Game1.currentLocation.characters.OfType<GreenSlime>())
        {
            if (!slime.Player.IsLocalPlayer || !slime.withinPlayerThreshold())
            {
                continue;
            }

            if (State.AlliedSlimes[0] is null)
            {
                slime.Set_Piped(Game1.player);
                State.AlliedSlimes[0] = slime.Get_Piped();
                break;
            }

            if (Game1.player.HasProfession(Profession.Piper, true) && State.AlliedSlimes[1] is null)
            {
                slime.Set_Piped(Game1.player);
                State.AlliedSlimes[1] = slime.Get_Piped();
                break;
            }
        }
    }
}
