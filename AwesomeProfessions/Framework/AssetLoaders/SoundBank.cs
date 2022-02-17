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
internal static class SoundBank
{
    /// <summary>The library of playable sounds.</summary>
    internal static Dictionary<SFX, SoundEffect> Collection { get; } = new();

    /// <summary>Construct an instance.</summary>
    internal static void LoadCollection(string modPath)
    {
        foreach (var file in Directory.GetFiles(Path.Combine(modPath, "assets", "sfx"), "*.wav"))
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (!Enum.TryParse<SFX>(fileName, out var sfx))
                    throw new InvalidOperationException($"Unexpected asset {file}.");

                using var fs = new FileStream(file, FileMode.Open);
                var soundEffect = SoundEffect.FromStream(fs);
                if (soundEffect is null) throw new FileLoadException($"Failed to load {file}.");

                Collection.Add(sfx, soundEffect);
            }
            catch (Exception ex)
            {
                Log.E(ex.Message);
            }
    }

    /// <summary>Play the specified sound effect.</summary>
    /// <param name="sfx">An <see cref="SFX"/> sfx.</param>
    internal static void Play(SFX sfx)
    {
        try
        {
            if (!Collection.TryGetValue(sfx, out var soundEffect))
                throw new ContentLoadException(
                    $"The asset '{sfx}.wav' was not loaded correctly. Make sure the file exists.");
            
            soundEffect.Play(Game1.options.soundVolumeLevel, 0f, 0f);
        }
        catch (Exception ex)
        {
            Log.E(ex.Message);
        }
    }
}