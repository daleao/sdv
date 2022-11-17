namespace DaLion.Ligo.Modules.Professions.Patches.Foraging;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
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
        var anyPlayerIsArborist = Game1.game1.DoesAnyPlayerHaveProfession(Profession.Arborist, out var n);
        if (__instance.growthStage.Value > __state || !anyPlayerIsArborist || !__instance.CanGrow())
        {
            return;
        }

        if (__instance.treeType.Value == Tree.mahoganyTree)
        {
            if (Game1.random.NextDouble() < 0.075 * n ||
                (__instance.fertilized.Value && Game1.random.NextDouble() < 0.3 * n))
            {
                ++__instance.growthStage.Value;
            }
        }
        else if (Game1.random.NextDouble() < 0.1 * n)
        {
            ++__instance.growthStage.Value;
        }
    }

    #endregion harmony patches
}
