namespace DaLion.Stardew.Rings.Framework;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Ardalis.SmartEnum;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.Extensions.Xna;
using DaLion.Stardew.Rings.Framework.Resonance;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>A gemstone which can be applied to an Infinity Band.</summary>
/// <remarks>
///     Each <see cref="Gemstone"/> vibrates with a characteristic wavelength, which allows it to resonate with
///     others in the <see cref="DiatonicScale"/> of <see cref="Gemstone"/>.
/// </remarks>
public abstract class Gemstone : SmartEnum<Gemstone>, IEquatable<Gemstone>, IComparable<Gemstone>
{
    #region enum entries

    /// <summary>The Ruby gemstone.</summary>
    public static readonly Gemstone Ruby;

    /// <summary>The Aquamarine gemstone.</summary>
    public static readonly Gemstone Aquamarine;

    /// <summary>The Amethyst gemstone.</summary>
    public static readonly Gemstone Amethyst;

    /// <summary>The Garnet gemstone.</summary>
    public static readonly Gemstone Garnet;

    /// <summary>The Emerald gemstone.</summary>
    public static readonly Gemstone Emerald;

    /// <summary>The Jade gemstone.</summary>
    public static readonly Gemstone Jade;

    /// <summary>The Topaz gemstone.</summary>
    public static readonly Gemstone Topaz;

    #endregion enum entries

    /// <summary>Look-up to obtain the corresponding <see cref="Gemstone"/> from a ring index.</summary>
    private static readonly Dictionary<int, Gemstone> _FromRing;

    /// <summary>The canonical <see cref="DiatonicScale"/> with <see cref="Ruby"/> as the root.</summary>
    private static readonly DiatonicScale _RubyScale;

    static Gemstone()
    {
        _FromRing = new Dictionary<int, Gemstone>();

        Ruby = new RubyGemstone();
        Aquamarine = new AquamarineGemstone();
        Amethyst = new AmethystGemstone();
        Garnet = new GarnetGemstone();
        Emerald = new EmeraldGemstone();
        Jade = new JadeGemstone();
        Topaz = new TopazGemstone();

        _RubyScale = new DiatonicScale();
    }

    /// <summary>Initializes a new instance of the <see cref="Gemstone"/> class.</summary>
    /// <param name="name">The gemstone's name.</param>
    /// <param name="value">The gemstone's canonical index in the <see cref="DiatonicScale"/> of <see cref="Ruby"/>.</param>
    /// <param name="objectIndex">The index of the corresponding <see cref="SObject"/>.</param>
    /// <param name="ringIndex">The index of the corresponding <see cref="StardewValley.Objects.Ring"/>.</param>
    /// <param name="frequency">The characteristic wavelength with which the <see cref="Gemstone"/> vibrates.</param>
    /// <param name="color">The characteristic color which results from <see cref="Frequency"/>.</param>
    protected Gemstone(string name, int value, int objectIndex, int ringIndex, float frequency, Color color)
        : base(name, value)
    {
        this.ObjectIndex = objectIndex;
        this.RingIndex = ringIndex;
        _FromRing[ringIndex] = this;

        this.Frequency = frequency;
        this.Color = color;
        this.InverseColor = color.Inverse();
    }

    /// <summary>Gets the index of the corresponding <see cref="SObject"/>.</summary>
    public int ObjectIndex { get; }

    /// <summary>Gets the index of the corresponding <see cref="StardewValley.Objects.Ring"/>.</summary>
    public int RingIndex { get; }

    /// <summary>Gets the characteristic frequency with which the <see cref="Gemstone"/> vibrates.</summary>
    /// <remarks>Measured in units of inverse <see cref="Ruby"/> wavelengths.</remarks>
    public float Frequency { get; }

    /// <summary>Gets the characteristic color which results from <see cref="Frequency"/>.</summary>
    public Color Color { get; }

    /// <summary>Gets the inverse of <see cref="Color"/>.</summary>
    public Color InverseColor { get; }

    /// <summary>Gets the second <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Second => _RubyScale[(this.Value + 1) % 7];

    /// <summary>Gets the third <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Third => _RubyScale[(this.Value + 2) % 7];

    /// <summary>Gets the fourth <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Fourth => _RubyScale[(this.Value + 3) % 7];

    /// <summary>Gets the fifth <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Fifth => _RubyScale[(this.Value + 4) % 7];

    /// <summary>Gets the sixth <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Sixth => _RubyScale[(this.Value + 5) % 7];

    /// <summary>Gets the seventh <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Seventh => _RubyScale[(this.Value + 6) % 7];

    /// <summary>Get the gemstone associated with the specified ring index.</summary>
    /// <param name="ringIndex">The index of a gemstone ring.</param>
    /// <returns>The <see cref="Gemstone"/> which embedded in the <see cref="StardewValley.Objects.Ring"/> with the specified <paramref name="ringIndex"/>.</returns>
    public static Gemstone FromRing(int ringIndex)
    {
        return _FromRing[ringIndex];
    }

    /// <summary>Try to get the gemstone associated with the specified ring index.</summary>
    /// <param name="ringIndex">The index of a gemstone ring.</param>
    /// <param name="gemstone">The matched gemstone, if any.</param>
    /// <returns><see langword="true"/> if a matching gemstone exists, otherwise <see langword="false"/>.</returns>
    public static bool TryFromRing(int ringIndex, [NotNullWhen(true)] out Gemstone? gemstone)
    {
        return _FromRing.TryGetValue(ringIndex, out gemstone);
    }

    /// <summary>
    ///     Gets the ascending diatonic <see cref="HarmonicInterval"/> between this and some other
    ///     <see cref="Gemstone"/>.
    /// </summary>
    /// <param name="other">Some other <see cref="Gemstone"/>.</param>
    /// <returns>The <see cref="IntervalNumber"/> of the <see cref="HarmonicInterval"/> between this and <paramref name="other"/>.</returns>
    public IntervalNumber IntervalWith(Gemstone other)
    {
        return other.Value >= this.Value
            ? (IntervalNumber)(other.Value - this.Value)
            : (IntervalNumber)(7 + other.Value - this.Value);
    }

    /// <summary>
    ///     Resonates with the specified <paramref name="richness"/>, adding the corresponding stat bonuses to
    ///     <paramref name="who"/>.
    /// </summary>
    /// <param name="richness">The resonance richness.</param>
    /// <param name="who">The farmer.</param>
    public abstract void Resonate(float richness, Farmer who);

    /// <summary>Removes the corresponding resonance stat bonuses from <paramref name="who"/>.</summary>
    /// <param name="richness">The resonance richness.</param>
    /// <param name="who">The farmer.</param>
    public abstract void Dissonate(float richness, Farmer who);

    /// <inheritdoc />
    public bool Equals(Gemstone? other)
    {
        return base.Equals(other);
    }

    /// <inheritdoc />
    public int CompareTo(Gemstone? other)
    {
        return base.CompareTo(other);
    }

    /// <summary>Adds the <see cref="Gemstone"/>'s stat bonus to a buffer.</summary>
    /// <param name="buffer">Shared buffer of aggregated stat modifiers.</param>
    /// <param name="magnitude">A multiplier over the base stat modifiers.</param>
    internal abstract void Buffer(StatBuffer buffer, float magnitude = 1f);

    #region implementations

    #region ruby

    /// <inheritdoc cref="Gemstone"/>
    private sealed class RubyGemstone : Gemstone
    {
        /// <summary>Initializes a new instance of the <see cref="Gemstone.RubyGemstone"/> class.</summary>
        public RubyGemstone()
            : base(
                "Ruby",
                0,
                Constants.RubyIndex,
                Constants.RubyRingIndex,
                1f,
                new Color(225, 57, 57))
        {
        }

        /// <inheritdoc />
        public override void Resonate(float richness, Farmer who)
        {
            who.attackIncreaseModifier += 0.1f * richness;
        }

        /// <inheritdoc />
        public override void Dissonate(float richness, Farmer who)
        {
            who.attackIncreaseModifier -= 0.1f * richness;
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float magnitude = 1f)
        {
            buffer.DamageModifier += 0.1f * magnitude;
        }
    }

    #endregion ruby

    #region aquamarine

    /// <inheritdoc cref="Gemstone"/>
    private sealed class AquamarineGemstone : Gemstone
    {
        /// <summary>Initializes a new instance of the <see cref="Gemstone.AquamarineGemstone"/> class.</summary>
        public AquamarineGemstone()
            : base(
                "Aquamarine",
                1,
                Constants.AquamarineIndex,
                Constants.AquamarineRingIndex,
                9f / 8f,
                new Color(35, 144, 170))
        {
        }

        /// <inheritdoc />
        public override void Resonate(float richness, Farmer who)
        {
            who.critChanceModifier += 0.1f * richness;
        }

        /// <inheritdoc />
        public override void Dissonate(float richness, Farmer who)
        {
            who.critChanceModifier -= 0.1f * richness;
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float magnitude = 1f)
        {
            buffer.CritChanceModifier += 0.1f * magnitude;
        }
    }

    #endregion aquamarine

    #region amethyst

    /// <inheritdoc cref="Gemstone"/>
    private sealed class AmethystGemstone : Gemstone
    {
        /// <summary>Initializes a new instance of the <see cref="Gemstone.AmethystGemstone"/> class.</summary>
        public AmethystGemstone()
            : base(
                "Amethyst",
                2,
                Constants.AmethystIndex,
                Constants.AmethystRingIndex,
                32f / 27f,
                new Color(111, 60, 196))
        {
        }

        /// <inheritdoc />
        public override void Resonate(float richness, Farmer who)
        {
            who.knockbackModifier += 0.1f * richness;
        }

        /// <inheritdoc />
        public override void Dissonate(float richness, Farmer who)
        {
            who.knockbackModifier -= 0.1f * richness;
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float magnitude = 1f)
        {
            buffer.KnockbackModifier += 0.1f * magnitude;
        }
    }

    #endregion amethyst

    #region garnet

    /// <inheritdoc cref="Gemstone"/>
    private sealed class GarnetGemstone : Gemstone
    {
        /// <summary>Initializes a new instance of the <see cref="Gemstone.GarnetGemstone"/> class.</summary>
        public GarnetGemstone()
            : base(
                "Garnet",
                3,
                ModEntry.GarnetIndex,
                ModEntry.GarnetRingIndex,
                4f / 3f,
                new Color(152, 29, 45))
        {
        }

        /// <inheritdoc />
        public override void Resonate(float richness, Farmer who)
        {
            who.Increment(DataFields.CooldownReduction, 0.1f * richness);
        }

        /// <inheritdoc />
        public override void Dissonate(float richness, Farmer who)
        {
            who.Increment(DataFields.CooldownReduction, -0.1f * richness);
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float magnitude = 1f)
        {
            buffer.CooldownReduction += 0.1f * magnitude;
        }
    }

    #endregion garnet

    #region emerald

    /// <inheritdoc cref="Gemstone"/>
    private sealed class EmeraldGemstone : Gemstone
    {
        /// <summary>Initializes a new instance of the <see cref="Gemstone.EmeraldGemstone"/> class.</summary>
        public EmeraldGemstone()
            : base(
                "Emerald",
                4,
                Constants.EmeraldIndex,
                Constants.EmeraldRingIndex,
                3f / 2f,
                new Color(4, 128, 54))
        {
        }

        /// <inheritdoc />
        public override void Resonate(float richness, Farmer who)
        {
            who.weaponSpeedModifier += 0.1f * richness;
        }

        /// <inheritdoc />
        public override void Dissonate(float richness, Farmer who)
        {
            who.weaponSpeedModifier -= 0.1f * richness;
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float magnitude = 1f)
        {
            buffer.SwingSpeedModifier += 0.1f * magnitude;
        }
    }

    #endregion emerald

    #region jade

    /// <inheritdoc cref="Gemstone"/>
    private sealed class JadeGemstone : Gemstone
    {
        /// <summary>Initializes a new instance of the <see cref="Gemstone.JadeGemstone"/> class.</summary>
        public JadeGemstone()
            : base(
                "Jade",
                5,
                Constants.JadeIndex,
                Constants.JadeRingIndex,
                27f / 16f,
                new Color(117, 150, 99))
        {
        }

        /// <inheritdoc />
        public override void Resonate(float richness, Farmer who)
        {
            who.critPowerModifier += 0.5f * richness;
        }

        /// <inheritdoc />
        public override void Dissonate(float richness, Farmer who)
        {
            who.critPowerModifier -= 0.5f * richness;
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float magnitude = 1f)
        {
            buffer.CritPowerModifier += 0.5f * magnitude;
        }
    }

    #endregion jade

    #region topaz

    /// <inheritdoc cref="Gemstone"/>
    private sealed class TopazGemstone : Gemstone
    {
        /// <summary>Initializes a new instance of the <see cref="Gemstone.TopazGemstone"/> class.</summary>
        public TopazGemstone()
            : base(
                "Topaz",
                6,
                Constants.TopazIndex,
                Constants.TopazRingIndex,
                16f / 9f,
                new Color(220, 143, 8))
        {
        }

        /// <inheritdoc />
        public override void Resonate(float richness, Farmer who)
        {
            if (ModEntry.Config.RebalancedRings)
            {
                who.resilience += richness > 1 ? (int)Math.Ceiling(3f * richness) : (int)Math.Floor(3f * richness);
            }
            else
            {
                who.weaponPrecisionModifier += 0.1f * richness;
            }
        }

        /// <inheritdoc />
        public override void Dissonate(float richness, Farmer who)
        {
            if (ModEntry.Config.RebalancedRings)
            {
                who.resilience -= richness > 1 ? (int)Math.Ceiling(3f * richness) : (int)Math.Floor(3f * richness);
            }
            else
            {
                who.weaponPrecisionModifier -= 0.1f * richness;
            }
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float magnitude = 1f)
        {
            if (ModEntry.Config.RebalancedRings)
            {
                buffer.AddedDefense += magnitude switch
                {
                    1f => 3,
                    > 1f => (int)Math.Ceiling(3f * magnitude),
                    < 1f => (int)Math.Floor(3f * magnitude),
                    _ => 0,
                };
            }
            else
            {
                buffer.PrecisionModifier += 0.1f * magnitude;
            }
        }
    }

    #endregion topaz

    #endregion implementations
}
