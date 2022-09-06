namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolEndUsingPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ToolEndUsingPatch()
    {
        Target = RequireMethod<Tool>(nameof(Tool.endUsing));
    }

    #region harmony patches

    /// <summary>Do shockwave.</summary>
    [HarmonyPostfix]
    private static void ToolEndUsingPostfix(Farmer who)
    {
        var tool = who.CurrentTool;
        if (who.toolPower <= 0 || tool is not (Axe or Pickaxe)) return;

        var power = who.toolPower;
#pragma warning disable CS8509
        var radius = tool switch
#pragma warning restore CS8509
        {
            Axe => ModEntry.Config.AxeConfig.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            Pickaxe => ModEntry.Config.PickaxeConfig.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            _ => 1
        };

        ModEntry.Shockwave.Value = new(radius, who, Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
    }

    #endregion harmony patches
}