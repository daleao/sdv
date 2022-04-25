namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal class FarmerEatObjectPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerEatObjectPatch()
    {
        Original = RequireMethod<Farmer>(nameof(Farmer.eatObject));
    }

    #region harmony patches

    /// <summary>Patch to prevent Frenzied Brute from eating.</summary>
    [HarmonyPrefix]
    private static bool FarmerEatObjectPrefix()
    {
        if (ModEntry.PlayerState.RegisteredUltimate?.IsActive != true) return true; // run original logic

        Game1.playSound("cancel");
        Game1.showRedMessage(ModEntry.ModHelper.Translation.Get("ulti.canteat"));
        return false; // don't run original logic
    }

    #endregion harmony patches
}