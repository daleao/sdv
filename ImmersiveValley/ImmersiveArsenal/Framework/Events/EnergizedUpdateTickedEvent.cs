namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class EnergizedUpdateTickedEvent : UpdateTickedEvent
{
    private const int BUFF_SHEET_INDEX_I = 42;

    private readonly int _buffId = (ModEntry.Manifest.UniqueID + "Energized").GetHashCode();
    
    private uint _lastStepsTaken;
    private uint _stepCounter;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnergizedUpdateTickedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        _lastStepsTaken = Game1.stats.StepsTaken;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.stats.StepsTaken > _lastStepsTaken && Game1.stats.StepsTaken % 6 == 0)
        {
            ++ModEntry.EnergizeStacks.Value;
            _lastStepsTaken = Game1.stats.StepsTaken;
        }

        if (ModEntry.EnergizeStacks.Value <= 0 || Game1.player.hasBuff(_buffId)) return;

        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                "Energized",
                ModEntry.i18n.Get("enchantments.energized"))
            {
                which = _buffId,
                sheetIndex = BUFF_SHEET_INDEX_I,
                millisecondsDuration = 0,
                description = string.Empty
            }
        );
    }
}