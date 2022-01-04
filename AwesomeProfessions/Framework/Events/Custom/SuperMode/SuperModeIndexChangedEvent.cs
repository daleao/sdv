using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;

namespace TheLion.Stardew.Professions.Framework.Events.Custom.SuperMode;

public delegate void SuperModeIndexChangedEventHandler(int newIndex);

internal class SuperModeIndexChangedEvent : BaseEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public override void Hook()
    {
        ModEntry.State.Value.SuperModeIndexChanged += OnSuperModeIndexChanged;
    }

    /// <summary>Unhook this event from the event listener.</summary>
    public override void Unhook()
    {
        ModEntry.State.Value.SuperModeIndexChanged -= OnSuperModeIndexChanged;
    }

    /// <summary>Raised when SuperModeIndex is set to a new value.</summary>
    public void OnSuperModeIndexChanged(int newIndex)
    {
        ModEntry.Subscriber.UnsubscribeSuperModeEvents();
        ModEntry.State.Value.SuperModeGaugeValue = 0;

        if (newIndex is > 0 and (< 26 or >= 30))
            throw new ArgumentException($"Unexpected Super Mode index {newIndex}.");

        ModEntry.Data.Value.Write("SuperModeIndex", ModEntry.State.Value.SuperModeIndex.ToString());
        if (ModEntry.State.Value.SuperModeIndex < 0)
        {
            ModEntry.Log("Unregistered Super Mode.", LogLevel.Info);
            return;
        }

        var whichSuperMode = Utility.Professions.NameOf(newIndex);
        switch (whichSuperMode)
        {
            case "Brute":
                ModEntry.State.Value.SuperModeGlowColor = Color.OrangeRed;
                ModEntry.State.Value.SuperModeOverlayColor = Color.OrangeRed;
                ModEntry.State.Value.SuperModeSFX = "brute_rage";
                break;

            case "Poacher":
                ModEntry.State.Value.SuperModeGlowColor = Color.MediumPurple;
                ModEntry.State.Value.SuperModeOverlayColor = Color.MidnightBlue;
                ModEntry.State.Value.SuperModeSFX = "poacher_ambush";
                break;

            case "Desperado":
                ModEntry.State.Value.SuperModeGlowColor = Color.DarkGoldenrod;
                ModEntry.State.Value.SuperModeOverlayColor = Color.SandyBrown;
                ModEntry.State.Value.SuperModeSFX = "desperado_blossom";
                break;

            case "Piper":
                ModEntry.State.Value.SuperModeGlowColor = Color.LimeGreen;
                ModEntry.State.Value.SuperModeOverlayColor = Color.DarkGreen;
                ModEntry.State.Value.SuperModeSFX = "piper_fluidity";
                break;
        }

        ModEntry.State.Value.SuperModeGaugeAlpha = 1f;
        ModEntry.State.Value.ShouldShakeSuperModeGauge = false;
        ModEntry.Subscriber.SubscribeSuperModeEvents();

        var key = whichSuperMode.ToLower();
        var professionDisplayName = ModEntry.ModHelper.Translation.Get(key + ".name.male");
        var buffName = ModEntry.ModHelper.Translation.Get(key + ".buff");
        ModEntry.Log($"Registered to {professionDisplayName}'s {buffName}.", LogLevel.Info);
    }
}