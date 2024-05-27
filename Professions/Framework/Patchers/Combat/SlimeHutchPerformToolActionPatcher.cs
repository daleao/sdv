namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeHutchPerformToolActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlimeHutchPerformToolActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlimeHutchPerformToolActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SlimeHutch>(nameof(SlimeHutch.performToolAction));
    }

    #region harmony patches

    /// <summary>Patch to increase Prestiged Piper Hutch capacity.</summary>
    [HarmonyPrefix]
    private static bool SlimeHutchPerformToolActionPrefix(SlimeHutch __instance, ref bool __result, Tool t, int tileX, int tileY)
    {
        if (__instance.waterSpots.Length != 6)
        {
            return true; // run original logic
        }

        try
        {
            if (t is WateringCan && tileX == 16 && tileY.IsIn(5..10))
            {
                __instance.waterSpots[tileY - 5] = true;
            }

            __result = false;
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
