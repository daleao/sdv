// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

using AssetLoaders;
using Extensions;

#endregion using directives

/// <summary>Handles Brute ultimate activation.</summary>
internal sealed class Frenzy : Ultimate
{
    //private double _elapsedSinceDoT;
    public const float PCT_INCREMENT_PER_RAGE_F = 0.01f;

    /// <summary>Construct an instance.</summary>
    internal Frenzy()
    {
        Meter = new(this, Color.OrangeRed);
        Overlay = new(Color.OrangeRed);
        EnableEvents();
    }

    #region public properties
    
    public static int BuffId { get; } = ModEntry.Manifest.UniqueID.GetHashCode() + (int) UltimateIndex.Brute + 4;

    public override SFX ActivationSfx => SFX.BruteRage;
    public override Color GlowColor => Color.OrangeRed;
    public override UltimateIndex Index => UltimateIndex.Brute;

    #endregion public properties

    #region public methods

    /// <inheritdoc />
    public override void Activate()
    {
        base.Activate();

        // fear
        foreach (var monster in Game1.currentLocation.characters.OfType<Monster>()
                     .Where(m => m.Player.IsLocalPlayer))
        {
            monster.addedSpeed -= monster.Speed;
            monster.WriteData("Feared", true.ToString());
            monster.WriteData("FearTimer", 1000.ToString());
            ModEntry.PlayerState.FearedMonsters.Add(monster);
        }

        //// fully recover health
        //var who = Game1.player;
        //var healed = who.maxHealth - who.health;
        //who.health = who.maxHealth;
        //who.currentLocation.debris.Add(new(healed,
        //    new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));
        
        ModEntry.PlayerState.BruteKillCounter = 0;

        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.ModHelper.Translation.Get("brute.ulti"))
            {
                which = BuffId,
                sheetIndex = 48,
                glow = GlowColor,
                millisecondsDuration = (int) (15000 * ((double) MaxValue / BASE_MAX_VALUE_I) * 1.0 / ModEntry.Config.UltimateDrainFactor),
                description = ModEntry.ModHelper.Translation.Get("brute.ultidesc")
            }
        );
    }

    /// <inheritdoc />
    public override void Deactivate()
    {
        base.Deactivate();

        Game1.buffsDisplay.removeOtherBuff(BuffId);

        var who = Game1.player;
        var healed = (int) (who.maxHealth * ModEntry.PlayerState.BruteKillCounter * 0.05f);
        who.health = Math.Min(who.health + healed, who.maxHealth);
        who.currentLocation.debris.Add(new(healed,
            new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));
    }

    /// <inheritdoc />
    public override void Countdown(double elapsed)
    {
        ChargeValue -= elapsed * 0.12 / 18.0;

        var feared = ModEntry.PlayerState.FearedMonsters.ToArray();
        foreach (var monster in feared)
        {
            monster.IncrementData("FearTimer", -elapsed);
            if (monster.ReadDataAs<int>("FearTimer") > 0) continue;

            monster.addedSpeed += monster.Speed;
            monster.WriteData("FearTimer", null);
            ModEntry.PlayerState.FearedMonsters.Remove(monster);
        }

        //_elapsedSinceDoT += elapsed;
        //if (_elapsedSinceDoT < 2000) return;

        //_elapsedSinceDoT = 0.0;
        //var who = Game1.player;
        //var fivePct = (int) (who.maxHealth * 0.05);
        //who.health = Math.Max(who.health - fivePct, 1);
        //who.currentLocation.debris.Add(new(fivePct,
        //    new(who.getStandingX() + 8, who.getStandingY()), Color.Red, 1f, who));

        //if (who.health <= fivePct) Deactivate();
    }

    #endregion public methods
}