namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using Common.Extensions;
using Display;
using Extensions;
using Microsoft.Xna.Framework.Audio;
using Sounds;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class DesperadoUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DesperadoUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        if (Game1.player.CurrentTool is not Slingshot slinghsot)
        {
            Disable();
            return;
        }

        var hasQuincyEnchantment = slinghsot.enchantments.FirstOrDefault(e =>
            e.GetType().FullName?.ContainsAllOf("ImmersiveSlingshots", "QuincyEnchantment") == true) is not null;
        if ((slinghsot.attachments.Count == 0 || slinghsot.attachments[0] is null) && !hasQuincyEnchantment)
        {
            Disable();
            return;
        }

        Manager.Enable<DesperadoRenderedHudEvent>();
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        Game1.player.stopJittering();
        SFX.SinWave?.Stop(AudioStopOptions.Immediate);
        Manager.Disable<DesperadoRenderedHudEvent>();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var firer = Game1.player;
        if (firer.CurrentTool is not Slingshot slingshot || !firer.usingSlingshot)
        {
            Disable();
            return;
        }

        var overcharge = slingshot.GetOvercharge(firer);
        if (overcharge <= 0f) return;

        firer.jitterStrength = Math.Max(0f, overcharge - 0.5f);

        if (Game1.soundBank is null) return;

        SFX.SinWave ??= Game1.soundBank.GetCue("SinWave");
        if (!SFX.SinWave.IsPlaying) SFX.SinWave.Play();

        SFX.SinWave.SetVariable("Pitch", 2400f * overcharge);
    }
}