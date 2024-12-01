namespace DaLion.Harmonics.Framework;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ardalis.SmartEnum;
using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Enchantments;
using StardewValley.Objects;
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

    /// <summary>Look-up to obtain the corresponding <see cref="Gemstone"/> from a <see cref="SObject"/> index.</summary>
    private static readonly Dictionary<string, Gemstone> FromObjectDict;

    /// <summary>Look-up to obtain the corresponding <see cref="Gemstone"/> from a <see cref="Ring"/> index.</summary>
    private static readonly Dictionary<string, Gemstone> FromRingDict;

    /// <summary>The canonical <see cref="DiatonicScale"/> with <see cref="Ruby"/> as the root.</summary>
    private static readonly DiatonicScale RubyScale;

    private static readonly List<double> VolumeSpace =
        MathUtils.LinSpace(0d, 4d, 100).Select(x => Math.Exp(x - 5d)).ToList();

    private static readonly List<double> SineSpace =
        MathUtils.LinSpace(0d, 2 * Math.PI, 60).Select(Math.Sin).ToList();

    private static int _fadeStepIndex;

    private static int _modulationStepIndex;

    static Gemstone()
    {
        FromObjectDict = [];
        FromRingDict = [];

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
    /// <param name="objectId">The qualified ID of the corresponding <see cref="SObject"/>.</param>
    /// <param name="ringId">The qualified ID of the corresponding <see cref="Ring"/>.</param>
    /// <param name="glowFrequency">The characteristic wavelength with which the <see cref="Gemstone"/> vibrates.</param>
    /// <param name="stoneColor">The characteristic color of the stone itself.</param>
    /// <param name="glowColor">The characteristic glow of the emitted lightsource.</param>
    protected Gemstone(
        string name,
        int value,
        string objectId,
        string ringId,
        float glowFrequency,
        Color stoneColor,
        Color glowColor)
        : base(name, value)
    {
        this.ObjectId = objectId;
        FromObjectDict[objectId] = this;

        this.RingId = ringId;
        FromRingDict[ringId] = this;

        this.DisplayName = _I18n.Get("ui." + name.ToLower());
        this.GlowFrequency = glowFrequency;
        this.StoneColor = stoneColor;
        this.GlowColor = glowColor.Inverse();
        this.TextColor = this.StoneColor.ChangeValue(0.8f);

        this.NaturalPitch = this.Harmonics[this];
        for (var i = 0; i < 7; i++)
        {
            this.Harmonics[i] += this.NaturalPitch;
        }
    }

    /// <summary>Gets the localized name of the <see cref="Gemstone"/>.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the index of the corresponding <see cref="SObject"/>.</summary>
    public string ObjectId { get; }

    /// <summary>Gets the index of the corresponding <see cref="Ring"/>.</summary>
    public string RingId { get; }

    /// <summary>Gets the pitch adjustment to the game's 440 Hz sine wave in order to produce the natural frequency for this <see cref="Gemstone"/>.</summary>
    public int NaturalPitch { get; }

    /// <summary>Gets the pitch adjustments for every note in the corresponding <see cref="DiatonicScale"/>.</summary>
    public int[] Harmonics { get; } = { 0, 200, 400, 500, 700, 900, 1100 };

    /// <summary>Gets the <see cref="ICue"/> of the <see cref="Gemstone"/>'s vibration.</summary>
    public ICue Cue { get; } = Game1.soundBank.GetCue("SinWave");

    /// <summary>Gets the characteristic frequency with which the <see cref="Gemstone"/> vibrates.</summary>
    /// <remarks>Measured in units of inverse <see cref="Ruby"/> wavelengths.</remarks>
    public float GlowFrequency { get; }

    /// <summary>Gets the characteristic color which results from <see cref="GlowFrequency"/>.</summary>
    public Color StoneColor { get; }

    /// <summary>Gets the inverse of <see cref="StoneColor"/>.</summary>
    public Color GlowColor { get; }

    /// <summary>Gets the color used to render text. A slightly darker tone of <see cref="StoneColor"/>.</summary>
    public Color TextColor { get; }

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

    /// <summary>Gets the corresponding <see cref="BaseWeaponEnchantment"/> type.</summary>
    public abstract Type EnchantmentType { get; }

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

    /// <summary>Gets the gemstone associated with the specified object index.</summary>
    /// <param name="objectId">The qualified ID of a gemstone object.</param>
    /// <returns>The <see cref="Gemstone"/> which embedded in the <see cref="SObject"/> with the specified <paramref name="objectId"/>.</returns>
    internal static Gemstone FromObject(string objectId)
    {
        return FromObjectDict[objectId];
    }

    /// <summary>Try to get the gemstone associated with the specified object index.</summary>
    /// <param name="objectId">The qualified ID of a gemstone object.</param>
    /// <param name="gemstone">The matched gemstone, if any.</param>
    /// <returns><see langword="true"/> if a matching gemstone exists, otherwise <see langword="false"/>.</returns>
    internal static bool TryFromObject(string objectId, [NotNullWhen(true)] out Gemstone? gemstone)
    {
        return FromObjectDict.TryGetValue(objectId, out gemstone);
    }

    /// <summary>Gets the gemstone associated with the specified ring index.</summary>
    /// <param name="ringId">The qualified ID of a gemstone ring.</param>
    /// <returns>The <see cref="Gemstone"/> which embedded in the <see cref="Ring"/> with the specified <paramref name="ringId"/>.</returns>
    internal static Gemstone FromRing(string ringId)
    {
        return FromRingDict[ringId];
    }

    /// <summary>Try to get the gemstone associated with the specified ring index.</summary>
    /// <param name="ringId">The qualified ID of a gemstone ring.</param>
    /// <param name="gemstone">The matched gemstone, if any.</param>
    /// <returns><see langword="true"/> if a matching gemstone exists, otherwise <see langword="false"/>.</returns>
    internal static bool TryFromRing(string ringId, [NotNullWhen(true)] out Gemstone? gemstone)
    {
        return FromRingDict.TryGetValue(ringId, out gemstone);
    }

    /// <summary>Gets the static gemstone instance with the specified <paramref name="type"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of a <see cref="Gemstone"/>.</param>
    /// <returns>The <see cref="Gemstone"/> whose type matches <paramref name="type"/>, if any, otherwise <see langword="null"/>.</returns>
    internal static Gemstone? FromType(Type type)
    {
        return List.FirstOrDefault(gemstone => gemstone.GetType() == type);
    }

    /// <summary>
    ///     Resonates with the specified <paramref name="amplitude"/>, adding the corresponding stat bonuses to a
    ///     <see cref="BuffEffects"/> object.
    /// </summary>
    /// <param name="effects">The <see cref="BuffEffects"/>.</param>
    /// <param name="amplitude">The resonance amplitude.</param>
    internal abstract void Resonate(BuffEffects effects, float amplitude);

    /// <summary>
    ///     Resonates with a forge, adding the corresponding stat bonuses to the
    ///     <see cref="weapon"/>.
    /// </summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="forge">The forge as <see cref="BaseWeaponEnchantment"/>.</param>
    internal abstract void Resonate(MeleeWeapon weapon, BaseWeaponEnchantment forge);

    /// <summary>
    ///     Quenches a forge resonance, removing the corresponding stat bonuses from the
    ///     <see cref="weapon"/>.
    /// </summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="forge">The forge as <see cref="BaseWeaponEnchantment"/>.</param>
    internal abstract void Quench(MeleeWeapon weapon, BaseWeaponEnchantment forge);

    /// <summary>Adds the <see cref="Gemstone"/>'s corresponding stat bonus to the specified <see cref="StatBuffer"/>.</summary>
    /// <param name="buffer">Shared buffer of aggregated stat modifiers.</param>
    /// <param name="multiplier">Optional value to multiply the default stat bonus.</param>
    internal abstract void Buffer(StatBuffer buffer, float multiplier = 1f);

    /// <summary>Begins playback of the sine wave <see cref="ICue"/> for this <see cref="Gemstone"/>.</summary>
    internal void PlayCue()
    {
        if (this.Cue.IsPlaying)
        {
            return;
        }

        this.Cue.SetVariable("Pitch", this.Harmonics[0]);
        this.Cue.Play();
        try
        {
            if (!this.Cue.IsPitchBeingControlledByRPC)
            {
                this.Cue.Pitch = Utility.Lerp(-1f, 1f, this.Harmonics[0] / 2400f);
            }
        }
        catch (Exception)
        {
            // ignored
        }

        this.Cue.Volume = 0f;
    }

    /// <summary>Ceases playback of the sine wave <see cref="ICue"/> for this <see cref="Gemstone"/>.</summary>
    internal void StopCue()
    {
        if (!this.Cue.IsPlaying)
        {
            return;
        }

        this.Cue.Stop(AudioStopOptions.Immediate);
        _fadeStepIndex = 0;
    }

    /// <summary>Fades in the sine wave <see cref="ICue"/> volume.</summary>
    internal void FadeIn()
    {
        if (++_fadeStepIndex < VolumeSpace.Count)
        {
            this.Cue.Volume = (float)VolumeSpace[_fadeStepIndex];
        }
    }

    /// <summary>Modulates the sine wave <see cref="ICue"/> pitch.</summary>
    internal void Modulate()
    {
        if (++_modulationStepIndex >= SineSpace.Count)
        {
            _modulationStepIndex = 0;
        }

        this.Cue.SetVariable("Pitch", this.Harmonics[0] + (int)(SineSpace[_modulationStepIndex] * 10d));
        try
        {
            if (!this.Cue.IsPitchBeingControlledByRPC)
            {
                this.Cue.Pitch = Utility.Lerp(
                    -1f,
                    1f,
                    (this.Harmonics[0] / 2400f) + (float)(SineSpace[_modulationStepIndex] * 10d));
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

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
                QualifiedObjectIds.Ruby,
                QualifiedObjectIds.RubyRing,
                1f,
                new Color(225, 57, 57),
                new Color(245, 75, 20, 230))
        {
        }

        /// <inheritdoc />
        public override Type EnchantmentType => typeof(RubyEnchantment);

        /// <inheritdoc />
        internal override void Resonate(BuffEffects effects, float amplitude)
        {
            effects.AttackMultiplier.Value += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void Resonate(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            var data = weapon.GetData();
            if (data == null)
            {
                return;
            }

            var baseMin = data.MinDamage;
            var baseMax = data.MaxDamage;
            weapon.minDamage.Value += Math.Max(1, (int)(baseMin * 0.05f)) * forge.GetLevel();
            weapon.maxDamage.Value += Math.Max(1, (int)(baseMax * 0.05f)) * forge.GetLevel();
        }

        /// <inheritdoc />
        internal override void Quench(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            var data = weapon.GetData();
            if (data == null)
            {
                return;
            }

            var baseMin = data.MinDamage;
            var baseMax = data.MaxDamage;
            weapon.minDamage.Value -= Math.Max(1, (int)(baseMin * 0.05f)) * forge.GetLevel();
            weapon.maxDamage.Value -= Math.Max(1, (int)(baseMax * 0.05f)) * forge.GetLevel();
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float multiplier = 1f)
        {
            buffer.AttackMultiplier += 0.1f * multiplier;
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
                QualifiedObjectIds.Aquamarine,
                QualifiedObjectIds.AquamarineRing,
                9f / 8f,
                new Color(35, 144, 170),
                new Color(18, 160, 250, 240))
        {
        }

        /// <inheritdoc />
        public override Type EnchantmentType => typeof(AquamarineEnchantment);

        /// <inheritdoc />
        internal override void Resonate(BuffEffects effects, float amplitude)
        {
            effects.CriticalChanceMultiplier.Value += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void Resonate(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.critChance.Value += (int)(0.023f * forge.GetLevel());
        }

        /// <inheritdoc />
        internal override void Quench(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.critChance.Value -= (int)(0.023f * forge.GetLevel());
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float multiplier = 1f)
        {
            buffer.CriticalChanceMultiplier += 0.1f * multiplier;
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
                QualifiedObjectIds.Amethyst,
                QualifiedObjectIds.AmethystRing,
                5f / 4f,
                new Color(111, 60, 196),
                new Color(220, 50, 250, 240))
        {
        }

        /// <inheritdoc />
        public override Type EnchantmentType => typeof(AmethystEnchantment);

        /// <inheritdoc />
        internal override void Resonate(BuffEffects effects, float amplitude)
        {
            effects.KnockbackMultiplier.Value += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void Resonate(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.knockback.Value += 0.5f * forge.GetLevel();
        }

        /// <inheritdoc />
        internal override void Quench(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.knockback.Value -= 0.5f * forge.GetLevel();
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float multiplier = 1f)
        {
            buffer.KnockbackMultiplier += 0.1f * multiplier;
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
                $"(O){GarnetStoneId}",
                $"(O){GarnetRingId}",
                4f / 3f,
                new Color(152, 29, 45),
                new Color(245, 75, 20, 230))
        {
        }

        /// <inheritdoc />
        public override Type EnchantmentType => typeof(GarnetEnchantment);

        /// <inheritdoc />
        internal override void Resonate(BuffEffects effects, float amplitude)
        {
            effects.Get_CooldownReduction().Value += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void Resonate(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.Get_CooldownReduction().Value += 0.05f * forge.GetLevel();
        }

        /// <inheritdoc />
        internal override void Quench(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.Get_CooldownReduction().Value -= 0.05f * forge.GetLevel();
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float multiplier = 1f)
        {
            buffer.CooldownReduction += 0.1f * multiplier;
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
                QualifiedObjectIds.Emerald,
                QualifiedObjectIds.EmeraldRing,
                3f / 2f,
                new Color(4, 128, 54),
                new Color(10, 220, 40, 220))
        {
        }

        /// <inheritdoc />
        public override Type EnchantmentType => typeof(EmeraldEnchantment);

        /// <inheritdoc />
        internal override void Resonate(BuffEffects effects, float amplitude)
        {
            effects.WeaponSpeedMultiplier.Value += 0.1f * amplitude;
        }

        /// <inheritdoc />
        internal override void Resonate(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.speed.Value += (int)(2.5f * forge.GetLevel());
        }

        /// <inheritdoc />
        internal override void Quench(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.speed.Value -= (int)(2.5f * forge.GetLevel());
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float multiplier = 1f)
        {
            buffer.WeaponSpeedMultiplier += 0.1f * multiplier;
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
                QualifiedObjectIds.Jade,
                QualifiedObjectIds.JadeRing,
                5f / 3f,
                new Color(117, 150, 99),
                new Color(10, 220, 40, 220))
        {
        }

        /// <inheritdoc />
        public override Type EnchantmentType => typeof(JadeEnchantment);

        /// <inheritdoc />
        internal override void Resonate(BuffEffects effects, float amplitude)
        {
            effects.CriticalPowerMultiplier.Value += 0.5f * amplitude;
        }

        /// <inheritdoc />
        internal override void Resonate(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.critMultiplier.Value += (int)(0.25f * forge.GetLevel());
        }

        /// <inheritdoc />
        internal override void Quench(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.critMultiplier.Value += (int)(0.25f * forge.GetLevel());
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float multiplier = 1f)
        {
            buffer.CriticalPowerMultiplier += 0.5f * multiplier;
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
                QualifiedObjectIds.Topaz,
                QualifiedObjectIds.TopazRing,
                15f / 8f,
                new Color(220, 143, 8),
                new Color(255, 150, 10, 220))
        {
        }

        /// <inheritdoc />
        public override Type EnchantmentType => typeof(TopazEnchantment);

        /// <inheritdoc />
        internal override void Resonate(BuffEffects effects, float amplitude)
        {
            effects.Defense.Value += 1f * amplitude;
        }

        /// <inheritdoc />
        internal override void Resonate(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.addedDefense.Value += (int)(0.5f * forge.GetLevel());
        }

        /// <inheritdoc />
        internal override void Quench(MeleeWeapon weapon, BaseWeaponEnchantment forge)
        {
            weapon.addedDefense.Value += (int)(0.5f * forge.GetLevel());
        }

        /// <inheritdoc />
        internal override void Buffer(StatBuffer buffer, float multiplier = 1f)
        {
            buffer.Defense += 1f * multiplier;
        }
    }

    #endregion topaz

    #endregion implementations
}
