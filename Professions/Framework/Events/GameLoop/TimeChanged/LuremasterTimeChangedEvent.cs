namespace DaLion.Professions.Framework.Events.GameLoop.TimeChanged;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Extensions;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LuremasterTimeChangedEvent"/> class.</summary>
[UsedImplicitly]
internal sealed class LuremasterTimeChangedEvent : TimeChangedEvent
{
    private static readonly double[] CatchProbabilityByAttempts = new double[120];

    private static readonly Func<int, int, double> GaussianProbabilityDistribution = (x, u) => Math.Exp(-Math.Pow(x - u, 2d) / 288d) / 30.0795;

    /// <summary>Initializes a new instance of the <see cref="LuremasterTimeChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    public LuremasterTimeChangedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        CatchProbabilityByAttempts[0] = 0d;
        for (var i = 1; i < 120; i++)
        {
            CatchProbabilityByAttempts[i] = CatchProbabilityByAttempts[i - 1] + GaussianProbabilityDistribution(i, 60);
        }
    }

    /// <inheritdoc />
    protected override void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e)
    {
        Game1.game1.EnumerateAllCrabPots().ForEach(crabPot =>
        {
            if (crabPot.bait.Value is null)
            {
                return;
            }

            var owner = crabPot.GetOwner();
            if (!owner.HasProfessionOrLax(Profession.Luremaster))
            {
                return;
            }

            var isOwnedByPrestigedLuremaster = owner.HasProfessionOrLax(Profession.Luremaster, true);
            if (crabPot.IsBlockedFromAdditionalCatches())
            {
                return;
            }

            var chance = CatchProbabilityByAttempts[crabPot.Get_CatchAttempts()];
            if (!Game1.random.NextBool(chance))
            {
                crabPot.IncrementCatchAttempts();
                return;
            }

            Log.D($"Crab Pot instance succeeded in Luremaster additional capture at {e.NewTime} hours. Running day update...");
            crabPot.DayUpdate();
            Log.D("Day update complete.");
            if (isOwnedByPrestigedLuremaster)
            {
                crabPot.ResetCatchAttempts();
            }
            else
            {
                crabPot.BlockAdditionalCatches();
            }
        });
    }
}
