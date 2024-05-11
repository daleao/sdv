namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using DaLion.Shared.Classes;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationExplodePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationExplodePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationExplodePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.explode));
    }

    #region harmony patches

    /// <summary>Patch to increase Demolitionist explosion radius.</summary>
    [HarmonyPrefix]
    private static void GameLocationExplodePrefix(ref int radius, Farmer? who, bool destroyObjects)
    {
        if (destroyObjects && who?.HasProfession(Profession.Demolitionist) == true)
        {
            radius++;
        }
    }

    /// <summary>Patch for Blaster double coal chance + Demolitionist speed burst.</summary>
    [HarmonyPostfix]
    private static void GameLocationExplodePostfix(
        GameLocation __instance, Vector2 tileLocation, int radius, Farmer? who, int damage_amount, bool destroyObjects)
    {
        if (who is null || !destroyObjects)
        {
            return;
        }

        var isBlaster = who.HasProfession(Profession.Blaster);
        var isDemolitionist = who.HasProfession(Profession.Demolitionist);
        if (!isBlaster && !isDemolitionist)
        {
            return;
        }

        radius *= 2; // the body of GameLocation.explode divides the radius by 2 to produce HoeDirt, so we need this to restore the original explosion radius
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

        if (!isDemolitionist)
        {
            return;
        }

        // get excited speed buff
        if (who.IsLocalPlayer)
        {
            var distanceFromEpicenter = who.SquaredTileDistance(tileLocation);
            if (distanceFromEpicenter <= radius * radius)
            {
                State.DemolitionistAdrenaline = 4;
            }

            // ReSharper disable once PossibleLossOfFraction
            if (distanceFromEpicenter <= radius / 2)
            {
                State.DemolitionistAdrenaline += 2;
            }
        }

        if (who.HasProfession(Profession.Demolitionist, true) && radius > 2)
        {
            State.ChainedExplosions.Add(new ChainedExplosion(__instance, tileLocation, radius, damage_amount, who));
        }
    }

    #endregion harmony patches

    private static void CreateExtraDebris(
        GameLocation location,
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
            if (!location.objects.TryGetValue(tile, out var tileObj) || !tileObj.IsStone())
            {
                continue;
            }

            int tileX = (int)tile.X, tileY = (int)tile.Y;
            if (isBlaster)
            {
                if (location.IsOutdoors || location.treatAsOutdoors.Value)
                {
                    Blaster_GameLocationBreakStone(
                        location,
                        chanceModifier,
                        tileX,
                        tileY,
                        isPrestigedBlaster,
                        r,
                        who);
                }

                if (!MineShaft.IsGeneratedLevel(location, out _))
                {
                    Blaster_GameLocationOnStoneDestroyed(
                        location,
                        tileObj,
                        tileX,
                        tileY,
                        isPrestigedBlaster,
                        r,
                        who);
                }
                else
                {
                    Blaster_MineShaftCheckStoneForItems(
                        location,
                        chanceModifier,
                        tile,
                        tileObj,
                        tileX,
                        tileY,
                        isPrestigedBlaster,
                        r,
                        who);
                }
            }

            if (!isDemolitionist || (!isPrestigedDemolitionist && !r.NextBool()))
            {
                continue;
            }

            location.OnStoneDestroyed(tileObj.ItemId, tileX, tileY, who);
            Log.D("[Demolitionist]: Invoked additional call to OnStoneDestroyed");
        }
    }

    private static void Blaster_MineShaftCheckStoneForItems(
        GameLocation location,
        double chanceModifier,
        Vector2 tile,
        SObject stone,
        int tileX,
        int tileY,
        bool isPrestigedBlaster,
        Random r,
        Farmer who)
    {
        // NOTE: this method also calls GameLocation.breakStone which also produces coal, but only outdoors which never applies here

        var stoneModifier = stone.ItemId is "40" or "42" ? 1.2 : 0.8;
        var chance = 0.05 * (1.0 + chanceModifier) * stoneModifier;
        chance *= 2d; // vanilla is so stingy that it's not viable to play as a Blaster, so we give this another x2 buff
        if (!r.NextBool(chance))
        {
            return;
        }

        const double blasterMultiplier = 2d;
        var addedCoalChance = who.hasBuff("dwarfStatue_2") ? 0.1 : 0.0;
        chance = (0.25 * blasterMultiplier) + addedCoalChance;
        if (r.NextBool(chance) || (isPrestigedBlaster && r.NextBool(chance)))
        {
            Game1.createObjectDebris(
                QualifiedObjectIds.Coal,
                tileX,
                tileY,
                who.UniqueMultiplayerID,
                location);
            Log.D("[Blaster]: Made extra coal from MineShaft.checkStoneForItems!");
            if (isPrestigedBlaster)
            {
                Game1.createObjectDebris(
                    QualifiedObjectIds.Coal,
                    tileX,
                    tileY,
                    who.UniqueMultiplayerID,
                    location);
                Log.D("[Blaster]: Made extra prestiged coal from MineShaft.checkStoneForItems!");
            }

            Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                .broadcastSprites(
                    location,
                    new TemporaryAnimatedSprite(
                        25,
                        new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize),
                        Color.White,
                        8,
                        Game1.random.NextBool(),
                        80f,
                        0,
                        -1,
                        -1f,
                        128));
        }
    }

    private static void Blaster_GameLocationOnStoneDestroyed(
        GameLocation location,
        SObject stone,
        int tileX,
        int tileY,
        bool isPrestigedBlaster,
        Random r,
        Farmer who)
    {
        if (stone.ItemId is not ("343" or "450") || Game1.stats.DaysPlayed <= 1)
        {
            return;
        }

        const double blasterMultiplier = 2d;
        var addedCoalChance = who.hasBuff("dwarfStatue_2") ? 0.03 : 0.0;
        var chance = (0.035 * blasterMultiplier) + addedCoalChance;
        if (r.NextBool(chance) || (isPrestigedBlaster && r.NextBool(chance)))
        {
            Game1.createObjectDebris(
                QualifiedObjectIds.Coal,
                tileX,
                tileY,
                who.UniqueMultiplayerID,
                location);
            Log.D("[Blaster]: Made extra coal from GameLocation.OnStoneDestroyed!");
            if (isPrestigedBlaster)
            {
                Game1.createObjectDebris(
                    QualifiedObjectIds.Coal,
                    tileX,
                    tileY,
                    who.UniqueMultiplayerID,
                    location);
                Log.D("[Blaster]: Made extra prestiged coal from GameLocation.OnStoneDestroyed!");
            }
        }
    }

    private static void Blaster_GameLocationBreakStone(
        GameLocation location,
        double chanceModifier,
        int tileX,
        int tileY,
        bool isPrestigedBlaster,
        Random r,
        Farmer who)
    {
        var coalChance = 0.05 * (1.0 + chanceModifier);
        coalChance += who.hasBuff("dwarfStatue_2") ? 0.025 : 0.0;
        if (r.NextBool(coalChance) || (isPrestigedBlaster && r.NextBool(coalChance)))
        {
            Game1.createObjectDebris(
                QualifiedObjectIds.Coal,
                tileX,
                tileY,
                who.UniqueMultiplayerID,
                location);
            Log.D("[Blaster]: Made extra coal from GameLocation.breakStone!");
            if (isPrestigedBlaster)
            {
                Game1.createObjectDebris(
                    QualifiedObjectIds.Coal,
                    tileX,
                    tileY,
                    who.UniqueMultiplayerID,
                    location);
                Log.D("[Blaster]: Made extra prestiged coal from GameLocation.breakStone!");
            }
        }
    }
}
