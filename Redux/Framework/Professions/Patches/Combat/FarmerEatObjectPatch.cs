namespace DaLion.Redux.Framework.Professions.Patches.Combat;

#region using directives

using DaLion.Redux.Framework.Professions.VirtualProperties;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerEatObjectPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerEatObjectPatch"/> class.</summary>
    internal FarmerEatObjectPatch()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.eatObject));
    }

    #region harmony patches

    /// <summary>Patch to prevent Frenzied Brute from eating.</summary>
    [HarmonyPrefix]
    private static bool FarmerEatObjectPrefix()
    {
        if (Game1.player.Get_Ultimate()?.IsActive != true)
        {
            return true; // run original logic
        }

        Game1.playSound("cancel");
        Game1.showRedMessage(ModEntry.i18n.Get("ulti.canteat"));
        return false; // don't run original logic
    }

    #endregion harmony patches
}
