namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.Display.RenderedWorld;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class DesperadoOverchargeUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DesperadoOverchargeUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DesperadoOverchargeUpdateTickedEvent(EventManager manager)
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

        this.Manager.Enable<DesperadoRenderedWorldEvent>();
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        Game1.player.stopJittering();
        SoundEffectPlayer.SinWave?.Stop(AudioStopOptions.Immediate);
        this.Manager.Disable<DesperadoRenderedWorldEvent>();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var firer = Game1.player;
        if (firer.CurrentTool is not Slingshot slingshot || !firer.usingSlingshot || slingshot.CanAutoFire())
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

        SoundEffectPlayer.SinWave ??= Game1.soundBank.GetCue("SinWave");
        if (!SoundEffectPlayer.SinWave.IsPlaying)
        {
            SoundEffectPlayer.SinWave.Play();
        }

        SoundEffectPlayer.SinWave.SetVariable("Pitch", 2400f * overchargePct);
    }
}
