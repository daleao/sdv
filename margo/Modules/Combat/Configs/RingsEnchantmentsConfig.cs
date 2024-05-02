namespace DaLion.Overhaul.Modules.Combat.Configs;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for CMBT.</summary>
public sealed class RingsEnchantmentsConfig
{
    private bool _rebalancedRings = true;
    private bool _craftableGemstoneRings = true;
    private bool _enableInfinityBand = true;
    private bool _audibleGemstones = true;
    private uint _chordSoundDuration = 1000;
    private bool _colorfulResonances = true;
    private LightsourceTexture _resonanceLightsourceTexture = LightsourceTexture.Patterned;
    private bool _newPrismaticEnchantments = true;

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

    /// <summary>Gets a value indicating whether to improve certain underwhelming rings.</summary>
    [JsonProperty]
    [GMCMPriority(200)]
    public bool RebalancedRings
    {
        get => this._rebalancedRings;
        internal set
        {
            if (value == this._rebalancedRings)
            {
                return;
            }

            this._rebalancedRings = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
        }
    }

    /// <summary>Gets a value indicating whether to add new combat recipes for crafting gemstone rings.</summary>
    [JsonProperty]
    [GMCMPriority(201)]
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

    /// <summary>Gets a value indicating whether to replace the Iridium Band recipe and effect.</summary>
    [JsonProperty]
    [GMCMPriority(202)]
    public bool EnableInfinityBand
    {
        get => this._enableInfinityBand;
        internal set
        {
            if (value == this._enableInfinityBand)
            {
                return;
            }

            this._enableInfinityBand = value;
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
            ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
        }
    }

    /// <summary>Gets a value indicating whether to allow gemstone resonance to take place.</summary>
    [JsonProperty]
    [GMCMPriority(203)]
    public bool EnableGemstoneResonance { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to allow gemstone resonance to take place.</summary>
    [JsonProperty]
    [GMCMPriority(204)]
    public bool AudibleGemstones
    {
        get => this._audibleGemstones && Game1.options.soundVolumeLevel > 0f;
        internal set => this._audibleGemstones = value;
    }

    /// <summary>Gets a value indicating whether to allow gemstone resonance to take place.</summary>
    [JsonProperty]
    [GMCMPriority(205)]
    [GMCMRange(500, 2500)]
    [GMCMInterval(100)]
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
    [GMCMPriority(206)]
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
                Game1.player.Get_ResonatingChords().ForEach(chord => chord.ResetLightSource());
            }
        }
    }

    /// <summary>Gets a value indicating the texture that should be used as the resonance light source.</summary>
    [JsonProperty]
    [GMCMPriority(207)]
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
                Game1.player.Get_ResonatingChords().ForEach(chord => chord.ResetLightSource());
            }
        }
    }

    /// <summary>Gets a value indicating whether to improve certain underwhelming gemstone effects.</summary>
    [JsonProperty]
    [GMCMPriority(208)]
    public bool RebalancedGemstones { get; internal set; } = true;

    /// <summary>Gets a value indicating whether to replace vanilla weapon enchantments with all-new melee and ranged enchantments.</summary>
    [JsonProperty]
    [GMCMPriority(209)]
    public bool NewPrismaticEnchantments
    {
        get => this._newPrismaticEnchantments;
        internal set
        {
            if (value == this._newPrismaticEnchantments)
            {
                return;
            }

            this._newPrismaticEnchantments = value;
            if (Context.IsWorldReady)
            {
                Reflector.GetStaticFieldSetter<List<BaseEnchantment>?>(typeof(BaseEnchantment), "_enchantments")
                    .Invoke(null);
            }
        }
    }
}
