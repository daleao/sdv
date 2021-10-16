using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;

namespace TheLion.Stardew.Professions.Framework.AssetLoaders
{
	public class SoundEffectLoader
	{
		public Dictionary<string, SoundEffect> SoundByName { get; } = new();

		/// <summary>Construct an instance.</summary>
		public SoundEffectLoader()
		{
			foreach (var file in Directory.GetFiles(Path.Combine(ModEntry.ModPath, "assets", "sfx"), "*.wav"))
				try
				{
					// load .wav
					using var fs = new FileStream(file, FileMode.Open);
					var soundEffect = SoundEffect.FromStream(fs);

					//// load .ogg
					//var soundEffect = OggLoader.Load(file);
					
					if (soundEffect == null) throw new FileLoadException();
					SoundByName.Add(Path.GetFileNameWithoutExtension(file), soundEffect);
				}
				catch (Exception ex)
				{
					ModEntry.Log($"Failed to load {file}. Loader returned {ex}", LogLevel.Error);
				}
		}
	}
}