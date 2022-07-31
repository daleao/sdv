namespace DaLion.Stardew.Professions.Framework.TreasureHunts;

#region using directives

using Common.Extensions.Stardew;
using Common.Multiplayer;
using Events.Display;
using Events.GameLoop;
using Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualProperties;

#endregion using directives

/// <summary>Manages treasure hunt events for Prospector profession.</summary>
internal sealed class ProspectorHunt : TreasureHunt
{
    /// <summary>Construct an instance.</summary>
    internal ProspectorHunt()
    {
        huntStartedMessage = ModEntry.i18n.Get("prospector.huntstarted");
        huntFailedMessage = ModEntry.i18n.Get("prospector.huntfailed");
        iconSourceRect = new(48, 672, 16, 16);
    }

    #region public methods

    /// <inheritdoc />
    public override bool TryStart(GameLocation location)
    {
        if (!location.Objects.Any() || !TryStart()) return false;

        TreasureTile = ChooseTreasureTile(location);
        if (TreasureTile is null) return false;

        huntLocation = location;
        timeLimit = (uint)(location.Objects.Count() * ModEntry.Config.ProspectorHuntHandicap);
        elapsed = 0;
        ModEntry.EventManager.Enable<PointerUpdateTickedEvent>();
        ModEntry.EventManager.Enable<ProspectorHuntRenderedHudEvent>();
        ModEntry.EventManager.Enable<ProspectorHuntUpdateTickedEvent>();
        Game1.addHUDMessage(new HuntNotification(huntStartedMessage, iconSourceRect));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat($"{Game1.player.Name} is hunting for treasure.");

            if (Game1.player.HasProfession(Profession.Prospector, true))
            {
                Game1.player.get_IsHuntingTreasure().Value = true;
                if (!Context.IsMainPlayer)
                    ModEntry.Broadcaster.Message("HuntIsOn", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
                else
                    ModEntry.EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
            }
        }

        OnStarted();
        return true;
    }

    /// <inheritdoc />
    public override void ForceStart(GameLocation location, Vector2 target)
    {
        ForceStart();

        TreasureTile = target;
        huntLocation = location;
        timeLimit = (uint)(location.Objects.Count() * ModEntry.Config.ProspectorHuntHandicap);
        elapsed = 0;
        ModEntry.EventManager.Enable<PointerUpdateTickedEvent>();
        ModEntry.EventManager.Enable<ProspectorHuntRenderedHudEvent>();
        ModEntry.EventManager.Enable<ProspectorHuntUpdateTickedEvent>();
        Game1.addHUDMessage(new HuntNotification(huntStartedMessage, iconSourceRect));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat($"{Game1.player.Name} is hunting for treasure.");

            if (Game1.player.HasProfession(Profession.Prospector, true))
            {
                Game1.player.get_IsHuntingTreasure().Value = true;
                if (!Context.IsMainPlayer)
                    ModEntry.Broadcaster.Message("HuntIsOn", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
                else
                    ModEntry.EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
            }
        }

        OnStarted();
    }

    /// <inheritdoc />
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(huntFailedMessage));
        Game1.player.Write("ProspectorHuntStreak", "0");
        End(false);
    }

    #endregion public methods

    #region protected methods

    /// <inheritdoc />
    protected override Vector2? ChooseTreasureTile(GameLocation location)
    {
        Vector2 v;
        var failsafe = 0;
        do
        {
            if (failsafe > 69) return null;

            v = location.Objects.Keys.ElementAtOrDefault(random.Next(location.Objects.Keys.Count()));
            ++failsafe;
        } while (!location.Objects.TryGetValue(v, out var obj) || !obj.IsStone() || obj.IsResourceNode());

        return v;
    }

    /// <inheritdoc />
    protected override void CheckForCompletion()
    {
        if (TreasureTile is null || Game1.currentLocation.Objects.ContainsKey(TreasureTile.Value)) return;

        GetStoneTreasure();

        var shaft = (MineShaft)huntLocation;
        if (shaft.shouldCreateLadderOnThisLevel() && !shaft.GetLadderTiles().Any())
            shaft.createLadderDown((int)TreasureTile!.Value.X, (int)TreasureTile!.Value.Y);

        Game1.player.Increment("ProspectorHuntStreak");
        End(true);
    }

    /// <inheritdoc />
    protected override void End(bool found)
    {
        Game1.player.get_IsHuntingTreasure().Value = false;
        ModEntry.EventManager.Disable<ProspectorHuntRenderedHudEvent>();
        ModEntry.EventManager.Disable<ProspectorHuntUpdateTickedEvent>();
        TreasureTile = null;
        if (!Context.IsMultiplayer || Context.IsMainPlayer ||
            !Game1.player.HasProfession(Profession.Prospector, true)) return;

        Broadcaster.SendPublicChat(found
            ? $"{Game1.player.Name} has found the treasure!"
            : $"{Game1.player.Name} failed to find the treasure.");
        ModEntry.Broadcaster.Message("HuntIsOff", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);

        OnEnded(found);
    }

    #endregion protected methods

    #region private methods

    /// <summary>Spawn hunt spoils as debris.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private void GetStoneTreasure()
    {
        var mineLevel = ((MineShaft)huntLocation).mineLevel;
        Dictionary<int, int> treasuresAndQuantities = new();

        if (random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            treasuresAndQuantities.Add(890, random.Next(1, 3) + (random.NextDouble() < 0.25 ? 2 : 0)); // qi beans

        switch (random.Next(3))
        {
            case 0:
                {
                    if (mineLevel > 120 && random.NextDouble() < 0.06)
                        treasuresAndQuantities.Add(386, random.Next(1, 3)); // iridium ore

                    List<int> possibles = new();
                    if (mineLevel > 80) possibles.Add(384); // gold ore

                    if (mineLevel > 40 && (possibles.Count == 0 || random.NextDouble() < 0.6))
                        possibles.Add(380); // iron ore

                    if (possibles.Count == 0 || random.NextDouble() < 0.6) possibles.Add(378); // copper ore

                    possibles.Add(382); // coal
                    treasuresAndQuantities.Add(possibles.ElementAt(random.Next(possibles.Count)),
                        random.Next(2, 7) * random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.015 ? 2 : 1);
                    if (random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
                    {
                        var key = treasuresAndQuantities.Keys.Last();
                        treasuresAndQuantities[key] *= 2;
                    }

                    break;
                }
            case 1:
                {
                    if (Game1.player.archaeologyFound.Any() && random.NextDouble() < 0.5) // artifacts
                        treasuresAndQuantities.Add(random.NextDouble() < 0.5 ? random.Next(579, 590) : 535, 1);
                    else
                        treasuresAndQuantities.Add(382, random.Next(1, 4)); // coal

                    break;
                }
            case 2:
                {
                    switch (random.Next(3))
                    {
                        case 0: // geodes
                            switch (mineLevel)
                            {
                                case > 80:
                                    treasuresAndQuantities.Add(
                                        537 + (random.NextDouble() < 0.4 ? random.Next(-2, 0) : 0),
                                        random.Next(1, 4)); // magma geode or worse
                                    break;

                                case > 40:
                                    treasuresAndQuantities.Add(536 + (random.NextDouble() < 0.4 ? -1 : 0),
                                        random.Next(1, 4)); // frozen geode or worse
                                    break;

                                default:
                                    treasuresAndQuantities.Add(535, random.Next(1, 4)); // regular geode
                                    break;
                            }

                            if (random.NextDouble() < 0.05 + Game1.player.LuckLevel * 0.03)
                            {
                                var key = treasuresAndQuantities.Keys.Last();
                                treasuresAndQuantities[key] *= 2;
                            }

                            break;

                        case 1: // minerals
                            if (mineLevel < 20)
                            {
                                treasuresAndQuantities.Add(382, random.Next(1, 4)); // coal
                                break;
                            }

                            switch (mineLevel)
                            {
                                case > 80:
                                    treasuresAndQuantities.Add(
                                        random.NextDouble() < 0.3 ? 82 : random.NextDouble() < 0.5 ? 64 : 60,
                                        random.Next(1, 3)); // fire quartz else ruby or emerald
                                    break;

                                case > 40:
                                    treasuresAndQuantities.Add(
                                        random.NextDouble() < 0.3 ? 84 : random.NextDouble() < 0.5 ? 70 : 62,
                                        random.Next(1, 3)); // frozen tear else jade or aquamarine
                                    break;

                                default:
                                    treasuresAndQuantities.Add(
                                        random.NextDouble() < 0.3 ? 86 : random.NextDouble() < 0.5 ? 66 : 68,
                                        random.Next(1, 3)); // earth crystal else amethyst or topaz
                                    break;
                            }

                            if (random.NextDouble() < 0.028 * mineLevel / 12)
                                treasuresAndQuantities.Add(72, 1); // diamond
                            else treasuresAndQuantities.Add(80, random.Next(1, 3)); // quartz

                            break;

                        case 2: // special items
                            var luckModifier = Math.Max(0, 1.0 + Game1.player.DailyLuck * mineLevel / 4);
                            var streak = Game1.player.Read<uint>("ProspectorHuntStreak");
                            if (random.NextDouble() < 0.025 * luckModifier && !Game1.player.specialItems.Contains(31))
                                treasuresAndQuantities.Add(-1, 1); // femur

                            if (random.NextDouble() < 0.010 * luckModifier && !Game1.player.specialItems.Contains(60))
                                treasuresAndQuantities.Add(-2, 1); // ossified blade

                            if (random.NextDouble() < 0.01 * luckModifier * Math.Pow(2, streak))
                                treasuresAndQuantities.Add(74, 1); // prismatic shard

                            if (treasuresAndQuantities.Count == 0)
                                treasuresAndQuantities.Add(72, 1); // consolation diamond
                            break;
                    }

                    break;
                }
        }

        foreach (var p in treasuresAndQuantities)
            switch (p.Key)
            {
                case -1:
                    Game1.createItemDebris(new MeleeWeapon(31) { specialItem = true },
                        new Vector2(TreasureTile!.Value.X, TreasureTile.Value.Y) + new Vector2(32f, 32f),
                        random.Next(4), Game1.currentLocation);
                    break;

                case -2:
                    Game1.createItemDebris(new MeleeWeapon(60) { specialItem = true },
                        new Vector2(TreasureTile!.Value.X, TreasureTile.Value.Y) + new Vector2(32f, 32f),
                        random.Next(4), Game1.currentLocation);
                    break;

                default:
                    Game1.createMultipleObjectDebris(p.Key, (int)TreasureTile!.Value.X, (int)TreasureTile.Value.Y,
                        p.Value, Game1.player.UniqueMultiplayerID, Game1.currentLocation);
                    break;
            }
    }

    #endregion private methods
}