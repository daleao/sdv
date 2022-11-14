namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class PickaxeDoFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PickaxeDoFunctionPatcher"/> class.</summary>
    internal PickaxeDoFunctionPatcher()
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
