namespace DaLion.Stardew.Rings.Framework;

internal struct StatModifiers
{
    public float DamageModifier;
    public float CritChanceModifier;
    public float CritPowerModifier;
    public float SwingSpeedModifier;
    public float KnockbackModifier;
    public float PrecisionModifier;
    public int AddedDefense;
    public int AddedImmunity;
    public float CooldownReduction;

    public bool Any()
    {
        return DamageModifier != 0f || CritChanceModifier != 0f || CritPowerModifier != 0f || SwingSpeedModifier != 0f ||
               KnockbackModifier != 0f || PrecisionModifier != 0f || AddedDefense != 0 || AddedImmunity != 0 ||
               CooldownReduction != 0f;
    }
}