namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.Display;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Sounds;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class DesperadoUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DesperadoUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DesperadoUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        if (Game1.player.CurrentTool is not Slingshot)
        {
            this.Disable();
            return;
        }

        this.Manager.Enable<DesperadoRenderedHudEvent>();
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        Game1.player.stopJittering();
        Sfx.SinWave?.Stop(AudioStopOptions.Immediate);
        this.Manager.Disable<DesperadoRenderedHudEvent>();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var firer = Game1.player;
        if (firer.CurrentTool is not Slingshot slingshot || !firer.usingSlingshot)
        {
            this.Disable();
            return;
        }

        var overchargePct = slingshot.GetOvercharge() - 1f;
        if (overchargePct <= 0f)
        {
            return;
        }

        firer.jitterStrength = Math.Max(0f, overchargePct);

        if (Game1.soundBank is null)
        {
            return;
        }

        Sfx.SinWave ??= Game1.soundBank.GetCue("SinWave");
        if (!Sfx.SinWave.IsPlaying)
        {
            Sfx.SinWave.Play();
        }

        Sfx.SinWave.SetVariable("Pitch", 2400f * overchargePct);
    }
}
