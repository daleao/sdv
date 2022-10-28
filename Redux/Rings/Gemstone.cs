namespace DaLion.Redux.Rings;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ardalis.SmartEnum;
using DaLion.Redux.Rings.Resonance;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>A gemstone which can be applied to an Infinity Band.</summary>
/// <remarks>
///     Each <see cref="Gemstone"/> vibrates with a characteristic wavelength, which allows it to resonate with
///     others in the <see cref="DiatonicScale"/> of <see cref="Gemstone"/>.
/// </remarks>
public abstract class Gemstone : SmartEnum<Gemstone>, IEquatable<Gemstone>, IComparable<Gemstone>, IGemstone
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
    private static readonly Dictionary<int, Gemstone> FromRingDict;

    /// <summary>The canonical <see cref="DiatonicScale"/> with <see cref="Ruby"/> as the root.</summary>
    private static readonly DiatonicScale RubyScale;

    static Gemstone()
    {
        FromRingDict = new Dictionary<int, Gemstone>();

        Ruby = new RubyGemstone();
        Aquamarine = new AquamarineGemstone();
        Amethyst = new AmethystGemstone();
        Garnet = new GarnetGemstone();
        Emerald = new EmeraldGemstone();
        Jade = new JadeGemstone();
        Topaz = new TopazGemstone();

        RubyScale = new DiatonicScale();
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
        FromRingDict[ringIndex] = this;

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
    public Gemstone Second => RubyScale[(this.Value + 1) % 7];

    /// <summary>Gets the third <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Third => RubyScale[(this.Value + 2) % 7];

    /// <summary>Gets the fourth <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Fourth => RubyScale[(this.Value + 3) % 7];

    /// <summary>Gets the fifth <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Fifth => RubyScale[(this.Value + 4) % 7];

    /// <summary>Gets the sixth <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Sixth => RubyScale[(this.Value + 5) % 7];

    /// <summary>Gets the seventh <see cref="Gemstone"/> in the corresponding <see cref="DiatonicScale"/>.</summary>
    public Gemstone Seventh => RubyScale[(this.Value + 6) % 7];

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

    /// <summary>Get the gemstone associated with the specified ring index.</summary>
    /// <param name="ringIndex">The index of a gemstone ring.</param>
    /// <returns>The <see cref="Gemstone"/> which embedded in the <see cref="StardewValley.Objects.Ring"/> with the specified <paramref name="ringIndex"/>.</returns>
    internal static Gemstone FromRing(int ringIndex)
    {
        return FromRingDict[ringIndex];
    }

    /// <summary>Try to get the gemstone associated with the specified ring index.</summary>
    /// <param name="ringIndex">The index of a gemstone ring.</param>
    /// <param name="gemstone">The matched gemstone, if any.</param>
    /// <returns><see langword="true"/> if a matching gemstone exists, otherwise <see langword="false"/>.</returns>
    internal static bool TryFromRing(int ringIndex, [NotNullWhen(true)] out Gemstone? gemstone)
    {
        return FromRingDict.TryGetValue(ringIndex, out gemstone);
    }

    /// <summary>Get the static gemstone instance with the specified <paramref name="type"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of a <see cref="Gemstone"/>.</param>
    /// <returns>The <see cref="Gemstone"/> whose type matches <paramref name="type"/>, if any, otherwise <see langword="null"/>.</returns>
    internal static Gemstone? FromType(Type type)
    {
        return List.FirstOrDefault(gemstone => gemstone.GetType() == type);
    }

    /// <summary>
    ///     Resonates with the specified <paramref name="amplitude"/>, adding the corresponding stat bonuses to
    ///     <paramref name="who"/>.
    /// </summary>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    /// <param name="amplitude">The resonance amplitude.</param>
    internal abstract void ResonateInRing(Farmer who, float amplitude);

    /// <summary>Removes the corresponding resonance stat bonuses from <paramref name="who"/>.</summary>
    /// <param name="who">The <see cref="Farmer"/>.</param>
    /// <param name="amplitude">The resonance amplitude.</param>
    internal abstract void DissonateInRing(Farmer who, float amplitude);

    /// <summary>Resonates with the specified <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="amplitude">The resonance amplitude.</param>
    internal abstract void ResonateInWeapon(MeleeWeapon weapon, float amplitude);

    /// <summary>Removes the corresponding resonance stat bonuses from the <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="amplitude">The resonance amplitude.</param>
    internal abstract void DissonateInWeapon(MeleeWeapon weapon, float amplitude);

    /// <summary>Resonates with the specified <paramref name="slingshot"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="amplitude">The resonance amplitude.</param>
    /// <remarks>Requires Immersive Rings.</remarks>
    internal abstract void ResonateInSlingshot(Slingshot slingshot, float amplitude);

    /// <summary>Removes the corresponding resonance stat bonuses from the <paramref name="slingshot"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="amplitude">The resonance amplitude.</param>
    /// <remarks>Requires Immersive Rings.</remarks>
    internal abstract void DissonateInSlingshot(Slingshot slingshot, float amplitude);

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
        internal RubyGemstone()
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
        internal override void ResonateInRing(Farmer who, float amplitude)
        {
            who.attackIncreaseModifier += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void DissonateInRing(Farmer who, float amplitude)
        {
            who.attackIncreaseModifier -= 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void ResonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            var array = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\weapons")[weapon.InitialParentTileIndex].Split('/');
            var baseMin = Convert.ToInt32(array[2]);
            var baseMax = Convert.ToInt32(array[3]);
            weapon.minDamage.Value += Math.Max(1, (int)(baseMin * 0.1f * amplitude));
            weapon.maxDamage.Value += Math.Max(1, (int)(baseMax * 0.1f * amplitude));
        }

        /// <inheritdoc />
        internal override void DissonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            var array = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\weapons")[weapon.InitialParentTileIndex].Split('/');
            var baseMin = Convert.ToInt32(array[2]);
            var baseMax = Convert.ToInt32(array[3]);
            weapon.minDamage.Value -= Math.Max(1, (int)(baseMin * 0.1f * amplitude));
            weapon.maxDamage.Value -= Math.Max(1, (int)(baseMax * 0.1f * amplitude));
        }

        /// <inheritdoc />
        internal override void ResonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("RubyResonance", amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("RubyResonance", -amplitude);
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
        internal AquamarineGemstone()
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
        internal override void ResonateInRing(Farmer who, float amplitude)
        {
            who.critChanceModifier += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void DissonateInRing(Farmer who, float amplitude)
        {
            who.critChanceModifier -= 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void ResonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
        }

        /// <inheritdoc />
        internal override void DissonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
        }

        /// <inheritdoc />
        internal override void ResonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("AquamarineResonance", amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("AquamarineResonance", -amplitude);
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
        internal AmethystGemstone()
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
        internal override void ResonateInRing(Farmer who, float amplitude)
        {
            who.knockbackModifier += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void DissonateInRing(Farmer who, float amplitude)
        {
            who.knockbackModifier -= 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void ResonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            weapon.knockback.Value += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void DissonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            weapon.knockback.Value -= 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void ResonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("AmethystResonance", amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("AmethystResonance", -amplitude);
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
        internal GarnetGemstone()
            : base(
                "Garnet",
                3,
                Globals.GarnetIndex,
                Globals.GarnetRingIndex,
                4f / 3f,
                new Color(152, 29, 45))
        {
        }

        /// <inheritdoc />
        internal override void ResonateInRing(Farmer who, float amplitude)
        {
            who.Increment(DataFields.CooldownReduction, amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInRing(Farmer who, float amplitude)
        {
            who.Increment(DataFields.CooldownReduction, -amplitude);
        }

        /// <inheritdoc />
        internal override void ResonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            weapon.Increment("GarnetResonance", amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            weapon.Increment("GarnetResonance", -amplitude);
        }

        /// <inheritdoc />
        internal override void ResonateInSlingshot(Slingshot slingshot, float amplitude)
        {
        }

        /// <inheritdoc />
        internal override void DissonateInSlingshot(Slingshot slingshot, float amplitude)
        {
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
        internal EmeraldGemstone()
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
        internal override void ResonateInRing(Farmer who, float amplitude)
        {
            who.weaponSpeedModifier += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void DissonateInRing(Farmer who, float amplitude)
        {
            who.weaponSpeedModifier -= 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void ResonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            weapon.Increment("EmeraldResonance", amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            weapon.Increment("EmeraldResonance", -amplitude);
        }

        /// <inheritdoc />
        internal override void ResonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("EmeraldResonance", amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("EmeraldResonance", -amplitude);
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
        internal JadeGemstone()
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
        internal override void ResonateInRing(Farmer who, float amplitude)
        {
            who.critPowerModifier += 0.5f * amplitude;
        }

        /// <inheritdoc />
        internal override void DissonateInRing(Farmer who, float amplitude)
        {
            who.critPowerModifier -= 0.5f * amplitude;
        }

        /// <inheritdoc />
        internal override void ResonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
        }

        /// <inheritdoc />
        internal override void DissonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
        }

        /// <inheritdoc />
        internal override void ResonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("JadeResonance", amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("JadeResonance", -amplitude);
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
        internal TopazGemstone()
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
        internal override void ResonateInRing(Farmer who, float amplitude)
        {
            if (ModEntry.Config.Rings.RebalancedRings)
            {
                //who.resilience += amplitude > 1 ? (int)Math.Ceiling(3f * amplitude) : (int)Math.Floor(3f * amplitude);
                who.Increment("TopazResonance", amplitude);
            }
            else
            {
                who.weaponPrecisionModifier += 0.1f * amplitude;
            }
        }

        /// <inheritdoc />
        internal override void DissonateInRing(Farmer who, float amplitude)
        {
            if (ModEntry.Config.Rings.RebalancedRings)
            {
                //who.resilience -= amplitude > 1 ? (int)Math.Ceiling(3f * amplitude) : (int)Math.Floor(3f * amplitude);
                who.Increment("TopazResonance", -amplitude);
            }
            else
            {
                who.weaponPrecisionModifier -= 0.1f * amplitude;
            }
        }

        /// <inheritdoc />
        internal override void ResonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            weapon.Increment("TopazResonance", amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInWeapon(MeleeWeapon weapon, float amplitude)
        {
            weapon.Increment("TopazResonance", -amplitude);
        }

        /// <inheritdoc />
        internal override void ResonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("TopazResonance", amplitude);
        }

        /// <inheritdoc />
        internal override void DissonateInSlingshot(Slingshot slingshot, float amplitude)
        {
            slingshot.Increment("TopazResonance", -amplitude);
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float magnitude = 1f)
        {
            if (ModEntry.Config.Rings.RebalancedRings)
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
