namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[HarmonyPatch(typeof(Axe), nameof(Axe.DoFunction))]
internal sealed class AxeDoFunctionPatcher
{
    /// <summary>Charge shockwave stamina cost.</summary>
    private static void Postfix(Farmer who)
    {
        var power = who.toolPower;
        if (power <= 0)
        {
            return;
        }

        who.Stamina -=
            (int)Math.Round(Math.Sqrt(Math.Max((2 * (power + 1)) - (who.ForagingLevel * 0.1f), 0.1f) *
                                      (int)Math.Pow(2d * (power + 1), 2d))) *
            (float)Math.Pow(Config.Axe.StaminaCostMultiplier, power);
    }
}
