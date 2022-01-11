using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using TheLion.Stardew.Professions.Framework.Events.Display.RenderedWorld;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;
using TheLion.Stardew.Professions.Framework.Events.Input.ButtonsChanged;
using TheLion.Stardew.Professions.Framework.Events.Player.Warped;
using TheLion.Stardew.Professions.Framework.Sounds;

namespace TheLion.Stardew.Professions.Framework.SuperMode;

/// <summary>Main handler for Super Mode functionality.</summary>
public class SuperMode
{
    private const int BUFF_SHEET_INDEX_OFFSET_I = 10, SUPERMODE_SHEET_INDEX_OFFSET_I = 22, BASE_ACTIVATION_DELAY_I = 60;

    private int _activationTimer = (int) (BASE_ACTIVATION_DELAY_I * ModEntry.Config.SuperModeActivationDelay);

    /// <summary>Construct an instance.</summary>
    /// <param name="index">The currently registered Super Mode profession's index.</param>
    public SuperMode(SuperModeIndex index)
    {
        if (index <= SuperModeIndex.None)
            throw new ArgumentOutOfRangeException(nameof(index), index,
                "Tried to initialize empty or illegal Super Mode.");

        Index = index;
        Init();

        // enable events
        ModEntry.EventManager.EnableAllForSuperMode();
        if (Index == SuperModeIndex.Piper) ModEntry.EventManager.Enable(typeof(PiperWarpedEvent));
    }

    ~SuperMode()
    {
        ModEntry.EventManager.DisableAllForSuperMode();
        if (Index == SuperModeIndex.Piper) ModEntry.EventManager.Disable(typeof(PiperWarpedEvent));
    }

    public bool IsActive { get; private set; }
    public SuperModeIndex Index { get; }
    public SuperModeGauge Gauge { get; private set; }
    public SuperModeOverlay Overlay { get; private set; }
    public Color GlowColor { get; private set; }
    public SFX ActivationSfx { get; private set; }

    /// <summary>Activate Super Mode for the local player.</summary>
    public void Activate()
    {
        IsActive = true;

        // fade in overlay and begin countdown
        ModEntry.EventManager.Enable(typeof(SuperModeActiveRenderedWorldEvent),
            typeof(SuperModeGaugeCountdownUpdateTickedEvent), typeof(SuperModeOverlayFadeInUpdateTickedEvent));

        // stop displaying super stat buff, awaiting activation and shaking gauge
        ModEntry.EventManager.Disable(typeof(SuperModeBuffDisplayUpdateTickedEvent),
            typeof(SuperModeButtonsChangedEvent), typeof(SuperModeGaugeShakeUpdateTickedEvent));

        // play sound effect
        ModEntry.SoundBox.Play(ModEntry.State.Value.SuperMode.ActivationSfx);

        // add Super Mode buff
        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) Index + 4;
        var professionIndex = (int) ModEntry.State.Value.SuperMode.Index;
        var professionName = Utility.Professions.NameOf(professionIndex);

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == buffId);
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
                    which = buffId,
                    sheetIndex = professionIndex + SUPERMODE_SHEET_INDEX_OFFSET_I,
                    glow = ModEntry.State.Value.SuperMode.GlowColor,
                    millisecondsDuration = (int) (SuperModeGauge.MaxValue * ModEntry.Config.SuperModeDrainFactor * 10),
                    description = ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".supermdesc")
                }
            );
        }

        // notify peers
        ModEntry.ModHelper.Multiplayer.SendMessage(Index, "ToggledSuperMode/On",
            new[] {ModEntry.Manifest.UniqueID});

        // apply immediate effects
        switch (Index)
        {
            case SuperModeIndex.Poacher:
                ActivateForPoacher();
                break;

            case SuperModeIndex.Piper:
                ActivateForPiper();
                break;
        }
    }

    /// <summary>Deactivate Super Mode for the local player.</summary>
    public void Deactivate()
    {
        IsActive = false;

        // fade out overlay
        ModEntry.EventManager.Enable(typeof(SuperModeOverlayFadeOutUpdateTickedEvent));

        // stop gauge countdown
        ModEntry.EventManager.Disable(typeof(SuperModeGaugeCountdownUpdateTickedEvent));

        // remove buff if necessary
        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) Index + 4;
        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == buffId);
        if (buff is not null) Game1.buffsDisplay.otherBuffs.Remove(buff);

        // stop glowing if necessary
        Game1.player.stopGlowing();

        // notify peers
        ModEntry.ModHelper.Multiplayer.SendMessage(Index, "ToggledSuperMode/Off",
            new[] {ModEntry.Manifest.UniqueID});

        // remove piper effects
        if (Index != SuperModeIndex.Piper) return;

        // de-power
        foreach (var slime in ModEntry.State.Value.PipedSlimeScales.Keys)
            slime.DamageToFarmer = (int) Math.Round(slime.DamageToFarmer / slime.Scale);

        // de-gorge
        ModEntry.EventManager.Enable(typeof(SlimeDeflationUpdateTickedEvent));
    }

    /// <summary>Handle changes to <see cref="ModConfig.SuperModKey" />.</summary>
    public void ReceiveInput()
    {
        if (ModEntry.Config.SuperModeKey.JustPressed() && Gauge.IsFull && !IsActive)
        {
            if (ModEntry.Config.HoldKeyToActivateSuperMode)
                ModEntry.EventManager.Enable(typeof(SuperModeInputUpdateTickedEvent));
            else
                Activate();
        }
        else if (ModEntry.Config.SuperModeKey.GetState() == SButtonState.Released)
        {
            _activationTimer = (int) (BASE_ACTIVATION_DELAY_I * ModEntry.Config.SuperModeActivationDelay);
            ModEntry.EventManager.Disable(typeof(SuperModeInputUpdateTickedEvent));
        }
    }

    /// <summary>Countdown the Super Mode activation timer.</summary>
    public void UpdateInput()
    {
        --_activationTimer;
        if (_activationTimer > 0) return;

        Activate();
        _activationTimer = (int) (BASE_ACTIVATION_DELAY_I * ModEntry.Config.SuperModeActivationDelay);
        ModEntry.EventManager.Disable(typeof(SuperModeInputUpdateTickedEvent));
    }

    /// <summary>Add the Super Stat buff associated with this Super Mode index to the local player.</summary>
    public void AddBuff()
    {
        if (Gauge.CurrentValue < 10.0) return;

        var buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) Index;
        var professionName = Utility.Professions.NameOf((int) Index);
        var magnitude = GetBuffMagnitude();
        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == buffId);
        if (buff == null)
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
                    0,
                    0,
                    0,
                    1,
                    professionName,
                    ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".buff"))
                {
                    which = buffId,
                    sheetIndex = (int) Index + BUFF_SHEET_INDEX_OFFSET_I,
                    millisecondsDuration = 0,
                    description = ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".buffdesc",
                        new {magnitude})
                });
    }

    #region private methods

    /// <summary>Initialize private fields following an index change.</summary>
    private void Init()
    {
        Gauge = new();
        Overlay = new(Index);

        switch (Index)
        {
            case SuperModeIndex.Brute:
                GlowColor = Color.OrangeRed;
                ActivationSfx = SFX.BruteRage;
                break;

            case SuperModeIndex.Poacher:
                GlowColor = Color.MediumPurple;
                ActivationSfx = SFX.PoacherAmbush;
                break;

            case SuperModeIndex.Piper:
                GlowColor = Color.LimeGreen;
                ActivationSfx = SFX.PiperFluidity;
                break;

            case SuperModeIndex.Desperado:
                GlowColor = Color.DarkGoldenrod;
                ActivationSfx = SFX.DesperadoBlossom;
                break;
        }

        var key = Index.ToString().ToLower();
        var professionDisplayName = ModEntry.ModHelper.Translation.Get(key + ".name.male");
        var buffName = ModEntry.ModHelper.Translation.Get(key + ".buff");
        ModEntry.Log($"Initialized Super Mode as {professionDisplayName}'s {buffName}.", ModEntry.DefaultLogLevel);
    }

    /// <summary>Get the current magnitude of the Super Mode buff to display.</summary>
    private string GetBuffMagnitude()
    {
#pragma warning disable CS8509
        return Index switch
#pragma warning restore CS8509
        {
            SuperModeIndex.Brute => ((Utility.Professions.GetBruteBonusDamageMultiplier(Game1.player) - 1.15) * 100f)
                .ToString("0.0"),
            SuperModeIndex.Poacher => Utility.Professions.GetPoacherCritDamageMultiplier().ToString("0.0"),
            SuperModeIndex.Piper => Utility.Professions.GetPiperSlimeSpawnAttempts().ToString("0"),
            SuperModeIndex.Desperado => ((Utility.Professions.GetDesperadoBulletPower() - 1f) * 100f).ToString("0.0")
        };
    }

    #endregion private methods

    #region private static methods

    /// <summary>Hide the player from monsters that may have already seen him/her.</summary>
    private static void ActivateForPoacher()
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
    private static void ActivateForPiper()
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

            ModEntry.State.Value.PipedSlimeScales.Add(greenSlime, greenSlime.Scale);
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

        ModEntry.EventManager.Enable(typeof(SlimeInflationUpdateTickedEvent));
    }

    #endregion private static methods
}