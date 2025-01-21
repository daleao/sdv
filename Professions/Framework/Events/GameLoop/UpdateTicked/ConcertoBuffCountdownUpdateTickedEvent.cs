namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Buffs;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ConcertoBuffCountdownUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ConcertoBuffCountdownUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    private int _timer;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._timer = (int)(PiperConcertoBuff.BASE_DURATION_MS * 0.06f * LimitBreak.DurationMultiplier);
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        this.Manager.Enable<SlimeDeflationUpdateTickedEvent>();
        foreach (var (_, piped) in GreenSlime_Piped.Values)
        {
            if (!piped.IsSummoned)
            {
                piped.Burst();
            }
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (this._timer-- <= 0)
        {
            this.Disable();
        }
    }
}
