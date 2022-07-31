namespace DaLion.Stardew.Arsenal.Framework.Patches.Combat;

#region using directives

using Common;
using Enchantments;
using HarmonyLib;
using StardewValley.Tools;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetAutoFireRatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetAutoFireRatePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.GetAutoFireRate));
        Prefix!.priority = Priority.High;
        Prefix!.after = new[] { "DaLion.ImmersiveProfessions" };
    }

    #region harmony patches

    /// <summary>Implement <see cref="GatlingEnchantment"/> effect.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyAfter("DaLion.ImmersiveProfessions")]
    private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref float __result)
    {
        try
        {
            if (!__instance.hasEnchantmentOfType<GatlingEnchantment>()) return true; // run original logic

            __result = 0.45f;
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