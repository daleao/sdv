namespace DaLion.Stardew.Rings.Framework;

#region using directives

using Ardalis.SmartEnum;
using Common.Extensions.Stardew;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#endregion using directives

public class Resonance : SmartEnum<Resonance>
{
    #region enum entries

    public static readonly Resonance Amethyst = new("Amethyst", Constants.AMETHYST_RING_INDEX_I);
    public static readonly Resonance Topaz = new("Topaz", Constants.TOPAZ_RING_INDEX_I);
    public static readonly Resonance Aquamarine = new("Aquamarine", Constants.AQUAMARINE_RING_INDEX_I);
    public static readonly Resonance Jade = new("Jade", Constants.JADE_RING_INDEX_I);
    public static readonly Resonance Emerald = new("Emerald", Constants.EMERALD_RING_INDEX_I);
    public static readonly Resonance Ruby = new("Ruby", Constants.RUBY_RING_INDEX_I);
    public static readonly Resonance Garnet = new("Garnet", ModEntry.GarnetRingIndex);

    #endregion enum entries

    #region look-up tables

    /// <summary>Gemstone pairs that produce a constructive resonance.</summary>
    private static readonly IReadOnlyDictionary<Resonance, Resonance> _pairs = new Dictionary<Resonance, Resonance>()
    {
        { Amethyst, Topaz },
        { Topaz, Amethyst },
        { Aquamarine, Jade },
        { Jade, Aquamarine },
        { Emerald, Ruby },
        { Ruby, Emerald },
        { Garnet, Garnet }
    };

    /// <summary>Gemstone pairs that produce a dampening effect.</summary>
    private static readonly IReadOnlyDictionary<Resonance, Resonance> _antipairs = new Dictionary<Resonance, Resonance>()
    {
        { Amethyst, Jade },
        { Topaz, Emerald },
        { Aquamarine, Ruby },
        { Jade, Amethyst },
        { Emerald, Topaz },
        { Ruby, Aquamarine }
    };

    #endregion look-up tables

    private int? _lightSourceId;

    /// <summary>Construct an instance.</summary>
    /// <param name="name">The gemstone name.</param>
    /// <param name="value">The gemstone ring index.</param>
    public Resonance(string name, int value) : base(name, value)
    {
        DisplayName = ModEntry.i18n.Get("resonance." + name.ToLowerInvariant());
    }

    /// <summary>Get the localized name for this resonance.</summary>
    public string DisplayName { get; }

    /// <summary>Get the resonance intensity.</summary>
    public int Magnitude { get; set; }

    /// <summary>Get the corresponding gemstone color.</summary>
    public Color Color => Utils.ColorByGemstone[Utils.GemstoneByRing[Value]];

    /// <summary>Apply resonance's effect to the farmer.</summary>
    /// <param name="who">The farmer.</param>
    public void OnEquip(Farmer who)
    {
        var location = who.currentLocation;
        if (_lightSourceId.HasValue)
        {
            location.removeLightSource(_lightSourceId.Value);
            _lightSourceId = null;
        }

        _lightSourceId = ModEntry.Manifest.UniqueID.GetHashCode() + (int)who.UniqueMultiplayerID + Value;
        while (location.sharedLights.ContainsKey(_lightSourceId!.Value)) ++_lightSourceId;

        location.sharedLights[_lightSourceId.Value] = new(1, new(who.Position.X + 21f, who.Position.Y + 64f),
            2.5f * Magnitude, Color, _lightSourceId.Value, LightSource.LightContext.None, who.UniqueMultiplayerID);

        switch (Value)
        {
            case Constants.RUBY_RING_INDEX_I:
                who.attackIncreaseModifier += 0.01f * Magnitude;
                break;
            case Constants.AQUAMARINE_RING_INDEX_I:
                who.critChanceModifier += 0.01f * Magnitude;
                break;
            case Constants.JADE_RING_INDEX_I:
                who.critPowerModifier += 0.05f * Magnitude;
                break;
            case Constants.EMERALD_RING_INDEX_I:
                who.weaponSpeedModifier += 0.01f * Magnitude;
                break;
            case Constants.AMETHYST_RING_INDEX_I:
                who.knockbackModifier += 0.01f * Magnitude;
                break;
            case Constants.TOPAZ_RING_INDEX_I:
                if (ModEntry.Config.RebalancedRings)
                {
                    who.resilience += (int)(0.5 * Magnitude);
                    who.immunity += Magnitude;
                }
                else
                {
                    who.weaponPrecisionModifier += 0.01f * Magnitude;
                }
                break;
            default:
                if (Value == ModEntry.GarnetRingIndex)
                    who.Increment("CooldownReduction", 0.01f * Magnitude);

                break;
        }
    }

    /// <summary>Remove resonance's effect from the farmer.</summary>
    /// <param name="who">The farmer.</param>
    public void OnUnequip(Farmer who)
    {
        switch (Value)
        {
            case Constants.RUBY_RING_INDEX_I:
                who.attackIncreaseModifier -= 0.01f * Magnitude;
                break;
            case Constants.AQUAMARINE_RING_INDEX_I:
                who.critChanceModifier -= 0.01f * Magnitude;
                break;
            case Constants.JADE_RING_INDEX_I:
                who.critPowerModifier -= 0.05f * Magnitude;
                break;
            case Constants.EMERALD_RING_INDEX_I:
                who.weaponSpeedModifier -= 0.01f * Magnitude;
                break;
            case Constants.AMETHYST_RING_INDEX_I:
                who.knockbackModifier -= 0.01f;
                break;
            case Constants.TOPAZ_RING_INDEX_I:
                if (ModEntry.Config.RebalancedRings)
                {
                    who.resilience -= (int)(0.5 * Magnitude);
                    who.immunity -= Magnitude;
                }
                else
                {
                    who.weaponPrecisionModifier -= 0.01f * Magnitude;
                }
                break;
            default:
                if (Value == ModEntry.GarnetRingIndex)
                    who.Increment("CooldownReduction", -0.01f * Magnitude);

                break;
        }
    }

    /// <summary>Get the corresponding resonant pair.</summary>
    public Resonance GetPair() => _pairs[this];

    /// <summary>Get the corresponding dampening pair.</summary>
    public Resonance? GetAntipair() => _antipairs.TryGetValue(this, out var anti) ? anti : null;

    public static Resonance operator +(Resonance resonance, int magnitude)
    {
        resonance.Magnitude += magnitude;
        return resonance;
    }

    public static Resonance operator ++(Resonance resonance)
    {
        ++resonance.Magnitude;
        return resonance;
    }

    public static Resonance operator -(Resonance resonance, int magnitude)
    {
        resonance.Magnitude -= magnitude;
        return resonance;
    }

    public static Resonance operator --(Resonance resonance)
    {
        --resonance.Magnitude;
        return resonance;
    }
}