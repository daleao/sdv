namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetForagingLevelPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetForagingLevelPatch"/> class.</summary>
    internal FarmerGetForagingLevelPatch()
    {
        this.Target = this.RequireMethod<Farmer>("get_ForagingLevel");
    }

    #region harmony patches

    /// <summary>Master Axe enchantment effect.</summary>
    [HarmonyPostfix]
    private static void FarmerGetForagingLevelPostfix(Farmer __instance, ref int __result)
    {
        if (__instance.CurrentTool is Axe axe && axe.hasEnchantmentOfType<MasterEnchantment>())
        {
            ++__result;
        }
    }

    #endregion harmony patches
}
