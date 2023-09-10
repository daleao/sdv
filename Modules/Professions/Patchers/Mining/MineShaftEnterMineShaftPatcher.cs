namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftEnterMineShaftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftEnterMineShaftPatcher"/> class.</summary>
    internal MineShaftEnterMineShaftPatcher()
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.enterMineShaft));
    }

    #region harmony patches

    /// <summary>Increment Spelunker buff on shaft interaction.</summary>
    [HarmonyPostfix]
    private static void MineShaftEnterMineShaftPostfix()
    {
        if (Game1.player.HasProfession(Profession.Spelunker))
        {
            Game1.player.AddSpelunkerMomentum();
        }
    }

    #endregion harmony patches
}
