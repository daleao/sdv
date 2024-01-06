namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.TimeChanged;

#region using directives

using System.Threading.Tasks;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class LuremasterTimeChangedEvent : TimeChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LuremasterTimeChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal LuremasterTimeChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e)
    {
        Parallel.ForEach(Game1.game1.IterateAllWithLocation<CrabPot>(), pair =>
        {
            var (instance, location) = pair;
            if (instance.heldObject.Value is not null)
            {
                return;
            }

            var owner = instance.GetOwner();
            var max = owner.HasProfessionOrLax(Profession.Luremaster, true)
                ? 2
                : owner.HasProfessionOrLax(Profession.Luremaster)
                    ? 1
                    : 0;
            if (max == 0 || instance.Get_Successes() >= max)
            {
                return;
            }

            var chance = 1d / ((max == 2 ? 8d : 12d) - instance.Get_Attempts());
            if (Game1.random.NextDouble() > chance)
            {
                instance.IncrementAttempts();
                return;
            }

            Log.D("Crab Pot instance succeeded in Luremaster additional capture! Running day update...");
            instance.DayUpdate(location);
            Log.D("Day update complete.");
            instance.IncrementSuccesses();
            instance.ResetAttempts();
        });
    }
}
