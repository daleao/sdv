namespace DaLion.Overhaul;

#region using directives

using System.IO;
using Ardalis.SmartEnum;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Shared.Exceptions;
using Microsoft.Xna.Framework.Audio;

#endregion using directives

/// <summary>A custom <see cref="SoundEffect"/> that can be played through the game's <see cref="SoundBank"/>.</summary>
public sealed class SoundEffectPlayer : SmartEnum<SoundEffectPlayer>
{
    #region enum entries

    /// <summary>The <see cref="SoundEffectPlayer"/> played when <see cref="Modules.Professions.Ultimates.Frenzy"/> activates.</summary>
    public static readonly SoundEffectPlayer BruteRage = new("BruteRage", 0);

    /// <summary>The <see cref="SoundEffectPlayer"/> played when <see cref="Modules.Professions.Ultimates.Ambush"/> activates.</summary>
    public static readonly SoundEffectPlayer PoacherAmbush = new("PoacherCloak", 1);

    /// <summary>The <see cref="SoundEffectPlayer"/> played when a <see cref="Profession.Poacher"/> successfully steals an item.</summary>
    public static readonly SoundEffectPlayer PoacherSteal = new("PoacherSteal", 2);

    /// <summary>The <see cref="SoundEffectPlayer"/> played when <see cref="Modules.Professions.Ultimates.Concerto"/> activates.</summary>
    public static readonly SoundEffectPlayer PiperConcerto = new("PiperProvoke", 3);

    /// <summary>The <see cref="SoundEffectPlayer"/> played when <see cref="Modules.Professions.Ultimates.DeathBlossom"/> activates.</summary>
    public static readonly SoundEffectPlayer DesperadoBlossom = new("DesperadoWhoosh", 4);

    /// <summary>The <see cref="SoundEffectPlayer"/> played when the Statue of Prestige does its magic.</summary>
    public static readonly SoundEffectPlayer GunCock = new("GunCock", 5);

    /// <summary>The <see cref="SoundEffectPlayer"/> played when the Statue of Prestige does its magic.</summary>
    public static readonly SoundEffectPlayer DogStatuePrestige = new("DogStatuePrestige", 6);

    /// <summary>The <see cref="SoundEffectPlayer"/> played when the Statue of Prestige does its magic.</summary>
    public static readonly SoundEffectPlayer Chill = new("Chill", 7);

    /// <summary>The <see cref="SoundEffectPlayer"/> played when the Statue of Prestige does its magic.</summary>
    public static readonly SoundEffectPlayer ChillingShot = new("ChillingShot", 8);

    /// <summary>The <see cref="SoundEffectPlayer"/> played when the Statue of Prestige does its magic.</summary>
    public static readonly SoundEffectPlayer PlasmaShot = new("PlasmaShot", 9);

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="SoundEffectPlayer"/> class.</summary>
    /// <param name="name">The sound effect name.</param>
    /// <param name="value">The sound effect enum index.</param>
    private SoundEffectPlayer(string name, int value)
        : base(name, value)
    {
        var path = Path.Combine(ModHelper.DirectoryPath, "assets", "sounds", name + ".wav");
        using var fs = new FileStream(path, FileMode.Open);
        var soundEffect = SoundEffect.FromStream(fs);
        if (soundEffect is null)
        {
            ThrowHelperExtensions.ThrowFileLoadException($"Failed to load audio at {path}.");
        }

        CueDefinition cueDefinition = new()
        {
            name = name,
            instanceLimit = 1,
            limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest,
        };

        cueDefinition.SetSound(soundEffect, Game1.audioEngine.GetCategoryIndex("Sound"));
        Game1.soundBank.AddCue(cueDefinition);
    }

    /// <summary>Gets the sound played by a charging <see cref="StardewValley.Tools.FishingRod"/> or <see cref="StardewValley.Tools.Slingshot"/>.</summary>
    public static ICue? SinWave { get; internal set; } = Game1.soundBank?.GetCue("SinWave");

    /// <summary>Plays the corresponding <see cref="SoundEffect"/>.</summary>
    public void Play()
    {
        Game1.currentLocation.playSound(this.Name);
    }

    /// <summary>Plays the corresponding <see cref="SoundEffect"/> after the specified delay.</summary>
    /// <param name="delay">The delay in milliseconds.</param>
    public void PlayAfterDelay(int delay)
    {
        DelayedAction.playSoundAfterDelay(this.Name, delay);
    }
}
