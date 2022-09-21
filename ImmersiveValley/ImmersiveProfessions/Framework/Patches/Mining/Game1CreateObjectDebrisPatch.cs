namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using System;
using System.Reflection;
using DaLion.Common;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Professions.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1CreateObjectDebrisPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="Game1CreateObjectDebrisPatch"/> class.</summary>
    internal Game1CreateObjectDebrisPatch()
    {
        this.Target = this.RequireMethod<Game1>(
            nameof(Game1.createObjectDebris),
            new[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) });
    }

    #region harmony patches

    /// <summary>Patch for Gemologist mineral quality and increment counter for mined minerals.</summary>
    [HarmonyPrefix]
    private static bool Game1CreateObjectDebrisPrefix(
        int objectIndex, int xTile, int yTile, long whichPlayer, GameLocation location)
    {
        try
        {
            var who = Game1.getFarmer(whichPlayer);
            if (!who.HasProfession(Profession.Gemologist) || !new SObject(objectIndex, 1).IsPreciousRock())
            {
                return true; // run original logic
            }

            location.debris.Add(new Debris(
                objectIndex,
                new Vector2((xTile * 64) + 32, (yTile * 64) + 32),
                who.getStandingPosition()) { itemQuality = who.GetGemologistMineralQuality() });

            who.Increment("GemologistMineralsCollected");
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
