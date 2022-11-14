namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Common.Enchantments;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolGetMaxForgesPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ToolGetMaxForgesPatch"/> class.</summary>
    internal ToolGetMaxForgesPatch()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.GetMaxForges));
    }

    #region harmony patches

    /// <summary>Add extra Infinity forge slot.</summary>
    [HarmonyPostfix]
    private static void ToolGetMaxForgesPostfix(Tool __instance, ref int __result)
    {
        if (__instance.hasEnchantmentOfType<InfinityEnchantment>())
        {
            ++__result;
        }
    }

    #endregion harmony patches
}
