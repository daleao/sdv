namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
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
        this.Prefix!.after = new[] { OverhaulModule.Professions.Namespace };
    }

    #region harmony patches

    /// <summary>Implement slingshot special gatling effect.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyAfter("DaLion.Overhaul.Modules.Professions")]
    private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref bool __result)
    {
        try
        {
            __result = __instance.Get_IsOnSpecial();
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
