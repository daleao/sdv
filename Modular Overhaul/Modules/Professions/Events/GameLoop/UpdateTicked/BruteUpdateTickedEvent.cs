namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Core.Events;
using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
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
    protected override void OnEnabled()
    {
        this.Manager.Enable<OutOfCombatOneSecondUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (ProfessionsModule.State.BruteRageCounter <= 0)
        {
            this.Disable();
            return;
        }

        // decay counter every 5 seconds after 25 seconds out of combat
        if (Game1.game1.ShouldTimePass() && Globals.SecondsOutOfCombat > 25 && e.IsMultipleOf(300))
        {
            ProfessionsModule.State.BruteRageCounter--;
        }

        if (player.hasBuff(this._buffId))
        {
            return;
        }

        var magnitude = ProfessionsModule.State.BruteRageCounter * 0.01f;
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
                0,
                1,
                "Brute",
                I18n.Get("brute.title" + (Game1.player.IsMale ? ".male" : ".female")) + " " +
                I18n.Get("brute.buff.name"),
                () => ProfessionsModule.State.BruteRageCounter,
                100)
            {
                which = this._buffId,
                sheetIndex = Profession.BruteRageSheetIndex,
                millisecondsDuration = 0,
                description = Game1.player.HasProfession(Profession.Brute, true)
                    ? I18n.Get("brute.buff.desc", new { damage = magnitude.ToString("P0") })
                    : I18n.Get(
                        "brute.buff.desc.prestiged",
                        new { damage = magnitude.ToString("P1"), speed = (magnitude / 2f).ToString("P1") }),
            });
    }
}
