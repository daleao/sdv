namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Core;
using DaLion.Professions.Framework.Buffs;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="BruteRageUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class BruteRageUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.BruteRageCounter > 0;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;

        // decay counter every 5 seconds after 25 seconds out of combat
        var expiry = player.HasProfession(Profession.Brute, true) ? 40 : 20;
        if (Game1.game1.ShouldTimePass() && CoreMod.State.SecondsOutOfCombat > expiry && e.IsMultipleOf(300))
        {
            State.BruteRageCounter--;
        }

        player.applyBuff(new BruteRageBuff());
    }
}
