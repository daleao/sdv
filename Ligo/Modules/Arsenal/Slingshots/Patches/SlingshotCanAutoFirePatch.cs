namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using System.Reflection;
using DaLion.Ligo.Modules.Arsenal.Slingshots.Enchantments;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanAutoFirePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCanAutoFirePatch"/> class.</summary>
    internal SlingshotCanAutoFirePatch()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.CanAutoFire));
        this.Prefix!.priority = Priority.High;
        this.Prefix!.after = new[] { LigoModule.Professions.Namespace };
    }

    #region harmony patches

    /// <summary>Implement <see cref="GatlingEnchantment"/> effect.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyAfter("Ligo.Modules.Professions")]
    private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref bool __result)
    {
        try
        {
            __result = __instance.hasEnchantmentOfType<GatlingEnchantment>() &&
                       (!ModEntry.Config.EnableProfessions || ModEntry.Config.Professions.ModKey.IsDown());
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
