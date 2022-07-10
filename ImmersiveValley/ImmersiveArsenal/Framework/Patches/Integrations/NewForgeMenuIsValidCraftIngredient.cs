namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class NewForgeMenuIsValidCraftIngredient : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal NewForgeMenuIsValidCraftIngredient()
    {
        try
        {
            Target = "SpaceCore.Interface.NewForgeMenu".ToType().RequireMethod("IsValidCraftIngredient");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Allow forging with Hero Soul.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftIngredientPostfix(ref bool __result, Item item)
    {
        if (item.IsHeroSoul()) __result = true;
    }

    #endregion harmony patches
}