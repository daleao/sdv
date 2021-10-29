using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;

namespace TheLion.Stardew.Professions.Framework.AssetLoaders
{
	public class SoundEffectLoader
	{
		/// <summary>Construct an instance.</summary>
		public SoundEffectLoader(string modPath)
		{
			foreach (var file in Directory.GetFiles(Path.Combine(modPath, "assets", "sfx"), "*.wav"))
				try
				{
					// load .wav
					using var fs = new FileStream(file, FileMode.Open);
					var soundEffect = SoundEffect.FromStream(fs);

					//// load .ogg
					//var soundEffect = OggLoader.Load(file);

					if (soundEffect is null) throw new FileLoadException();
					SoundByName.Add(Path.GetFileNameWithoutExtension(file), soundEffect);
				}
				catch (Exception ex)
				{
					ModEntry.Log($"Failed to load {file}. Loader returned {ex}", LogLevel.Error);
				}
		}

		public Dictionary<string, SoundEffect> SoundByName { get; } = new();
	}
}