namespace DaLion.Overhaul.Modules.Professions.Configs;

#region using directives

using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>The user-configurable settings for PRFS.</summary>
public sealed class ControlsUiConfig
{
    private float _trackingPointerScale = 1.2f;
    private float _trackingPointerBobRate = 1f;

    /// <summary>Gets mod key used by Prospector and Scavenger professions.</summary>
    [JsonProperty]
    [GMCMPriority(401)]
    public KeybindList ModKey { get; internal set; } = KeybindList.Parse("LeftShift, LeftShoulder");

    /// <summary>Gets the size of the pointer used to track objects by Prospector and Scavenger professions.</summary>
    [JsonProperty]
    [GMCMPriority(402)]
    [GMCMRange(0.2f, 5f)]
    [GMCMInterval(0.2f)]
    public float TrackingPointerScale
    {
        get => this._trackingPointerScale;
        internal set
        {
            this._trackingPointerScale = value;
            if (HudPointer.Instance.IsValueCreated)
            {
                HudPointer.Instance.Value.Scale = value;
            }
        }
    }

    /// <summary>Gets the speed at which the tracking pointer bounces up and down (higher is faster).</summary>
    [JsonProperty]
    [GMCMPriority(403)]
    [GMCMRange(0.5f, 2f)]
    [GMCMInterval(0.05f)]
    public float TrackingPointerBobRate
    {
        get => this._trackingPointerBobRate;
        internal set
        {
            this._trackingPointerBobRate = value;
            if (HudPointer.Instance.IsValueCreated)
            {
                HudPointer.Instance.Value.BobRate = value;
            }
        }
    }

    /// <summary>Gets a value indicating whether Prospector and Scavenger will only track off-screen object while <see cref="ModKey"/> is held.</summary>
    [JsonProperty]
    [GMCMPriority(404)]
    public bool DisableAlwaysTrack { get; internal set; } = false;

    /// <summary>Gets a value indicating whether to restore the legacy purple arrow for Prospector Hunts, instead of the new audio cues.</summary>
    [JsonProperty]
    [GMCMPriority(405)]
    public bool UseLegacyProspectorHunt { get; internal set; } = false;

    /// <summary>
    ///     Gets a value indicating whether to display the MAX icon below fish in the Collections Menu which have been caught at the
    ///     maximum size.
    /// </summary>
    [JsonProperty]
    [GMCMPriority(406)]
    public bool ShowFishCollectionMaxIcon { get; internal set; } = true;
}
