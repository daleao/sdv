namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using Common;
using Enchantments;
using HarmonyLib;
using StardewValley.Tools;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanAutoFirePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotCanAutoFirePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.CanAutoFire));
        Prefix!.priority = Priority.High;
        Prefix!.after = new[] { "DaLion.ImmersiveProfessions" };
    }

    #region harmony patches

    /// <summary>Implement <see cref="GatlingEnchantment"/> effect.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyAfter("DaLion.ImmersiveProfessions")]
    private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref bool __result)
    {
        try
        {
            __result = __instance.hasEnchantmentOfType<GatlingEnchantment>();
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