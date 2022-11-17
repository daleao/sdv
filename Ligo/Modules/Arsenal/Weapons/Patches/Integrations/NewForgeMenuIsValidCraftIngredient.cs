namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Weapons.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[Integration("spacechase0.SpaceCore")]
internal sealed class NewForgeMenuIsValidCraftIngredient : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidCraftIngredient"/> class.</summary>
    internal NewForgeMenuIsValidCraftIngredient()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.IsValidCraftIngredient));
    }

    #region harmony patches

    /// <summary>Allow forging with Hero Soul.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftIngredientPostfix(ref bool __result, Item item)
    {
        if (item.IsHeroSoul())
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
