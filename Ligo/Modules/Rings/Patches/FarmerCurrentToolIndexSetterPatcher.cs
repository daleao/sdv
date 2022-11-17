namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using DaLion.Ligo.Modules.Rings.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolIndexSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolIndexSetterPatcher"/> class.</summary>
    internal FarmerCurrentToolIndexSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.CurrentToolIndex));
    }

    #region harmony patches

    /// <summary>Reset applied arsenal resonances.</summary>
    [HarmonyPrefix]
    private static void FarmerCurrentToolIndexSetterPrefix(Farmer __instance, NetInt ___currentToolIndex)
    {
        if (!ModEntry.Config.EnableArsenal)
        {
            return;
        }

        var currentItem = __instance.Items[___currentToolIndex];
        switch (currentItem)
        {
            case MeleeWeapon weapon:
                weapon.RemoveResonances();
                break;
            case Slingshot slingshot:
                slingshot.RemoveResonances();
                break;
            default:
                return;
        }
    }

    /// <summary>Reset applied arsenal resonances.</summary>
    [HarmonyPostfix]
    private static void FarmerCurrentToolIndexSetterPostfix(Farmer __instance, NetInt ___currentToolIndex)
    {
        if (!ModEntry.Config.EnableArsenal)
        {
            return;
        }

        var currentItem = __instance.Items[___currentToolIndex];
        switch (currentItem)
        {
            case MeleeWeapon weapon:
                weapon.RecalculateResonances();
                break;
            case Slingshot slingshot:
                slingshot.RecalculateResonances();
                break;
            default:
                return;
        }
    }

    #endregion harmony patches
}
