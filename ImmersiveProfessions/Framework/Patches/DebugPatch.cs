namespace DaLion.Stardew.Professions.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal class DebugPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal DebugPatch()
    {
#if DEBUG
        //Original = RequireMethod<CraftingRecipe>(nameof(CraftingRecipe.doesFarmerHaveIngredientsInInventory));
#endif
    }

    #region harmony patches

    [HarmonyPrefix]
    private static bool DebugPrefix()
    {
        Log.D("DebugPatch called!");
        return false;
    }

    #endregion harmony patches
}