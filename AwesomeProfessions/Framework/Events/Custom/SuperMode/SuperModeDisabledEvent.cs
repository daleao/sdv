using System;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

namespace TheLion.Stardew.Professions.Framework.Events.Custom.SuperMode;

public delegate void SuperModeDisabledEventHandler();

internal class SuperModeDisabledEvent : BaseEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public override void Hook()
    {
        ModEntry.State.Value.SuperModeDisabled += OnSuperModeDisabled;
    }

    /// <summary>Unhook this event from the event listener.</summary>
    public override void Unhook()
    {
        ModEntry.State.Value.SuperModeDisabled -= OnSuperModeDisabled;
    }

    /// <summary>Raised when IsSuperModeActive is set to false.</summary>
    public void OnSuperModeDisabled()
    {
        // remove countdown and fade out overlay
        ModEntry.Subscriber.Subscribe(new SuperModeOverlayFadeOutUpdateTickedEvent());
        ModEntry.Subscriber.Unsubscribe(typeof(SuperModeCountdownUpdateTickedEvent));

        // notify peers
        ModEntry.ModHelper.Multiplayer.SendMessage(ModEntry.State.Value.SuperModeIndex, "SuperModeDisabled",
            new[] { ModEntry.Manifest.UniqueID });

        // unsubscribe self
        ModEntry.Subscriber.Unsubscribe(GetType());

        // remove permanent effects
        if (ModEntry.State.Value.SuperModeIndex != Utility.Professions.IndexOf("Piper")) return;

        // depower
        foreach (var slime in ModEntry.State.Value.PipedSlimeScales.Keys)
            slime.DamageToFarmer = (int)Math.Round(slime.DamageToFarmer / slime.Scale);

        // degorge
        ModEntry.Subscriber.Subscribe(new SlimeDeflationUpdateTickedEvent());
    }
}