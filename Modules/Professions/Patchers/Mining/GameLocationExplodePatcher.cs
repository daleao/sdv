namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Classes;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
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
        CreateExtraDebris(
            __instance,
            circle,
            chanceModifier,
            isBlaster,
            isPrestigedBlaster,
            isDemolitionist,
            isPrestigedDemolitionist,
            r,
            who);

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

    private static void CreateExtraDebris(
        GameLocation __instance,
        CircleTileGrid circle,
        double chanceModifier,
        bool isBlaster,
        bool isPrestigedBlaster,
        bool isDemolitionist,
        bool isPrestigedDemolitionist,
        Random r,
        Farmer who)
    {
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
                    Blaster_MineShaftCheckStoneForItems(
                        __instance,
                        chanceModifier,
                        tile,
                        tileObj,
                        tileX,
                        tileY,
                        isPrestigedBlaster,
                        r,
                        who);
                }
                else
                {
                    Blaster_GameLocationOnStoneDestroyed(
                        __instance,
                        tileObj,
                        tileX,
                        tileY,
                        isPrestigedBlaster,
                        r,
                        who);
                    Blaster_GameLocationBreakStone(
                        __instance,
                        chanceModifier,
                        tileX,
                        tileY,
                        isPrestigedBlaster,
                        r,
                        who);
                }
            }

            if (!isDemolitionist || r.NextDouble() >= (isPrestigedDemolitionist ? 0.4 : 0.2))
            {
                continue;
            }

            Demolitionist_BonusQiBeans(
                __instance,
                tileX,
                tileY,
                isPrestigedDemolitionist,
                r,
                who);
            Demolitionist_GameLocationOnStoneDestroyed(
                __instance,
                tileObj,
                tileX,
                tileY,
                isPrestigedDemolitionist,
                r,
                who);
            Demolitionist_VolcanoDungeonBreakStone(
                __instance,
                tile,
                tileObj,
                isPrestigedDemolitionist,
                r,
                who);
            Demolitionist_GameLocationBreakStone(
                __instance,
                chanceModifier,
                tileObj,
                tileX,
                tileY,
                isPrestigedDemolitionist,
                r,
                who);
        }
    }

    private static void Blaster_MineShaftCheckStoneForItems(
        GameLocation __instance,
        double chanceModifier,
        Vector2 tile,
        SObject tileObj,
        int tileX,
        int tileY,
        bool isPrestigedBlaster,
        Random r,
        Farmer who)
    {
        // this method calls GameLocation.breakStone which also produces coal, but only outdoors which never applies here
        if (r.NextDouble() < 0.5 *
            (1.0 + chanceModifier) * // we multiplied this by x10, from 0.05 to 0.5, because vanilla is super stingy and it's impossible to get any coal
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

        var success = 0.5 * (1.0 + chanceModifier) * (tileObj.ParentSheetIndex is 40 or 42 ? 1.2 : 0.8);
        // since I'm generous, add a whole third check for prestiged
        if (!isPrestigedBlaster || r.NextDouble() > success + ((1 - success) * 0.4375))
        {
            return;
        }

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

    private static void Blaster_GameLocationOnStoneDestroyed(
        GameLocation __instance,
        SObject tileObj,
        int tileX,
        int tileY,
        bool isPrestigedBlaster,
        Random r,
        Farmer who)
    {
        if (tileObj.ParentSheetIndex is not (343 or 450))
        {
            return;
        }

        if (Game1.stats.DaysPlayed <= 1 || (r.NextDouble() > 0.035 && (!isPrestigedBlaster || r.NextDouble() > 0.035)))
        {
            return;
        }

        Game1.createObjectDebris(
            ObjectIds.Coal,
            tileX,
            tileY,
            who.UniqueMultiplayerID,
            __instance);
        Log.D("[Blaster]: Made extra coal from GameLocation.OnStoneDestroyed!");
        if (!isPrestigedBlaster)
        {
            return;
        }

        Game1.createObjectDebris(
            ObjectIds.Coal,
            tileX,
            tileY,
            who.UniqueMultiplayerID,
            __instance);
        Log.D("[Blaster]: Made extra prestiged coal from GameLocation.OnStoneDestroyed!");
    }

    private static void Blaster_GameLocationBreakStone(
        GameLocation __instance,
        double chanceModifier,
        int tileX,
        int tileY,
        bool isPrestigedBlaster,
        Random r,
        Farmer who)
    {
        var success = 0.05 * (1.0 + chanceModifier);
        if (!(__instance.IsOutdoors || __instance.treatAsOutdoors.Value) ||
            (r.NextDouble() > success && (!isPrestigedBlaster || r.NextDouble() > success)))
        {
            return;
        }

        Game1.createObjectDebris(
            ObjectIds.Coal,
            tileX,
            tileY,
            who.UniqueMultiplayerID,
            __instance);
        Log.D("[Blaster]: Made extra coal from GameLocation.breakStone!");
        if (!isPrestigedBlaster)
        {
            return;
        }

        Game1.createObjectDebris(
            ObjectIds.Coal,
            tileX,
            tileY,
            who.UniqueMultiplayerID,
            __instance);
        Log.D("[Blaster]: Made extra prestiged coal from GameLocation.breakStone!");
    }

    private static void Demolitionist_BonusQiBeans(
        GameLocation __instance,
        int tileX,
        int tileY,
        bool isPrestigedDemolitionist,
        Random r,
        Farmer who)
    {
        if (!Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS") ||
            (r.NextDouble() > 0.02 && (!isPrestigedDemolitionist || r.NextDouble() > 0.02)))
        {
            return;
        }

        Game1.createMultipleObjectDebris(
            ObjectIds.QiBean,
            tileX,
            tileY,
            1,
            who.UniqueMultiplayerID,
            __instance);
        if (isPrestigedDemolitionist)
        {
            Game1.createMultipleObjectDebris(
                ObjectIds.QiBean,
                tileX,
                tileY,
                1,
                who.UniqueMultiplayerID,
                __instance);
        }
    }

    private static void Demolitionist_GameLocationOnStoneDestroyed(
        GameLocation __instance,
        SObject tileObj,
        int tileX,
        int tileY,
        bool isPrestigedDemolitionist,
        Random r,
        Farmer who)
    {
        if (__instance is MineShaft || tileObj.ParentSheetIndex is not (343 or 450))
        {
            return;
        }

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

    private static void Demolitionist_VolcanoDungeonBreakStone(
        GameLocation __instance,
        Vector2 tile,
        SObject tileObj,
        bool isPrestigedDemolitionist,
        Random r,
        Farmer who)
    {
        if (__instance is not VolcanoDungeon || tileObj.ParentSheetIndex is not (>= 845 and <= 847) ||
            (r.NextDouble() > 0.005 && (!isPrestigedDemolitionist || r.NextDouble() > 0.005)))
        {
            return;
        }

        Game1.createObjectDebris(
            ObjectIds.MummifiedBat,
            (int)tile.X,
            (int)tile.Y,
            who.UniqueMultiplayerID,
            __instance);
        Log.D("[Demolitionist]: Made extra stuff from VolcanoDungeon.breakStone!");
        if (!isPrestigedDemolitionist)
        {
            return;
        }

        Game1.createObjectDebris(
            ObjectIds.MummifiedBat,
            (int)tile.X,
            (int)tile.Y,
            who.UniqueMultiplayerID,
            __instance);
        Log.D("[Demolitionist]: Made extra prestiged stuff from VolcanoDungeon.breakStone!");
    }

    private static void Demolitionist_GameLocationBreakStone(
        GameLocation __instance,
        double chanceModifier,
        SObject tileObj,
        int tileX,
        int tileY,
        bool isPrestigedDemolitionist,
        Random r,
        Farmer who)
    {
        if (Maps.ResourceFromNode.TryGetValue(
                tileObj.ParentSheetIndex == ObjectIds.Stone_Node_Gemstone
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
            if (!isPrestigedDemolitionist)
            {
                return;
            }

            Game1.createObjectDebris(
                resourceId,
                tileX,
                tileY,
                who.UniqueMultiplayerID,
                __instance);
            Log.D($"[Demolitionist]: Made extra prestiged resource {resourceId} from GameLocation.breakStone!");
        }
        else if (tileObj.ParentSheetIndex == ObjectIds.Stone_Node_MysticStone &&
                 r.NextDouble() < 0.25) // special case for mystic stone dropping prismatic shard
        {
            Game1.createObjectDebris(
                ObjectIds.PrismaticShard,
                tileX,
                tileY,
                who.UniqueMultiplayerID,
                __instance);
            Log.D("[Demolitionist]: Made extra Prismatic Shard from GameLocation.breakStone!");
            if (!isPrestigedDemolitionist)
            {
                return;
            }

            Game1.createObjectDebris(
                ObjectIds.PrismaticShard,
                tileX,
                tileY,
                who.UniqueMultiplayerID,
                __instance);
            Log.D("[Demolitionist]: Made extra prestiged Prismatic Shard from GameLocation.breakStone!");
        }
        else if (__instance is MineShaft shaft)
        {
            // bonus geode
            var success = 0.022 * (1.0 + chanceModifier);
            if (r.NextDouble() < success || (isPrestigedDemolitionist && r.NextDouble() < success))
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
            success = 0.005 * (1.0 + chanceModifier);
            if (shaft.mineLevel > 20 &&
                (r.NextDouble() < success || (isPrestigedDemolitionist && r.NextDouble() < success)))
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
            success = 0.05 * (1.0 + chanceModifier) * (tileObj.ParentSheetIndex is 40 or 42 ? 1.2 : 0.8);
            if (r.NextDouble() < success || (isPrestigedDemolitionist && r.NextDouble() < success))
            {
                Game1.createObjectDebris(
                    shaft.getOreIndexForLevel(shaft.mineLevel, r),
                    tileX,
                    tileY,
                    who.UniqueMultiplayerID,
                    __instance);
                Log.D("[Demolitionist]: Made extra ore from GameLocation.breakStone!");
                if (!isPrestigedDemolitionist)
                {
                    return;
                }

                Game1.createObjectDebris(
                    shaft.getOreIndexForLevel(shaft.mineLevel, r),
                    tileX,
                    tileY,
                    who.UniqueMultiplayerID,
                    __instance);
                Log.D("[Demolitionist]: Made extra prestiged ore from GameLocation.breakStone!");
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
                if (!isPrestigedDemolitionist)
                {
                    return;
                }

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
