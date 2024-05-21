namespace DaLion.Professions.Framework.Events.GameLoop.TimeChanged;

#region using directives

using System.Threading.Tasks;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Extensions;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LuremasterTimeChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class LuremasterTimeChangedEvent(EventManager? manager = null)
    : TimeChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e)
    {
        Parallel.ForEach(Game1.game1.EnumerateAllCrabPots(), crabPot =>
        {
            if (crabPot.bait.Value is null)
            {
                return;
            }

            var owner = crabPot.GetOwner();
            var max = owner.HasProfession(Profession.Luremaster, true)
                ? 2
                : owner.HasProfessionOrLax(Profession.Luremaster)
                    ? 1
                    : 0;
            if (max == 0 || crabPot.Get_Successes() >= max)
            {
                return;
            }

            var chance = 1d / ((max == 2 ? 8d : 12d) - crabPot.Get_Attempts());
            if (!Game1.random.NextBool(chance))
            {
                crabPot.IncrementAttempts();
                return;
            }

            Log.D("Crab Pot instance succeeded in Luremaster additional capture! Running day update...");
            crabPot.DayUpdate();
            Log.D("Day update complete.");
            crabPot.IncrementSuccesses();
            crabPot.ResetAttempts();
        });
    }
}
