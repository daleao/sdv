namespace DaLion.Stardew.Professions.Framework.AssetLoaders;

#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using StardewValley;

#endregion using directives

/// <summary>Gathers and allows playing custom mod sound assets.</summary>
internal static class SoundBox
{
    /// <summary>Construct an instance.</summary>
    internal static void Load(string modPath)
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
                    SoundBank.Add(sfx, soundEffect);
            }
            catch (Exception ex)
            {
                Log.E($"Failed to load {file}. Loader returned {ex}");
            }
    }

    /// <summary>The library of playable sounds.</summary>
    public static Dictionary<SFX, SoundEffect> SoundBank { get; } = new();

    /// <summary>Play the specified sound effect.</summary>
    /// <param name="id">An <see cref="SFX"/> id.</param>
    public static void Play(SFX id)
    {
        try
        {
            if (SoundBank.TryGetValue(id, out var sfx))
                sfx.Play(Game1.options.soundVolumeLevel, 0f, 0f);
            else throw new ContentLoadException();
        }
        catch (Exception ex)
        {
            Log.E($"Couldn't play file 'assets/sfx/{id}.wav'. Make sure the file exists. {ex}");
        }
    }
}