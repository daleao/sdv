namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolGetMaxForgesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolGetMaxForgesPatcher"/> class.</summary>
    internal ToolGetMaxForgesPatcher()
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

        __result = __instance.ParentSheetIndex switch
        {
            Constants.BasicSlingshotIndex => 1,
            Constants.MasterSlingshotIndex => 2,
            Constants.GalaxySlingshotIndex => 3,
            _ => 0,
        };
        return false; // don't run original logic
    }

    #endregion harmony patches
}
