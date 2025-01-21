namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1CreateObjectDebrisPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1CreateObjectDebrisPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal Game1CreateObjectDebrisPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Game1>(
            nameof(Game1.createObjectDebris),
            [typeof(string), typeof(int), typeof(int), typeof(long), typeof(GameLocation)]);
    }

    #region harmony patches

    /// <summary>Patch for Gemologist mineral quality and increment counter for mined minerals.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Game1CreateObjectDebrisPrefix(
        string id, int xTile, int yTile, long whichPlayer, GameLocation location)
    {
        var who = Game1.GetPlayer(whichPlayer, onlyOnline: true);
        if (who?.HasProfession(Profession.Gemologist) != true)
        {
            return true; // run original logic
        }

        var debris = ItemRegistry.Create(id);
        if (!debris.IsGemOrMineral())
        {
            return true; // run original logic
        }

        try
        {
            Data.AppendToGemologistMineralsCollected(debris.ItemId, who);
            location.debris.Add(new Debris(
                id,
                new Vector2((xTile * 64) + 32, (yTile * 64) + 32),
                who.getStandingPosition()) { itemQuality = who.GetGemologistMineralQuality() });
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
