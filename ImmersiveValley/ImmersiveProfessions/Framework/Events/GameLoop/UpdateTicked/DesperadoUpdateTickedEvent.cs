namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Common.Events;
using DaLion.Common.Extensions;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Events.Display;
using DaLion.Stardew.Professions.Framework.Sounds;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class DesperadoUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DesperadoUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DesperadoUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        if (Game1.player.CurrentTool is not Slingshot slinghsot)
        {
            this.Disable();
            return;
        }

        var hasQuincyEnchantment = slinghsot.enchantments.FirstOrDefault(e =>
            e.GetType().FullName?.ContainsAllOf("ImmersiveSlingshots", "QuincyEnchantment") == true) is not null;
        if ((slinghsot.attachments.Count <= 0 || slinghsot.attachments[0] is null) && !hasQuincyEnchantment)
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

        var overcharge = slingshot.GetOvercharge(firer);
        if (overcharge <= 0f)
        {
            return;
        }

        firer.jitterStrength = Math.Max(0f, overcharge - 0.5f);

        if (Game1.soundBank is null)
        {
            return;
        }

        Sfx.SinWave ??= Game1.soundBank.GetCue("SinWave");
        if (!Sfx.SinWave.IsPlaying)
        {
            Sfx.SinWave.Play();
        }

        Sfx.SinWave.SetVariable("Pitch", 2400f * overcharge);
    }
}
