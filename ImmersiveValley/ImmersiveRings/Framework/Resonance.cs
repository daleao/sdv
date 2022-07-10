using StardewValley;

namespace DaLion.Stardew.Rings.Framework;

#region using directives

using Ardalis.SmartEnum;

#endregion using directives

public class Resonance : SmartEnum<Resonance>
{
    #region enum entries

    public static readonly Resonance Ruby = new("Ruby", Constants.RUBY_RING_INDEX_I);
    public static readonly Resonance Aquamarine = new("Aquamarine", Constants.AQUAMARINE_RING_INDEX_I);
    public static readonly Resonance Jade = new("Jade", Constants.JADE_RING_INDEX_I);
    public static readonly Resonance Emerald = new("Emerald", Constants.EMERALD_RING_INDEX_I);
    public static readonly Resonance Amethyst = new("Amethyst", Constants.AMETHYST_RING_INDEX_I);
    public static readonly Resonance Topaz = new("Topaz", Constants.TOPAZ_RING_INDEX_I);

    #endregion enum entries

    /// <summary>Construct an instance.</summary>
    /// <param name="name">The gemstone name.</param>
    /// <param name="value">The gemstone ring index.</param>
    public Resonance(string name, int value) : base(name, value)
    {
        DisplayName = ModEntry.i18n.Get("resonance." + name.ToLowerInvariant());
    }

    /// <summary>Get the localized name for this resonance.</summary>
    public string DisplayName { get; }

    /// <summary>Apply resonance's effect to the farmer.</summary>
    /// <param name="who">The farmer.</param>
    public void OnEquip(Farmer who)
    {
        switch (Value)
        {
            case Constants.RUBY_RING_INDEX_I:
                who.attackIncreaseModifier += 0.04f;
                break;
            case Constants.AQUAMARINE_INDEX_I:
                who.critChanceModifier += 0.04f;
                break;
            case Constants.JADE_RING_INDEX_I:
                who.critPowerModifier += 0.12f;
                break;
            case Constants.EMERALD_RING_INDEX_I:
                who.weaponSpeedModifier += 0.04f;
                break;
            case Constants.AMETHYST_RING_INDEX_I:
                who.knockbackModifier += 0.04f;
                break;
            case Constants.TOPAZ_RING_INDEX_I:
                if (ModEntry.Config.RebalancedRings) who.resilience += 1;
                else who.weaponPrecisionModifier += 0.04f;
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
                who.attackIncreaseModifier -= 0.04f;
                break;
            case Constants.AQUAMARINE_INDEX_I:
                who.critChanceModifier -= 0.04f;
                break;
            case Constants.JADE_RING_INDEX_I:
                who.critPowerModifier -= 0.12f;
                break;
            case Constants.EMERALD_RING_INDEX_I:
                who.weaponSpeedModifier -= 0.04f;
                break;
            case Constants.AMETHYST_RING_INDEX_I:
                who.knockbackModifier -= 0.04f;
                break;
            case Constants.TOPAZ_RING_INDEX_I:
                if (ModEntry.Config.RebalancedRings) who.resilience += 1;
                else who.weaponPrecisionModifier -= 0.04f;
                break;
        }
    }
}