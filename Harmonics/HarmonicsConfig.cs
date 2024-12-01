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
    private bool _craftableGemstoneRings = true;
    private bool _audibleGemstones = true;
    private uint _chordSoundDuration = 1000;
    private bool _colorfulResonances = true;
    private LightsourceTexture _resonanceLightsourceTexture = LightsourceTexture.Patterned;

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
        get => this._craftableGemstoneRings;
        internal set
        {
            if (value == this._craftableGemstoneRings)
            {
                return;
            }

            this._craftableGemstoneRings = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
        }
    }

    /// <summary>Gets a value indicating whether gemstone frequencies can be heard.</summary>
    [JsonProperty]
    [GMCMPriority(1)]
    public bool AudibleGemstones
    {
        get => this._audibleGemstones && Game1.options.soundVolumeLevel > 0f;
        internal set => this._audibleGemstones = value;
    }

    /// <summary>Gets the duration of the chord sound cue, in milliseconds.</summary>
    [JsonProperty]
    [GMCMPriority(2)]
    [GMCMRange(500, 2500, 100)]
    public uint ChordSoundDuration
    {
        get => this._chordSoundDuration;
        internal set
        {
            this._chordSoundDuration = value;
            if (Context.IsWorldReady)
            {
                Chord.RecalculateLinSpace();
            }
        }
    }

    /// <summary>Gets a value indicating whether the resonance glow should inherit the root note's color.</summary>
    [JsonProperty]
    [GMCMPriority(3)]
    public bool ColorfulResonances
    {
        get => this._colorfulResonances;
        internal set
        {
            if (value == this._colorfulResonances)
            {
                return;
            }

            this._colorfulResonances = value;
            if (Context.IsWorldReady)
            {
                State.ResonantChords.Values.ForEach(chord => chord.ResetLightSource());
            }
        }
    }

    /// <summary>Gets a value indicating the texture that should be used as the resonance light source.</summary>
    [JsonProperty]
    [GMCMPriority(4)]
    public LightsourceTexture ResonanceLightsourceTexture
    {
        get => this._resonanceLightsourceTexture;
        internal set
        {
            if (value == this._resonanceLightsourceTexture)
            {
                return;
            }

            this._resonanceLightsourceTexture = value;
            if (Context.IsWorldReady)
            {
                State.ResonantChords.Values.ForEach(chord => chord.ResetLightSource());
            }
        }
    }
}
