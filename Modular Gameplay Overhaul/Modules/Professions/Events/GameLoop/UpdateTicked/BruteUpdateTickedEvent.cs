namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Core.VirtualProperties;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class BruteUpdateTickedEvent : UpdateTickedEvent
{
    private readonly int _buffId = (Manifest.UniqueID + Profession.Brute).GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="BruteUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BruteUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player.Get_BruteRageCounter() <= 0)
        {
            return;
        }

        // decay counter every 5 seconds after 25 seconds out of combat
        if ((Game1.game1.IsActiveNoOverlay || !Game1.options.pauseWhenOutOfFocus) && Game1.shouldTimePass() &&
            e.IsMultipleOf(300) && player.Get_SecondsOutOfCombat() > 25)
        {
            player.Decrement_BruteRageCounter();
        }

        if (player.hasBuff(this._buffId))
        {
            return;
        }

        var magnitude = player.Get_BruteRageCounter() * 0.01f;
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
                "Brute",
                I18n.Get("brute.title" + (Game1.player.IsMale ? ".title" : ".female")) + " " +
                I18n.Get("brute.buff.name"))
            {
                which = this._buffId,
                sheetIndex = Profession.BruteRageSheetIndex,
                millisecondsDuration = 0,
                description = Game1.player.HasProfession(Profession.Brute, true)
                    ? I18n.Get("brute.buff.desc", new { damage = magnitude.ToString("0.0") })
                    : I18n.Get(
                        "brute.buff.desc.prestiged",
                        new { damage = magnitude.ToString("0.0"), speed = (magnitude / 2f).ToString("0.0") }),
            });
    }
}
