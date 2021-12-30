using System.Linq;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events;

public delegate void SuperModeEnabledEventHandler();

internal class SuperModeEnabledEvent : BaseEvent
{
    private const int SHEET_INDEX_OFFSET_I = 22;

    /// <summary>Hook this event to the event listener.</summary>
    public override void Hook()
    {
        ModState.SuperModeEnabled += OnSuperModeEnabled;
    }

    /// <summary>Unhook this event from the event listener.</summary>
    public override void Unhook()
    {
        ModState.SuperModeEnabled -= OnSuperModeEnabled;
    }

    /// <summary>Raised when IsSuperModeActive is set to true.</summary>
    public void OnSuperModeEnabled()
    {
        var whichSuperMode = Utility.Professions.NameOf(ModState.SuperModeIndex);

        // remove bar shake timer
        ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBuffDisplayUpdateTickedEvent),
            typeof(SuperModeBarShakeTimerUpdateTickedEvent));
        ModState.ShouldShakeSuperModeGauge = false;

        // fade in overlay
        ModEntry.Subscriber.Subscribe(new SuperModeRenderedWorldEvent(),
            new SuperModeOverlayFadeInUpdateTickedEvent());

        // play sound effect
        ModEntry.SoundBox.Play(ModState.SuperModeSFX);

        // add countdown event
        ModEntry.Subscriber.Subscribe(new SuperModeCountdownUpdateTickedEvent());

        // display buff
        var buffID = ModEntry.Manifest.UniqueID.GetHashCode() + ModState.SuperModeIndex + 4;
        var professionIndex = ModState.SuperModeIndex;
        var professionName = Utility.Professions.NameOf(professionIndex);

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffID);
        if (buff is null)
        {
            Game1.buffsDisplay.otherBuffs.Clear();
            Game1.buffsDisplay.addOtherBuff(
                new(0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    professionName == "Poacher" ? -1 : 0,
                    0,
                    0,
                    1,
                    "SuperMode",
                    ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".superm"))
                {
                    which = buffID,
                    sheetIndex = professionIndex + SHEET_INDEX_OFFSET_I,
                    glow = ModState.SuperModeGlowColor,
                    millisecondsDuration =
                        (int) (ModState.SuperModeGaugeMaxValue * ModEntry.Config.SuperModeDrainFactor / 60f) * 1000,
                    description = ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".supermdesc")
                }
            );
        }

        // notify peers
        ModEntry.ModHelper.Multiplayer.SendMessage(ModState.SuperModeIndex, "SuperModeEnabled",
            new[] {ModEntry.Manifest.UniqueID});

        switch (whichSuperMode)
        {
            // apply immediate effects
            case "Poacher":
                DoEnablePoacherSuperMode();
                break;

            case "Piper":
                DoEnablePiperSuperMode();
                break;
        }

        // unsubscribe self and wait for disabled
        ModEntry.Subscriber.Unsubscribe(GetType());
        ModEntry.Subscriber.Subscribe(new SuperModeDisabledEvent());
    }

    /// <summary>Hide the player from monsters that may have already seen him/her.</summary>
    private static void DoEnablePoacherSuperMode()
    {
        foreach (var monster in Game1.currentLocation.characters.OfType<Monster>()
                     .Where(m => m.Player.IsLocalPlayer))
        {
            monster.focusedOnFarmers = false;
            switch (monster)
            {
                case DustSpirit dustSpirit:
                    ModEntry.ModHelper.Reflection.GetField<bool>(dustSpirit, "chargingFarmer").SetValue(false);
                    ModEntry.ModHelper.Reflection.GetField<bool>(dustSpirit, "seenFarmer").SetValue(false);
                    break;

                case AngryRoger angryRoger:
                    ModEntry.ModHelper.Reflection.GetField<NetBool>(angryRoger, "seenPlayer").GetValue().Set(false);
                    break;

                case Bat bat:
                    ModEntry.ModHelper.Reflection.GetField<NetBool>(bat, "seenPlayer").GetValue().Set(false);
                    break;

                case Ghost ghost:
                    ModEntry.ModHelper.Reflection.GetField<NetBool>(ghost, "seenPlayer").GetValue().Set(false);
                    break;

                case RockGolem rockGolem:
                    ModEntry.ModHelper.Reflection.GetField<NetBool>(rockGolem, "seenPlayer").GetValue().Set(false);
                    break;
            }
        }
    }

    /// <summary>Enflate Slimes and apply mutations.</summary>
    private static void DoEnablePiperSuperMode()
    {
        foreach (var greenSlime in Game1.currentLocation.characters.OfType<GreenSlime>()
                     .Where(slime => slime.Scale < 2f))
        {
            if (Game1.random.NextDouble() <= 0.012 + Game1.player.team.AverageDailyLuck() / 10.0)
            {
                if (Game1.currentLocation is MineShaft && Game1.player.team.SpecialOrderActive("Wizard2"))
                    greenSlime.makePrismatic();
                else greenSlime.hasSpecialItem.Value = true;
            }

            ModState.PipedSlimeScales.Add(greenSlime, greenSlime.Scale);
        }

        var bigSlimes = Game1.currentLocation.characters.OfType<BigSlime>().ToList();
        for (var i = bigSlimes.Count - 1; i >= 0; --i)
        {
            bigSlimes[i].Health = 0;
            bigSlimes[i].deathAnimation();
            var toCreate = Game1.random.Next(2, 5);
            while (toCreate-- > 0)
            {
                Game1.currentLocation.characters.Add(new GreenSlime(bigSlimes[i].Position, Game1.CurrentMineLevel));
                var justCreated = Game1.currentLocation.characters[^1];
                justCreated.setTrajectory((int) (bigSlimes[i].xVelocity / 8 + Game1.random.Next(-2, 3)),
                    (int) (bigSlimes[i].yVelocity / 8 + Game1.random.Next(-2, 3)));
                justCreated.willDestroyObjectsUnderfoot = false;
                justCreated.moveTowardPlayer(4);
                justCreated.Scale = 0.75f + Game1.random.Next(-5, 10) / 100f;
                justCreated.currentLocation = Game1.currentLocation;
            }
        }

        ModEntry.Subscriber.Subscribe(new SlimeInflationUpdateTickedEvent());
    }
}