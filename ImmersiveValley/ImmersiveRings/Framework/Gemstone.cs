namespace DaLion.Stardew.Rings.Framework;

#region using directives

using Ardalis.SmartEnum;
using Common.Exceptions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#endregion using directives

internal class Gemstone : SmartEnum<Gemstone>
{
    private static readonly Dictionary<int, Gemstone> _fromRing = new();

    #region enum entries

    public static readonly Gemstone Amethyst = new("Amethyst Gemstone", Constants.AMETHYST_INDEX_I);
    public static readonly Gemstone Topaz = new("Topaz Gemstone", Constants.TOPAZ_INDEX_I);
    public static readonly Gemstone Aquamarine = new("Aquamarine Gemstone", Constants.AQUAMARINE_INDEX_I);
    public static readonly Gemstone Jade = new("Jade Gemstone", Constants.JADE_INDEX_I);
    public static readonly Gemstone Emerald = new("Emerald Gemstone", Constants.EMERALD_INDEX_I);
    public static readonly Gemstone Ruby = new("Ruby Gemstone", Constants.RUBY_INDEX_I);
    public static readonly Gemstone Garnet = new("Garnet Gemstone", ModEntry.GarnetIndex);

    #endregion enum entries

    /// <summary>Construct an instance.</summary>
    /// <param name="name">The gemstone name.</param>
    /// <param name="value">The gemstone index.</param>
    public Gemstone(string name, int value) : base(name, value)
    {
        switch (value)
        {
            case Constants.AMETHYST_INDEX_I:
                RingIndex = Constants.AMETHYST_RING_INDEX_I;
                Color = new(111, 60, 196);
                Resonance = Resonance.Amethyst;
                break;
            case Constants.TOPAZ_INDEX_I:
                RingIndex = Constants.TOPAZ_RING_INDEX_I;
                Color = new(220, 143, 8);
                Resonance = Resonance.Topaz;
                break;
            case Constants.AQUAMARINE_INDEX_I:
                RingIndex = Constants.AQUAMARINE_RING_INDEX_I;
                Color = new(35, 144, 170);
                Resonance = Resonance.Aquamarine;
                break;
            case Constants.JADE_INDEX_I:
                RingIndex = Constants.JADE_RING_INDEX_I;
                Color = new(117, 150, 99);
                Resonance = Resonance.Jade;
                break;
            case Constants.EMERALD_INDEX_I:
                RingIndex = Constants.EMERALD_RING_INDEX_I;
                Color = new(4, 128, 54);
                Resonance = Resonance.Emerald;
                break;
            case Constants.RUBY_INDEX_I:
                RingIndex = Constants.RUBY_RING_INDEX_I;
                Color = new(225, 57, 57);
                Resonance = Resonance.Ruby;
                break;
            default:
                if (value == ModEntry.GarnetIndex)
                {
                    RingIndex = ModEntry.GarnetRingIndex;
                    Color = new(152, 29, 45);
                    Resonance = Resonance.Garnet;
                }
                else
                {
                    ThrowHelperExtensions.ThrowUnexpectedEnumValueException<Gemstone, int>(value);
                }
                break;
        }

        _fromRing.Add(RingIndex, this);
    }

    /// <summary>The index of the corresponding ring.</summary>
    public int RingIndex { get; }

    /// <summary>The color of the gemstone.</summary>
    public Color Color { get; }

    /// <summary>The corresponding <see cref="Resonance"/>.</summary>
    public Resonance Resonance { get; }

    /// <summary>Apply stat modifier to a shared aggregate.</summary>
    /// <param name="modifiers">Rolling aggregate of stat modifiers.</param>
    public void ApplyModifier(ref StatModifiers modifiers)
    {
        switch (Value)
        {
            case Constants.RUBY_INDEX_I:
                modifiers.DamageModifier += 0.1f;
                break;
            case Constants.AQUAMARINE_INDEX_I:
                modifiers.CritChanceModifier += 0.1f;
                break;
            case Constants.JADE_INDEX_I:
                modifiers.CritPowerModifier += 0.5f;
                break;
            case Constants.EMERALD_INDEX_I:
                modifiers.SwingSpeedModifier += 0.1f;
                break;
            case Constants.AMETHYST_INDEX_I:
                modifiers.KnockbackModifier += 0.1f;
                break;
            case Constants.TOPAZ_INDEX_I:
                if (ModEntry.Config.RebalancedRings) modifiers.AddedDefense += 3;
                else modifiers.PrecisionModifier += 0.1f;
                break;
            default:
                if (Value != ModEntry.GarnetIndex) break;

                modifiers.CooldownReduction += 0.1f;
                break;
        }
    }

    /// <summary>Get the gemstone associated with the specified ring index.</summary>
    /// <param name="ringIndex">The index of a gemstone ring.</param>
    public static Gemstone FromRing(int ringIndex) => _fromRing[ringIndex];

    /// <summary>Try to get the gemstone associated with the specified ring index.</summary>
    /// <param name="ringIndex">The index of a gemstone ring.</param>
    /// <param name="gemstone">The matched gemstone, if any.</param>
    /// <returns><see langword="true"/> if a matching gemstone exists, otherwise <see langword="false"/>.</returns>
    public static bool TryFromRing(int ringIndex, [NotNullWhen(true)] out Gemstone? gemstone) => _fromRing.TryGetValue(ringIndex, out gemstone);
}