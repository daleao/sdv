namespace DaLion.Redux.Framework.Tools.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class AxeDoFunctionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="AxeDoFunctionPatch"/> class.</summary>
    internal AxeDoFunctionPatch()
    {
        this.Target = this.RequireMethod<Axe>(nameof(Axe.DoFunction));
    }

    #region harmony patches

    /// <summary>Charge shockwave stamina cost.</summary>
    [HarmonyPostfix]
    private static void AxeDoFunctionPostfix(Farmer who)
    {
        var power = who.toolPower;
        if (power <= 0)
        {
            return;
        }

        who.Stamina -=
            (int)Math.Round(Math.Sqrt(Math.Max((2 * (power + 1)) - (who.ForagingLevel * 0.1f), 0.1f) *
                                      (int)Math.Pow(2d * (power + 1), 2d))) *
            (float)Math.Pow(ModEntry.Config.Tools.StaminaCostMultiplier, power);
    }

    #endregion harmony patches
}
