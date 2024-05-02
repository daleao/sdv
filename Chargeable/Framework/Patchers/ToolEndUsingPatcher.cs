namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using System.Linq;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[HarmonyPatch(typeof(Tool), nameof(Tool.endUsing))]
internal sealed class ToolEndUsingPatcher
{
    /// <summary>Do shockwave.</summary>
    private static void Postfix(Farmer who)
    {
        var tool = who.CurrentTool;
        if (who.toolPower.Value <= 0 || tool is not (Axe or Pickaxe))
        {
            return;
        }

        var power = who.toolPower.Value;
        uint radius = tool switch
        {
            Axe => Config.Axe.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            Pickaxe => Config.Pick.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            _ => 1,
        };

        State.Shockwaves.Add(
            new Shockwave(radius, who, Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
    }
}
