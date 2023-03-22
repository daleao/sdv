namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
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

    /// <summary>Custom forge slots for slingshots.</summary>
    [HarmonyPrefix]
    private static bool ToolGetMaxForgesPrefix(Tool __instance, ref int __result)
    {
        if (__instance is not Slingshot slingshot || !ArsenalModule.Config.Slingshots.EnableForges)
        {
            return true; // run original logic
        }

        try
        {
            __result = slingshot.InitialParentTileIndex switch
            {
                ItemIDs.BasicSlingshot => 1,
                ItemIDs.MasterSlingshot => 2,
                ItemIDs.GalaxySlingshot => 3,
                ItemIDs.InfinitySlingshot => 4,
                _ => 0,
            };

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
