namespace DaLion.Core.Framework;

#region using directives

using System.IO;
using DaLion.Shared.Exceptions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

#endregion using directives

/// <summary>A custom <see cref="SoundEffect"/> that can be played through the game's <see cref="SoundBank"/>.</summary>
public sealed class SoundBox
{
    #region enum entries

    /// <summary>The <see cref="SoundBox"/> played when an enemy is chilled.</summary>
    public static readonly SoundBox Chill = new("Chill");

    /// <summary>The <see cref="SoundBox"/> played when an enemy is frozen.</summary>
    public static readonly SoundBox Freeze = new("Freeze");

    #endregion enum entries

    /// <summary>Initializes a new instance of the <see cref="SoundBox"/> class.</summary>
    /// <param name="name">The sound effect name.</param>
    private SoundBox(string name)
    {
        this.Name = name;
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

    /// <summary>Gets the name of the effect.</summary>
    public string Name { get; }

    /// <summary>Play a game sound for the local player.</summary>
    /// <param name="location">The location in which the sound is playing, if applicable.</param>
    /// <param name="position">The tile position from which the sound is playing, or <see langword="null"/> if it's playing throughout the location. Ignored in location is <see langword="null"/>.</param>
    /// <param name="pitch">The pitch modifier to apply, or <see langword="null"/> for the default pitch.</param>
    public void PlayLocal(GameLocation? location = null, Vector2? position = null, int? pitch = null)
    {
        if (location is not null)
        {
            location.localSound(this.Name, position, pitch);
        }
        else
        {
            Game1.playSound(this.Name, pitch);
        }
    }

    /// <summary>Play a game sound for all players who can hear it.</summary>
    /// <param name="location">The location in which the sound is playing.</param>
    /// <param name="position">The tile position from which the sound is playing, or <c>null</c> if it's playing throughout the location.</param>
    /// <param name="pitch">The pitch modifier to apply, or <c>null</c> for the default pitch.</param>
    public void PlayAll(GameLocation location, Vector2? position = null, int? pitch = null)
    {
        location.playSound(this.Name, position, pitch);
    }

    /// <summary>Plays the corresponding <see cref="SoundEffect"/> after the specified delay.</summary>
    /// <param name="delay">The desired delay, in milliseconds.</param>
    /// <param name="location">The location in which the sound is playing, if applicable.</param>
    /// <param name="position">The tile position from which the sound is playing, or <see langword="null"/> if it's playing throughout the location.</param>
    /// <param name="pitch">The pitch modifier to apply, or -1 for the default pitch.</param>
    public void PlayAfterDelay(int delay, GameLocation? location = null, Vector2? position = null, int pitch = -1)
    {
        DelayedAction.playSoundAfterDelay(this.Name, delay, location, position, pitch);
    }
}
