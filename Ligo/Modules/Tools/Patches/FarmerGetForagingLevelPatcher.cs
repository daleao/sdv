namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetForagingLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetForagingLevelPatcher"/> class.</summary>
    internal FarmerGetForagingLevelPatcher()
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
