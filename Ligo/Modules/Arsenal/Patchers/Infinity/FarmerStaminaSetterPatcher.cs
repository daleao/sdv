namespace DaLion.Ligo.Modules.Arsenal.Patchers.Infinity;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerStaminaSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerStaminaSetterPatcher"/> class.</summary>
    internal FarmerStaminaSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.Stamina));
    }

    #region harmony patches

    /// <summary>Record pre-consumption stamina.</summary>
    [HarmonyPrefix]
    private static void FarmerStaminaSetterPrefix(Farmer __instance, ref float __state)
    {
        __state = __instance.Stamina;
    }

    /// <summary>Double stamina consumption when cursed.</summary>
    [HarmonyPostfix]
    private static void FarmerStaminaSetterPostfix(Farmer __instance, float __state, float value)
    {
        if (!__instance.Read<bool>(DataFields.Cursed))
        {
            return;
        }

        var diff = __state - value;
        if (diff <= 0)
        {
            return;
        }

        __instance.stamina -= diff;
        if (__instance.stamina < 0)
        {
            __instance.stamina = 0;
        }
    }

    #endregion harmony patches
}
