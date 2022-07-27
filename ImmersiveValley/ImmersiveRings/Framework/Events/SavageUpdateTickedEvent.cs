namespace DaLion.Stardew.Rings.Framework.Events;

#region using directives

using Common.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class SavageUpdateTickedEvent : UpdateTickedEvent
{
    private const int RING_SHEET_INDEX_I = 523;
    private readonly int _buffId;
    private readonly string _buffSource;
    private readonly string _buffDescription;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal SavageUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        _buffId = (ModEntry.Manifest.UniqueID + "Savage").GetHashCode();
        _buffSource =
            ModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>("Data/ObjectInformation")[RING_SHEET_INDEX_I]
                .Split('/')[0];
        _buffDescription = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.472") + Environment.NewLine +
                           Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.473");
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var savageness = ModEntry.SavageExcitedness.Value;
        if (savageness <= 0) Disable();

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == _buffId);
        if (buff is not null) return;

        Game1.buffsDisplay.addOtherBuff(
            new(0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                savageness,
                0,
                0,
                1,
                "Savage Ring",
                _buffSource)
            {
                which = _buffId,
                sheetIndex = ModEntry.IsImmersiveProfessionsLoaded ? 41 : 9,
                millisecondsDuration = 1111,
                description = _buffDescription,
                glow = Color.Cyan
            }
        );

        var buffDecay = savageness switch
        {
            > 6 => 3,
            >= 4 => 2,
            _ => 1
        };

        ModEntry.SavageExcitedness.Value = Math.Max(0, savageness - buffDecay);
    }
}