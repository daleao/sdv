namespace DaLion.Redux.Tools.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class PickaxeDoFunctionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="PickaxeDoFunctionPatch"/> class.</summary>
    internal PickaxeDoFunctionPatch()
    {
        this.Target = this.RequireMethod<Pickaxe>(nameof(Pickaxe.DoFunction));
    }

    #region harmony patches

    /// <summary>Charge shockwave stamina cost.</summary>
    [HarmonyPostfix]
    private static void PickaxeDoFunctionPostfix(Farmer who)
    {
        var power = who.toolPower;
        if (power <= 0)
        {
            return;
        }

        who.Stamina -=
            (int)Math.Round(Math.Sqrt(Math.Max((2 * (power + 1)) - (who.MiningLevel * 0.1f), 0.1f) *
                                      (int)Math.Pow(2d * (power + 1), 2d))) *
            (float)Math.Pow(ModEntry.Config.Tools.StaminaCostMultiplier, power);
    }

    #endregion harmony patches
}
