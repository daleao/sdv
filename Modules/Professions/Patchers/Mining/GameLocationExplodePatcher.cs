namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Classes;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using DaLion.Shared.Maps;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationExplodePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationExplodePatcher"/> class.</summary>
    internal GameLocationExplodePatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.explode));
    }

    #region harmony patches

    /// <summary>Patch for Blaster double coal chance + Demolitionist speed burst.</summary>
    [HarmonyPostfix]
    private static void GameLocationExplodePostfix(
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
        var circle = new CircleTileGrid(tileLocation, (uint)radius);

        // this behemoth aggregates resource drops from at least 3 different vanilla methods
        // it's not entirely clear when each one is used, but they are all replicated here to be sure
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
                    if (r.NextDouble() < 0.5 * (1.0 + chanceModifier) * // we multiplied this by x10, from 0.05 to 0.5, because vanilla is super stingy and it's impossible to get any coal
                        (tileObj.ParentSheetIndex is 40 or 42 ? 1.2 : 0.8) &&
                        (r.NextDouble() < 0.25 || (isPrestigedBlaster && r.NextDouble() < 0.25)))
                    {
                        Game1.createObjectDebris(
                            ObjectIds.Coal,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        Log.D("[Blaster]: Made extra coal from MineShaft.checkStoneForItems!");
                        if (isPrestigedBlaster)
                        {
                            Game1.createObjectDebris(
                                ObjectIds.Coal,
                                tileX,
                                tileY,
                                who.UniqueMultiplayerID,
                                __instance);
                            Log.D("[Blaster]: Made extra prestiged coal from MineShaft.checkStoneForItems!");
                        }

                        Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
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

                    if (isPrestigedBlaster)
                    {
                        // since I'm generous, add a whole third check for prestiged
                        if (r.NextDouble() < 0.5 * (1.0 + chanceModifier) *
                            (tileObj.ParentSheetIndex is 40 or 42 ? 1.2 : 0.8) &&
                            (r.NextDouble() < 0.25 || (isPrestigedBlaster && r.NextDouble() < 0.25)))
                        {
                            Game1.createObjectDebris(
                                ObjectIds.Coal,
                                tileX,
                                tileY,
                                who.UniqueMultiplayerID,
                                __instance);
                            Log.D("[Blaster]: Made even more extra coal from MineShaft.checkStoneForItems!");
                            if (isPrestigedBlaster)
                            {
                                Game1.createObjectDebris(
                                    ObjectIds.Coal,
                                    tileX,
                                    tileY,
                                    who.UniqueMultiplayerID,
                                    __instance);
                                Log.D(
                                    "[Blaster]: Made even more extra prestiged coal from MineShaft.checkStoneForItems");
                            }

                            Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
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
                                ObjectIds.Coal,
                                tileX,
                                tileY,
                                who.UniqueMultiplayerID,
                                __instance);
                            Log.D("[Blaster]: Made extra coal from GameLocation.OnStoneDestroyed!");
                            if (isPrestigedBlaster)
                            {
                                Game1.createObjectDebris(
                                    ObjectIds.Coal,
                                    tileX,
                                    tileY,
                                    who.UniqueMultiplayerID,
                                    __instance);
                                Log.D("[Blaster]: Made extra prestiged coal from GameLocation.OnStoneDestroyed!");
                            }
                        }
                    }

                    // perform check from GameLocation.breakStone
                    if (((__instance.IsOutdoors || __instance.treatAsOutdoors.Value) &&
                         r.NextDouble() < 0.05 * (1.0 + chanceModifier)) ||
                        (isPrestigedBlaster && r.NextDouble() < 0.05 * (1.0 + chanceModifier)))
                    {
                        Game1.createObjectDebris(
                            ObjectIds.Coal,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        Log.D("[Blaster]: Made extra coal from GameLocation.breakStone!");
                        if (isPrestigedBlaster)
                        {
                            Game1.createObjectDebris(
                                ObjectIds.Coal,
                                tileX,
                                tileY,
                                who.UniqueMultiplayerID,
                                __instance);
                            Log.D("[Blaster]: Made extra prestiged coal from GameLocation.breakStone!");
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
                    Log.D("[Demolitionist]: Made extra geodes!");
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            535 + (Game1.stats.DaysPlayed > 60 && r.NextDouble() < 0.2 ? 1 :
                                Game1.stats.DaysPlayed > 120 && r.NextDouble() < 0.2 ? 2 : 0),
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        Log.D("[Demolitionist]: Made extra prestiged geodes!");
                    }
                }

                // bonus stone
                if ((r.NextDouble() < 0.01 || (isPrestigedDemolitionist && r.NextDouble() < 0.01)) &&
                    Game1.stats.DaysPlayed > 1)
                {
                    Game1.createObjectDebris(
                        ObjectIds.Stone,
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                    Log.D("[Demolitionist]: Made extra stone!");
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            ObjectIds.Stone,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        Log.D("[Demolitionist]: Made extra prestiged stone!");
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
                Log.D("[Demolitionist]: Made extra stuff from VolcanoDungeon.breakStone!");
                if (isPrestigedDemolitionist)
                {
                    Game1.createObjectDebris(
                        827,
                        (int)tile.X,
                        (int)tile.Y,
                        who.UniqueMultiplayerID,
                        __instance);
                    Log.D("[Demolitionist]: Made extra prestiged stuff from VolcanoDungeon.breakStone!");
                }
            }

            // whether MineShaft or not, ends up calling GameLocation.breakStone
            if (ResourceFromNodeId.Map.TryGetValue(
                    tileObj.ParentSheetIndex == 44
                        ? r.Next(1, 8) * 2
                        : tileObj.ParentSheetIndex, // replace gem node with random, well, gem node
                    out var resourceId))
            {
                Game1.createObjectDebris(
                    resourceId,
                    tileX,
                    tileY,
                    who.UniqueMultiplayerID,
                    __instance);
                Log.D($"[Demolitionist]: Made extra resource {resourceId} from GameLocation.breakStone!");
                if (isPrestigedDemolitionist)
                {
                    Game1.createObjectDebris(
                        resourceId,
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                    Log.D($"[Demolitionist]: Made extra prestiged resource {resourceId} from GameLocation.breakStone!");
                }
            }
            else if (tileObj.ParentSheetIndex == 46 &&
                     r.NextDouble() < 0.25) // special case for mystic stone dropping prismatic shard
            {
                Game1.createObjectDebris(
                    SObject.prismaticShardIndex,
                    tileX,
                    tileY,
                    who.UniqueMultiplayerID,
                    __instance);
                Log.D("[Demolitionist]: Made extra Prismatic Shard from GameLocation.breakStone!");
                if (isPrestigedDemolitionist)
                {
                    Game1.createObjectDebris(
                        SObject.prismaticShardIndex,
                        tileX,
                        tileY,
                        who.UniqueMultiplayerID,
                        __instance);
                    Log.D("[Demolitionist]: Made extra prestiged Prismatic Shard from GameLocation.breakStone!");
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
                    Log.D("[Demolitionist]: Made extra geode from GameLocation.breakStone!");
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            whichGeode,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        Log.D("[Demolitionist]: Made extra prestiged geode from GameLocation.breakStone!");
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
                    Log.D("[Demolitionist]: Made extra Omni Geode from GameLocation.breakStone!");
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            749,
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        Log.D("[Demolitionist]: Made extra prestiged Omni Geode from GameLocation.breakStone!");
                    }
                }

                // bonus ore
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
                    Log.D("[Demolitionist]: Made extra ore from GameLocation.breakStone!");
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createObjectDebris(
                            shaft.getOreIndexForLevel(shaft.mineLevel, r),
                            tileX,
                            tileY,
                            who.UniqueMultiplayerID,
                            __instance);
                        Log.D("[Demolitionist]: Made extra prestiged ore from GameLocation.breakStone!");
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
                    Log.D("[Demolitionist]: Made extra something...");
                    if (isPrestigedDemolitionist)
                    {
                        Game1.createDebris(
                            14,
                            tileX,
                            tileY,
                            1,
                            __instance);
                        Log.D("[Demolitionist]: Made extra prestiged something...");
                    }
                }
            }
        }

        if (!who.IsLocalPlayer || !isDemolitionist || !ProfessionsModule.Config.DemolitionistGetExcited)
        {
            return;
        }

        // get excited speed buff
        var distanceFromEpicenter = (int)(tileLocation - who.getTileLocation()).Length();
        if (distanceFromEpicenter <= radius + 1)
        {
            ProfessionsModule.State.DemolitionistExcitedness = 4;
        }

        if (distanceFromEpicenter <= (radius / 2) + 1)
        {
            ProfessionsModule.State.DemolitionistExcitedness += 2;
        }

        if (ProfessionsModule.State.DemolitionistExcitedness > 0)
        {
            EventManager.Enable<DemolitionistUpdateTickedEvent>();
        }
    }

    #endregion harmony patches
}
