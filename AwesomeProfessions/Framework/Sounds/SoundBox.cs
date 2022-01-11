using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Sounds;

/// <summary>Gathers and allows playing custom mod sound assets.</summary>
public class SoundBox
{
    /// <summary>Construct an instance.</summary>
    public SoundBox(string modPath)
    {
        foreach (var file in Directory.GetFiles(Path.Combine(modPath, "assets", "sfx"), "*.wav"))
            try
            {
                // load .wav
                using var fs = new FileStream(file, FileMode.Open);
                var soundEffect = SoundEffect.FromStream(fs);

                if (soundEffect is null) throw new FileLoadException();

                var fileName = Path.GetFileNameWithoutExtension(file);
                if (Enum.TryParse<SFX>(fileName, out var sfx))
                    SoundEffectByEnum.Add(sfx, soundEffect);
            }
            catch (Exception ex)
            {
                ModEntry.Log($"Failed to load {file}. Loader returned {ex}", LogLevel.Error);
            }
    }

    public Dictionary<SFX, SoundEffect> SoundEffectByEnum { get; } = new();

    /// <summary>Play the specified sound effect.</summary>
    /// <param name="id">An <see cref="SFX"/> id.</param>
    public void Play(SFX id)
    {
        try
        {
            if (SoundEffectByEnum.TryGetValue(id, out var sfx))
                sfx.Play(Game1.options.soundVolumeLevel, 0f, 0f);
            else throw new ContentLoadException();
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Couldn't play file 'assets/sfx/{id}.wav'. Make sure the file exists. {ex}",
                LogLevel.Error);
        }
    }
}