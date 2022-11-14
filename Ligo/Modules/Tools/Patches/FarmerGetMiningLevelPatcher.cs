namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;
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
            ++__result;
        }
    }

    #endregion harmony patches
}
