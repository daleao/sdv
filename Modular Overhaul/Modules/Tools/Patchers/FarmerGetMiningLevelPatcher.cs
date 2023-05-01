namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetMiningLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetMiningLevelPatcher"/> class.</summary>
    internal FarmerGetMiningLevelPatcher()
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
            __result++;
        }
    }

    #endregion harmony patches
}
