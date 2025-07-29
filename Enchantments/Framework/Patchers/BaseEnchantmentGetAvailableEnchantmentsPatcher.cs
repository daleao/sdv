namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetAvailableEnchantmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetAvailableEnchantmentsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BaseEnchantmentGetAvailableEnchantmentsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetAvailableEnchantments));
    }

    #region harmony patches

    /// <summary>Out with the old and in with the new enchants.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static bool BaseEnchantmentGetAvailableEnchantmentsPrefix(ref List<BaseEnchantment>? ____enchantments, ref List<BaseEnchantment> __result)
    {
        if (____enchantments is not null)
        {
            __result = ____enchantments;
            return false; // don't run original logic
        }

        // The base tool enchantments
        ____enchantments = [
            new AutoHookEnchantment(),
            new ArchaeologistEnchantment(),
            new BottomlessEnchantment(),
            new EfficientToolEnchantment(),
            new GenerousEnchantment(),
            new FisherEnchantment(),
            new MasterEnchantment(),
            new PowerfulEnchantment(),
            new PreservingEnchantment(),
            new ReachingToolEnchantment(),
            new ShavingEnchantment(),
            new SwiftToolEnchantment(),
            new HaymakerEnchantment(),
        ];

        // The Springmyst combat enchantments
        var enchantmentTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetExecutingAssembly())
            .Where(t => t.IsAssignableTo(typeof(BaseEnchantment)) &&
                        (t.Namespace?.Contains("DaLion.Enchantments.Framework.Enchantments") ?? false));
        foreach (var type in enchantmentTypes)
        {
            if (Activator.CreateInstance(type) is BaseEnchantment enchantment)
            {
                ____enchantments.Add(enchantment);
            }
        }

        __result = ____enchantments;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
