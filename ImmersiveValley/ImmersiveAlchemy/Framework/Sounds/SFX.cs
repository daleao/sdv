namespace DaLion.Stardew.Alchemy.Framework.Sounds;

#region using directives

using Ardalis.SmartEnum;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using StardewValley;

#endregion using directives

public sealed class SFX : SmartEnum<SFX>
{
    #region enum entries



    #endregion enum entries

    /// <summary>Construct an instance.</summary>
    /// <param name="name">The profession name.</param>
    /// <param name="value">The profession index.</param>
    public SFX(string name, int value) : base(name, value)
    {
        var path = Path.Combine(ModEntry.ModHelper.DirectoryPath, "assets", "sfx", name + ".wav");
        using var fs = new FileStream(path, FileMode.Open);
        var soundEffect = SoundEffect.FromStream(fs);
        if (soundEffect is null) throw new FileLoadException($"Failed to load audio at {path}.");

        CueDefinition cueDefinition = new()
        {
            name = name,
            instanceLimit = 1,
            limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest
        };
        cueDefinition.SetSound(soundEffect, Game1.audioEngine.GetCategoryIndex("Sound"));

        Game1.soundBank.AddCue(cueDefinition);
    }

    /// <summary>Play the corresponding <see cref="SoundEffect"/>.</summary>
    public void Play()
    {
        Game1.playSound(Name);
    }
}