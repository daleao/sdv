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
        var power = who.toolPower.Value;
        if (power <= 0)
        {
            return;
        }

        who.Stamina -= (int)Math.Max((2 * (power + 1)) - (who.ForagingLevel * 0.1f), 0.1f) * (int)Math.Pow(power, 2d) *
                       Config.Axe.StaminaCostMultiplier;
    }
}
