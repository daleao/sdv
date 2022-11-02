namespace DaLion.Redux.Framework.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class VampiricEnchantmentGetNamePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="VampiricEnchantmentGetNamePatch"/> class.</summary>
    internal VampiricEnchantmentGetNamePatch()
    {
        this.Target = this.RequireMethod<VampiricEnchantment>(nameof(VampiricEnchantment.GetName));
    }

    #region harmony patches

    /// <summary>Renames Vampiric enchant.</summary>
    [HarmonyPrefix]
    private static bool VampiricEnchantmentGetNamePrefix(ref string __result)
    {
        if (!ModEntry.Config.Arsenal.Weapons.OverhauledEnchants)
        {
            return true; // run original logic
        }

        __result = ModEntry.i18n.Get("enchantments.vampiric");
        return false; // don't run original logic
    }

    #endregion harmony patches
}
