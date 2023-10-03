namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Core.Events;
using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class WarriorUpdateTickedEvent : UpdateTickedEvent
{
    private readonly int _buffId;
    private readonly string _buffDescription;
    private readonly string _buffSource;

    /// <summary>Initializes a new instance of the <see cref="WarriorUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WarriorUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        this._buffId = (Manifest.UniqueID + "Warrior").GetHashCode();
        this._buffSource =
            ModHelper.GameContent
                .Load<Dictionary<int, string>>("Data/ObjectInformation")[ObjectIds.WarriorRing]
                .SplitWithoutAllocation('/')[0]
                .ToString();
        this._buffDescription = I18n.Buffs_Warrior_Desc();
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this.Manager.Enable<OutOfCombatOneSecondUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (CombatModule.State.WarriorKillCount <= 0)
        {
            this.Disable();
            return;
        }

        // decay counter every 5 seconds after 25 seconds out of combat
        if (Game1.game1.ShouldTimePass() && ModEntry.State.SecondsOutOfCombat > 25 && e.IsMultipleOf(300))
        {
            CombatModule.State.WarriorKillCount--;
        }

        if (Game1.player.hasBuff(this._buffId))
        {
            return;
        }

        Game1.buffsDisplay.addOtherBuff(
            new StackableBuff(
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
                CombatModule.State.WarriorKillCount,
                1,
                "Warrior Ring",
                this._buffSource,
                () => CombatModule.State.WarriorKillCount,
                30)
            {
                which = this._buffId,
                sheetIndex = 20,
                millisecondsDuration = 0,
                description =
                    this._buffDescription + Environment.NewLine +
                    I18n.Ui_Buffs_Warrior(CombatModule.State.WarriorKillCount / 3),
                glow = Color.DarkRed,
            });
    }
}
