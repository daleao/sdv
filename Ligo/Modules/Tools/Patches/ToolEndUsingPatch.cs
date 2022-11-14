namespace DaLion.Ligo.Modules.Tools.Patches;

#region using directives

using System.Linq;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

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
            Axe => ModEntry.Config.Tools.Axe.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            Pickaxe => ModEntry.Config.Tools.Pick.RadiusAtEachPowerLevel.ElementAtOrDefault(power - 1),
            _ => 1,
        };

        ModEntry.State.Tools.Shockwave = new Shockwave(radius, who, Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
    }

    #endregion harmony patches
}
