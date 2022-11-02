namespace DaLion.Redux.Framework.Rings.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Redux.Framework;
using DaLion.Shared.Events;
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
        this._buffId = (ModEntry.Manifest.UniqueID + "Warrior").GetHashCode();
        this._buffSource =
            ModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>("Data/ObjectInformation")[Constants.WarriorRingIndex]
                .Split('/')[0];
        this._buffDescription = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.468");
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.State.Rings.WarriorKillCount < 10)
        {
            return;
        }

        if (Game1.player.hasBuff(this._buffId))
        {
            return;
        }

        var magnitude = ModEntry.State.Rings.WarriorKillCount / 10;
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
                magnitude,
                1,
                "Warrior Ring",
                this._buffSource)
            {
                which = this._buffId,
                sheetIndex = 20,
                millisecondsDuration = 0,
                description = this._buffDescription,
                glow = Color.DarkRed,
            });
    }
}
