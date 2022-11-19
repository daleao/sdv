namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EnergizedUpdateTickedEvent : UpdateTickedEvent
{
    private readonly Dictionary<Farmer, uint> _previousStepsByFarmer = new();

    /// <summary>Initializes a new instance of the <see cref="EnergizedUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnergizedUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._previousStepsByFarmer[Game1.player] = Game1.stats.StepsTaken;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var farmer = Game1.player;
        var energized = (farmer.CurrentTool as MeleeWeapon)?.GetEnchantmentOfType<EnergizedEnchantment>();
        if (energized is null)
        {
            this.Disable();
            return;
        }

        if (e.IsOneSecond)
        {
            var gained = (Game1.stats.StepsTaken - this._previousStepsByFarmer[farmer]) / 3;
            if (gained > 0)
            {
                energized.Stacks += (int)gained;
                this._previousStepsByFarmer[farmer] = Game1.stats.StepsTaken;
            }
        }

        if (energized.Stacks <= 0 || farmer.hasBuff(EnergizedEnchantment.BuffId))
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
                which = EnergizedEnchantment.BuffId,
                sheetIndex = EnergizedEnchantment.BuffSheetIndex,
                millisecondsDuration = 0,
                description = string.Empty,
            });
    }
}
