namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using System.Reflection;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanAutoFirePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCanAutoFirePatcher"/> class.</summary>
    internal SlingshotCanAutoFirePatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.CanAutoFire));
        this.Prefix!.priority = Priority.High;
        this.Prefix!.before = new[] { LigoModule.Arsenal.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to add Desperado auto-fire during Ultimate.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("Ligo.Modules.Arsenal")]
    private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref bool __result)
    {
        try
        {
            __result = __instance.getLastFarmerToUse().Get_Ultimate() is DeathBlossom { IsActive: true };
            return !__result;
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
