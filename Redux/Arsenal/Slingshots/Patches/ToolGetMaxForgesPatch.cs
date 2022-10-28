namespace DaLion.Redux.Arsenal.Slingshots.Patches;

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

    /// <summary>Allow Slingshot forges.</summary>
    [HarmonyPrefix]
    private static bool ToolGetMaxForgesPrefix(Tool __instance, ref int __result)
    {
        if (__instance is not Slingshot)
        {
            return true; // run original logic
        }

        __result = 3;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
