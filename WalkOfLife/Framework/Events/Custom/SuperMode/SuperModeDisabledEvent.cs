using System;

namespace TheLion.Stardew.Professions.Framework.Events;

public delegate void SuperModeDisabledEventHandler();

internal class SuperModeDisabledEvent : BaseEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public override void Hook()
    {
        ModState.SuperModeDisabled += OnSuperModeDisabled;
    }

    /// <summary>Unhook this event from the event listener.</summary>
    public override void Unhook()
    {
        ModState.SuperModeDisabled -= OnSuperModeDisabled;
    }

    /// <summary>Raised when IsSuperModeActive is set to false.</summary>
    public void OnSuperModeDisabled()
    {
        // remove countdown and fade out overlay
        ModEntry.Subscriber.Subscribe(new SuperModeOverlayFadeOutUpdateTickedEvent());
        ModEntry.Subscriber.Unsubscribe(typeof(SuperModeCountdownUpdateTickedEvent));

        // notify peers
        ModEntry.ModHelper.Multiplayer.SendMessage(ModState.SuperModeIndex, "SuperModeDisabled",
            new[] {ModEntry.Manifest.UniqueID});

        // unsubscribe self
        ModEntry.Subscriber.Unsubscribe(GetType());

        // remove permanent effects
        if (ModState.SuperModeIndex != Utility.Professions.IndexOf("Piper")) return;

        // depower
        foreach (var slime in ModState.PipedSlimeScales.Keys)
            slime.DamageToFarmer = (int) Math.Round(slime.DamageToFarmer / slime.Scale);

        // degorge
        ModEntry.Subscriber.Subscribe(new SlimeDeflationUpdateTickedEvent());
    }
}