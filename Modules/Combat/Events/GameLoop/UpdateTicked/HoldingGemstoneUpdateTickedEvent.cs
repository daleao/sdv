namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HoldingGemstoneUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="HoldingGemstoneUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal HoldingGemstoneUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <summary>Gets or sets the <see cref="Chord"/> to be played.</summary>
    internal static Gemstone? Gemstone { get; set; }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        if (Gemstone is null)
        {
            this.Disable();
            return;
        }

        Gemstone.PlayCue();
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        Gemstone?.StopCue();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Gemstone!.Modulate();
        if (e.Ticks % 100 == 0)
        {
            Gemstone.FadeIn();
        }
    }
}
