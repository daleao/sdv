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
        if (!who.IsLocalPlayer)
        {
            return;
        }

        var tool = who.CurrentTool;
        var power = who.toolPower.Value;
        if (power <= 0 || tool is not (Axe or Pickaxe))
        {
            return;
        }

        var radius = tool switch
        {
            Axe => Config.Axe.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            Pickaxe => Config.Pick.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            _ => 1,
        };

        if (radius <= 0)
        {
            return;
        }

        State.Shockwaves.Add(
            new Shockwave(radius, who, Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
    }
}
