namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetMiningLevelPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetMiningLevelPatch"/> class.</summary>
    internal FarmerGetMiningLevelPatch()
    {
        this.Target = this.RequireMethod<Farmer>("get_MiningLevel");
    }

    #region harmony patches

    /// <summary>Master Pick enchantment effect.</summary>
    [HarmonyPostfix]
    private static void FarmerGetMiningLevelPostfix(Farmer __instance, ref int __result)
    {
        if (__instance.CurrentTool is Pickaxe pickaxe && pickaxe.hasEnchantmentOfType<MasterEnchantment>())
        {
            ++__result;
        }
    }

    #endregion harmony patches
}
