namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class BruteUpdateTickedEvent : UpdateTickedEvent
{
    private readonly int _buffId = (ModEntry.Manifest.UniqueID + Profession.Brute).GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="BruteUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BruteUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.State.Professions.BruteRageCounter <= 0)
        {
            return;
        }

        if ((Game1.game1.IsActiveNoOverlay || !Game1.options.pauseWhenOutOfFocus) && Game1.shouldTimePass() &&
            ModEntry.State.Professions.BruteRageCounter > 0 &&
            e.IsOneSecond)
        {
            ++ModEntry.State.Professions.SecondsOutOfCombat;
            // decay counter every 5 seconds after 30 seconds out of combat
            if (ModEntry.State.Professions.SecondsOutOfCombat > 25 && e.IsMultipleOf(300))
            {
                --ModEntry.State.Professions.BruteRageCounter;
            }
        }

        if (Game1.player.hasBuff(this._buffId))
        {
            return;
        }

        var magnitude = (ModEntry.State.Professions.BruteRageCounter * Frenzy.PercentIncrementPerRage).ToString("P");
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
                ModEntry.i18n.Get("brute.title" + (Game1.player.IsMale ? ".title" : ".female")) + " " + ModEntry.i18n.Get("brute.buff.name"))
            {
                which = this._buffId,
                sheetIndex = Profession.BruteRageSheetIndex,
                millisecondsDuration = 0,
                description =
                    ModEntry.i18n.Get(
                        "brute.buff.desc" + (Game1.player.HasProfession(Profession.Brute, true)
                            ? ".prestiged"
                            : string.Empty),
                        new { magnitude }),
            });
    }
}
