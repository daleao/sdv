namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetMiningLevelPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerGetMiningLevelPatch()
    {
        Target = RequireMethod<Farmer>("get_MiningLevel");
    }

    #region harmony patches

    /// <summary>Master Pickaxe enchantment effect.</summary>
    [HarmonyPostfix]
    private static void FarmerGetMiningLevelPostfix(Farmer __instance, ref int __result)
    {
        if (__instance.CurrentTool is Pickaxe pickaxe && pickaxe.hasEnchantmentOfType<MasterEnchantment>())
            ++__result;
    }

    #endregion harmony patches
}