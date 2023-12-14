namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.TimeChanged;

#region using directives

using System.Threading.Tasks;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayStarted;
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
    protected override void OnEnabled()
    {
        this.Manager.Disable<MonitorLuremastersDayStartedEvent>();
    }

    /// <inheritdoc />
    protected override void OnTimeChangedImpl(object? sender, TimeChangedEventArgs e)
    {
        Parallel.ForEach(Game1.game1.IterateAllWithLocation<CrabPot>(), pair =>
        {
            if (pair.Instance.heldObject.Value is not null)
            {
                return;
            }

            var owner = pair.Instance.GetOwner();
            var max = owner.HasProfessionOrLax(VanillaProfession.Luremaster, true)
                ? 2
                : owner.HasProfessionOrLax(VanillaProfession.Luremaster)
                    ? 1
                    : 0;
            if (max == 0 || pair.Instance.Get_Successes() >= max)
            {
                return;
            }

            var chance = 1d / ((max == 2 ? 8d : 12d) - pair.Instance.Get_Attempts());
            if (Game1.random.NextDouble() > chance)
            {
                return;
            }

            pair.Instance.DayUpdate(pair.Location);
            pair.Instance.IncrementSuccesses();
            pair.Instance.ResetAttempts();
        });
    }
}
