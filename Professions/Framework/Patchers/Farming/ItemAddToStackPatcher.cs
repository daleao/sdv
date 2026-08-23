namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemAddToStackPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemAddToStackPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ItemAddToStackPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target =
            this.RequireMethod<Item>(nameof(Item.addToStack));
    }

    #region harmony patches

    /// <summary>Patch for nutrition stacking.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ItemAddToStackPostfix(Item __instance, int __result, Item otherStack)
    {
        if (__instance is not SObject { Category: (int)ObjectCategory.Eggs })
        {
            return;
        }

        var instancePotential = Data.ReadAs<double>(__instance, DataKeys.InheritedPotential);
        var otherPotential = Data.ReadAs<double>(otherStack, DataKeys.InheritedPotential);
        var addedFromOtherStack = otherStack.Stack - __result;
        var instancePreStack = __instance.Stack - addedFromOtherStack;
        var meanPotential = (int)(((instancePotential * instancePreStack) + (otherPotential * addedFromOtherStack)) / __instance.Stack);
        Data.Write(__instance, DataKeys.InheritedPotential, meanPotential.ToString());
    }

    #endregion harmony patches
}
