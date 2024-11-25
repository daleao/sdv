namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Collections;
using StardewValley.Enchantments;

#endregion using directives

/// <summary>Extensions for <see cref="Tool"/> class.</summary>
public static class ToolExtensions
{
    /// <summary>Determines whether the specified <paramref name="tool"/> contains any <see cref="BaseEnchantment"/> of the specified <paramref name="enchantmentTypes"/>.</summary>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <param name="enchantmentTypes">The candidate <see cref="BaseEnchantment"/> types to search for.</param>
    /// <returns><see langword="true"/> if the <paramref name="tool"/> contains at least one enchantment of the specified <paramref name="enchantmentTypes"/>, otherwise <see langword="false"/>.</returns>
    public static bool HasAnyEnchantmentOf(this Tool tool, params Type[] enchantmentTypes)
    {
        return enchantmentTypes.Any(t => tool.enchantments.ContainsType(t));
    }

    /// <summary>Determines whether the specified <paramref name="tool"/> contains any <see cref="BaseEnchantment"/> of the specified <typeparamref name="TEnchantment"/> type.</summary>
    /// <typeparam name="TEnchantment">A <see cref="BaseEnchantment"/> type.</typeparam>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="tool"/> contains at least one enchantment of the specified <typeparamref name="TEnchantment"/> type, otherwise <see langword="false"/>.</returns>
    public static bool HasAnyEnchantmentOf<TEnchantment>(this Tool tool)
    {
        return tool.enchantments.Any(t => t is TEnchantment);
    }

    /// <summary>Counts the number of enchantments in the <paramref name="tool"/> which correspond to the specified <paramref name="enchantmentType"/>.</summary>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <param name="enchantmentType">The <see cref="Type"/>.</param>
    /// <returns>The number of <see cref="BaseEnchantment"/>s in <paramref name="tool"/> whith the specified <paramref name="enchantmentType"/>.</returns>
    internal static int CountEnchantmentsOfType(this Tool tool, Type enchantmentType)
    {
        return tool.enchantments.Count(enchantment => enchantment.GetType().Name.Contains(enchantmentType.Name));
    }

    /// <summary>Counts the number of enchantments in the <paramref name="tool"/> which correspond to the specified <typeparamref name="TEnchantment"/> type.</summary>
    /// <typeparam name="TEnchantment">A <see cref="BaseEnchantment"/> type.</typeparam>
    /// <param name="tool">The <see cref="Tool"/>.</param>
    /// <returns>The number of <see cref="BaseEnchantment"/>s in <paramref name="tool"/> with the specified <typeparamref name="TEnchantment"/> type.</returns>
    internal static int CountEnchantmentsOfType<TEnchantment>(this Tool tool)
        where TEnchantment : BaseEnchantment
    {
        return tool.enchantments.OfType<TEnchantment>().Count();
    }
}
