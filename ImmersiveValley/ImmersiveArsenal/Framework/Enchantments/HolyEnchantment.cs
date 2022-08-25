namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Holy Blade.</summary>
public class HolyEnchantment : BaseWeaponEnchantment
{
    public override bool IsSecondaryEnchantment() => true;

    public override bool IsForge() => false;

    public override int GetMaximumLevel() => 1;

    public override bool ShouldBeDisplayed() => false;
}