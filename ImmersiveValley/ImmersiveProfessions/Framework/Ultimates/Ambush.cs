namespace DaLion.Stardew.Professions.Framework.Ultimates;

#region using directives

using System.Linq;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using DaLion.Stardew.Professions.Framework.Sounds;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Handles Poacher ultimate activation.</summary>
public sealed class Ambush : Ultimate
{
    /// <summary>Initializes a new instance of the <see cref="Ambush"/> class.</summary>
    internal Ambush()
        : base("Ambush", 27, Color.MediumPurple, Color.MidnightBlue)
    {
    }

    /// <inheritdoc />
    public override string Description =>
        ModEntry.i18n.Get(this.Name.ToLower() + ".desc." + (this.IsGrantingCritBuff ? "revealed" : "hidden"));

    /// <inheritdoc />
    internal override int MillisecondsDuration =>
        (int)(15000 * ((double)this.MaxValue / BaseMaxValue) / ModEntry.Config.SpecialDrainFactor);

    /// <inheritdoc />
    internal override Sfx ActivationSfx => Sfx.PoacherAmbush;

    /// <inheritdoc />
    internal override Color GlowColor => Color.MediumPurple;

    /// <summary>Gets a value indicating whether determines whether the double crit. power buff is active.</summary>
    internal bool IsGrantingCritBuff =>
        this.IsActive || Game1.buffsDisplay.otherBuffs.Any(b => b.which == this.BuffId - 4);

    internal double SecondsOutOfAmbush { get; set; } = double.MaxValue;

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();

        this.SecondsOutOfAmbush = 0d;

        foreach (var monster in Game1.currentLocation.characters.OfType<Monster>()
                     .Where(m => m.Player?.IsLocalPlayer == true))
        {
            monster.focusedOnFarmers = false;
            switch (monster)
            {
                case AngryRoger:
                case Ghost:
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "seenPlayer").SetValue(false);
                    break;
                case Bat:
                case RockGolem:
                    ModEntry.ModHelper.Reflection.GetField<NetBool>(monster, "seenPlayer").GetValue().Value = false;
                    break;
                case DustSpirit:
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "seenFarmer").SetValue(false);
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "chargingFarmer").SetValue(false);
                    break;
                case ShadowGuy:
                case ShadowShaman:
                case Skeleton:
                    ModEntry.ModHelper.Reflection.GetField<bool>(monster, "spottedPlayer").SetValue(false);
                    break;
            }
        }

        var critBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == this.BuffId - 4);
        var duration = critBuff?.millisecondsDuration ?? this.MillisecondsDuration;

        Game1.buffsDisplay.removeOtherBuff(this.BuffId - 4);
        Game1.buffsDisplay.removeOtherBuff(this.BuffId);
        Game1.player.addedSpeed -= 2;
        Game1.buffsDisplay.addOtherBuff(
            new Buff(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                this.GetType().Name,
                this.DisplayName)
            {
                which = this.BuffId,
                sheetIndex = 49,
                glow = this.GlowColor,
                millisecondsDuration = duration,
                description = this.Description,
            });

        ModEntry.Events.Enable<AmbushUpdateTickedEvent>();
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == this.BuffId);
        var timeLeft = buff?.millisecondsDuration ?? 0;
        Game1.buffsDisplay.removeOtherBuff(this.BuffId);
        Game1.player.addedSpeed += 2;
        if (timeLeft < 100)
        {
            return;
        }

        var buffId = this.BuffId - 4;
        Game1.buffsDisplay.removeOtherBuff(buffId);
        Game1.buffsDisplay.addOtherBuff(
            new Buff(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                this.GetType().Name,
                this.DisplayName)
            {
                which = buffId, sheetIndex = 37, millisecondsDuration = timeLeft * 2, description = this.Description,
            });
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        this.ChargeValue -= this.MaxValue / 900d; // lasts 15s * 60 ticks/s -> 900 ticks
    }

    /// <inheritdoc />
    internal override string GetBuffPronoun()
    {
        return LocalizedContentManager.CurrentLanguageCode switch
        {
            LocalizedContentManager.LanguageCode.es => ModEntry.i18n.Get("pronoun.definite.female"),
            LocalizedContentManager.LanguageCode.fr or LocalizedContentManager.LanguageCode.pt =>
                ModEntry.i18n.Get("pronoun.definite.male"),
            _ => string.Empty,
        };
    }
}
