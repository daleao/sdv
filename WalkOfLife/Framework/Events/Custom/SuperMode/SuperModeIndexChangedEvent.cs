using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace TheLion.Stardew.Professions.Framework.Events;

public delegate void SuperModeIndexChangedEventHandler(int newIndex);

internal class SuperModeIndexChangedEvent : BaseEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public override void Hook()
    {
        ModState.SuperModeIndexChanged += OnSuperModeIndexChanged;
    }

    /// <summary>Unhook this event from the event listener.</summary>
    public override void Unhook()
    {
        ModState.SuperModeIndexChanged -= OnSuperModeIndexChanged;
    }

    /// <summary>Raised when SuperModeIndex is set to a new value.</summary>
    public void OnSuperModeIndexChanged(int newIndex)
    {
        ModEntry.Subscriber.UnsubscribeSuperModeEvents();
        ModState.SuperModeGaugeValue = 0;

        if (newIndex is > 0 and (< 26 or >= 30))
            throw new ArgumentException($"Unexpected Super Mode index {newIndex}.");

        ModEntry.Data.Write("SuperModeIndex", ModState.SuperModeIndex.ToString());
        if (ModState.SuperModeIndex < 0)
        {
            ModEntry.Log("Unregistered Super Mode.", LogLevel.Info);
            return;
        }

        var whichSuperMode = Utility.Professions.NameOf(newIndex);
        switch (whichSuperMode)
        {
            case "Brute":
                ModState.SuperModeGlowColor = Color.OrangeRed;
                ModState.SuperModeOverlayColor = Color.OrangeRed;
                ModState.SuperModeSFX = "brute_rage";
                break;

            case "Poacher":
                ModState.SuperModeGlowColor = Color.MediumPurple;
                ModState.SuperModeOverlayColor = Color.MidnightBlue;
                ModState.SuperModeSFX = "poacher_ambush";
                break;

            case "Desperado":
                ModState.SuperModeGlowColor = Color.DarkGoldenrod;
                ModState.SuperModeOverlayColor = Color.SandyBrown;
                ModState.SuperModeSFX = "desperado_blossom";
                break;

            case "Piper":
                ModState.SuperModeGlowColor = Color.LimeGreen;
                ModState.SuperModeOverlayColor = Color.DarkGreen;
                ModState.SuperModeSFX = "piper_fluidity";
                break;
        }

        ModState.SuperModeGaugeAlpha = 1f;
        ModState.ShouldShakeSuperModeGauge = false;
        ModEntry.Subscriber.SubscribeSuperModeEvents();

        var key = whichSuperMode.ToLower();
        var professionDisplayName = ModEntry.ModHelper.Translation.Get(key + ".name.male");
        var buffName = ModEntry.ModHelper.Translation.Get(key + ".buff");
        ModEntry.Log($"Registered to {professionDisplayName}'s {buffName}.", LogLevel.Info);
    }
}