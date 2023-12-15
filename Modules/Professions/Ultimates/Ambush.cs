namespace DaLion.Overhaul.Modules.Professions.Ultimates;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Handles Poacher ultimate activation.</summary>
public sealed class Ambush : Ultimate
{
    private const int BackStabSheetIndex = 37;

    /// <summary>Initializes a new instance of the <see cref="Ambush"/> class.</summary>
    internal Ambush()
        : base("Ambush", Profession.Poacher, Color.MediumPurple, Color.MidnightBlue)
    {
    }

    /// <inheritdoc />
    public override string DisplayName { get; } = I18n.Ambush_Title();

    /// <inheritdoc />
    public override string Description => this.IsGrantingCritBuff ? I18n.Ambush_Desc_Revealed() : I18n.Ambush_Desc_Hidden();

    /// <inheritdoc />
    internal override int MillisecondsDuration =>
        (int)(15000 * ((double)this.MaxValue / BaseMaxValue) / ProfessionsModule.Config.LimitDrainFactor);

    /// <inheritdoc />
    internal override SoundEffectPlayer ActivationSfx => SoundEffectPlayer.PoacherAmbush;

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

        for (var i = 0; i < Game1.currentLocation.characters.Count; i++)
        {
            if (Game1.currentLocation.characters[i] is not Monster { IsMonster: true, Player.IsLocalPlayer: true } monster)
            {
                continue;
            }

            monster.focusedOnFarmers = false;
            switch (monster)
            {
                case AngryRoger:
                case Ghost:
                    ModHelper.Reflection.GetField<bool>(monster, "seenPlayer").SetValue(false);
                    break;
                case Bat:
                case RockGolem:
                    ModHelper.Reflection.GetField<NetBool>(monster, "seenPlayer").GetValue().Value = false;
                    break;
                case DustSpirit:
                    ModHelper.Reflection.GetField<bool>(monster, "seenFarmer").SetValue(false);
                    ModHelper.Reflection.GetField<bool>(monster, "chargingFarmer").SetValue(false);
                    break;
                case ShadowGuy:
                case ShadowShaman:
                case Skeleton:
                    ModHelper.Reflection.GetField<bool>(monster, "spottedPlayer").SetValue(false);
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
                sheetIndex = this.BuffSheetIndex,
                glow = this.GlowColor,
                millisecondsDuration = duration,
                description = this.Description,
            });

        EventManager.Enable<AmbushUpdateTickedEvent>();
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
                which = buffId - 4,
                sheetIndex = BackStabSheetIndex,
                millisecondsDuration = timeLeft * 2,
                description = this.Description,
            });
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        this.ChargeValue -= this.MaxValue / 900d; // lasts 15s * 60 ticks/s -> 900 ticks
    }
}
