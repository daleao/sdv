namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerEatObjectPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerEatObjectPatch()
    {
        Target = RequireMethod<Farmer>(nameof(Farmer.eatObject));
    }

    #region harmony patches

    /// <summary>Patch to prevent Frenzied Brute from eating.</summary>
    [HarmonyPrefix]
    private static bool FarmerEatObjectPrefix()
    {
        if (Game1.player.get_IsUltimateActive().Value) return true; // run original logic

        Game1.playSound("cancel");
        Game1.showRedMessage(ModEntry.i18n.Get("ulti.canteat"));
        return false; // don't run original logic
    }

    #endregion harmony patches
}