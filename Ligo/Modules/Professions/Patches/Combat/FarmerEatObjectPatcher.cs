namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerEatObjectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerEatObjectPatcher"/> class.</summary>
    internal FarmerEatObjectPatcher()
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
