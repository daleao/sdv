using System;

namespace DaLion.Stardew.Rings.Framework;

#region using directives

using Ardalis.SmartEnum;
using Common.Extensions.Stardew;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>A phenomenon of increased amplitude caused by interfering waves.</summary>
internal abstract class Resonance : SmartEnum<Resonance>
{
    #region enum entries

    public static readonly Resonance Amethyst = new AmethystResonance();
    public static readonly Resonance Topaz = new TopazResonance();
    public static readonly Resonance Aquamarine = new AquamarineResonance();
    public static readonly Resonance Jade = new JadeResonance();
    public static readonly Resonance Emerald = new EmeraldResonance();
    public static readonly Resonance Ruby = new RubyResonance();
    public static readonly Resonance Garnet = new GarnetResonance();

    #endregion enum entries
    
    /// <summary>Construct an instance.</summary>
    /// <param name="name">The gemstone name.</param>
    /// <param name="value">The gemstone ring index.</param>
    protected Resonance(string name, int value) : base(name, value) { }

    /// <summary>Get the localized name for this resonance.</summary>
    public abstract string DisplayName { get; }

    /// <summary>Get the corresponding gemstone color.</summary>
    public abstract Color Color { get; }

    /// <summary>Get the corresponding resonant pair.</summary>
    public abstract Resonance? Pair { get; }

    /// <summary>Get the corresponding dampening pair.</summary>
    public abstract Resonance? Antipair { get; }

    /// <summary>Apply stat modifier to a shared aggregate.</summary>
    /// <param name="modifiers">Rolling aggregate of stat modifiers.</param>
    /// <param name="intensity">The resonance intensity.</param>
    public abstract void ApplyToModifiers(ref StatModifiers modifiers, int intensity);

    /// <summary>Apply resonance effect to the farmer.</summary>
    /// <param name="intensity">The resonance intensity.</param>
    /// <param name="location">The current location.</param>
    /// <param name="who">The wielder.</param>
    public void OnEquip(int intensity, GameLocation location, Farmer who)
    {
        OnEquipImpl(intensity, who);
    }

    /// <summary>Remove resonance effect from the farmer.</summary>
    /// <param name="intensity">The resonance intensity.</param>
    /// <param name="location">The current location.</param>
    /// <param name="who">The wielder.</param>
    public void OnUnequip(int intensity, GameLocation location, Farmer who)
    {
        OnUnequipImpl(intensity, who);
    }

    /// <summary>Add specific resonance effect from the farmer.</summary>
    /// <param name="intensity">The resonance intensity.</param>
    /// <param name="who">The farmer.</param>
    protected abstract void OnEquipImpl(int intensity, Farmer who);

    /// <summary>Remove specific resonance effect from the farmer.</summary>
    /// <param name="intensity">The resonance intensity.</param>
    /// <param name="who">The farmer.</param>
    protected abstract void OnUnequipImpl(int intensity, Farmer who);

    #region amethyst

    private sealed class AmethystResonance : Resonance
    {
        /// <inheritdoc />
        public AmethystResonance() : base("Amethyst Resonance", Constants.AMETHYST_RING_INDEX_I) { }

        /// <inheritdoc />
        public override string DisplayName { get; } = ModEntry.i18n.Get("resonance.amethyst");

        /// <inheritdoc />
        public override Color Color { get; } = new(255 - 111, 255 - 60, 255 - 196);

        /// <inheritdoc />
        public override Resonance? Pair => Topaz;

        /// <inheritdoc />
        public override Resonance? Antipair => Jade;

        /// <inheritdoc />
        public override void ApplyToModifiers(ref StatModifiers modifiers, int intensity)
        {
            modifiers.KnockbackModifier += 0.01f * intensity;
        }

        /// <inheritdoc />
        protected override void OnEquipImpl(int intensity, Farmer who)
        {
            who.knockbackModifier += 0.01f * intensity;
        }

        /// <inheritdoc />
        protected override void OnUnequipImpl(int intensity, Farmer who)
        {
            who.knockbackModifier -= 0.01f * intensity;
        }
    }

    #endregion amethyst

    #region topaz

    private sealed class TopazResonance : Resonance
    {
        /// <inheritdoc />
        public TopazResonance() : base("Topaz Resonance", Constants.TOPAZ_RING_INDEX_I) { }

        /// <inheritdoc />
        public override string DisplayName { get; } = ModEntry.i18n.Get("resonance.topaz");

        /// <inheritdoc />
        public override Color Color { get; } = new(255 - 220, 255 - 143, 255 - 8);

        /// <inheritdoc />
        public override Resonance? Pair => Amethyst;

        /// <inheritdoc />
        public override Resonance? Antipair => Emerald;

        /// <inheritdoc />
        public override void ApplyToModifiers(ref StatModifiers modifiers, int intensity)
        {
            if (ModEntry.Config.RebalancedRings)
            {
                modifiers.AddedDefense += (int)Math.Round(0.5 * intensity);
                modifiers.AddedImmunity += (int)Math.Round(0.5 * intensity);
            }
            else
            {
                modifiers.PrecisionModifier += 0.01f * intensity;
            }
        }

        /// <inheritdoc />
        protected override void OnEquipImpl(int intensity, Farmer who)
        {
            if (ModEntry.Config.RebalancedRings)
            {
                who.resilience += (int)Math.Round(0.5 * intensity);
                who.immunity += (int)Math.Round(0.5 * intensity);
            }
            else
            {
                who.weaponPrecisionModifier += 0.01f * intensity;
            }
        }

        /// <inheritdoc />
        protected override void OnUnequipImpl(int intensity, Farmer who)
        {
            if (ModEntry.Config.RebalancedRings)
            {
                who.resilience -= (int)Math.Round(0.5 * intensity);
                who.immunity -= (int)Math.Round(0.5 * intensity);
            }
            else
            {
                who.weaponPrecisionModifier -= 0.01f * intensity;
            }
        }
    }

    #endregion topaz

    #region aquamarine

    private sealed class AquamarineResonance : Resonance
    {
        /// <inheritdoc />
        public AquamarineResonance() : base("Aquamarine Resonance", Constants.AQUAMARINE_RING_INDEX_I) { }

        /// <inheritdoc />
        public override string DisplayName { get; } = ModEntry.i18n.Get("resonance.aquamarine");

        /// <inheritdoc />
        public override Color Color { get; } = new(255 - 35, 255 - 144, 255 - 170);

        /// <inheritdoc />
        public override Resonance? Pair => Jade;

        /// <inheritdoc />
        public override Resonance? Antipair => Ruby;

        /// <inheritdoc />
        public override void ApplyToModifiers(ref StatModifiers modifiers, int intensity)
        {
            modifiers.CritChanceModifier += 0.01f * intensity;
        }

        /// <inheritdoc />
        protected override void OnEquipImpl(int intensity, Farmer who)
        {
            who.critChanceModifier += 0.01f * intensity;
        }

        /// <inheritdoc />
        protected override void OnUnequipImpl(int intensity, Farmer who)
        {
            who.critChanceModifier -= 0.01f * intensity;
        }
    }

    #endregion aquamarine

    #region jade

    private sealed class JadeResonance : Resonance
    {
        /// <inheritdoc />
        public JadeResonance() : base("Jade Resonance", Constants.JADE_RING_INDEX_I) { }

        /// <inheritdoc />
        public override string DisplayName { get; } = ModEntry.i18n.Get("resonance.aquamarine");

        /// <inheritdoc />
        public override Color Color { get; } = new(255 - 117, 255 - 150, 255 - 99);

        /// <inheritdoc />
        public override Resonance? Pair => Aquamarine;

        /// <inheritdoc />
        public override Resonance? Antipair => Amethyst;

        /// <inheritdoc />
        public override void ApplyToModifiers(ref StatModifiers modifiers, int intensity)
        {
            modifiers.CritPowerModifier += 0.05f * intensity;
        }

        /// <inheritdoc />
        protected override void OnEquipImpl(int intensity, Farmer who)
        {
            who.critPowerModifier += 0.05f * intensity;
        }

        /// <inheritdoc />
        protected override void OnUnequipImpl(int intensity, Farmer who)
        {
            who.critPowerModifier -= 0.05f * intensity;
        }
    }

    #endregion jade

    #region emerald

    private sealed class EmeraldResonance : Resonance
    {
        /// <inheritdoc />
        public EmeraldResonance() : base("Emerald Resonance", Constants.EMERALD_RING_INDEX_I) { }

        /// <inheritdoc />
        public override string DisplayName { get; } = ModEntry.i18n.Get("resonance.emerald");

        /// <inheritdoc />
        public override Color Color { get; } = new(255 - 4, 255 - 128, 255 - 54);

        /// <inheritdoc />
        public override Resonance? Pair => Ruby;

        /// <inheritdoc />
        public override Resonance? Antipair => Topaz;

        /// <inheritdoc />
        public override void ApplyToModifiers(ref StatModifiers modifiers, int intensity)
        {
            modifiers.SwingSpeedModifier += 0.01f * intensity;
        }

        /// <inheritdoc />
        protected override void OnEquipImpl(int intensity, Farmer who)
        {
            who.weaponSpeedModifier += 0.01f * intensity;
        }

        /// <inheritdoc />
        protected override void OnUnequipImpl(int intensity, Farmer who)
        {
            who.weaponSpeedModifier -= 0.01f * intensity;
        }
    }

    #endregion emerald

    #region ruby

    private sealed class RubyResonance : Resonance
    {
        /// <inheritdoc />
        public RubyResonance() : base("Ruby Resonance", Constants.RUBY_RING_INDEX_I) { }

        /// <inheritdoc />
        public override string DisplayName { get; } = ModEntry.i18n.Get("resonance.ruby");

        /// <inheritdoc />
        public override Color Color { get; } = new(255 - 225, 255 - 57, 255 - 57);

        /// <inheritdoc />
        public override Resonance? Pair => Emerald;

        /// <inheritdoc />
        public override Resonance? Antipair => Aquamarine;

        /// <inheritdoc />
        public override void ApplyToModifiers(ref StatModifiers modifiers, int intensity)
        {
            modifiers.DamageModifier += 0.01f * intensity;
        }

        /// <inheritdoc />
        protected override void OnEquipImpl(int intensity, Farmer who)
        {
            who.attackIncreaseModifier += 0.01f * intensity;
        }

        /// <inheritdoc />
        protected override void OnUnequipImpl(int intensity, Farmer who)
        {
            who.attackIncreaseModifier -= 0.01f * intensity;
        }
    }

    #endregion ruby

    #region garnet

    private sealed class GarnetResonance : Resonance
    {
        /// <inheritdoc />
        public GarnetResonance() : base("Garnet Resonance", ModEntry.GarnetRingIndex) { }

        /// <inheritdoc />
        public override string DisplayName { get; } = ModEntry.i18n.Get("resonance.garnet");

        /// <inheritdoc />
        public override Color Color { get; } = new(255 - 152, 255 - 29, 255 - 45);

        /// <inheritdoc />
        public override Resonance? Pair => null;

        /// <inheritdoc />
        public override Resonance? Antipair => null;

        /// <inheritdoc />
        public override void ApplyToModifiers(ref StatModifiers modifiers, int intensity)
        {
            modifiers.CooldownReduction += 0.01f * intensity;
        }

        /// <inheritdoc />
        protected override void OnEquipImpl(int intensity, Farmer who)
        {
            who.Increment("CooldownReduction", 0.01f * intensity);
        }

        /// <inheritdoc />
        protected override void OnUnequipImpl(int intensity, Farmer who)
        {
            who.Increment("CooldownReduction", -0.01f * intensity);
        }
    }

    #endregion garnet
}