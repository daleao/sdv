namespace DaLion.Redux.Professions.Patches.Mining;

#region using directives

using DaLion.Redux.Core.Extensions;
using DaLion.Redux.Professions.Events.GameLoop;
using DaLion.Redux.Professions.Extensions;
using DaLion.Shared.Classes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationExplodePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationExplodePatch"/> class.</summary>
    internal GameLocationExplodePatch()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.explode));
    }

    #region harmony patches

    /// <summary>Patch for Blaster double coal chance + Demolitionist speed burst.</summary>
    [HarmonyPrefix]
    private static void GameLocationExplodePrefix(
        GameLocation __instance, Vector2 tileLocation, int radius, Farmer? who)
    {
        if (who is null)
        {
            return;
        }

        var isBlaster = who.HasProfession(Profession.Blaster);
        var isDemolitionist = who.HasProfession(Profession.Demolitionist);
        if (!isBlaster && !isDemolitionist)
        {
            return;
        }

        var isPrestigedBlaster = who.HasProfession(Profession.Blaster, true);
        var isPrestigedDemolitionist = who.HasProfession(Profession.Demolitionist, true);
        var chanceModifier = (who.DailyLuck / 2.0) + (who.LuckLevel * 0.001) + (who.MiningLevel * 0.005);
        var r = new Random(Guid.NewGuid().GetHashCode());
        var circle = new CircleTileGrid(tileLocation, radius);
        foreach (var tile in circle.Tiles)
        {
            if (!__instance.objects.TryGetValue(tile, out var tileObj) || !tileObj.IsStone())
            {
                continue;
            }

            int tileX = (int)tile.X, tileY = (int)tile.Y;
            if (isBlaster)
            {
                if (__instance is MineShaft)
                {
                    // perform check from MineShaft.checkStoneForItems
                    // this method calls GameLocation.breakStone which also produces coal, but only outside which never applies here
                    if (r.NextDouble() < 0.05 * (1.0 + chanceModifier) *
                        (tileObj.ParentSheetIndex is 40 or 42 ? 1.2 : 0.8) &&
                        (r.NextDouble() < 0.25 || (isPrestigedBlaster && r.NextDouble() < 0.25)))
                    {
                        Game1.createObjectDebris(
                            SObject.coal,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        if (isPrestigedBlaster)
                        {
                            Game1.createObjectDebris(
                                SObject.coal,
                                tileX,
                                tileY,
                                who.UniqueMultiplayerID,
                                __instance);
                        }

                        ModEntry.Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplyer").Invoke()
                            .broadcastSprites(
                                __instance,
                                new TemporaryAnimatedSprite(
                                    25,
                                    new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize),
                                    Color.White,
                                    8,
                                    Game1.random.NextDouble() < 0.5,
                                    80f,
                                    0,
                                    -1,
                                    -1f,
                                    128));
                    }

                    if (!isPrestigedBlaster)
                    {
                        continue;
                    }

                    // since I'm generous, add a whole third check for prestiged
                    if (r.NextDouble() < 0.05 * (1.0 + chanceModifier) *
                        (tileObj.ParentSheetIndex is 40 or 42 ? 1.2 : 0.8) &&
                        (r.NextDouble() < 0.25 || (isPrestigedBlaster && r.NextDouble() < 0.25)))
                    {
                        Game1.createObjectDebris(
                            SObject.coal,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        if (isPrestigedBlaster)
                        {
                            Game1.createObjectDebris(
                                SObject.coal,
                                tileX,
                                tileY,
                                who.UniqueMultiplayerID,
                                __instance);
                        }

                        ModEntry.Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplyer").Invoke()
                            .broadcastSprites(
                                __instance,
                                new TemporaryAnimatedSprite(
                                    25,
                                    new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize),
                                    Color.White,
                                    8,
                                    Game1.random.NextDouble() < 0.5,
                                    80f,
                                    0,
                                    -1,
                                    -1f,
                                    128));
                    }
                }
                else
                {
                    // perform initial check from GameLocation.OnStoneDestroyed
                    if (tileObj.ParentSheetIndex is 343 or 450)
                    {
                        if ((r.NextDouble() < 0.035 || (isPrestigedBlaster && r.NextDouble() < 0.035)) &&
                            Game1.stats.DaysPlayed > 1)
                        {
                            Game1.createObjectDebris(
                                SObject.coal,
                                tileX,
                                tileY,
                                who.UniqueMultiplayerID,
                                __instance);
                            if (isPrestigedBlaster)
                            {
                                Game1.createObjectDebris(
                                    SObject.coal,
                                    tileX,
                                    tileY,
                                    who.UniqueMultiplayerID,
                                    __instance);
                            }
                        }
                    }

                    // perform check from GameLocation.breakStone
                    if (((__instance.IsOutdoors || __instance.treatAsOutdoors.Value) &&
                         r.NextDouble() < 0.05 * (1.0 + chanceModifier)) ||
                        (isPrestigedBlaster && r.NextDouble() < 0.05 * (1.0 + chanceModifier)))
                    {
                        Game1.createObjectDebris(
                            SObject.coal,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        if (isPrestigedBlaster)
                        {
                            Game1.createObjectDebris(
                                SObject.coal,
                                tileX,
                                tileY,
                                who.UniqueMultiplayerID,
                                __instance);
                        }
                    }
                }
            }

            if (!isDemolitionist)
            {
                continue;
            }

            if (Game1.random.NextDouble() >= 0.2 * (isPrestigedDemolitionist ? 2.0 : 1.0))
            {
                continue;
            }

            // give some bonus qi beans
            if (Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS") &&
                (r.NextDouble() < 0.02 || (isPrestigedDemolitionist && r.NextDouble() < 0.02)))
            {
                Game1.createMultipleObjectDebris(
                    890,
                    tileX,
                    tileY,
                    1,
                    who.UniqueMultiplayerID,
                    __instance);
                if (isPrestigedDemolitionist)
                {
                    Game1.createMultipleObjectDebris(
                        890,
                        tileX,
                        tileY,
                        1,
                        who.UniqueMultiplayerID,
                        __instance);
                }
            }

            // perform initial checks from GameLocation.OnStoneDestroyed
            if (__instance is not MineShaft && tileObj.ParentSheetIndex is 343 or 450)
            {
                // bonus geodes
                if ((r.NextDouble() < 0.035 || (isPrestigedDemolitionist && r.NextDouble() < 0.035)) &&
                    Game1.stats.DaysPlayed > 1)
                {
                    Game1.createObjectDebris(
                        535 + (Game1.stats.DaysPlayed > 60 && r.NextDouble() < 0.2 ? 1 :
                            Game1.stats.DaysPlayed > 120 && r.NextDouble() < 0.2 ? 2 : 0),
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            535 + (Game1.stats.DaysPlayed > 60 && r.NextDouble() < 0.2 ? 1 :
                                Game1.stats.DaysPlayed > 120 && r.NextDouble() < 0.2 ? 2 : 0),
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                    }
                }

                // bonus stone
                if ((r.NextDouble() < 0.01 || (isPrestigedDemolitionist && r.NextDouble() < 0.01)) &&
                    Game1.stats.DaysPlayed > 1)
                {
                    Game1.createObjectDebris(
                        390,
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            390,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                    }
                }
            }

            // special case for VolcanoDungeon.breakStone
            if (__instance is VolcanoDungeon &&
                (tileObj.ParentSheetIndex >= 845) & (tileObj.ParentSheetIndex <= 847) && (r.NextDouble() < 0.005 ||
                    (isPrestigedDemolitionist && r.NextDouble() < 0.005)))
            {
                Game1.createObjectDebris(
                    827,
                    (int)tile.X,
                    (int)tile.Y,
                    who.UniqueMultiplayerID,
                    __instance);
                if (isPrestigedDemolitionist)
                {
                    Game1.createObjectDebris(
                        827,
                        (int)tile.X,
                        (int)tile.Y,
                        who.UniqueMultiplayerID,
                        __instance);
                }
            }

            // whether MineShaft or not, ends up calling GameLocation.breakStone
            if (Collections.ResourceFromStoneId.TryGetValue(
                    tileObj.ParentSheetIndex == 44
                        ? r.Next(1, 8) * 2
                        : tileObj.ParentSheetIndex, // replace gem node with random, well, gem node
                    out var resourceIndex))
            {
                Game1.createObjectDebris(
                    resourceIndex,
                    tileX,
                    tileY,
                    who.UniqueMultiplayerID,
                    __instance);
                if (isPrestigedDemolitionist)
                {
                    Game1.createObjectDebris(
                        resourceIndex,
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                }
            }
            else if (tileObj.ParentSheetIndex == 46 &&
                     r.NextDouble() < 0.25) // special case for mystic stone dropping prismatic shard
            {
                Game1.createObjectDebris(
                    74,
                    tileX,
                    tileY,
                    who.UniqueMultiplayerID,
                    __instance);
                if (isPrestigedDemolitionist)
                {
                    Game1.createObjectDebris(
                        74,
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                }
            }
            else if (__instance is MineShaft shaft)
            {
                // bonus geode
                if (r.NextDouble() < 0.022 * (1.0 + chanceModifier) ||
                    (isPrestigedDemolitionist && r.NextDouble() < 0.022 * (1.0 + chanceModifier)))
                {
                    var mineArea = shaft.getMineArea();
                    var whichGeode = mineArea == 121
                        ? 749
                        : 535 + mineArea switch
                        {
                            40 => 1,
                            80 => 2,
                            _ => 0,
                        };

                    Game1.createObjectDebris(
                        whichGeode,
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            whichGeode,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                    }
                }

                // bonus omni geode
                if (shaft.mineLevel > 20 && (r.NextDouble() < 0.005 * (1.0 + chanceModifier) ||
                                             (isPrestigedDemolitionist &&
                                              r.NextDouble() < 0.005 * (1.0 + chanceModifier))))
                {
                    Game1.createObjectDebris(
                        749,
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            749,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                    }
                }

                // bonus resource
                if (r.NextDouble() <
                    0.05 * (1.0 + chanceModifier) * (tileObj.ParentSheetIndex is 40 or 42 ? 1.2 : 0.8) ||
                    (isPrestigedDemolitionist && r.NextDouble() < 0.05 * (1.0 + chanceModifier) *
                        (tileObj.ParentSheetIndex is 40 or 42 ? 1.2 : 0.8)))
                {
                    Game1.createObjectDebris(
                        shaft.getOreIndexForLevel(shaft.mineLevel, r),
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            shaft.getOreIndexForLevel(shaft.mineLevel, r),
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                    }
                }
                else if (r.NextDouble() < 0.5 || (isPrestigedDemolitionist && r.NextDouble() < 0.5))
                {
                    Game1.createDebris(
                        14,
                        tileX,
                        tileY,
                        1,
                        __instance);
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createDebris(
                            14,
                            tileX,
                            tileY,
                            1,
                            __instance);
                    }
                }
            }
        }

        if (!who.IsLocalPlayer || !isDemolitionist || !ModEntry.Config.Professions.EnableGetExcited)
        {
            return;
        }

        // get excited speed buff
        var distanceFromEpicenter = (int)(tileLocation - who.getTileLocation()).Length();
        if (distanceFromEpicenter < (radius * 2) + 1)
        {
            ModEntry.State.Professions.DemolitionistExcitedness = 4;
        }

        if (distanceFromEpicenter < radius + 1)
        {
            ModEntry.State.Professions.DemolitionistExcitedness += 2;
        }

        ModEntry.Events.Enable<DemolitionistUpdateTickedEvent>();
    }

    #endregion harmony patches
}
