namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class EnergizedUpdateTickedEvent : UpdateTickedEvent
{
    private const int BuffSheetIndex = 42;

    private readonly int _buffId = (ModEntry.Manifest.UniqueID + "Energized").GetHashCode();

    private uint _previousStepsTaken;

    /// <summary>Initializes a new instance of the <see cref="EnergizedUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnergizedUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._previousStepsTaken = Game1.stats.StepsTaken;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.stats.StepsTaken > this._previousStepsTaken && Game1.stats.StepsTaken % 6 == 0)
        {
            ++ModEntry.State.EnergizeStacks;
            this._previousStepsTaken = Game1.stats.StepsTaken;
        }

        if (ModEntry.State.EnergizeStacks <= 0 || Game1.player.hasBuff(this._buffId))
        {
            return;
        }

        Game1.buffsDisplay.addOtherBuff(
            new Buff(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                "Energized",
                ModEntry.i18n.Get("enchantments.energized"))
            {
                which = this._buffId,
                sheetIndex = BuffSheetIndex,
                millisecondsDuration = 0,
                description = string.Empty,
            });
    }
}
