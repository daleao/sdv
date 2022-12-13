namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1CreateObjectDebrisPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1CreateObjectDebrisPatcher"/> class.</summary>
    internal Game1CreateObjectDebrisPatcher()
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
            if (!who.HasProfession(Profession.Gemologist) || !new SObject(objectIndex, 1).IsGemOrMineral())
            {
                return true; // run original logic
            }

            location.debris.Add(new Debris(
                objectIndex,
                new Vector2((xTile * 64) + 32, (yTile * 64) + 32),
                who.getStandingPosition()) { itemQuality = who.GetGemologistMineralQuality() });

            who.Increment(DataFields.GemologistMineralsCollected);
            var collected = who.Read<int>(DataFields.GemologistMineralsCollected);
            if (collected == ProfessionsModule.Config.MineralsNeededForBestQuality / 2)
            {
                Game1.game1.GlobalUpgradeCrystalariums(SObject.highQuality, who);
            }
            else if (collected == ProfessionsModule.Config.MineralsNeededForBestQuality)
            {
                Game1.game1.GlobalUpgradeCrystalariums(SObject.bestQuality, who);
            }

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
