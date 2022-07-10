namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes Infinity weapons.</summary>
public class InfinityEnchantment : BaseWeaponEnchantment
{
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    public override bool IsForge()
    {
        return false;
    }

    public override int GetMaximumLevel()
    {
        return 1;
    }

    public override bool ShouldBeDisplayed()
    {
        return false;
    }
}