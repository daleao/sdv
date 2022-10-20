namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using System.Reflection;
using DaLion.Stardew.Slingshots.Framework.Enchantments;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanAutoFirePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCanAutoFirePatch"/> class.</summary>
    internal SlingshotCanAutoFirePatch()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.CanAutoFire));
        this.Prefix!.priority = Priority.High;
        this.Prefix!.after = new[] { "DaLion.ImmersiveProfessions" };
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
