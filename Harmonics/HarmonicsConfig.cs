namespace DaLion.Harmonics;

#region using directives

using DaLion.Harmonics.Framework;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Config schema for the Ponds mod.</summary>
public sealed class HarmonicsConfig
{
    #region dropdown enums

    /// <summary>The texture that should be used as the resonance light source.</summary>
    public enum LightsourceTexture
    {
        /// <summary>The default, Vanilla sconce light texture.</summary>
        Sconce = 4,

        /// <summary>A more opaque sconce light texture.</summary>
        Stronger = 100,

        /// <summary>A floral-patterned light texture.</summary>
        Patterned = 101,
    }

    #endregion dropdown enums

    /// <summary>Gets a value indicating whether to add new combat recipes for crafting gemstone rings.</summary>
    [JsonProperty]
    [GMCMPriority(0)]
    public bool CraftableGemstoneRings
    {
        get;
        internal set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
        }
    } = true;

    /// <summary>Gets a value indicating whether gemstone frequencies can be heard.</summary>
    [JsonProperty]
    [GMCMPriority(1)]
    public bool AudibleGemstones
    {
        get => field && Game1.options.soundVolumeLevel > 0f;
        internal set;
    } = true;

    /// <summary>Gets the duration of the chord sound cue, in milliseconds.</summary>
    [JsonProperty]
    [GMCMPriority(2)]
    [GMCMRange(500, 2500, 100)]
    public uint ChordSoundDuration
    {
        get;
        internal set
        {
            field = value;
            if (Context.IsWorldReady)
            {
                Chord.RecalculateLinSpace();
            }
        }
    } = 1000;

    /// <summary>Gets a value indicating whether the resonance glow should inherit the root note's color.</summary>
    [JsonProperty]
    [GMCMPriority(3)]
    public bool ColorfulResonances
    {
        get;
        internal set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            if (Context.IsWorldReady)
            {
                State.ResonantChords.Values.ForEach(chord => chord.ResetLightSource());
            }
        }
    } = true;

    /// <summary>Gets a value indicating the texture that should be used as the resonance light source.</summary>
    [JsonProperty]
    [GMCMPriority(4)]
    public LightsourceTexture ResonanceLightsourceTexture
    {
        get;
        internal set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            if (Context.IsWorldReady)
            {
                State.ResonantChords.Values.ForEach(chord => chord.ResetLightSource());
            }
        }
    } = LightsourceTexture.Patterned;
}
