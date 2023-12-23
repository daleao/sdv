namespace DaLion.Overhaul.Modules.Professions.Patchers.Foraging;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeDayUpdatePatcher"/> class.</summary>
    internal TreeDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to increase Abrorist tree growth odds.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool TreeDayUpdatePrefix(Tree __instance, ref int __state)
    {
        __state = __instance.growthStage.Value;
        return true; // run original logic
    }

    /// <summary>Patch to increase Abrorist non-fruit tree growth odds.</summary>
    [HarmonyPostfix]
    private static void TreeDayUpdatePostfix(Tree __instance, int __state)
    {
        if (__instance.growthStage.Value > __state || !__instance.Read<bool>(DataKeys.PlantedByArborist) ||
            !__instance.CanGrow())
        {
            return;
        }

        if (__instance.treeType.Value == Tree.mahoganyTree)
        {
            if (Game1.random.NextDouble() < 0.075 ||
                (__instance.fertilized.Value && Game1.random.NextDouble() < 0.3))
            {
                __instance.growthStage.Value++;
            }
        }
        else if (Game1.random.NextDouble() < 0.1)
        {
            __instance.growthStage.Value++;
        }
    }

    #endregion harmony patches
}
