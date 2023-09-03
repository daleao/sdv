namespace DaLion.Overhaul.Modules.Professions.TreasureHunts;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions.Events.Display.RenderedHud;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Networking;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Tools;

#endregion using directives

/// <summary>Manages treasure hunt events for Prospector profession.</summary>
internal sealed class ProspectorHunt : TreasureHunt
{
    /// <summary>Initializes a new instance of the <see cref="ProspectorHunt"/> class.</summary>
    internal ProspectorHunt()
        : base(
            TreasureHuntType.Prospector,
            I18n.Prospector_Huntstarted(),
            I18n.Prospector_Huntfailed(),
            new Rectangle(48, 672, 16, 16))
    {
    }

    /// <summary>Gets the current hunt location as <see cref="MineShaft"/>.</summary>
    private MineShaft? Shaft => this.Location as MineShaft;

    /// <inheritdoc />
    public override bool TryStart(GameLocation location)
    {
        if (!location.Objects.Any() || !this.TryStart())
        {
            return false;
        }

        this.TreasureTile = this.ChooseTreasureTile(location);
        if (this.TreasureTile is null)
        {
            return false;
        }

        this.Location = location;
#if DEBUG
        this.TimeLimit = int.MaxValue;
#elif RELEASE
        this.TimeLimit = (uint)(location.Objects.Count() * ProfessionsModule.Config.ProspectorHuntHandicap);
        this.TimeLimit = Math.Max(this.TimeLimit, 30);
#endif
        this.Elapsed = 0;
        EventManager.Enable<ProspectorHuntRenderedHudEvent>();
        EventManager.Enable<ProspectorHuntUpdateTickedEvent>();
        HudPointer.Instance.Value.ShouldBob = true;
        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat($"{Game1.player.Name} is hunting for treasure.");

            if (Game1.player.HasProfession(Profession.Prospector, true))
            {
                Game1.player.Get_IsHuntingTreasure().Value = true;
                if (!Context.IsMainPlayer)
                {
                    ModEntry.Broadcaster.MessagePeer("HuntIsOn", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
                }
                else
                {
                    EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
                }
            }
        }

        this.OnStarted();
        return true;
    }

    /// <inheritdoc />
    public override void ForceStart(GameLocation location, Vector2 target)
    {
        this.ForceStart();

        this.TreasureTile = target;
        this.Location = location;
        this.TimeLimit = (uint)(location.Objects.Count() * ProfessionsModule.Config.ProspectorHuntHandicap);
        this.Elapsed = 0;
        EventManager.Enable<ProspectorHuntRenderedHudEvent>();
        EventManager.Enable<ProspectorHuntUpdateTickedEvent>();
        HudPointer.Instance.Value.ShouldBob = true;
        Game1.addHUDMessage(new HuntNotification(this.HuntStartedMessage, this.IconSourceRect));
        if (Context.IsMultiplayer)
        {
            Broadcaster.SendPublicChat($"{Game1.player.Name} is hunting for treasure.");

            if (Game1.player.HasProfession(Profession.Prospector, true))
            {
                Game1.player.Get_IsHuntingTreasure().Value = true;
                if (!Context.IsMainPlayer)
                {
                    ModEntry.Broadcaster.MessagePeer("HuntIsOn", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
                }
                else
                {
                    EventManager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
                }
            }
        }

        this.OnStarted();
    }

    /// <inheritdoc />
    public override void Fail()
    {
        Game1.addHUDMessage(new HuntNotification(this.HuntFailedMessage));
        Game1.player.Write(DataKeys.ProspectorHuntStreak, "0");
        this.End(false);
    }

    /// <inheritdoc />
    protected override Vector2? ChooseTreasureTile(GameLocation location)
    {
        Vector2 v;
        var failSafe = 0;
        do
        {
            if (failSafe > 69) // nice
            {
                return null;
            }

            v = location.Objects.Keys.ElementAtOrDefault(this.Random.Next(location.Objects.Keys.Count()));
            failSafe++;
        }
        while (!location.Objects.TryGetValue(v, out var obj) || !obj.IsStone() || obj.IsResourceNode());

        return v;
    }

    /// <inheritdoc />
    protected override void CheckForCompletion()
    {
        if (this.TreasureTile is null || this.Shaft is null)
        {
            this.End(false);
            return;
        }

        if (Game1.currentLocation.Objects.ContainsKey(this.TreasureTile.Value))
        {
            return;
        }

        this.GetStoneTreasure(this.Shaft.mineLevel);
        if (this.Shaft.shouldCreateLadderOnThisLevel() && !this.Shaft.GetLadderTiles().Any())
        {
            this.Shaft.createLadderDown((int)this.TreasureTile!.Value.X, (int)this.TreasureTile!.Value.Y);
        }

        Game1.player.Increment(DataKeys.ProspectorHuntStreak);
        this.End(true);
    }

    /// <inheritdoc />
    protected override void End(bool found)
    {
        Game1.player.Get_IsHuntingTreasure().Value = false;
        EventManager.Disable<ProspectorHuntRenderedHudEvent>();
        EventManager.Disable<ProspectorHuntUpdateTickedEvent>();
        HudPointer.Instance.Value.ShouldBob = false;
        this.TreasureTile = null;
        if (!Context.IsMultiplayer || Context.IsMainPlayer ||
            !Game1.player.HasProfession(Profession.Prospector, true))
        {
            return;
        }

        Broadcaster.SendPublicChat(found
            ? $"{Game1.player.Name} has found the treasure!"
            : $"{Game1.player.Name} failed to find the treasure.");
        ModEntry.Broadcaster.MessagePeer("HuntIsOff", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);

        this.OnEnded(found);
    }

    /// <summary>Spawns hunt spoils as debris.</summary>
    /// <remarks>Adapted from FishingRod.openTreasureMenuEndFunction.</remarks>
    private void GetStoneTreasure(int mineLevel)
    {
        Dictionary<int, int> treasuresAndQuantities = new();

        if (this.Random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        {
            treasuresAndQuantities.Add(
                890,
                this.Random.Next(1, 3) + (this.Random.NextDouble() < 0.25 ? 2 : 0)); // qi beans
        }

        switch (this.Random.Next(3))
        {
            case 0:
            {
                if (mineLevel > 120 && this.Random.NextDouble() < 0.06)
                {
                    treasuresAndQuantities.Add(386, this.Random.Next(1, 3)); // iridium ore
                }

                List<int> possibles = new();
                if (mineLevel > 80)
                {
                    possibles.Add(384); // gold ore
                }

                if (mineLevel > 40 && (possibles.Count == 0 || this.Random.NextDouble() < 0.6))
                {
                    possibles.Add(380); // iron ore
                }

                if (possibles.Count == 0 || this.Random.NextDouble() < 0.6)
                {
                    possibles.Add(378); // copper ore
                }

                possibles.Add(382); // coal
                treasuresAndQuantities.Add(
                    possibles.ElementAt(this.Random.Next(possibles.Count)),
                    this.Random.Next(2, 7) * this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.015) ? 2 : 1);
                if (this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.03))
                {
                    var key = treasuresAndQuantities.Keys.Last();
                    treasuresAndQuantities[key] *= 2;
                }

                break;
            }

            case 1:
            {
                if (Game1.player.archaeologyFound.Any() && this.Random.NextDouble() < 0.5)
                {
                    treasuresAndQuantities.Add(this.Random.NextDouble() < 0.5 ? this.Random.Next(579, 590) : 535, 1); // artifact
                }
                else
                {
                    treasuresAndQuantities.Add(382, this.Random.Next(1, 4)); // coal
                }

                break;
            }

            case 2:
            {
                switch (this.Random.Next(3))
                {
                    case 0:
                        // geodes
                        switch (mineLevel)
                        {
                            case > 80:
                                treasuresAndQuantities.Add(
                                    537 + (this.Random.NextDouble() < 0.4 ? this.Random.Next(-2, 0) : 0),
                                    this.Random.Next(1, 4)); // magma geode or worse
                                break;

                            case > 40:
                                treasuresAndQuantities.Add(
                                    536 + (this.Random.NextDouble() < 0.4 ? -1 : 0),
                                    this.Random.Next(1, 4)); // frozen geode or worse
                                break;

                            default:
                                treasuresAndQuantities.Add(535, this.Random.Next(1, 4)); // regular geode
                                break;
                        }

                        if (this.Random.NextDouble() < 0.05 + (Game1.player.LuckLevel * 0.03))
                        {
                            var key = treasuresAndQuantities.Keys.Last();
                            treasuresAndQuantities[key] *= 2;
                        }

                        break;

                    case 1: // minerals
                        if (mineLevel < 20)
                        {
                            treasuresAndQuantities.Add(382, this.Random.Next(1, 4)); // coal
                            break;
                        }

                        switch (mineLevel)
                        {
                            case > 80:
                                treasuresAndQuantities.Add(
                                    this.Random.NextDouble() < 0.3 ? 82 : this.Random.NextDouble() < 0.5 ? 64 : 60,
                                    this.Random.Next(1, 3)); // fire quartz else ruby or emerald
                                break;

                            case > 40:
                                treasuresAndQuantities.Add(
                                    this.Random.NextDouble() < 0.3 ? 84 : this.Random.NextDouble() < 0.5 ? 70 : 62,
                                    this.Random.Next(1, 3)); // frozen tear else jade or aquamarine
                                break;

                            default:
                                treasuresAndQuantities.Add(
                                    this.Random.NextDouble() < 0.3 ? 86 : this.Random.NextDouble() < 0.5 ? 66 : 68,
                                    this.Random.Next(1, 3)); // earth crystal else amethyst or topaz
                                break;
                        }

                        if (this.Random.NextDouble() < 0.028 * mineLevel / 12)
                        {
                            treasuresAndQuantities.Add(SObject.diamondIndex, 1);
                        }
                        else
                        {
                            treasuresAndQuantities.Add(SObject.quartzIndex, this.Random.Next(1, 3));
                        }

                        break;

                    case 2: // special items
                        var luckModifier = Math.Max(0, 1.0 + (Game1.player.DailyLuck * mineLevel / 4));
                        var streak = Game1.player.Read<uint>(DataKeys.ProspectorHuntStreak);
                        if (this.Random.NextDouble() < 0.025 * luckModifier * streak && !Game1.player.specialItems.Contains(31))
                        {
                            treasuresAndQuantities.Add(-1, 1); // femur
                        }

                        if (this.Random.NextDouble() < 0.01 * luckModifier * streak && !Game1.player.specialItems.Contains(60))
                        {
                            treasuresAndQuantities.Add(-2, 1); // ossified blade
                        }

                        if (this.Random.NextDouble() < 0.01 * luckModifier * Math.Pow(2, streak))
                        {
                            treasuresAndQuantities.Add(SObject.prismaticShardIndex, 1);
                        }

                        if (treasuresAndQuantities.Count == 0)
                        {
                            treasuresAndQuantities.Add(SObject.diamondIndex, 1);
                        }

                        break;
                }

                break;
            }
        }

        foreach (var (treasure, quantity) in treasuresAndQuantities)
        {
            switch (treasure)
            {
                case -1:
                    Game1.createItemDebris(
                        new MeleeWeapon(31),
                        new Vector2(this.TreasureTile!.Value.X, this.TreasureTile.Value.Y) + new Vector2(32f, 32f),
                        this.Random.Next(4),
                        Game1.currentLocation);
                    break;

                case -2:
                    Game1.createItemDebris(
                        new MeleeWeapon(60),
                        new Vector2(this.TreasureTile!.Value.X, this.TreasureTile.Value.Y) + new Vector2(32f, 32f),
                        this.Random.Next(4),
                        Game1.currentLocation);
                    break;

                default:
                    Game1.createMultipleObjectDebris(
                        treasure,
                        (int)this.TreasureTile!.Value.X,
                        (int)this.TreasureTile.Value.Y,
                        quantity,
                        Game1.player.UniqueMultiplayerID,
                        Game1.currentLocation);
                    break;
            }
        }
    }
}
