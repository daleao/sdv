namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using System.Linq;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolEndUsingPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ToolEndUsingPatch"/> class.</summary>
    internal ToolEndUsingPatch()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.endUsing));
    }

    #region harmony patches

    /// <summary>Do shockwave.</summary>
    [HarmonyPostfix]
    private static void ToolEndUsingPostfix(Farmer who)
    {
        var tool = who.CurrentTool;
        if (who.toolPower <= 0 || tool is not (Axe or Pickaxe))
        {
            return;
        }

        var power = who.toolPower;
#pragma warning disable CS8509
        var radius = tool switch
#pragma warning restore CS8509
        {
            Axe => ModEntry.Config.AxeConfig.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            Pickaxe => ModEntry.Config.PickaxeConfig.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            _ => 1,
        };

        ModEntry.State.Shockwave = new Shockwave(radius, who, Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
    }

    #endregion harmony patches
}
