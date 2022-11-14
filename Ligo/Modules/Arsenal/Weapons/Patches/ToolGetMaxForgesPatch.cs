namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
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

    /// <summary>Custom Weapon forge slots.</summary>
    [HarmonyPrefix]
    private static bool ToolGetMaxForgesPrefix(Tool __instance, ref int __result)
    {
        if (__instance is not MeleeWeapon)
        {
            return true; // run original logic
        }

        __result = 3; // implement level-based forge count
        return false; // don't run original logic
    }

    #endregion harmony patches
}
