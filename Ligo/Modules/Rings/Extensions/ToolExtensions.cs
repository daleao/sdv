namespace DaLion.Ligo.Modules.Rings.Extensions;

#region using directives

using System.Linq;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Ring"/> class.</summary>
internal static class ToolExtensions
{
    /// <summary>Determines whether the <paramref name="tool"/> has the <see cref="BaseWeaponEnchantment"/> which corresponds to the specified <paramref name="gemstone"/>.</summary>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <param name="gemstone">A <see cref="Gemstone"/>.</param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="tool"/> has at least one corresponding <see cref="BaseWeaponEnchantment"/> to <paramref name="gemstone"/>, otherwise <see langword="false"/>.
    /// </returns>
    internal static bool CanResonateWith(this Tool tool, Gemstone gemstone)
    {
        return tool.enchantments.OfType<BaseWeaponEnchantment>()
            .Any(enchantment => enchantment.GetType() == gemstone.EnchantmentType);
    }

    /// <summary>Determines whether the <paramref name="tool"/> has the <see cref="BaseWeaponEnchantment"/> which corresponds to the specified <typeparamref name="TGemstone"/>.</summary>
    /// <typeparam name="TGemstone">A <see cref="Gemstone"/> type.</typeparam>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="tool"/> has at least one corresponding <see cref="BaseWeaponEnchantment"/> to <typeparamref name="TGemstone"/>, otherwise <see langword="false"/>.
    /// </returns>
    internal static bool CanResonateWith<TGemstone>(this Tool tool)
        where TGemstone : Gemstone
    {
        var gemstone = Gemstone.FromType(typeof(TGemstone));
        return gemstone is not null && tool.enchantments.OfType<BaseWeaponEnchantment>()
            .Any(enchantment => enchantment.GetType() == gemstone.EnchantmentType);
    }

    /// <summary>Counts the number of <see cref="BaseWeaponEnchantment"/>s in the <paramref name="tool"/> which correspond to the specified <paramref name="gemstone"/>.</summary>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <param name="gemstone">A <see cref="Gemstone"/>.</param>
    /// <returns>The number of <see cref="BaseWeaponEnchantment"/>s in <paramref name="tool"/> which correspond to <paramref name="gemstone"/>.</returns>
    internal static int CountEnchantmentsOfType(this Tool tool, Gemstone gemstone)
    {
        return tool.enchantments.OfType<BaseWeaponEnchantment>()
            .Count(enchantment => enchantment.GetType().Name.Contains(gemstone.Name));
    }

    /// <summary>Counts the number of <see cref="BaseWeaponEnchantment"/>s in the <paramref name="tool"/> which correspond to the specified <typeparamref name="TGemstone"/>.</summary>
    /// <typeparam name="TGemstone">A <see cref="Gemstone"/> type.</typeparam>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <returns>The number of <see cref="BaseWeaponEnchantment"/>s in <paramref name="tool"/> which correspond to <typeparamref name="TGemstone"/>.</returns>
    internal static int CountEnchantmentsOfType<TGemstone>(this Tool tool)
        where TGemstone : Gemstone
    {
        var gemstone = Gemstone.FromType(typeof(TGemstone));
        if (gemstone is null)
        {
            return 0;
        }

        return tool.enchantments.OfType<BaseWeaponEnchantment>()
            .Count(enchantment => enchantment.GetType().Name.Contains(gemstone.Name));
    }
}
