namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class AxeDoFunctionPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal AxeDoFunctionPatch()
    {
        Target = RequireMethod<Axe>(nameof(Axe.DoFunction));
    }

    #region harmony patches

    /// <summary>Charge shockwave stamina cost.</summary>
    [HarmonyPostfix]
    private static void AxeDoFunctionPostfix(Farmer who)
    {
        var power = who.toolPower;
        if (power <= 0) return;

        who.Stamina -=
            (int)Math.Round(Math.Sqrt(Math.Max(2 * (power + 1) - who.ForagingLevel * 0.1f, 0.1f) *
                                      (int)Math.Pow(2d * (power + 1), 2d))) *
            (float)Math.Pow(ModEntry.Config.StaminaCostMultiplier, power);
    }

    #endregion harmony patches
}